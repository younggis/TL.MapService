using DAL.Common.Helper;
using DAL.Common.LOG;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TL.MapClient.Handler
{
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public class Handler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string type = context.Request["YT"];
            if (string.IsNullOrEmpty(type)) { return; }
            if (type.Equals("request"))//请求次数
            { GetRequestNumber(context); }
            if (type.Equals("success"))//请求成功
            { GetSuccessResponseNumber(context); }
            if (type.Equals("error"))//请求失败
            { GetErrorResponseNumber(context); }
            if (type.Equals("delete"))//清除几天前数据
            { DeleteResponse(context); }
        }
        private void GetRequestNumber(HttpContext context)
        {
            Hashtable map = new Hashtable();
            DataTable dtTable = new DataTable();
            string type = HttpUtility.UrlDecode(context.Request["type"]);
            try
            {
                SqlserverHelper helper = new SqlserverHelper();
                dtTable = helper.GetRequestNumber(type);
                map.Add("state", "成功");
                map.Add("msg", "");
            }
            catch (Exception ex)
            {
                Log4Helper.Debug(ex.Message, ex);
                map.Add("state", "失败");
                map.Add("msg", ex.Message);
            }
            map.Add("data", dtTable);
            string json = JsonConvert.SerializeObject(map);
            context.Response.Write(json);
        }
        private void GetSuccessResponseNumber(HttpContext context)
        {
            Hashtable map = new Hashtable();
            DataTable dtTable = new DataTable();
            string type = HttpUtility.UrlDecode(context.Request["type"]);
            string success = HttpUtility.UrlDecode(context.Request["success"]);
            try
            {
                SqlserverHelper helper = new SqlserverHelper();
                dtTable = helper.GetSuccessResponseNumber(type, success);
                map.Add("state", "成功");
                map.Add("msg", "");
            }
            catch (Exception ex)
            {
                Log4Helper.Debug(ex.Message, ex);
                map.Add("state", "失败");
                map.Add("msg", ex.Message);
            }
            map.Add("data", dtTable);
            string json = JsonConvert.SerializeObject(map);
            context.Response.Write(json);
        }
        private void GetErrorResponseNumber(HttpContext context)
        {
            Hashtable map = new Hashtable();
            DataTable dtTable = new DataTable();
            string type = HttpUtility.UrlDecode(context.Request["type"]);
            string error = HttpUtility.UrlDecode(context.Request["error"]);
            try
            {
                SqlserverHelper helper = new SqlserverHelper();
                dtTable = helper.GetErrorResponseNumber(type, error);
                map.Add("state", "成功");
                map.Add("msg", "");
            }
            catch (Exception ex)
            {
                Log4Helper.Debug(ex.Message, ex);
                map.Add("state", "失败");
                map.Add("msg", ex.Message);
            }
            map.Add("data", dtTable);
            string json = JsonConvert.SerializeObject(map);
            context.Response.Write(json);
        }

        private void DeleteResponse(HttpContext context)
        {
            Hashtable map = new Hashtable();
            int request = 0;
            int success = 0;
            int fail = 0;
            int day = int.Parse(context.Request["type"]);
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
            context.Response.Write(json);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}