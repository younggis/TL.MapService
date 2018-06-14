using Common.Logging;
using DAL.Common.Helper;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;

namespace TL.MapService
{
    public class DeleteHistoryJob:IJob
    {
        private readonly static string keephisdays = ConfigurationManager.AppSettings["keephisdays"];//切片存放文件目录

        public void Execute(IJobExecutionContext context)
        {

            DeleteResponse(keephisdays);
            // do something
        }

        private void DeleteResponse(string daylength)
        {
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
            }
            catch (Exception ex)
            {
                Log4Helper.Debug(ex.Message, ex);
            }
        }
    }
}