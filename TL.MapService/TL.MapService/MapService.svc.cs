using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TL.MapService
{
    [ServiceContract(Namespace = "TL.MapService")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [JavascriptCallbackBehavior(UrlParameterName = "jsoncallback")]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]//实例化模式——会话模式，并发模型——多线程
    public class MapService
    {


        // 要使用 HTTP GET，请添加 [WebGet] 特性。(默认 ResponseFormat 为 WebMessageFormat.Json)
        // 要创建返回 XML 的操作，
        //     请添加 [WebGet(ResponseFormat=WebMessageFormat.Xml)]，
        //     并在操作正文中包括以下行:
        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
        [OperationContract]
        public void DoWork()
        {
            // 在此处添加操作实现
            return;
        }

        private static readonly string Redisbool = ConfigurationManager.AppSettings["redisbool"];//是否读写分离

        private static readonly string Defaultmaptype = ConfigurationManager.AppSettings["defaultmap"];//默认地图服务

        private static readonly string Dirfilepath = ConfigurationManager.AppSettings["filepath"];//存放文件位置

        private static readonly string Tilepath = ConfigurationManager.AppSettings["tilepath"];//已存在切片发布地址

        // 在此处添加更多操作并使用 [OperationContract] 标记它们
        /// <summary>
        /// 是否配置为使用缓存
        /// </summary>
        /// <param name="maptype">地图类型</param>
        /// <param name="x">行号</param>
        /// <param name="y">列号</param>
        /// <param name="z">地图等级</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json)]
        public async Task<Stream> GetMap(string maptype, string x, string y, string z)
        {
            WriteFileHelper.AddList("reqnum");
            //if (Redisbool == "true" || Redisbool == "TRUE")
            //{
            //    return await GeteMapService(maptype, x, y, z);
            //}
            //else
            //{
            //    return GeteMapServer(maptype, x, y, z);
            //}

            return GeteMapServer(maptype, x, y, z);
        }

        /// <summary>
        /// 带缓存功能的地图服务
        /// </summary>
        /// <param name="maptype">地图类型</param>
        /// <param name="x">行号</param>
        /// <param name="y">列号</param>
        /// <param name="z">地图等级</param>
        /// <returns></returns>
        public async Task<Stream> GeteMapService(string maptype, string x, string y, string z)
        {
            if (string.IsNullOrEmpty(maptype))
            {
                maptype = Defaultmaptype;
            }
            string key = maptype + "-" + JsonHelper.GetValueByKey(maptype).version + "-" + z + "-" + x + "-" + y;
            string base64 = (await SeRedisHelper.GetAsync(key)).ToString();
            if (!string.IsNullOrEmpty(base64))
            {
                try
                {
                    byte[] bytes = Convert.FromBase64String(base64);
                    Stream strem = new MemoryStream(bytes);
                    WriteFileHelper.AddList("redsuc");
                    return strem;
                }
                catch (Exception e)
                {
                    string base404 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAQAAAAEAAQMAAABmvDolAAAAA1BMVEX8+fKNaX6qAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAAH0lEQVRo3u3BAQ0AAADCoPdPbQ43oAAAAAAAAAAAvg0hAAABfxmcpwAAAABJRU5ErkJggg==";
                    WriteFileHelper.AddList("rederr");
                    Log4Helper.WriteLog("redis读取错误！");
                    Log4Helper.WriteLog(e);
                    byte[] bytes = Convert.FromBase64String(base404);
                    Stream strem = new MemoryStream(bytes);
                    return strem;
                }
            }
            else
            {
                string filepath = GetMapDir(maptype, x, y, z);
                string folderpath = CreateMapDir(maptype, x, y, z);
                DirectoryInfo dir = new DirectoryInfo(folderpath);
                if (!dir.Exists)
                {
                    try
                    {
                        dir.Create();
                    }
                    catch (Exception e)
                    {
                        Log4Helper.WriteLog("文件夹创建失败！");
                        Log4Helper.WriteLog(e);
                    }
                }
                if (File.Exists(filepath))
                {
                    try
                    {
                        
                        FileStream fs = new FileStream(filepath, FileMode.Open,
                            FileAccess.Read, FileShare.Read);
                        WriteFileHelper.AddList("folsuc");
                        return fs;
                    }
                    catch (Exception e)
                    {
                        string base404 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAQAAAAEAAQMAAABmvDolAAAAA1BMVEX8+fKNaX6qAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAAH0lEQVRo3u3BAQ0AAADCoPdPbQ43oAAAAAAAAAAAvg0hAAABfxmcpwAAAABJRU5ErkJggg==";
                        
                        Log4Helper.WriteLog("文件夹读取失败！");
                        Log4Helper.WriteLog(e);
                        byte[] bytes = Convert.FromBase64String(base404);
                        Stream strem = new MemoryStream(bytes);
                        WriteFileHelper.AddList("folerr");
                        return strem;
                    }
                }
                else
                {
                    string url = GetMapUrl(maptype, x, y, z);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Timeout = 60000;
                    request.KeepAlive = false;
                    request.Method = "GET";
                    request.ServicePoint.ConnectionLimit = 512;
                    request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.1.4322)"; // 
                    request.AllowAutoRedirect = true; //是否允许302
                    HttpWebResponse response = null;
                    Stream stream = null;
                    try
                    {
                        response = request.GetResponse() as HttpWebResponse;
                        stream = response.GetResponseStream();
                        byte[] bytes = StreamToBytes(stream);
                        WriteFileHelper.AddQueue(maptype, x, y, z, bytes);
                        WriteFileHelper.AddList("websuc");
                        Stream str = new MemoryStream(bytes);
                        return str;
                    }
                    catch (Exception e)
                    {
                        string base404 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAQAAAAEAAQMAAABmvDolAAAAA1BMVEX8+fKNaX6qAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAAH0lEQVRo3u3BAQ0AAADCoPdPbQ43oAAAAAAAAAAAvg0hAAABfxmcpwAAAABJRU5ErkJggg==";

                        Log4Helper.WriteLog("网络请求失败！");
                        Log4Helper.WriteLog(e);
                        byte[] bytes = Convert.FromBase64String(base404);
                        Stream strem = new MemoryStream(bytes);
                        WriteFileHelper.AddList("weberr");
                        return strem;
                    }
                    finally
                    {
                        request.Abort();
                        if (response != null)
                        {
                            response.Close();
                        }
                        if (stream != null)
                        {
                            stream.Close();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 不带缓存功能的地图服务
        /// </summary>
        /// <param name="maptype">地图类型</param>
        /// <param name="x">行号</param>
        /// <param name="y">列号</param>
        /// <param name="z">地图等级</param>
        /// <returns></returns>
        public Stream GeteMapServer(string maptype, string x, string y, string z)
        {
            if (string.IsNullOrEmpty(maptype))
            {
                maptype = Defaultmaptype;
            }
            string filepath = GetMapDir(maptype, x, y, z);
            string folderpath = CreateMapDir(maptype, x, y, z);
            DirectoryInfo dir = new DirectoryInfo(folderpath);
            if (!dir.Exists)
            {
                try
                {
                    dir.Create();
                    AddSecurityControll2Folder(folderpath);//向文件夹添加权限
                }
                catch (Exception e)
                {
                    Log4Helper.WriteLog("文件夹创建失败！");
                    Log4Helper.WriteLog(e);
                }
            }
            if (File.Exists(filepath))//
            {
                try
                {
                    FileStream fs = new FileStream(filepath, FileMode.Open,
                            FileAccess.Read, FileShare.Read);
                    WriteFileHelper.AddList("folsuc");
                    return fs;
                }
                catch (Exception e)
                {
                    string base404 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAQAAAAEAAQMAAABmvDolAAAAA1BMVEX8+fKNaX6qAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAAH0lEQVRo3u3BAQ0AAADCoPdPbQ43oAAAAAAAAAAAvg0hAAABfxmcpwAAAABJRU5ErkJggg==";
                    Log4Helper.WriteLog("文件夹读取失败！");
                    Log4Helper.WriteLog(e);
                    byte[] bytes = Convert.FromBase64String(base404);
                    Stream strem = new MemoryStream(bytes);
                    WriteFileHelper.AddList("folerr");
                    return strem;
                }
            }
            else
            {
                string url = GetMapUrl(maptype, x, y, z);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 6000;
                request.KeepAlive = false;
                request.Method = "GET";
                request.ServicePoint.ConnectionLimit = 512;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.1.4322)"; // 
                request.AllowAutoRedirect = true; //是否允许302
                HttpWebResponse response = null;
                Stream stream = null;
                try
                {
                    response = request.GetResponse() as HttpWebResponse;
                    stream = response.GetResponseStream();
                    byte[] bytes = StreamToBytes(stream);
                    WriteFileHelper.AddQueue(maptype, x, y, z, bytes);
                    WriteFileHelper.AddList("websuc");
                    Stream str = new MemoryStream(bytes);
                    return str;
                }
                catch (Exception e)
                {
                    string base404 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAQAAAAEAAQMAAABmvDolAAAAA1BMVEX8+fKNaX6qAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAAH0lEQVRo3u3BAQ0AAADCoPdPbQ43oAAAAAAAAAAAvg0hAAABfxmcpwAAAABJRU5ErkJggg==";
                    Log4Helper.WriteLog("网络请求失败！");
                    Log4Helper.WriteLog(e);
                    byte[] bytes = Convert.FromBase64String(base404);
                    Stream strem = new MemoryStream(bytes);
                    WriteFileHelper.AddList("weberr");
                    return strem;
                }
                finally
                {
                    request.Abort();
                    if (response != null)
                    {
                        response.Close();
                    }
                    if (stream != null)
                    {
                        stream.Close();
                    }
                }
            }
        }


        /// <summary>
        /// 为文件添加users，everyone用户组的完全控制权限
        /// </summary>
        /// <param name="filePath"></param>
        public static void AddSecurityControll2File(string filePath)
        {
            //获取文件信息
            FileInfo fileInfo = new FileInfo(filePath);
            //获得该文件的访问权限
            System.Security.AccessControl.FileSecurity fileSecurity = fileInfo.GetAccessControl();
            //添加ereryone用户组的访问权限规则 完全控制权限
            fileSecurity.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
            //添加Users用户组的访问权限规则 完全控制权限
            fileSecurity.AddAccessRule(new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow));
            //设置访问权限
            fileInfo.SetAccessControl(fileSecurity);
        }


        /// <summary>
        ///为文件夹添加users，everyone用户组的完全控制权限
        /// </summary>
        /// <param name="dirPath"></param>
        public static void AddSecurityControll2Folder(string dirPath)
        {
            //获取文件夹信息
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            //获得该文件夹的所有访问权限
            System.Security.AccessControl.DirectorySecurity dirSecurity = dir.GetAccessControl(AccessControlSections.All);
            //设定文件ACL继承
            InheritanceFlags inherits = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
            //添加ereryone用户组的访问权限规则 完全控制权限
            FileSystemAccessRule everyoneFileSystemAccessRule = new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, inherits, PropagationFlags.None, AccessControlType.Allow);
            //添加Users用户组的访问权限规则 完全控制权限
            FileSystemAccessRule usersFileSystemAccessRule = new FileSystemAccessRule("Users", FileSystemRights.FullControl, inherits, PropagationFlags.None, AccessControlType.Allow);
            bool isModified = false;
            dirSecurity.ModifyAccessRule(AccessControlModification.Add, everyoneFileSystemAccessRule, out isModified);
            dirSecurity.ModifyAccessRule(AccessControlModification.Add, usersFileSystemAccessRule, out isModified);
            //设置访问权限
            dir.SetAccessControl(dirSecurity);
        }


        //网络流转byte[]
        public static byte[] StreamToBytes(Stream stream)
        {
            List<byte> bytes = new List<byte>();
            int temp = stream.ReadByte();
            while (temp != -1)
            {
                bytes.Add((byte)temp);
                temp = stream.ReadByte();
            }
            return bytes.ToArray();
        }
        //将byte[]写入redis缓存
        public static void WriteRedis(string key, byte[] bytes)
        {
            Task.Run(() =>
            {
                SeRedisHelper.StringSet(key, Convert.ToBase64String(bytes));
            });
        }
        
        //地图切片路径
        public static string GetMapDir(string maptype, string x, string y, string z)
        {
            string mappath = Dirfilepath + CreateFolder(maptype) + z + "\\" + x + "\\" + y + JsonHelper.GetValueByKey(maptype).suffix;
            return mappath;
        }
        //生成切片地址
        public static string GetTileDir(string maptype, string x, string y, string z)
        {
            string mappath = Tilepath + CreateFolder(maptype) + z + "\\" + x + "\\" + y + JsonHelper.GetValueByKey(maptype).suffix;
            return mappath;
        }
        //文件夹绝对路径
        public static string CreateMapDir(string maptype, string x, string y, string z)
        {
            string folderpath = Dirfilepath + CreateFolder(maptype) + z + "\\" + x + "\\";
            return folderpath;
        }
        //图片类型
        public static bool GetTileType(string maptype)
        {
            string type = JsonHelper.GetValueByKey(maptype).suffix;
            return type == ".png"?true:false;
        }
        //根据文件夹绝对目录、版本和类型生成文件路径
        public static string CreateFolder(string maptype)
        {

            return JsonHelper.GetValueByKey(maptype).folder + JsonHelper.GetValueByKey(maptype).version + JsonHelper.GetValueByKey(maptype).type;
        }
        //生成在线地图路径
        public static string GetMapUrl(string maptype, string x, string y, string z)
        {
            string mapurl = JsonHelper.GetValueByKey(maptype).url;
            int minserver = JsonHelper.GetValueByKey(maptype).minserver;
            int maxserver = JsonHelper.GetValueByKey(maptype).maxserver;
            Random ran = new Random();
            int server = ran.Next(minserver, maxserver);
            return mapurl.Replace("{n}", server.ToString()).Replace("{x}", x).Replace("{y}", y).Replace("{z}", z);
        }
    }
}
