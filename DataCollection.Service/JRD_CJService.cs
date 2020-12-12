using DataCollection.Service.Interface;
using DataCollection.Utils.Repository;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataCollection.Service
{
    public class JRD_CJService : BaseRepository, IJRD_CJService
    {
        public JRD_CJService(SqlSugarClient db) : base(db)
        {

        }

    }
}
