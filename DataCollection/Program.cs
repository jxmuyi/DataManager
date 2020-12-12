using System;
using System.Threading.Tasks;
using DataCollection.Common;
using DataCollection.Jobs.HisJob;
using DataCollection.Utils;

namespace DataCollection
{
    class Program
    {
        static void Main(string[] args)
        {
            LogHelper.DebugConsole("Hello World!");
            Init();
            HisTask(100,1);
            LogHelper.DebugConsole("Hello World2!");
            Console.ReadLine();
        }
        #region 初始化配置
        static void Init()
        {
            AutoMapperHelper.InitMapper(new string[] { "DataCollection.Models" });
        }
        #endregion

        #region DBFirst
        /// <summary>
        /// dbFirst创建class文件
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="nameSpace"></param>
        /// <param name="className"></param>
        static void CreateClassFile(string dirPath, string nameSpace, string className)
        {
            var sqlHelper = new SqlSugarSetup();
            sqlHelper.CreateClassFileByName(dirPath, nameSpace, className);
        }

        #endregion

        static async void HisTask(int minute,int repeatCount)
        {
            var triggle = QuartzHelper.CreateTimeIntervalTriggle(minute, repeatCount);
            triggle.JobDataMap.Add("date1", "2019-01-01");
            triggle.JobDataMap.Add("date2", "2020-01-01");
           await QuartzHelper.StartJob<PatientInfoJob>(nameof(HisTask), triggle, nameof(PatientInfoJob), nameof(HisTask));
        }
    }
}
