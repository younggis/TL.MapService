using System;
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
    public class DeleteTile
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
        /// <summary>
        /// 局部更行地图
        /// </summary>
        /// <param name="maptype">地图类型</param>
        /// <param name="minx">左下角点x坐标</param>
        /// <param name="maxx">右上角点x坐标</param>
        /// <param name="miny">右上角点y坐标</param>
        /// <param name="maxy">左下角点y坐标</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public bool UpdateMap(string maptype, double minx, double maxy, double maxx, double miny)
        {
            return UpdateHelper.DeleteByUrl(maptype, minx, maxy, maxx, miny);
        }
    }
}
