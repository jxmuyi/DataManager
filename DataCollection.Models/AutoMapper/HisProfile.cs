using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using DataCollection.Models.JRD_CJ;

namespace DataCollection.Models.AutoMapper
{
    public class HisProfile:Profile
    {
        public HisProfile()
        {
            CreateMap<PatientInfo_BaseDto, PatientInfo_Base>().ReverseMap();
        }
    }
}
