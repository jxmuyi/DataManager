using System;
using System.Linq;
using System.Text;

namespace DataCollection.Models.JRD_CJ
{
    [SqlSugar.SugarTable("PatientInfo_Base", "JRD_CJ")]
    public partial class PatientInfo_Base
    {
           public PatientInfo_Base(){


           }
           /// <summary>
           /// Desc:医保/农合明细类别
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string PatientMedicalInsuranceIdentity {get;set;}

           /// <summary>
           /// Desc:修改人员
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string UpdateUser {get;set;}

           /// <summary>
           /// Desc:修改日期
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? UpdateTime {get;set;}

           /// <summary>
           /// Desc:身份证号，业务主键
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string SocialNo {get;set;}

           /// <summary>
           /// Desc:社保卡
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string SINCardNo {get;set;}

           /// <summary>
           /// Desc:病人性别
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string Sex {get;set;}

           /// <summary>
           /// Desc:联系电话
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string Phone {get;set;}

           /// <summary>
           /// Desc:类型 01门诊02住院03体检
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string PatientType {get;set;}

           /// <summary>
           /// Desc:病人姓名
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string PatientName {get;set;}

           /// <summary>
           /// Desc:病人医保类别 2自费 A医保 N农合
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string PatientMedicalInsuranceType {get;set;}

           /// <summary>
           /// Desc:地址
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string Address {get;set;}

        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        /// <summary>
        /// Desc:主键索引
        /// Default:
        /// Nullable:False
        /// </summary>           
        public long ID {get;set;}

           /// <summary>
           /// Desc:居民健康卡号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string HealthCardNo {get;set;}

           /// <summary>
           /// Desc:创建人员
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CreateUser {get;set;}

           /// <summary>
           /// Desc:创建日期
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? CreateTime {get;set;}

           /// <summary>
           /// Desc:业务编号补充，住院病人为住院次数，门诊病人为空
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string BusinessNoExtend {get;set;}

           /// <summary>
           /// Desc:业务编号，门诊病人为就诊号，住院病人为病例号
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string BusinessNo {get;set;}

           /// <summary>
           /// Desc:业务日期，住院入院日期或门诊就诊日期
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? BusinessDate {get;set;}

           /// <summary>
           /// Desc:出生日期
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? Birthday {get;set;}

    }
}
