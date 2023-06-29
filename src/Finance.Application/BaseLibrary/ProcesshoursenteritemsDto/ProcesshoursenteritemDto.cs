using Abp.Application.Services.Dto;
using System;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 列表
    /// </summary>
    public class ProcesshoursenteritemDto: EntityDto<long>
    {
        public System.DateTime Creationtime { get; set; }
        public System.Nullable<System.Int64> Creatoruserid { get; set; }
        public System.Nullable<System.Int64> DeleterUserId { get; set; }
        public System.DateTime DeleteTime { get; set; }
        public bool IsDeleted { get; set; }
        public decimal LaborHour { get; set; }
        public System.DateTime Lastmodificationtime { get; set; }
        public decimal Lastmodifieruserid { get; set; }
        public decimal MachineHour { get; set; }
        public decimal NumberPersonnel { get; set; }
        public decimal ProcessHoursenterId { get; set; }
        public int Year { get; set; }
    }
}