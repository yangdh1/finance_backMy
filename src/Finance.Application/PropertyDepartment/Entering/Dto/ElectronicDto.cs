using Finance.PropertyDepartment.Entering.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.Entering.Dto
{
    /// <summary> 
    /// 电子BOM表单 交互类
    /// </summary>
    public class ElectronicDto
    {
        /// <summary>
        /// id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 零件的id
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 电子料BOM Id
        /// </summary>
        public long ElectronicId { get; set; }
        /// <summary>
        /// 物料大类
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 物料种类
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 物料编号
        /// </summary>
        public string SapItemNum { get; set; }
        /// <summary>
        /// 材料名称
        /// </summary>
        public string SapItemName { get; set; }
        /// <summary>
        /// 项目物料的使用量
        /// </summary>
        public List<YearOrValueMode> MaterialsUseCount { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// 系统单价（原币）
        /// </summary>
        public List<YearOrValueMode> SystemiginalCurrency { get; set; }
        /// <summary>
        /// 项目物料的年降率
        /// </summary>
        public List<YearOrValueMode> InTheRate { get; set; }
        /// <summary>
        /// 原币
        /// </summary>
        public List<YearOrValueMode> IginalCurrency { get; set; }
        /// <summary>
        /// 本位币
        /// </summary>
        public List<YearOrValueMode> StandardMoney { get; set; }
        /// <summary>
        /// 物料返利金额
        /// </summary>
        public decimal? RebateMoney { get; set; } = 0.0M;
        /// <summary>
        /// MOQ
        /// </summary>
        public decimal MOQ { get; set; }
        /// <summary>
        /// 可用库存
        /// </summary>
        public int AvailableStock { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 确认人 Id
        /// </summary>
        public long PeopleId { get; set; }
        /// <summary>
        /// 确认人姓名
        /// </summary>
        public string PeopleName { get; set; }
        /// <summary>
        /// 是否提交 true/1 提交  false/0 未提交
        /// </summary>
        public bool IsSubmit { get; set; }
        /// <summary>
        /// 是否录入 true/1 录入  false/0 未录入
        /// </summary>
        public bool IsEntering { get; set; }
        /// <summary>
        /// ECCN码
        /// </summary> 
        public virtual string ECCNCode { get; set; }
    }
    public class IsALLElectronicDto
    {
        /// <summary>
        /// 是否全部提交完成
        /// </summary>
        public bool isAll { get; set; }

        public List<ElectronicDto> ElectronicDtos { get; set; }
    }
}
