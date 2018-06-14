using DAL.Common.Helper;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
    public class ServiceState
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
        //获取服务状态，即响应次数
        // 在此处添加更多操作并使用 [OperationContract] 标记它们
        [OperationContract]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public string GetState(string requestinterval, string successtype, string successinterval, string errortype, string errorinterval)
        {
            requestinterval = string.IsNullOrEmpty(requestinterval) ? "minute" : requestinterval;
            successtype = string.IsNullOrEmpty(successtype) ? "folder" : successtype;
            successinterval = string.IsNullOrEmpty(successinterval) ? "minute" : successinterval;
            errortype = string.IsNullOrEmpty(errortype) ? "folder" : errortype;
            errorinterval = string.IsNullOrEmpty(errorinterval) ? "minute" : errorinterval;
            return GetServiceState(requestinterval, successtype, successinterval, errortype, errorinterval);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestinterval">请求次数时间间隔</param>
        /// <param name="successtype">返回成功类型</param>
        /// <param name="successinterval">返回成功时间间隔</param>
        /// <param name="errortype">返回失败类型</param>
        /// <param name="errorinterval">返回失败时间间隔</param>
        /// <returns></returns>
        private string GetServiceState(string requestinterval, string successtype, string successinterval, string errortype, string errorinterval)
        {
            Hashtable map = new Hashtable();
            try
            {
                DataTable dtTable1 = new DataTable();
                DataTable dtTable2 = new DataTable();
                DataTable dtTable3 = new DataTable();
                SqlserverHelper helper = new SqlserverHelper();
                dtTable1 = helper.GetRequestNumber(requestinterval);
                dtTable2 = helper.GetSuccessResponseNumber(successtype, successinterval);
                dtTable3 = helper.GetErrorResponseNumber(errortype, errorinterval);
                Hashtable datamap = new Hashtable();
                datamap.Add("country", dtTable1);
                datamap.Add("province", dtTable2);
                datamap.Add("county", dtTable3);
                map.Add("data", datamap);
                map.Add("state", "成功");
                map.Add("msg", "");
            }
            catch (Exception ex)
            {
                Log4Helper.Debug(ex.Message, ex);
                map.Add("state", "失败");
                map.Add("msg", ex.Message);
            }
            string json = JsonConvert.SerializeObject(map);
            return json;
        }
    }
}
