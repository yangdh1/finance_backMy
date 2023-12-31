namespace Finance.Authorization.Roles
{
    public static class StaticRoleNames
    {
        public static class Host
        {
            public const string Admin = "Admin";

            public const string SalesMan = "营销部-业务员";
            public const string ProjectManager = "项目经理";
            public const string ElectronicsEngineer = "产品开发部-电子工程师";
            public const string ElectronicsBomAuditor = "产品开发部-电子bom审核员";
            public const string StructuralEngineer = "产品开发部-结构工程师";
            public const string StructuralBomAuditor = "产品开发部-结构bom审核员";
            public const string R_D_TRAuditor = "产品开发部-TR审核员";
            public const string ElectronicsPriceInputter = "资源管理部-电子单价录入员";
            public const string StructuralPriceInputter = "资源管理部-结构单价录入员";
            public const string ElectronicsPriceAuditor = "资源管理部-电子单价审核员";
            public const string StructuralPriceAuditor = "资源管理部-结构单价审核员";
            public const string LossRateInputter = "工程技术部-损耗率录入员";
            public const string ManHourInputter = "工程技术部-工序工时录入员";
            public const string LogisticsCostInputter = "生产管理部-物流成本录入员";
            public const string TestCostInputter = "品质保证部-实验费用录入员";
            public const string GageCostInputter = "品质保证部-检具费用录入员";
            public const string GeneralManager = "总经理";
            public const string TradeComplianceAuditor = "财务部-贸易合规审核员";
            public const string MarketTRAuditor = "市场部-TR主方案审核员";
            public const string FinanceParamsInputter = "财务部-财务参数录入员";
            public const string FinanceProductCostInputter = "财务部-制造成本录入员";

            public const string FinanceAdmin = "财务部-数据管理员";
            public const string BomConsultant = "产品开发部-BOM查阅者";

            public const string Timeliness = "Timeliness";
            public const string ProjectPriceAuditor = "项目部-核价审核员";
            public const string FinancePriceAuditor = "财务部-核价审核员";

            public const string ProjectSupervisor = "项目课长";
        }

        public static class Tenants
        {
            public const string Admin = "Admin";
        }
    }
}
