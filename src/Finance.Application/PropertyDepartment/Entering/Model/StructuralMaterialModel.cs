using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.Entering.Model
{
    /// <summary>
    /// 结构料单价录入 模型
    /// </summary>
    public class StructuralMaterialModel
    {
        /// <summary>
        /// 零件的id
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 结构料 Id
        /// </summary>
        public long StructureId { get; set; }
        /// <summary>
        /// 币种
        /// </summary>       
        public string Currency { get; set; }       
        /// <summary>
        /// Sop
        /// </summary>
        public List<YearOrValueMode> Sop { get; set; }
        /// <summary>
        /// 物料可返利金额
        /// </summary>
        public decimal RebateMoney { get; set; }
        /// <summary>
        /// 项目物料的使用量
        /// </summary>
        public List<YearOrValueMode> MaterialsUseCount { get; set; }
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
        /// ECCN码
        /// </summary> 
        public virtual string ECCNCode { get; set; }
    }
}
