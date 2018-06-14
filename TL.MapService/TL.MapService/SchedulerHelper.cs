using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Topshelf;

namespace TL.MapService
{
    public class SchedulerHelper
    {
        public static void Start()
        {
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler scheduler = sf.GetScheduler();    //创建调度实例
            //创建任务实例
            IJobDetail job = JobBuilder.Create<DeleteHistoryJob>().WithIdentity(new JobKey("deletehistoryjob")).Build();
            //创建触发器实例
            ITrigger trigger = TriggerBuilder.Create().StartAt(DateTime.Now.AddSeconds(0)).WithCronSchedule("0 0 0/1 * * ?").Build();

            scheduler.ScheduleJob(job, trigger);     //绑定触发器和任务
            scheduler.Start();   //启动监控
        }
    }
}