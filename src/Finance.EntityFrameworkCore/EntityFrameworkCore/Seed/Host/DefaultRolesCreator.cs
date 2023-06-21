using Abp.MultiTenancy;
using Finance.Authorization.Roles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.EntityFrameworkCore.Seed.Host
{
    public class DefaultRolesCreator
    {
        private readonly FinanceDbContext _context;

        public DefaultRolesCreator(FinanceDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateRoles();
        }

        private void CreateRoles()
        {
            var tenantId = FinanceConsts.MultiTenancyEnabled ? null : (int?)MultiTenancyConsts.DefaultTenantId;

            var rolesList = new List<Role>
            {
                new Role(tenantId, StaticRoleNames.Host.SalesMan , StaticRoleNames.Host.SalesMan ) { IsStatic = true, IsDefault = false },
                new Role(tenantId, StaticRoleNames.Host.ProjectManager , StaticRoleNames.Host.ProjectManager ) { IsStatic = true, IsDefault = false },
                new Role(tenantId, StaticRoleNames.Host.ElectronicsEngineer , StaticRoleNames.Host.ElectronicsEngineer ) { IsStatic = true, IsDefault = false },
                new Role(tenantId, StaticRoleNames.Host.ElectronicsBomAuditor , StaticRoleNames.Host.ElectronicsBomAuditor ) { IsStatic = true, IsDefault = false },
                new Role(tenantId, StaticRoleNames.Host.StructuralEngineer , StaticRoleNames.Host.StructuralEngineer ) { IsStatic = true, IsDefault = false },
                new Role(tenantId, StaticRoleNames.Host.StructuralBomAuditor , StaticRoleNames.Host.StructuralBomAuditor ) { IsStatic = true, IsDefault = false },
                new Role(tenantId, StaticRoleNames.Host.R_D_TRAuditor , StaticRoleNames.Host.R_D_TRAuditor ) { IsStatic = true, IsDefault = false },
                new Role(tenantId, StaticRoleNames.Host.ElectronicsPriceInputter , StaticRoleNames.Host.ElectronicsPriceInputter ) { IsStatic = true, IsDefault = false },
                new Role(tenantId, StaticRoleNames.Host.StructuralPriceInputter , StaticRoleNames.Host.StructuralPriceInputter ) { IsStatic = true, IsDefault = false },
                new Role(tenantId, StaticRoleNames.Host.ElectronicsPriceAuditor, StaticRoleNames.Host.ElectronicsPriceAuditor) { IsStatic = true, IsDefault = false },
                new Role(tenantId, StaticRoleNames.Host.StructuralPriceAuditor, StaticRoleNames.Host.StructuralPriceAuditor) { IsStatic = true, IsDefault = false },
                new Role(tenantId, StaticRoleNames.Host.LossRateInputter, StaticRoleNames.Host.LossRateInputter) { IsStatic = true, IsDefault = false },
                new Role(tenantId, StaticRoleNames.Host.ManHourInputter, StaticRoleNames.Host.ManHourInputter) { IsStatic = true, IsDefault = false },
                new Role(tenantId, StaticRoleNames.Host.LogisticsCostInputter, StaticRoleNames.Host.LogisticsCostInputter) { IsStatic = true, IsDefault = false },
                new Role(tenantId, StaticRoleNames.Host.TestCostInputter, StaticRoleNames.Host.TestCostInputter) { IsStatic = true, IsDefault = false },
                new Role(tenantId, StaticRoleNames.Host.GageCostInputter, StaticRoleNames.Host.GageCostInputter) { IsStatic = true, IsDefault = false },
                new Role(tenantId, StaticRoleNames.Host.GeneralManager, StaticRoleNames.Host.GeneralManager) { IsStatic = true, IsDefault = false },
                new Role(tenantId, StaticRoleNames.Host.TradeComplianceAuditor, StaticRoleNames.Host.TradeComplianceAuditor) { IsStatic = true, IsDefault = false },
                new Role(tenantId, StaticRoleNames.Host.MarketTRAuditor, StaticRoleNames.Host.MarketTRAuditor) { IsStatic = true, IsDefault = false },
                new Role(tenantId, StaticRoleNames.Host.FinanceParamsInputter, StaticRoleNames.Host.FinanceParamsInputter) { IsStatic = true, IsDefault = false },
                new Role(tenantId, StaticRoleNames.Host.FinanceProductCostInputter, StaticRoleNames.Host.FinanceProductCostInputter) { IsStatic = true, IsDefault = false },
                new Role(tenantId, StaticRoleNames.Host.FinanceAdmin, StaticRoleNames.Host.FinanceAdmin) { IsStatic = true, IsDefault = false },
                new Role(tenantId, StaticRoleNames.Host.BomConsultant, StaticRoleNames.Host.BomConsultant) { IsStatic = true, IsDefault = false },
                new Role(tenantId, StaticRoleNames.Host.ProjectPriceAuditor, StaticRoleNames.Host.ProjectPriceAuditor) { IsStatic = true, IsDefault = false },
                new Role(tenantId, StaticRoleNames.Host.FinancePriceAuditor, StaticRoleNames.Host.FinancePriceAuditor) { IsStatic = true, IsDefault = false },

                new Role(tenantId, StaticRoleNames.Host.Timeliness, StaticRoleNames.Host.Timeliness) { IsStatic = true, IsDefault = false, Description="时效性查询页面可查看的角色" },
                new Role(tenantId, StaticRoleNames.Host.ProjectSupervisor, StaticRoleNames.Host.ProjectSupervisor) { IsStatic = true, IsDefault = false, Description="项目课长"  },
            };

            var roles = _context.Roles.Select(p => p.Name).ToList();
            var noDb = rolesList.Select(p => p.Name).Where(p => !roles.Contains(p));
            if (noDb.Any())
            {
                _context.Roles.AddRange(rolesList.Where(p => noDb.Contains(p.Name)));
                _context.SaveChanges();
            }
        }
    }
}
