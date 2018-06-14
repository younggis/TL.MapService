using DAL.Common.Helper;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public class DeleteHistoty
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

        // 在此处添加更多操作并使用 [OperationContract] 标记它们

        //删除历史数据
        // 在此处添加更多操作并使用 [OperationContract] 标记它们
        [OperationContract]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public string DeleteOlder(string day)
        {
            if (string.IsNullOrEmpty(day))
            {
                day = "7";
            }
            return DeleteResponse(day);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="daylength">时间长度（天为单位）</param>
        /// <returns></returns>
        private string DeleteResponse(string daylength)
        {
            Hashtable map = new Hashtable();
            int request = 0;
            int success = 0;
            int fail = 0;
            int day = int.Parse(daylength);
            try
            {
                SqlserverHelper helper = new SqlserverHelper();
                request = helper.DeleteOnline(day);
                success = helper.DeleteSuccess(day);
                fail = helper.DeleteFail(day);
                map.Add("state", "成功");
                map.Add("msg", "");
            }
            catch (Exception ex)
            {
                Log4Helper.Debug(ex.Message, ex);
                map.Add("state", "失败");
                map.Add("msg", ex.Message);
            }
            map.Add("request", request);
            map.Add("success", success);
            map.Add("fail", fail);
            string json = JsonConvert.SerializeObject(map);
            return json;
        }
    }
}
