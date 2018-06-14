using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace TL.MapMonitor
{
    public class JsonHelper
    {
        /// <summary>
        /// 将对象序列化为JSON格式
        /// </summary>
        /// <param name="o">对象</param>
        /// <returns>json字符串</returns>
        public static string SerializeObject(object o)
        {
            string json = JsonConvert.SerializeObject(o);
            return json;
        }

        /// <summary>
        /// 解析JSON字符串生成对象实体
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json字符串(eg.{"ID":"112","Name":"石子儿"})</param>
        /// <returns>对象实体</returns>
        public static T DeserializeJsonToObject<T>(string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(T));
            T t = o as T;
            return t;
        }

        /// <summary>
        /// 解析JSON数组生成对象实体集合
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json数组字符串(eg.[{"ID":"112","Name":"石子儿"}])</param>
        /// <returns>对象实体集合</returns>
        public static List<T> DeserializeJsonToList<T>(string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<T>));
            List<T> list = o as List<T>;
            return list;
        }

        /// <summary>
        /// 反序列化JSON到给定的匿名对象.
        /// </summary>
        /// <typeparam name="T">匿名对象类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <param name="anonymousTypeObject">匿名对象</param>
        /// <returns>匿名对象</returns>
        public static T DeserializeAnonymousType<T>(string json, T anonymousTypeObject)
        {
            T t = JsonConvert.DeserializeAnonymousType(json, anonymousTypeObject);
            return t;
        }
        /// <summary>
        /// 读取本地json配置
        /// </summary>
        /// <returns></returns>
        public static string ReadJsonFile()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            using (FileStream fsRead = new FileStream(path + "config.json", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                StreamReader sr = new StreamReader(fsRead);
                string str1 = sr.ReadToEnd();

                return str1;
            }
        }


        public static List<MapServiceConfig> Mapserviceconfig;
        private static readonly object Locker = new object();
        public static List<MapServiceConfig> MapServiceManager
        {
            get
            {
                if (Mapserviceconfig == null)
                {
                    lock (Locker)
                    {
                        if (Mapserviceconfig != null) return Mapserviceconfig;
                        string config = ReadJsonFile();
                        Mapserviceconfig = DeserializeJsonToList<MapServiceConfig>(config);
                        return Mapserviceconfig;
                    }
                }
                return Mapserviceconfig;
            }
        }
        public static MapServiceConfig GetValueByKey(string maptype)
        {
            for (var i = 0; i < MapServiceManager.Count; i++)
            {
                if (MapServiceManager[i].maptype == maptype)
                {
                    return MapServiceManager[i];
                }
            }
            return null;
        }
    }

    public class MapServiceConfig
    {
        public string maptype { get; set; }
        public string url { get; set; }
        public string folder { get; set; }
        public string version { get; set; }
        public string type { get; set; }
        public int minserver { get; set; }
        public int maxserver { get; set; }
        public string suffix { get; set; }
    }
}