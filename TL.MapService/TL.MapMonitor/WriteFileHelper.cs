using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace TL.MapMonitor
{
    public class WriteFileHelper
    {
        private readonly static string Folderpath = ConfigurationManager.AppSettings["filepath"];//切片存放文件目录
        public static string CreateFolder(string maptype)
        {

            return JsonHelper.GetValueByKey(maptype).folder + JsonHelper.GetValueByKey(maptype).version + JsonHelper.GetValueByKey(maptype).type;
        }
        //删除切片
        public static bool DeleteFile(string maptype, string x, string y, string z)
        {
            string fullfilepath = Folderpath + CreateFolder(maptype) + z + "\\" + x + "\\" + y + JsonHelper.GetValueByKey(maptype).suffix; ;
            try
            {
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