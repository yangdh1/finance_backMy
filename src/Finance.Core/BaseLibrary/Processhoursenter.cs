using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.BaseLibrary
{
    [Table("ProcessHoursEnter")]
    public class Processhoursenter : FullAuditedEntity<long>
    {
        /// <summary>
        /// 组件id
        /// </summary>
        [Key]
        [Column("id")]
        public override long Id { get; set; }

        /// <summary>
        /// 流程审批id
        /// </summary>
        [Column("audit_flow_id")]
        public decimal AuditFlowId { get; set; }

        /// <summary>
        /// 软硬件总价
        /// </summary>
        [Column("develop_total_price")]
        public decimal? DevelopTotalPrice { get; set; }

        /// <summary>
        /// 设备1名称
        /// </summary>
        [Column("device_one_name")]
        [StringLength(200, ErrorMessage = "设备1名称长度不能超出200字符")]
        public string DeviceOneName { get; set; }

        /// <summary>
        /// 设备1数量
        /// </summary>
        [Column("device_one_number")]
        public decimal? DeviceOneNumber { get; set; }

        /// <summary>
        /// 设备1单价
        /// </summary>
        [Column("device_one_price")]
        public decimal? DeviceOnePrice { get; set; }

        /// <summary>
        /// 设备1状态
        /// </summary>
        [Column("device_one_status")]
        [StringLength(200, ErrorMessage = "设备1状态长度不能超出200字符")]
        public string DeviceOneStatus { get; set; }

        /// <summary>
        /// 设备3名称
        /// </summary>
        [Column("device_three_name")]
        [StringLength(200, ErrorMessage = "设备3名称长度不能超出200字符")]
        public string DeviceThreeName { get; set; }

        /// <summary>
        /// 设备3数量
        /// </summary>
        [Column("device_three_number")]
        public decimal? DeviceThreeNumber { get; set; }

        /// <summary>
        /// 设备3单价
        /// </summary>
        [Column("device_three_price")]
        public decimal? DeviceThreePrice { get; set; }

        /// <summary>
        /// 设备3状态
        /// </summary>
        [Column("device_three_status")]
        [StringLength(200, ErrorMessage = "设备3状态长度不能超出200字符")]
        public string DeviceThreeStatus { get; set; }

        /// <summary>
        /// 设备总价
        /// </summary>
        [Column("device_total_price")]
        public decimal? DeviceTotalPrice { get; set; }

        /// <summary>
        /// 设备2名称
        /// </summary>
        [Column("device_two_name")]
        [StringLength(200, ErrorMessage = "设备2名称长度不能超出200字符")]
        public string DeviceTwoName { get; set; }

        /// <summary>
        /// 设备2数量
        /// </summary>
        [Column("device_two_number")]
        public decimal? DeviceTwoNumber { get; set; }

        /// <summary>
        /// 设备2单价
        /// </summary>
        [Column("device_two_price")]
        public decimal? DeviceTwoPrice { get; set; }

        /// <summary>
        /// 设备2状态
        /// </summary>
        [Column("device_two_status")]
        [StringLength(38, ErrorMessage = "设备2状态长度不能超出38字符")]
        public string DeviceTwoStatus { get; set; }

        /// <summary>
        /// 开图软件
        /// </summary>
        [Column("figure_software")]
        [StringLength(200, ErrorMessage = "开图软件长度不能超出200字符")]
        public string FigureSoftware { get; set; }

        /// <summary>
        /// 检具名称
        /// </summary>
        [Column("fixture_gauge_name")]
        [StringLength(200, ErrorMessage = "检具名称长度不能超出200字符")]
        public string FixtureGaugeName { get; set; }

        /// <summary>
        /// 检具数量
        /// </summary>
        [Column("fixture_gauge_number")]
        public decimal? FixtureGaugeNumber { get; set; }

        /// <summary>
        /// 检具单价
        /// </summary>
        [Column("fixture_gauge_price")]
        public decimal? FixtureGaugePrice { get; set; }

        /// <summary>
        /// 治置1名称
        /// </summary>
        [Column("fixture_one_name")]
        [StringLength(200, ErrorMessage = "治置1名称长度不能超出200字符")]
        public string FixtureOneName { get; set; }

        /// <summary>
        /// 治置1数量
        /// </summary>
        [Column("fixture_one_number")]
        public decimal? FixtureOneNumber { get; set; }

        /// <summary>
        /// 治置1单价
        /// </summary>
        [Column("fixture_one_price")]
        public decimal? FixtureOnePrice { get; set; }

        /// <summary>
        /// 治置2名称
        /// </summary>
        [Column("fixture_two_name")]
        [StringLength(200, ErrorMessage = "治置2名称长度不能超出200字符")]

        public string FixtureTwoName { get; set; }

        /// <summary>
        /// 治置2数量
        /// </summary>
        [Column("fixture_two_number")]
        public decimal? FixtureTwoNumber { get; set; }

        /// <summary>
        /// 治置2单价
        /// </summary>
        [Column("fixture_two_price")]
        public decimal? FixtureTwoPrice { get; set; }

        /// <summary>
        /// 工装治具总价
        /// </summary>
        [Column("frock_fixture_one_price")]
        public decimal? FrockFixtureOnePrice { get; set; }

        /// <summary>
        /// 工装名称
        /// </summary>
        [Column("frock_name")]
        [StringLength(200, ErrorMessage = "工装名称长度不能超出200字符")]
        public string FrockName { get; set; }

        /// <summary>
        /// 工装数量
        /// </summary>
        [Column("frock_number")]
        public decimal? FrockNumber { get; set; }

        /// <summary>
        /// 工装单价
        /// </summary>
        [Column("frock_price")]
        public decimal? FrockPrice { get; set; }

        /// <summary>
        /// 硬件设备1名称
        /// </summary>
        [Column("hardware_device_one_name")]
        [StringLength(200, ErrorMessage = "硬件设备1名称长度不能超出200字符")]

        public string HardwareDeviceOneName { get; set; }

        /// <summary>
        /// 硬件设备1数量
        /// </summary>
        [Column("hardware_device_one_number")]
        public decimal? HardwareDeviceOneNumber { get; set; }

        /// <summary>
        /// 硬件设备1单价
        /// </summary>
        [Column("hardware_device_one_price")]
        public decimal? HardwareDeviceOnePrice { get; set; }

        /// <summary>
        /// 硬件设备总价
        /// </summary>
        [Column("hardware_device_total_price")]
        public decimal? HardwareDeviceTotalPrice { get; set; }

        /// <summary>
        /// 硬件设备2名称
        /// </summary>
        [Column("hardware_device_two_name")]
        [StringLength(200, ErrorMessage = "硬件设备2名称长度不能超出200字符")]

        public string HardwareDeviceTwoName { get; set; }

        /// <summary>
        /// 硬件设备2数量
        /// </summary>
        [Column("hardware_device_two_number")]
        public decimal? HardwareDeviceTwoNumber { get; set; }

        /// <summary>
        /// 硬件设备2单价
        /// </summary>
        [Column("hardware_device_two_price")]
        public decimal? HardwareDeviceTwoPrice { get; set; }

        /// <summary>
        /// 工序名称
        /// </summary>
        [Column("process_name")]
        [StringLength(200, ErrorMessage = "工序名称长度不能超出200字符")]
        public string ProcessName { get; set; }

        /// <summary>
        /// 工序编号
        /// </summary>
        [Column("process_number")]
        [StringLength(200, ErrorMessage = "工序编号长度不能超出200字符")]

        public string ProcessNumber { get; set; }

        /// <summary>
        /// 零件号
        /// </summary>
        [Column("product_id")]
        public decimal ProductId { get; set; }

        /// <summary>
        /// 软件单价
        /// </summary>
        [Column("software_price")]
        public decimal? SoftwarePrice { get; set; }

        /// <summary>
        /// 测试线名称
        /// </summary>
        [Column("test_line_name")]
        [StringLength(200, ErrorMessage = "测试线名称长度不能超出200字符")]
        public string TestLineName { get; set; }

        /// <summary>
        /// 测试线数量
        /// </summary>
        [Column("test_line_number")]
        public decimal? TestLineNumber { get; set; }

        /// <summary>
        /// 测试线单价
        /// </summary>
        [Column("test_line_price")]
        public decimal? TestLinePrice { get; set; }


    }
}
