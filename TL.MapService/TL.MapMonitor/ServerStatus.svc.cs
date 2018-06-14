using MSXML2;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace TL.MapMonitor
{
    [ServiceContract(Namespace = "TL.MapMonitor")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [JavascriptCallbackBehavior(UrlParameterName = "jsoncallback")]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]//实例化模式——会话模式，并发模型——多线程  
    public class ServerStatus
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
        private static readonly string Nginxurl = ConfigurationManager.AppSettings["nginxurl"];//nginx服务地址

        private static readonly string Serversurl = ConfigurationManager.AppSettings["serversurl"];//应用服务服务器地址
        // 在此处添加更多操作并使用 [OperationContract] 标记它们
        //获取服务状态
        // 在此处添加更多操作并使用 [OperationContract] 标记它们
        [OperationContract]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public string GetStatus()
        {
            Hashtable map = new Hashtable();
            map.Add("nginx", TestNginx());
            map.Add("server", TestWcf());
            string json = JsonConvert.SerializeObject(map);
            return json;
        }

        public Hashtable TestNginx()
        {
            Hashtable map = new Hashtable();
            string url = Nginxurl;
            ServerXMLHTTP http = new MSXML2.ServerXMLHTTP();
            http.setTimeouts(0, 3000, 1000, 1000);
            try
            {
                http.open("GET", url, false, null, null);
                
                http.send(url);
                int status = http.status;
                if (status == 200)
                {
                    //Console.WriteLine(System.Text.Encoding.Default.GetString(http.responseBody));
                    map.Add("status", "成功");
                }
                else
                {
                    map.Add("status", "不可用status:" + status.ToString());
                }
            }
            catch
            {
                map.Add("status", "不可用");
            }
            return map;
        }
        public Hashtable TestWcf()
        {
            Hashtable map = new Hashtable();
            string url = Serversurl;
            string[] urllist = url.Split(';');
            for (int i=0;i<urllist.Length;i++)
            {
                if(string.IsNullOrEmpty(urllist[i]))continue;
                ServerXMLHTTP http = new MSXML2.ServerXMLHTTP();
                http.setTimeouts(0, 3000, 1000, 1000);
                try
                {
                    http.open("GET", url, false, null, null);
                    http.send(url);
                    int status = http.status;
                    if (status == 200)
                    {
                        map.Add(urllist[i], "成功");
                    }
                    else
                    {
                        map.Add(urllist[i], "不可用status:" + status.ToString());
                    }
                }
                catch
                {
                    map.Add(urllist[i], "不可用");
                }
            }
            return map;
        }
    }
}
