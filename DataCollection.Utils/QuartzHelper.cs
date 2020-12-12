using Quartz;
using Quartz.Impl;
using System;
using System.Threading.Tasks;

namespace DataCollection.Utils
{
    public class QuartzHelper
    {
        private static ISchedulerFactory _schedulerFactory = null;
        static QuartzHelper()
        {
            _schedulerFactory = new StdSchedulerFactory();
        }

        public static ITrigger CreateDailyTimeIntervalTriggle(int hour,int minute)
        {
            var result = TriggerBuilder.Create().WithDailyTimeIntervalSchedule(e =>
            {
                e.StartingDailyAt(new TimeOfDay(hour, minute));
            }).Build();
            return result;
        }
        public static ITrigger CreateTimeIntervalTriggle(int minute,int repeatCount=0)
        {
            var result = TriggerBuilder.Create().WithSimpleSchedule(e =>
            {
                e.WithIntervalInMinutes(minute);
                if(repeatCount==0)
                {
                    e.RepeatForever();
                }
                else
                {
                    e.WithRepeatCount(repeatCount);
                }
            }).Build();
            return result;
        }
        public static async Task StartJob<T>(string scheduleName ,ITrigger trigger,string jobName,string groupName,string description=null) where T:IJob
        {
            var jobDetail = JobBuilder.Create<T>().WithIdentity(jobName, groupName).WithDescription(description).Build();
            var schedule =await _schedulerFactory.GetScheduler();
            await schedule.ScheduleJob(jobDetail, trigger);
            await schedule.Start();
        }
    }
}
