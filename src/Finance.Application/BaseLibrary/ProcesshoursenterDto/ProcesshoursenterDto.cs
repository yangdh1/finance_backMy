using Abp.Application.Services.Dto;
using System;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 列表
    /// </summary>
    public class ProcesshoursenterDto: EntityDto<long>
    {
        public bool IsDeleted { get; set; }
        public System.Nullable<System.Int64> DeleterUserId { get; set; }
        public System.Nullable<System.DateTime> DeletionTime { get; set; }
        public System.Nullable<System.DateTime> LastModificationTime { get; set; }
        public System.Nullable<System.Int64> LastModifierUserId { get; set; }
        public System.DateTime CreationTime { get; set; }
        public System.Nullable<System.Int64> CreatorUserId { get; set; }
        public decimal AuditFlowId { get; set; }
        public System.Nullable<System.Decimal> DevelopTotalPrice { get; set; }
        public string DeviceOneName { get; set; }
        public System.Nullable<System.Decimal> DeviceOneNumber { get; set; }
        public System.Nullable<System.Decimal> DeviceOnePrice { get; set; }
        public string DeviceOneStatus { get; set; }
        public string DeviceThreeName { get; set; }
        public System.Nullable<System.Decimal> DeviceThreeNumber { get; set; }
        public System.Nullable<System.Decimal> DeviceThreePrice { get; set; }
        public string DeviceThreeStatus { get; set; }
        public System.Nullable<System.Decimal> DeviceTotalPrice { get; set; }
        public string DeviceTwoName { get; set; }
        public System.Nullable<System.Decimal> DeviceTwoNumber { get; set; }
        public System.Nullable<System.Decimal> DeviceTwoPrice { get; set; }
        public string DeviceTwoStatus { get; set; }
        public string FigureSoftware { get; set; }
        public string FixtureGaugeName { get; set; }
        public System.Nullable<System.Decimal> FixtureGaugeNumber { get; set; }
        public System.Nullable<System.Decimal> FixtureGaugePrice { get; set; }
        public string FixtureOneName { get; set; }
        public System.Nullable<System.Decimal> FixtureOneNumber { get; set; }
        public System.Nullable<System.Decimal> FixtureOnePrice { get; set; }
        public string FixtureTwoName { get; set; }
        public System.Nullable<System.Decimal> FixtureTwoNumber { get; set; }
        public System.Nullable<System.Decimal> FixtureTwoPrice { get; set; }
        public System.Nullable<System.Decimal> FrockFixtureOnePrice { get; set; }
        public string FrockName { get; set; }
        public System.Nullable<System.Decimal> FrockNumber { get; set; }
        public System.Nullable<System.Decimal> FrockPrice { get; set; }
        public string HardwareDeviceOneName { get; set; }
        public System.Nullable<System.Decimal> HardwareDeviceOneNumber { get; set; }
        public System.Nullable<System.Decimal> HardwareDeviceOnePrice { get; set; }
        public System.Nullable<System.Decimal> HardwareDeviceTotalPrice { get; set; }
        public string HardwareDeviceTwoName { get; set; }
        public System.Nullable<System.Decimal> HardwareDeviceTwoNumber { get; set; }
        public System.Nullable<System.Decimal> HardwareDeviceTwoPrice { get; set; }
        public string ProcessName { get; set; }
        public string ProcessNumber { get; set; }
        public decimal ProductId { get; set; }
        public System.Nullable<System.Decimal> SoftwarePrice { get; set; }
        public string TestLineName { get; set; }
        public System.Nullable<System.Decimal> TestLineNumber { get; set; }
        public System.Nullable<System.Decimal> TestLinePrice { get; set; }
    }
}