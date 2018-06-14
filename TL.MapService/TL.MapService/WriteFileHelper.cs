using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using DAL.Common.Helper;

namespace TL.MapService
{
    public class TileQueueInfo
    {
        public string maptype { get; set; }
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
        public byte[] bytes { get; set; }
    }

    public class RedisQueue
    {
        public string key { get; set; }
        public byte[] bytes { get; set; }
    }


    public class WriteFileHelper
    {
        public readonly static WriteFileHelper Instance=new WriteFileHelper();

        private readonly static string Folderpath = ConfigurationManager.AppSettings["filepath"];//切片存放文件目录

        private readonly static int Timespace = int.Parse(ConfigurationManager.AppSettings["timespace"]);//写入数据库时间间隔

        private readonly static int Writefiletimespace = int.Parse(ConfigurationManager.AppSettings["writefiletimespace"]);//写入文件时间间隔

        private readonly static string Accessbool = ConfigurationManager.AppSettings["accessbool"];//是否监控请求，写入Access

        private static readonly string Redisbool = ConfigurationManager.AppSettings["redisbool"];//是否读写分离
        private WriteFileHelper()
        {
            
        }
        private static Queue<TileQueueInfo> listqueue = new Queue<TileQueueInfo>();

        private static Queue<RedisQueue> redislist = new Queue<RedisQueue>();


        private static long requestnumber = 0;//请求次数
        private static long rediserrorlist = 0;//redis请求失败
        private static long foldererrorlist = 0;//文件夹请求失败
        private static long weberrorlist = 0; //网络请求失败
        private static long redissuccesslist = 0;//redis请求成功
        private static long foldersuccesslist = 0;//文件夹请求成功
        private static long websuccesslist = 0; //网络请求成功

        public static void AddList(string type)//计数器
        {
            if (Accessbool == "true" || Accessbool == "TRUE")
            {
                switch (type)
                {
                    case "reqnum":
                        requestnumber++;
                        break;
                    case "rederr":
                        rediserrorlist++;
                        break;
                    case "folerr":
                        foldererrorlist++;
                        break;
                    case "weberr":
                        weberrorlist++;
                        break;
                    case "redsuc":
                        redissuccesslist++;
                        break;
                    case "folsuc":
                        foldersuccesslist++;
                        break;
                    case "websuc":
                        websuccesslist++;
                        break;
                }
            }
        }
        public static void AddQueue(string maptype, string x, string y, string z, byte[] bytes)
        {
            TileQueueInfo tilequeueinfo = new TileQueueInfo();
            tilequeueinfo.maptype = maptype;
            tilequeueinfo.x = x;
            tilequeueinfo.y = y;
            tilequeueinfo.z = z;
            tilequeueinfo.bytes = bytes;
            listqueue.Enqueue(tilequeueinfo);

            if (Redisbool.ToUpper() == "TRUE")
            {
                RedisQueue redisqueue = new RedisQueue();
                redisqueue.key = maptype + "-" + JsonHelper.GetValueByKey(maptype).version + "-" + z + "-" + x + "-" + y;
                redisqueue.bytes = bytes;
                redislist.Enqueue(redisqueue);
            }
        }
        public void Start()//启动线程
        {
            Thread thread = new Thread(ThreadStart);//写入文件
            thread.IsBackground = true;
            thread.Start();
            if (Redisbool.ToUpper() == "TRUE")
            {
                Thread threadredis = new Thread(ThreadWrite);//写入redis
                threadredis.IsBackground = true;
                threadredis.Start();
            }
            if (Accessbool.ToUpper() == "TRUE")
            {
                Thread threadsql = new Thread(ThreadSqlHelper);//监控请求
                threadsql.IsBackground = true;
                threadsql.Start();
            }
        }
        private void ThreadStart()
        {
            while (true)
            {
                if (listqueue.Count > 0)
                {
                    try
                    {
                        WriteFile();
                    }
                    catch (Exception ex)
                    {
                        Log4Helper.WriteLog(ex.ToString());
                    }
                }
                else
                {
                    //没有任务，休息5秒钟  
                    Thread.Sleep(Writefiletimespace);
                }
            }
        }

        private void ThreadWrite()
        {
            while (true)
            {
                if (listqueue.Count > 0)
                {
                    try
                    {
                        WriteRedis();
                    }
                    catch (Exception ex)
                    {
                        Log4Helper.WriteLog(ex.ToString());
                    }
                }
                else
                {
                    //没有任务，休息5秒钟  
                    Thread.Sleep(Timespace);
                }
            }
        }
        private void ThreadSqlHelper()//请求写入数据库
        {
            while (true)
            {
                try
                {
                    long result1 = SqlserverHelper.OnlineRequest(requestnumber);
                    long result2 = SqlserverHelper.FailResponse(rediserrorlist, "redis");
                    long result3 = SqlserverHelper.FailResponse(foldererrorlist, "folder");
                    long result4 = SqlserverHelper.FailResponse(weberrorlist, "web");
                    long result5 = SqlserverHelper.SuccessResponse(redissuccesslist, "redis");
                    long result6 = SqlserverHelper.SuccessResponse(foldersuccesslist, "folder");
                    long result7 = SqlserverHelper.SuccessResponse(websuccesslist, "web");
                    if (result1 < 1) Log4Helper.WriteLog("统计在线请求插入失败," + DateTime.Now);
                    if (result2 < 1) Log4Helper.WriteLog("redis失败请求插入失败," + DateTime.Now);
                    if (result3 < 1) Log4Helper.WriteLog("folder失败请求插入失败," + DateTime.Now);
                    if (result4 < 1) Log4Helper.WriteLog("web失败请求插入失败," + DateTime.Now);
                    if (result5 < 1) Log4Helper.WriteLog("redis成功请求插入失败," + DateTime.Now);
                    if (result6 < 1) Log4Helper.WriteLog("folder成功请求插入失败," + DateTime.Now);
                    if (result7 < 1) Log4Helper.WriteLog("web成功请求插入失败," + DateTime.Now);
                }
                catch (Exception ex)
                {
                    Log4Helper.WriteLog(ex.ToString());
                }
                finally
                {
                    requestnumber = 0;
                    rediserrorlist = 0;
                    foldererrorlist = 0;
                    weberrorlist = 0;
                    redissuccesslist = 0;
                    foldersuccesslist = 0;
                    websuccesslist = 0;
                    //没有任务，休息5秒钟  
                    Thread.Sleep(Timespace);
                } 
            }
        }
        private void WriteRedis()
        {
            while (redislist.Count > 0)
            {
                try
                {
                    RedisQueue redisinfo = redislist.Dequeue();
                    Task.Run(() =>
                    {
                        SeRedisHelper.StringSet(redisinfo.key, Convert.ToBase64String(redisinfo.bytes));
                    });
                }
                catch (Exception e)
                {
                    Log4Helper.WriteLog(e.ToString());
                }
            }
        }

        private void WriteFile()
        {
            while (listqueue.Count > 0)
            {
                try
                {
                    TileQueueInfo queueinfo = listqueue.Dequeue();
                    string mappath = Folderpath + CreateFolder(queueinfo.maptype) + queueinfo.z + "\\" + queueinfo.x + "\\" + queueinfo.y + JsonHelper.GetValueByKey(queueinfo.maptype).suffix;
                    WriteMapFileByByte(mappath, queueinfo.bytes);
                }
                catch (Exception e)
                {
                    Log4Helper.WriteLog(e.ToString());
                }
            }
        }
        //以队列方式写入缓存切片
        public static void WriteMapFileByByte(string filepath, byte[] bytes)
        {
            Task.Run(() =>
            {
                try
                {
                    FileStream fs = new FileStream(filepath, FileMode.Create, FileSystemRights.Modify, FileShare.ReadWrite, 20480, FileOptions.None);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Flush();
                    fs.Close();
                }
                catch (Exception e)
                {
                    Log4Helper.WriteLog("文件写入失败！");
                    Log4Helper.WriteLog(e);
                }
            });
        }
        public static string CreateFolder(string maptype)
        {

            return JsonHelper.GetValueByKey(maptype).folder + JsonHelper.GetValueByKey(maptype).version + JsonHelper.GetValueByKey(maptype).type;
        }

        //删除切片
        public static bool DeleteFile(string maptype, string x, string y, string z)
        {
            string fullfilepath = Folderpath + CreateFolder(maptype) + z + "\\" + x + "\\" + y + JsonHelper.GetValueByKey(maptype).suffix; ;
            string rediskey = maptype + "-" + JsonHelper.GetValueByKey(maptype).version + "-" + z + "-" + x + "-" + y;
            try
            {
                if (SeRedisHelper.HasKey(rediskey))
                {
                    SeRedisHelper.KeyDelete(rediskey);
                }
                if (File.Exists(fullfilepath))
                {
                    File.Delete(fullfilepath);
                }
                return true;
            }
            catch (Exception e)
            {
                Log4Helper.WriteLog("删除缓存失败！");
                Log4Helper.WriteLog(e);
                return false;
            }
        }

    }
}