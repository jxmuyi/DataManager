using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataCollection.Service.Interface;
using DataCollection.Utils.Repository;
using SqlSugar;

namespace DataCollection.Service
{
    public class HisService:BaseRepository,IHisService
    {
        public HisService(SqlSugarClient db) :base(db)
        { 
            
        }

       
    }
}
