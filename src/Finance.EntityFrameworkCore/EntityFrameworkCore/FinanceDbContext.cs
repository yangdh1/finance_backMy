﻿using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.DynamicEntityProperties;
using Abp.EntityHistory;
using Abp.Localization;
using Abp.MultiTenancy;
using Abp.Notifications;
using Abp.Organizations;
using Abp.Webhooks;
using Abp.Zero.EntityFrameworkCore;
using Finance.Audit;
using Finance.Authorization.Roles;
using Finance.Authorization.Users;
using Finance.BaseLibrary;
using Finance.EngineeringDepartment;
using Finance.Entering;
using Finance.FinanceMaintain;
using Finance.FinanceParameter;
using Finance.Hr;
using Finance.Infrastructure;
using Finance.MakeOffers;
using Finance.MultiTenancy;
using Finance.Nre;
using Finance.PriceEval;
using Finance.Processes;
using Finance.ProductDevelopment;
using Finance.ProductionControl;
using Finance.ProjectManagement;
using Finance.TradeCompliance;
using Finance.UpdateLog;
using Microsoft.EntityFrameworkCore;


namespace Finance.EntityFrameworkCore
{
    public class FinanceDbContext : AbpZeroDbContext<Tenant, Role, User, FinanceDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public virtual DbSet<FlowClearInfo> FlowClearInfo { set; get; }
        public virtual DbSet<FlowJumpInfo> FlowJumpInfo { set; get; }
        public virtual DbSet<FlowProcess> FlowProcessType { set; get; }
        public virtual DbSet<AuditFlow> AuditFlow { set; get; }
        public virtual DbSet<AuditFinishedProcess> AuditFinishedProcess { set; get; }
        public virtual DbSet<AuditCurrentProcess> AuditCurrentProcess { set; get; }
        public virtual DbSet<AuditFlowDetail> AuditFlowDetail { set; get; }
        public virtual DbSet<AuditFlowRight> AuditFlowRight { set; get; }
        public virtual DbSet<AuditFlowDelete> AuditFlowDelete { set; get; }
        public virtual DbSet<NoticeEmailInfo> NoticeEmailInfo { set; get; }
        public virtual DbSet<FinanceDictionary> FinanceDictionary { set; get; }
        public virtual DbSet<FinanceDictionaryDetail> FinanceDictionaryDetail { set; get; }
        public virtual DbSet<Department> Department { set; get; }
        public virtual DbSet<PriceEvaluation> PriceEvaluation { set; get; }
        public virtual DbSet<Pcs> Pcs { set; get; }
        public virtual DbSet<PcsYear> PcsYeay { set; get; }
        public virtual DbSet<ModelCount> ModelCount { set; get; }
        public virtual DbSet<ModelCountYear> ModelCountYeay { set; get; }
        public virtual DbSet<Requirement> Requirement { set; get; }
        public virtual DbSet<ProductInformation> ProductInformation { set; get; }

        public virtual DbSet<AllManufacturingCost> AllManufacturingCost { set; get; }

        

        public virtual DbSet<UserInputInfo> UserInputInfo { get; set; }
        public virtual DbSet<FileManagement> FileManagement { get; set; }
        // 基础单价表
        public virtual DbSet<UInitPriceForm> UInitPriceForm { set; get; }
        //财务维护 毛利率方案
        public virtual DbSet<GrossMarginForm> GrossMarginForm { set; get; }
        // 资源部电子物料录入
        public virtual DbSet<EnteringElectronic> EnteringElectronic { set; get; }
        // 资源部结构物料录入
        public virtual DbSet<StructureElectronic> StructureElectronic { set; get; }
        //汇率
        public virtual DbSet<ExchangeRate> ExchangeRate { set; get; }
        // Nre 项目管理部 手板件实体类
        public virtual DbSet<HandPieceCost> HandPieceCost { set; get; }
        // 实验费  实体类
        public virtual DbSet<LaboratoryFee> LaboratoryFee { set; get; }
        // Nre 资源部 模具清单实体类
        public virtual DbSet<MouldInventory> MouldInventory { set; get; }
        public virtual DbSet<QADepartmentQC> QADepartmentQC { set; get; }
        // Nre 品保录入 实验项目 实体类
        public virtual DbSet<QADepartmentTest> QADepartmentTest { set; get; }
        // Nre 项目管理部 其他费用实体类
        public virtual DbSet<RestsCost> RestsCost { set; get; }
        // 差旅费  实体类
        public virtual DbSet<TravelExpense> TravelExpense { set; get; }
        //NRE 营销部 实体类
        public virtual DbSet<QualityRatioYearInfo> QualityRatioYearInfo { set; get; }
        public virtual DbSet<InitialResourcesManagement> InitialResourcesManagement { set; get; }
        // 报价分析看板中的 动态单价表 实体类
        public virtual DbSet<DynamicUnitPriceOffers> DynamicUnitPriceOffers { set; get; }
        // 报价分析看板中的 汇总分析表  实体类
        public virtual DbSet<PooledAnalysisOffers> PooledAnalysisOffers { set; get; }
        //报价 项目看板实体类 实体类
        public virtual DbSet<ProjectBoardOffers> ProjectBoardOffers { set; get; }
        // 报价分析看板中的 产品单价表 实体类
        public virtual DbSet<UnitPriceOffers> UnitPriceOffers { set; get; }
        // 报价审核表 中的  内部核价信息
        public virtual DbSet<InternalInformation> InternalInformation { set; get; }
        // 报价审核表 中的 报价策略
        public virtual DbSet<BiddingStrategy> BiddingStrategy { set; get; }
        // 报价审核表
        public virtual DbSet<AuditQuotationList> AuditQuotationList { set; get; }
        // Nre Nre  零件是否全部录入 依据实体类
        public virtual DbSet<NreIsSubmit> NreIsSubmit { set; get; }
        // 回档文件列表实体类
        public virtual DbSet<DownloadListSave> DownloadListSave { set; get; }
        public virtual DbSet<StructureBomInfo> StructureBomInfo { get; set; }
        public virtual DbSet<ElectronicBomInfo> ElectronicBomInfo { get; set; }
        public virtual DbSet<ProductDevelopmentInput> ProductDevelopmentInput { get; set; }
        public virtual DbSet<ProductionControlInfo> ProductionControlInfo { get; set; }
        public virtual DbSet<LossRateInfo> LossRateInfo { get; set; }
        public virtual DbSet<LossRateYearInfo> LossRateYearInfo { get; set; }
        public virtual DbSet<EquipmentInfo> EquipmentInfo { get; set; }
        public virtual DbSet<TraceInfo> TraceInfo { get; set; }
        public virtual DbSet<ToolingFixtureInfo> ToolingFixtureInfo { get; set; }
        public virtual DbSet<WorkingHoursInfo> WorkingHoursInfo { get; set; }
        public virtual DbSet<YearInfo> YearInfo { get; set; }
        public virtual DbSet<QualityRatioEntryInfo> QualityCostProportionEntryInfo { get; set; }
        public virtual DbSet<RateEntryInfo> RateEntryInfo { get; set; }
        public virtual DbSet<ManufacturingCostInfo> ManufacturingCostInfo { get; set; }
        public virtual DbSet<UPHInfo> UPHInfo { get; set; }
        //贸易合规表
        public virtual DbSet<TradeComplianceCheck> TradeComplianceCheck { get; set; }
        //贸易合规中的材料表
        public virtual DbSet<ProductMaterialInfo> ProductMaterialInfo { get; set; }
        //电子BOM差异化表
        public virtual DbSet<ElecBomDifferent> ElecBomDifferent { get; set; }
        //电子BOM差异化表
        public virtual DbSet<StructBomDifferent> StructBomDifferent { get; set; }

        //产品开发部电子BOM输入信息备份
        public virtual DbSet<ElectronicBomInfoBak> ElectronicBomInfoBak { get; set; }
        //产品开发部结构BOM输入信息备份
        public virtual DbSet<StructureBomInfoBak> StructureBomInfoBak { get; set; }

        /// <summary>
        /// 时效性页面
        /// </summary>
        public virtual DbSet<Timeliness> Timeliness { get; set; }
        /// <summary>
        /// 版本表
        /// </summary>
        public virtual DbSet<Versions> Versions { get; set; }
        /// <summary>
        /// 更新日志表
        /// </summary>
        public virtual DbSet<UpdateLogInfo> UpdateLog { get; set; }

        // 基础库
        /// <summary>
        /// 基础库--设备库
        /// </summary>
        public virtual DbSet<FoundationDevice> FoundationDevice { get; set; }
        /// <summary>
        /// 基础库---设备明细
        /// </summary>
        public virtual DbSet<FoundationDeviceItem> FoundationDeviceItem { get; set; }
        /// <summary>
        /// 基础库--EMC
        /// </summary>
        public virtual DbSet<FoundationEmc> FoundationEmc { get; set; }
        /// <summary>
        /// 基础库--治具检具库界面
        /// </summary>
        public virtual DbSet<FoundationFixture> FoundationFixture { get; set; }
        /// <summary>
        /// 基础库---治具检具库界面明细
        /// </summary>
        public virtual DbSet<FoundationFixtureItem> FoundationFixtureItem { get; set; }
        /// <summary>
        /// 基础库--硬件库
        /// </summary>
        public virtual DbSet<FoundationHardware> FoundationHardware { get; set; }
        /// <summary>
        /// 基础库--硬件库明细
        /// </summary>
        public virtual DbSet<FoundationHardwareItem> FoundationHardwareItem { get; set; }
        /// <summary>
        /// 基础库--日志表
        /// </summary>
        public virtual DbSet<FoundationLogs> FoundationLogs { get; set; }
        /// <summary>
        /// 基础库--工装库
        /// </summary>
        public virtual DbSet<FoundationProcedure> FoundationProcedure { get; set; }
        /// <summary>
        /// 基础库--实验库环境
        /// </summary>
        public virtual DbSet<Foundationreliable> Foundationreliable { get; set; }
        /// <summary>
        /// 基础库--工序工时
        /// </summary>
        public virtual DbSet<FoundationReliableProcessHours> FoundationReliableProcessHours { get; set; }
        /// <summary>
        /// 基础库--标准工艺库
        /// </summary>
        public virtual DbSet<FoundationStandardTechnology> FoundationStandardTechnology { get; set; }
        /// <summary>
        /// 基础库--设备信息
        /// </summary>
        public virtual DbSet<FoundationTechnologyDevice> FoundationTechnologyDevice { get; set; }
        /// <summary>
        /// 基础库--工序工时工装治置
        /// </summary>
        public virtual DbSet<FoundationTechnologyFixture> FoundationTechnologyFixture { get; set; }
        /// <summary>
        /// 基础库--工序工时硬件设备
        /// </summary>
        public virtual DbSet<FoundationTechnologyFrock> FoundationTechnologyFrock { get; set; }
        /// <summary>
        /// 基础库--工时工序追溯部分(硬件及软件开发费用)
        /// </summary>
        public virtual DbSet<FoundationTechnologyHardware> FoundationTechnologyHardware { get; set; }
        /// <summary>
        /// 基础库--工时库
        /// </summary>
        public virtual DbSet<FoundationWorkingHour> FoundationWorkingHour { get; set; }
        /// <summary>
        /// 基础库--工时库明细
        /// </summary>
        public virtual DbSet<FoundationWorkingHourItem> FoundationWorkingHourItem { get; set; }
        /// <summary>
        /// 物流信息录入
        /// </summary>
        public virtual DbSet<Logisticscost> Logisticscost { get; set; }
        /// <summary>
        /// 工序维护表
        /// </summary>
        public virtual DbSet<BaseProcessMaintenance> BaseProcessMaintenance { get; set; }
        /// <summary>
        /// Bom录入
        /// </summary>
        public virtual DbSet<BomEnter> BomEnter { get; set; }
        /// <summary>
        /// Bom录入
        /// </summary>
        public virtual DbSet<BomEnterTotal> BomEnterTotal { get; set; }
        /// <summary>
        /// 工序工时导入
        /// </summary>
        public virtual DbSet<ProcessHoursEnter> ProcessHoursEnter { get; set; }
        /// <summary>
        /// 工序工时导入设备信息
        /// </summary> 
        public virtual DbSet<ProcessHoursEnterDevice> ProcessHoursEnterDevice { get; set; }
        /// <summary>
        /// 工序工时导入工装治具
        /// </summary>
        public virtual DbSet<ProcessHoursEnterFixture> ProcessHoursEnterFixture { get; set; }
        /// <summary>
        /// 工序工时导入硬件设备
        /// </summary>
        public virtual DbSet<ProcessHoursEnterFrock> ProcessHoursEnterFrock { get; set; }
        /// <summary>
        /// 工序工时导入年份数据
        /// </summary>
        public virtual DbSet<ProcessHoursEnteritem> ProcessHoursEnteritem { get; set; }
        /// <summary>
        /// 工序工时导入线体数量分摊率
        /// </summary>
        public virtual DbSet<ProcessHoursEnterLine> ProcessHoursEnterLine { get; set; }
        /// <summary>
        /// 工序工时导入UPH率
        /// </summary>
        public virtual DbSet<ProcessHoursEnterUph> ProcessHoursEnterUph { get; set; }

        public FinanceDbContext(DbContextOptions<FinanceDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// 重写OnModelCreating方法，配置映射
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 配置表名映射
            modelBuilder.Entity<FlowClearInfo>().ToTable("Af_FlowClearInfo");
            modelBuilder.Entity<FlowProcess>().ToTable("Af_FlowProcessType");
            modelBuilder.Entity<FlowJumpInfo>().ToTable("Af_FlowJumpInfo");
            modelBuilder.Entity<AuditFlow>().ToTable("Af");
            modelBuilder.Entity<AuditFinishedProcess>().ToTable("Af_Finished");
            modelBuilder.Entity<AuditCurrentProcess>().ToTable("Af_Current");
            modelBuilder.Entity<AuditFlowDetail>().ToTable("Afd");
            modelBuilder.Entity<AuditFlowRight>().ToTable("Afr");
            modelBuilder.Entity<AuditFlowDelete>().ToTable("Af_Delete");

            modelBuilder.Entity<NoticeEmailInfo>().ToTable("NoticeEmailInfo");

            modelBuilder.Entity<Tenant>().ToTable("T");
            modelBuilder.Entity<Edition>().ToTable("E");
            modelBuilder.Entity<FeatureSetting>().ToTable("F");
            modelBuilder.Entity<TenantFeatureSetting>().ToTable("Tfs");
            modelBuilder.Entity<EditionFeatureSetting>().ToTable("Efs");
            modelBuilder.Entity<BackgroundJobInfo>().ToTable("Bji");
            modelBuilder.Entity<UserAccount>().ToTable("Ua");
            var ni = modelBuilder.Entity<NotificationInfo>().ToTable("Ni");
            ni.Property(p => p.EntityTypeAssemblyQualifiedName).HasColumnName("Etaqn");
            modelBuilder.Entity<Role>().ToTable("R");
            var u = modelBuilder.Entity<User>().ToTable("U");
            u.Property(p => p.NormalizedEmailAddress).HasColumnName("Nea");
            u.Property(p => p.TenantId).HasColumnName("TId");
            var ul = modelBuilder.Entity<UserLogin>().ToTable("Ul");
            ul.Property(p => p.TenantId).HasColumnName("TId");
            ul.Property(p => p.LoginProvider).HasColumnName("Lp");

            var ula = modelBuilder.Entity<UserLoginAttempt>().ToTable("Ula");
            ula.Property(p => p.UserNameOrEmailAddress).HasColumnName("Unoea");
            ula.Property(p => p.TenancyName).HasColumnName("Tn");

            modelBuilder.Entity<UserRole>().ToTable("Ur");
            modelBuilder.Entity<UserClaim>().ToTable("Uc");
            modelBuilder.Entity<UserToken>().ToTable("Ut");
            modelBuilder.Entity<RoleClaim>().ToTable("Rc");
            modelBuilder.Entity<PermissionSetting>().ToTable("Ps");
            modelBuilder.Entity<RolePermissionSetting>().ToTable("Rps");
            modelBuilder.Entity<UserPermissionSetting>().ToTable("Ups");
            modelBuilder.Entity<Setting>().ToTable("S");

            var al = modelBuilder.Entity<AuditLog>().ToTable("Al");
            al.Property(p => p.TenantId).HasColumnName("TId");
            al.Property(p => p.ExecutionDuration).HasColumnName("Ed");


            modelBuilder.Entity<ApplicationLanguage>().ToTable("L");
            var lt = modelBuilder.Entity<ApplicationLanguageText>().ToTable("Lt");
            lt.Property(p => p.LanguageName).HasColumnName("Ln");
            modelBuilder.Entity<OrganizationUnit>().ToTable("Ou");
            var uou = modelBuilder.Entity<UserOrganizationUnit>().ToTable("Uou");
            uou.Property(p => p.TenantId).HasColumnName("TId");

            var our = modelBuilder.Entity<OrganizationUnitRole>().ToTable("Our");
            our.Property(p => p.OrganizationUnitId).HasColumnName("OuId");

            var tn = modelBuilder.Entity<TenantNotificationInfo>().ToTable("Tn");
            tn.Property(p => p.EntityTypeAssemblyQualifiedName).HasColumnName("Etaqn");
            var un = modelBuilder.Entity<UserNotificationInfo>().ToTable("Un");
            un.Property(p => p.CreationTime).HasColumnName("Ct");
            un.Property(p => p.UserId).HasColumnName("UId");

            var ns = modelBuilder.Entity<NotificationSubscriptionInfo>().ToTable("Ns");
            ns.Property(p => p.NotificationName).HasColumnName("Nn");
            ns.Property(p => p.EntityTypeName).HasColumnName("Etn");
            ns.Property(p => p.TenantId).HasColumnName("TId");
            ns.Property(p => p.EntityId).HasColumnName("EId");
            ns.Property(p => p.EntityTypeAssemblyQualifiedName).HasColumnName("Etaqn");



            var ec = modelBuilder.Entity<EntityChange>().ToTable("Ec");
            ec.Property(p => p.EntityTypeFullName).HasColumnName("Etfn");
            modelBuilder.Entity<EntityChangeSet>().ToTable("Ecs");
            modelBuilder.Entity<EntityPropertyChange>().ToTable("Epc");
            modelBuilder.Entity<WebhookEvent>().ToTable("We");
            modelBuilder.Entity<WebhookSubscriptionInfo>().ToTable("Ws");
            modelBuilder.Entity<WebhookSendAttempt>().ToTable("Wsa");
            modelBuilder.Entity<DynamicProperty>().ToTable("Dp");
            var dev = modelBuilder.Entity<DynamicPropertyValue>().ToTable("Dpv");
            dev.Property(p => p.TenantId).HasColumnName("TId");
            dev.Property(p => p.DynamicPropertyId).HasColumnName("DpId");


            var dep = modelBuilder.Entity<DynamicEntityProperty>().ToTable("Dep");
            dep.Property(p => p.TenantId).HasColumnName("TId");
            dep.Property(p => p.DynamicPropertyId).HasColumnName("DpId");
            dep.Property(p => p.EntityFullName).HasColumnName("Efn");

            var depv = modelBuilder.Entity<DynamicEntityPropertyValue>().ToTable("Depv");
            depv.Property(p => p.DynamicEntityPropertyId).HasColumnName("DepId");

            modelBuilder.Entity<UserInputInfo>().ToTable("UserInputInfo");
            modelBuilder.Entity<FileManagement>().ToTable("FileManagement");
            modelBuilder.Entity<ElectronicBomInfo>().ToTable("ElectronicBomInfo");
            modelBuilder.Entity<StructureBomInfo>().ToTable("StructureBomInfo");
            modelBuilder.Entity<ProductDevelopmentInput>().ToTable("ProductDevelopmentInput");
            modelBuilder.Entity<LossRateYearInfo>().ToTable("LossRateYearInfo");
            modelBuilder.Entity<ProductionControlInfo>().ToTable("ProductionControlInfo");
            modelBuilder.Entity<LossRateInfo>().ToTable("LossRateInfo");
            modelBuilder.Entity<EquipmentInfo>().ToTable("EquipmentInfo");
            modelBuilder.Entity<TraceInfo>().ToTable("TraceInfo");
            modelBuilder.Entity<ToolingFixtureInfo>().ToTable("ToolingFixtureInfo");
            modelBuilder.Entity<WorkingHoursInfo>().ToTable("WorkingHoursInfo");
            modelBuilder.Entity<YearInfo>().ToTable("YearInfo");
            modelBuilder.Entity<QualityRatioEntryInfo>().ToTable("QualityCostInfo");
            modelBuilder.Entity<QualityRatioYearInfo>().ToTable("QualityRatioYearInfo");
            modelBuilder.Entity<RateEntryInfo>().ToTable("RateEntryInfo");
            modelBuilder.Entity<ManufacturingCostInfo>().ToTable("ManufacturingCostInfo");
            modelBuilder.Entity<UPHInfo>().ToTable("UPHInfo");

            modelBuilder.Entity<TradeComplianceCheck>().ToTable("TradeComplianceCheck");
            modelBuilder.Entity<ProductMaterialInfo>().ToTable("ProductMaterialInfo");

            modelBuilder.Entity<ElecBomDifferent>().ToTable("ElecBomDifferent");
            modelBuilder.Entity<StructBomDifferent>().ToTable("StructBomDifferent");
            modelBuilder.Entity<ElectronicBomInfoBak>().ToTable("ElectronicBomInfoBak");
            modelBuilder.Entity<StructureBomInfoBak>().ToTable("StructureBomInfoBak");

            // 基础库
            modelBuilder.Entity<FoundationDevice>().ToTable("FoundationDevice");
            modelBuilder.Entity<FoundationDeviceItem>().ToTable("FoundationDeviceItem");
            modelBuilder.Entity<FoundationEmc>().ToTable("FoundationEmc");
            modelBuilder.Entity<FoundationFixture>().ToTable("FoundationFixture");
            modelBuilder.Entity<FoundationFixtureItem>().ToTable("FoundationFixtureItem");
            modelBuilder.Entity<FoundationHardware>().ToTable("FoundationHardware");
            modelBuilder.Entity<FoundationHardwareItem>().ToTable("FoundationHardwareItem");
            modelBuilder.Entity<FoundationLogs>().ToTable("FoundationLogs");
            modelBuilder.Entity<FoundationProcedure>().ToTable("FoundationProcedure");
            modelBuilder.Entity<Foundationreliable>().ToTable("FoundationReliable");
            modelBuilder.Entity<FoundationReliableProcessHours>().ToTable("FoundationReliableProcessHours");
            modelBuilder.Entity<FoundationStandardTechnology>().ToTable("FoundationStandardTechnology");
            modelBuilder.Entity<FoundationTechnologyDevice>().ToTable("FoundationTechnologyDevice");
            modelBuilder.Entity<FoundationTechnologyFixture>().ToTable("FoundationTechnologyFixture");
            modelBuilder.Entity<FoundationTechnologyFrock>().ToTable("FoundationTechnologyFrock");
            modelBuilder.Entity<FoundationTechnologyHardware>().ToTable("FoundationTechnologyHardware");
            modelBuilder.Entity<FoundationWorkingHour>().ToTable("FoundationWorkingHour");
            modelBuilder.Entity<FoundationWorkingHourItem>().ToTable("FoundationWorkingHourItem");
            modelBuilder.Entity<Logisticscost>().ToTable("LogisticsCost");
            modelBuilder.Entity<BaseProcessMaintenance>().ToTable("BaseProcessMaintenance");
            modelBuilder.Entity<BomEnter>().ToTable("BomEnter");
            modelBuilder.Entity<BomEnterTotal>().ToTable("BomEnterTotal");
            modelBuilder.Entity<ProcessHoursEnter>().ToTable("ProcessHoursEnter");
            modelBuilder.Entity<ProcessHoursEnterDevice>().ToTable("ProcessHoursEnterDevice");
            modelBuilder.Entity<ProcessHoursEnterFixture>().ToTable("ProcessHoursEnterFixture");
            modelBuilder.Entity<ProcessHoursEnterFrock>().ToTable("ProcessHoursEnterFrock");
            modelBuilder.Entity<ProcessHoursEnteritem>().ToTable("ProcessHoursEnteritem");
            modelBuilder.Entity<ProcessHoursEnterLine>().ToTable("ProcessHoursEnterLine");
            modelBuilder.Entity<ProcessHoursEnterUph>().ToTable("ProcessHoursEnterLine");

            base.OnModelCreating(modelBuilder);
        }
    }
}
