using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DataCollection.Common;
using DataCollection.Models;
using DataCollection.Service;
using DataCollection.Service.Interface;
using Quartz;
using Newtonsoft.Json;
using DataCollection.Utils;
using DataCollection.Models.JRD_CJ;

namespace DataCollection.Jobs.HisJob
{
    public class PatientInfoJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.Trigger.JobDataMap;
            var date1 = dataMap.GetString("date1");
            var date2 = dataMap.GetString("date2");
            using (var client = new SqlSugarSetup().GetSugarClient())
            {
                IHisService service = new HisService(client);
                string sql = $@"exec dbo.Proc_xxkdata_patientinfo '{date1}','{date2}'";
                var dynamicList = await service.QueryAdoSql<dynamic>(sql, "his");
                var hisPatientinfos = JsonHelper.JsonMapObjList<PatientInfo_BaseDto>(dynamicList);
                var jrd_patientinfos = AutoMapperHelper.mapper.Map<List<PatientInfo_Base>>(hisPatientinfos);
                IJRD_CJService jRD_CJService = new JRD_CJService(client);
                jRD_CJService.Add(jrd_patientinfos);
                //jrd_patientinfos.ForEach(async e =>
                //{
                //    await jRD_CJService.AddIfNotExists(e, f => f.BusinessNo == e.BusinessNo && (f.BusinessNoExtend == e.BusinessNoExtend||f.BusinessNoExtend == null));
                //});
                LogHelper.Info("PatientInfoJob Success!");
            }
        }
    }
}
