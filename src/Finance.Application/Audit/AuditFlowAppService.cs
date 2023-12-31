﻿using Abp.Application.Services;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using Finance.Audit.Dto;
using Finance.Authorization.Roles;
using Finance.Authorization.Users;
using Finance.Dto;
using Finance.Infrastructure;
using Finance.PriceEval;
using Finance.ProjectManagement;
using Finance.TradeCompliance;
using Finance.TradeCompliance.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Finance.Authorization.Roles.StaticRoleNames;

namespace Finance.Audit
{
    /// <summary>
    /// 审批流程应用方法服务.
    /// </summary>
    public class AuditFlowAppService : ApplicationService
    {        
        private readonly IRepository<AuditFlow, long> _auditFlowRepository;
        private readonly IRepository<AuditFinishedProcess, long> _auditFinishedProcessRepository;
        private readonly IRepository<AuditCurrentProcess, long> _auditCurrentProcessRepository;
        private readonly IRepository<AuditFlowDetail, long> _auditFlowDetailRepository;
        private readonly IRepository<AuditFlowRight, long> _auditFlowRightRepository;
        private readonly IRepository<AuditFlowDelete, long> _auditFlowDeleteRepository;
        private readonly IRepository<FlowProcess, long> _flowProcessRepository;
        private readonly IRepository<FlowJumpInfo, long> _flowJumpInfoRepository;
        private readonly IRepository<FlowClearInfo, long> _flowClearInfoRepository;
        private readonly IRepository<PriceEvaluation, long> _priceEvaluationRepository;
        private readonly IRepository<UserInputInfo, long> _userInputInfoRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<NoticeEmailInfo, long> _noticeEmailInfoRepository;
        private readonly IRepository<FinanceDictionaryDetail, string> _financeDictionaryDetailRepository;
        private readonly TradeComplianceAppService _tradeComplianceAppService;
        private long  _projectManager = 0;
        private List<string> _backProcessIdentifiers = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="auditFlowRepository"></param>
        /// <param name="auditFinishedProcessRepository"></param>
        /// <param name="auditCurrentProcessRepository"></param>
        /// <param name="auditFlowDetailRepository"></param>
        /// <param name="auditFlowRightRepository"></param>
        /// <param name="flowProcessRepository"></param>
        /// <param name="flowJumpInfoRepository"></param>
        /// <param name="flowClearInfoRepository"></param>
        /// <param name="priceEvaluationRepository"></param>
        /// <param name="userInputInfoRepository"></param>
        /// <param name="userRepository"></param>
        /// <param name="roleRepository"></param>
        /// <param name="userRoleRepository"></param>
        /// <param name="tradeComplianceAppService"></param>
        /// <param name="financeDictionaryDetailRepository"></param>
        /// <param name="noticeEmailInfoRepository"></param>
        public AuditFlowAppService(IRepository<AuditFlow, long> auditFlowRepository, IRepository<AuditFinishedProcess, long> auditFinishedProcessRepository, IRepository<AuditCurrentProcess, long> auditCurrentProcessRepository, IRepository<AuditFlowDetail, long> auditFlowDetailRepository, IRepository<AuditFlowRight, long> auditFlowRightRepository, IRepository<FlowProcess, long> flowProcessRepository, IRepository<FlowJumpInfo, long> flowJumpInfoRepository, IRepository<FlowClearInfo, long> flowClearInfoRepository, IRepository<PriceEvaluation, long> priceEvaluationRepository, IRepository<UserInputInfo, long> userInputInfoRepository, IRepository<User, long> userRepository, IRepository<Role> roleRepository, IRepository<UserRole, long> userRoleRepository, TradeComplianceAppService tradeComplianceAppService, IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository, IRepository<AuditFlowDelete, long> auditFlowDeleteRepository, IRepository<NoticeEmailInfo, long> noticeEmailInfoRepository)
        {
            _auditFlowRepository = auditFlowRepository;
            _auditFinishedProcessRepository = auditFinishedProcessRepository;
            _auditCurrentProcessRepository = auditCurrentProcessRepository;
            _auditFlowDetailRepository = auditFlowDetailRepository;
            _auditFlowRightRepository = auditFlowRightRepository;
            _flowProcessRepository = flowProcessRepository;
            _flowJumpInfoRepository = flowJumpInfoRepository;
            _flowClearInfoRepository = flowClearInfoRepository;
            _priceEvaluationRepository = priceEvaluationRepository;
            _userInputInfoRepository = userInputInfoRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _tradeComplianceAppService = tradeComplianceAppService;
            _financeDictionaryDetailRepository = financeDictionaryDetailRepository;
            _auditFlowDeleteRepository = auditFlowDeleteRepository;
            _noticeEmailInfoRepository = noticeEmailInfoRepository;
        }


        /// <summary>
        /// 根据用户ID获取关联的项目核价流程ID列表
        /// </summary>
        public async virtual Task<AuditFlowIdRetDto> GetAuditFlowIdsByUser(long userId)
        {
            AuditFlowIdRetDto auditFlowIdRetDto = new();
            var flowRightInfos = await _auditFlowRightRepository.GetAllListAsync(p => p.UserId == userId);
            if(flowRightInfos.Count > 0)
            {
                auditFlowIdRetDto.AuditFlowIdList = new();

                foreach (var flowRight in flowRightInfos)
                {
                    if(!auditFlowIdRetDto.AuditFlowIdList.Contains(flowRight.AuditFlowId))
                    {
                        auditFlowIdRetDto.AuditFlowIdList.Add(flowRight.AuditFlowId);
                    }
                }
                return auditFlowIdRetDto;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取用户有权限的流程
        /// </summary>
        /// <param name="processRightInputDto"></param>
        /// <returns></returns>
        public async virtual Task<AuditFlowRightDto> GetProcessRightsByFlowId(ProcessRightInputDto processRightInputDto)
        {
            AuditFlowRightDto auditFlowRightDto = new();
            var flowRightInfos = await _auditFlowRightRepository.GetAllListAsync(p => p.UserId == processRightInputDto.UserId);
            if (flowRightInfos.Count > 0)
            {
                auditFlowRightDto.AuditFlowRightList = new();
                foreach (var flowRight in flowRightInfos)
                {
                    if(flowRight.AuditFlowId == processRightInputDto.AuditFlowId)
                    {
                        auditFlowRightDto.AuditFlowRightList.Add(flowRight);
                    }
                }
                return auditFlowRightDto;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据流程标识符获取流程名称
        /// </summary>
        /// <param name="processIdentifier"></param>
        /// <returns></returns>
        public async virtual Task<string> GetProcessName(string processIdentifier)
        {
            var processTypeInfos = await _flowProcessRepository.GetAllListAsync(p => p.ProcessIdentifier == processIdentifier);
            if (processTypeInfos.Count > 0)
            {
                var processType = processTypeInfos.FirstOrDefault();
                return processType.ProcessName;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取当前用户关联的项目核价流程和界面
        /// </summary>
        public async virtual Task<List<AuditFlowRightInfoDto>> GetAllAuditFlowInfos()
        {
            List<AuditFlowRightInfoDto> auditFlowRightInfoDtoList = new();
            //登录的实例
            if(AbpSession.UserId is null)
            { 
                throw new FriendlyException(401, "请先登录");
            }
            var flowRightInfos = await _auditFlowRightRepository.GetAllListAsync(p => p.UserId == AbpSession.UserId.Value);
            foreach (var flowRight in flowRightInfos)
            {
                AuditFlowRightInfoDto auditFlowRightInfoDto = this.GetIndexOfRightInfoList(flowRight.AuditFlowId, auditFlowRightInfoDtoList);
                if(auditFlowRightInfoDto == null)
                {
                    auditFlowRightInfoDto = new();
                    auditFlowRightInfoDto.AuditFlowId = flowRight.AuditFlowId;
                    var priceEvaluations = await _priceEvaluationRepository.GetAllListAsync(p => p.AuditFlowId == flowRight.AuditFlowId);
                    if (priceEvaluations.Count > 0)
                    {
                        auditFlowRightInfoDto.AuditFlowTitle = priceEvaluations.FirstOrDefault().Title;
                    }
                    else
                    {
                        continue;
                    }
                    auditFlowRightInfoDto.AuditFlowRightDetailList = new();
                    auditFlowRightInfoDtoList.Add(auditFlowRightInfoDto);
                }
                if(flowRight.ProcessIdentifier == AuditFlowConsts.AF_ArchiveEnd)
                {
                    var flowList = await _auditFlowRepository.GetAllListAsync(p => p.Id == flowRight.AuditFlowId);
                    if (flowList.Count > 0)
                    {
                        if(!(flowList.FirstOrDefault()).IsValid)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                AuditFlowRightDetailDto auditFlowRightDetailDto = new()
                {
                    ProcessIdentifier = flowRight.ProcessIdentifier,
                    ProcessName = this.AuditFlowName(flowRight.ProcessIdentifier),
                    Right = flowRight.RightType,
                    IsRetype = flowRight.IsRetype,
                    JumpDescription = flowRight.Remark
                };
                auditFlowRightInfoDto.AuditFlowRightDetailList.Add(auditFlowRightDetailDto);
            }

            return auditFlowRightInfoDtoList;
        }

        private string AuditFlowName(string ProcessIdentifier)
        {
            var flowProcessList = _flowProcessRepository.GetAllList(p => p.ProcessIdentifier == ProcessIdentifier);
            if(flowProcessList.Count > 0)
            {
                return flowProcessList[0].ProcessName;
            }
            return null;
        }

        private AuditFlowRightInfoDto GetIndexOfRightInfoList(long id, List<AuditFlowRightInfoDto> list)
        {
            foreach (var flowRightInfoDto in list)
            {
                if(flowRightInfoDto.AuditFlowId == id)
                {
                    return flowRightInfoDto;
                }
            }

            return null;
        }

        /// <summary>
        /// 新建核价审批流程表录入
        /// </summary>
        public async virtual Task<long> SavaNewAuditFlowInfo(AuditFlowDto input)
        {
            //首先以映射转换对象
            var auditFlowInfo = ObjectMapper.Map<AuditFlow>(input);

            var flowInfos = await _auditFlowRepository.GetAllListAsync(p => p.QuoteProjectName == input.QuoteProjectName);
            if (flowInfos.Count > 0)
            {
                auditFlowInfo.QuoteVersion = flowInfos.Max(p => p.QuoteVersion) + 1;
            }
            else
            {
                auditFlowInfo.QuoteVersion = 1;
            }
            long flowId = await _auditFlowRepository.InsertAndGetIdAsync(auditFlowInfo);
            return flowId;
        }

        /// <summary>
        /// 保存项目经理信息
        /// </summary>
        /// <param name="projectManagerId"></param>
        /// <returns></returns>
        public void SavaProjectManagerInfo(long projectManagerId)
        {
            _projectManager = projectManagerId;
        }

        /// <summary>
        /// 保存项目核价看板的退回标识符
        /// </summary>
        /// <param name="identifiers"></param>
        /// <returns></returns>
        public void SavaBackProcessIdentifiers(List<string> identifiers)
        {
            List<string> tempIdentifiers = new List<string>(identifiers);

            foreach (var identifier in tempIdentifiers)
            {
                var clearIdentifierList = (from a in _flowClearInfoRepository.GetAllList(p => p.CurrentProcessIdentifier == identifier).Select(p => p.ClearProcessIdentifier).Distinct() select a).ToList();
                //不包括自身节点
                clearIdentifierList.Remove(identifier);
                //移除所有会重复流转的节点
                foreach (var clearIdentifier in clearIdentifierList)
                {
                    identifiers.Remove(clearIdentifier);
                }
            }
            _backProcessIdentifiers = identifiers;
        }

        /// <summary>
        /// 流程信息流转（单个流程）
        /// </summary>
        public async virtual Task<ReturnDto> UpdateAuditFlowInfo(AuditFlowDetailDto input)
        {
            ReturnDto returnDto = new();
            //登录的实例
            if (AbpSession.UserId is null)
            {
                throw new FriendlyException(401, "请先登录");
            }
            input.UserId = AbpSession.UserId.Value;

            if(input.ProcessIdentifier != AuditFlowConsts.AF_RequirementInput)
            {
                var auditFlowRight = await _auditFlowRightRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId && p.ProcessIdentifier == input.ProcessIdentifier && p.UserId == input.UserId && p.RightType == RIGHTTYPE.Edit);
                if (auditFlowRight.Count == 0)
                {
                    throw new FriendlyException("该界面已经处理完成");
                }
            }

            try
            {
                var flowInfo = await _auditFlowRepository.SingleAsync(p => p.Id == input.AuditFlowId);
                if (flowInfo != null)
                {
                    bool isExist = false;

                    //获取当前流程表里ID的最大值
                    long currentFormMaxId = 0;
                    var allCurrentProcesses = await _auditCurrentProcessRepository.GetAllListAsync();
                    if (allCurrentProcesses.Count > 0)
                    {
                        currentFormMaxId = allCurrentProcesses.Max(p => p.Id);
                    }

                    //获取已完成流程表里ID的最大值
                    long finishedFormMaxId = 0;
                    var allFinishedProcesses = await _auditFinishedProcessRepository.GetAllListAsync();
                    if (allFinishedProcesses.Count > 0)
                    {
                        finishedFormMaxId = allFinishedProcesses.Max(p => p.Id);
                    }

interfaceInput:
                    var flowDetailInfo = ObjectMapper.Map<AuditFlowDetail>(input);
                    var flowDetailRight = ObjectMapper.Map<AuditFlowRight>(input);
                    //获取当前核价流程表的已完成流程,并判断当前流程是否已在完成流程中
                    var finishedProcesses = await _auditFinishedProcessRepository.GetAllListAsync(p => p.AuditFlowId == flowInfo.Id);
                    foreach (var finishedProcess in finishedProcesses)
                    {
                        if(finishedProcess.FlowProcessIdentifier == input.ProcessIdentifier)
                        {
                            isExist = true;
                            break;
                        }
                    }

                    if (!isExist && input.Opinion != OPINIONTYPE.Reject)
                    { 
                        //新的已完成流程
                        AuditFinishedProcess auditFinishedProcess = new()
                        {
                            Id = ++finishedFormMaxId,
                            AuditFlowId = flowInfo.Id,
                            FlowProcessIdentifier = input.ProcessIdentifier
                        };
                        await _auditFinishedProcessRepository.InsertAsync(auditFinishedProcess);//新的已完成流程插入

                        // 项目经理查看权限表
                        List<AuditFlowRight> managerReadList = null;
                        if (AuditFlowConsts.AF_StructPriceInput == input.ProcessIdentifier || AuditFlowConsts.AF_NreInputMould == input.ProcessIdentifier || AuditFlowConsts.AF_ElectronicPriceInput == input.ProcessIdentifier)
                        {
                            //资源单价录入根据角色判断全部变成查看权限
                            string roleName;
                            if(AuditFlowConsts.AF_ElectronicPriceInput == input.ProcessIdentifier)
                            {
                                roleName = Host.ElectronicsPriceInputter;
                            }
                            else
                            {
                                roleName = Host.StructuralPriceInputter;
                            }
                            List<User> userList = await this.GetTheUserByRole(roleName);
                            foreach (var usr in userList)
                            {
                                AuditFlowRight flowRight = new()
                                {
                                    AuditFlowId = flowInfo.Id,
                                    ProcessIdentifier = input.ProcessIdentifier,
                                    UserId = usr.Id,
                                    RightType = RIGHTTYPE.ReadOnly,
                                    IsRetype = false
                                };
                                managerReadList = await this.InsertRightInfoAfterFinished(flowRight, managerReadList);
                            }
                            //增加其他的权限
                            AuditFlowRight otherFlowRight = new()
                            {
                                AuditFlowId = flowInfo.Id,
                                ProcessIdentifier = input.ProcessIdentifier,
                                RightType = RIGHTTYPE.ReadOnly,
                                IsRetype = false
                            };
                            await this.InsertOtherRightInfoAfterFinished(otherFlowRight);
                        }
                        else
                        {
                            AuditFlowRight flowRight = new()
                            {
                                AuditFlowId = flowInfo.Id,
                                ProcessIdentifier = input.ProcessIdentifier,
                                UserId = input.UserId,
                                RightType = RIGHTTYPE.ReadOnly,
                                IsRetype = false
                            };
                            managerReadList = await this.InsertRightInfoAfterFinished(flowRight, managerReadList);
                            await this.InsertOtherRightInfoAfterFinished(flowRight);
                        }

                        //更新一个list，包含所有已完成流程
                        finishedProcesses.Add(auditFinishedProcess);//把上面新的已完成流程添加到当前审批已完成流程list中

                        //已完成的流程要从当前流程中删除
                        var currentProcess = await _auditCurrentProcessRepository.GetAllListAsync(p => p.AuditFlowId == flowInfo.Id && p.FlowProcessIdentifier == input.ProcessIdentifier);
                        if (currentProcess.Count > 0)
                        {
                            await _auditCurrentProcessRepository.HardDeleteAsync(currentProcess.First());
                        }
                    }
                    var processNextList = await _flowJumpInfoRepository.GetAllListAsync(p => p.PreviousProcessIdentifier == input.ProcessIdentifier);//获取当前流程在跳转表中所有下一个流程的List
                    FlowJumpInfo[] nextJumpInfos = new FlowJumpInfo[processNextList.Count];
                    for (int i = 0; i < processNextList.Count; i++)
                    {
                        nextJumpInfos[i] = new();
                        nextJumpInfos[i].PreviousProcessIdentifier = processNextList[i].PreviousProcessIdentifier;
                        nextJumpInfos[i].Condition = processNextList[i].Condition;
                        nextJumpInfos[i].NextProcessIdentifier = processNextList[i].NextProcessIdentifier;
                    }

                    foreach (var processNext in nextJumpInfos)
                    {
returnloop:
                        if(processNext.Condition == input.Opinion)
                        {
                            if (input.Opinion == OPINIONTYPE.Reject)
                            {
                                //更新明细表里接收流程索引
                                if (flowDetailInfo.ProcessIdentifier == AuditFlowConsts.AF_PriceBoardAudit)
                                {
                                    foreach (var process in _backProcessIdentifiers)
                                    {
                                        flowDetailInfo.ReceiveProcessIdentifier = process;

                                        AuditFlowDetail auditFlowDetail = new AuditFlowDetail();
                                        auditFlowDetail.AuditFlowId = flowDetailInfo.AuditFlowId;
                                        auditFlowDetail.UserId = flowDetailInfo.UserId;
                                        auditFlowDetail.ProcessIdentifier = flowDetailInfo.ProcessIdentifier;
                                        auditFlowDetail.ReceiveProcessIdentifier = flowDetailInfo.ReceiveProcessIdentifier;
                                        auditFlowDetail.Opinion = flowDetailInfo.Opinion;
                                        auditFlowDetail.OpinionDescription = flowDetailInfo.OpinionDescription;

                                        currentFormMaxId = await this.ProcessRejectInfo(auditFlowDetail, currentFormMaxId);
                                    }
                                }
                                else
                                {
                                    flowDetailInfo.ReceiveProcessIdentifier = processNext.NextProcessIdentifier;
                                    currentFormMaxId = await this.ProcessRejectInfo(flowDetailInfo, currentFormMaxId);
                                }
                            }
                            else
                            {
                                //获取当前流程是跳转表中下一个流程的List
                                var processPrevList = await _flowJumpInfoRepository.GetAllListAsync(p => p.NextProcessIdentifier == processNext.NextProcessIdentifier);//获取下一个流程在跳转表中所有上一个流程的List
                                                                                                                                                                        //检查所有上一个流程都已经在完成流程里了
                                bool isAllFinished = true;
                                foreach (var processPrev in processPrevList)
                                {
                                    if(processPrev.Condition == OPINIONTYPE.Reject)
                                    {
                                        continue;
                                    }
                                    bool isFinished = false;
                                    foreach (var finishedProcess in finishedProcesses)
                                    {
                                        if (finishedProcess.FlowProcessIdentifier == processPrev.PreviousProcessIdentifier)
                                        {
                                            isFinished = true;//前置流程已经完成
                                            break;
                                        }
                                    }

                                    if (!isFinished)//如果有前置流程没完成，则说明下一个流程条件不满足
                                    {
                                        isAllFinished = false;
                                        break;
                                    }
                                }
                                AuditCurrentProcess auditCurrentProcess;
                                var currentList = await _auditCurrentProcessRepository.GetAllListAsync(p => p.AuditFlowId == flowInfo.Id && p.FlowProcessIdentifier == processNext.NextProcessIdentifier);
                                if (currentList.Count == 0)
                                {
                                    auditCurrentProcess = new()
                                    {
                                        Id = ++currentFormMaxId,
                                        AuditFlowId = flowInfo.Id,
                                        FlowProcessIdentifier = processNext.NextProcessIdentifier
                                    };
                                    if (isAllFinished)
                                    {
                                        if(auditCurrentProcess.FlowProcessIdentifier == AuditFlowConsts.AF_TradeApproval)
                                        {
                                            bool isTradeCompliance = true;
                                            //是否贸易合规判断
                                            isTradeCompliance = await _tradeComplianceAppService.IsProductsTradeComplianceOK(auditCurrentProcess.AuditFlowId);
                                            if (isTradeCompliance)
                                            {
                                                AuditFinishedProcess auditFinishedProcess = new()
                                                {
                                                    AuditFlowId = flowInfo.Id,
                                                    FlowProcessIdentifier = AuditFlowConsts.AF_TradeApproval,
                                                };
                                                finishedProcesses.Add(auditFinishedProcess);
                                                processNext.PreviousProcessIdentifier = AuditFlowConsts.AF_TradeApproval;
                                                processNext.Condition = OPINIONTYPE.Submit_Agreee;
                                                processNext.NextProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit;
                                                goto returnloop;
                                            }
                                            else
                                            {
                                                //贸易不合规，走不合规流程
                                                await _auditCurrentProcessRepository.InsertAsync(auditCurrentProcess);//插入当前流程
                                            }
                                        }
                                        else if (auditCurrentProcess.FlowProcessIdentifier == AuditFlowConsts.AF_TRAuditMKT)
                                        {
                                            flowDetailInfo.ReceiverId = (await this.GetTheUserByRole(Host.MarketTRAuditor)).FirstOrDefault().Id;
                                            flowDetailInfo.ReceiveProcessIdentifier = processNext.NextProcessIdentifier;
                                            returnDto = await this.InsertDetailInfo(flowDetailInfo);
                                            input.ProcessIdentifier = AuditFlowConsts.AF_TRAuditMKT;
                                            input.UserId = (await GetTheUserByRole(Host.MarketTRAuditor)).FirstOrDefault().Id;
                                            input.Opinion = OPINIONTYPE.Submit_Agreee;
                                            input.OpinionDescription = OpinionDescription.OD_MKT_TRMainAgree;
                                            goto interfaceInput;
                                        }
                                        else
                                        {
                                            //如果都到已完成流程里，则更新当前流程
                                            await _auditCurrentProcessRepository.InsertAsync(auditCurrentProcess);//插入当前流程
                                        }
                                    }
                                }
                                else
                                {
                                    auditCurrentProcess = currentList.FirstOrDefault();
                                }

                                flowDetailInfo.ReceiveProcessIdentifier = processNext.NextProcessIdentifier;
                                returnDto = await this.UpdateNextUserInfo(auditCurrentProcess, flowDetailInfo, isAllFinished);
                            }//end else if (input.Opinion == OPINIONTYPE.Reject)
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FriendlyException("获取审批流程信息失败，原因是" + ex.Message);
            }
            return returnDto;
        }


        private async Task<long> ProcessRejectInfo(AuditFlowDetail flowDetail, long currentProcessIdCount)
        {
            long returnId = 0;
            try
            {
                //清空已完成流程中的部分流程（需要清已完成流程表和权限表）
                if (flowDetail.ReceiveProcessIdentifier == AuditFlowConsts.AF_ArchiveEnd)
                {
                    //移除旧的当前流程
                    AuditCurrentProcess oldAuditCurrentProcess;
                    var currentList = await _auditCurrentProcessRepository.GetAllListAsync(p => p.AuditFlowId == flowDetail.AuditFlowId && p.FlowProcessIdentifier == flowDetail.ProcessIdentifier);
                    if (currentList.Count > 0)
                    {
                        oldAuditCurrentProcess = currentList.FirstOrDefault();
                        //更新当前流程
                        await _auditCurrentProcessRepository.HardDeleteAsync(oldAuditCurrentProcess);//删除当前流程
                    }

                    //添加新的当前流程
                    AuditCurrentProcess auditCurrentProcess;
                    currentList = await _auditCurrentProcessRepository.GetAllListAsync(p => p.AuditFlowId == flowDetail.AuditFlowId && p.FlowProcessIdentifier == flowDetail.ReceiveProcessIdentifier);
                    
                    if (currentList.Count > 0)
                    {
                        auditCurrentProcess = currentList.FirstOrDefault();
                        returnId = auditCurrentProcess.Id;
                    }
                    else
                    {
                        auditCurrentProcess = new()
                        {
                            Id = ++currentProcessIdCount,
                            AuditFlowId = flowDetail.AuditFlowId,
                            FlowProcessIdentifier = flowDetail.ReceiveProcessIdentifier
                        };
                        //更新当前流程表里的界面
                        await _auditCurrentProcessRepository.InsertAsync(auditCurrentProcess);//插入当前流程
                        returnId = currentProcessIdCount;
                    }
                    //更新权限表中的已有界面权限
                    List<AuditFlowRight> auditFlowRightlist = await _auditFlowRightRepository.GetAllListAsync(p => p.AuditFlowId == flowDetail.AuditFlowId && p.ProcessIdentifier == flowDetail.ProcessIdentifier);
                    foreach (AuditFlowRight auditFlowRight in auditFlowRightlist)
                    {
                        auditFlowRight.RightType = RIGHTTYPE.ReadOnly;
                        await _auditFlowRightRepository.UpdateAsync(auditFlowRight);
                    }
                }
                else
                {
                    //移除已完成流程
                    List<AuditFinishedProcess> auditFinishedProcesslist = (from a in await _flowClearInfoRepository.GetAllListAsync(p => p.CurrentProcessIdentifier == flowDetail.ReceiveProcessIdentifier)
                                                       join b in await _auditFinishedProcessRepository.GetAllListAsync(p => p.AuditFlowId == flowDetail.AuditFlowId) on a.ClearProcessIdentifier equals b.FlowProcessIdentifier
                                                       select b).ToList();
                    foreach (AuditFinishedProcess auditFinishedProcess in auditFinishedProcesslist)
                    {
                        await _auditFinishedProcessRepository.HardDeleteAsync(auditFinishedProcess);
                    }
                    //移除旧的当前流程
                    AuditCurrentProcess oldAuditCurrentProcess;
                    var currentList = await _auditCurrentProcessRepository.GetAllListAsync(p => p.AuditFlowId == flowDetail.AuditFlowId && p.FlowProcessIdentifier == flowDetail.ProcessIdentifier);
                    if (currentList.Count > 0)
                    {
                        oldAuditCurrentProcess = currentList.FirstOrDefault();
                        //更新当前流程
                        await _auditCurrentProcessRepository.HardDeleteAsync(oldAuditCurrentProcess);//删除当前流程
                    }

                    //添加新的当前流程
                    AuditCurrentProcess newAuditCurrentProcess;
                    currentList = await _auditCurrentProcessRepository.GetAllListAsync(p => p.AuditFlowId == flowDetail.AuditFlowId && p.FlowProcessIdentifier == flowDetail.ReceiveProcessIdentifier);
                    if (currentList.Count == 0)
                    {
                        newAuditCurrentProcess = new()
                        {
                            Id = ++currentProcessIdCount,
                            AuditFlowId = flowDetail.AuditFlowId,
                            FlowProcessIdentifier = flowDetail.ReceiveProcessIdentifier
                        };
                        //更新当前流程
                        await _auditCurrentProcessRepository.InsertAsync(newAuditCurrentProcess);//插入当前流程
                        returnId = currentProcessIdCount;
                    }
                    //移除在权限表中的已有流程
                    List<AuditFlowRight> auditFlowRightlist = (from a in await _flowClearInfoRepository.GetAllListAsync(p => p.CurrentProcessIdentifier == flowDetail.ReceiveProcessIdentifier)
                                                       join b in await _auditFlowRightRepository.GetAllListAsync(p => p.AuditFlowId == flowDetail.AuditFlowId) on a.ClearProcessIdentifier equals b.ProcessIdentifier
                                                       select b).ToList();
                    foreach (AuditFlowRight auditFlowRight in auditFlowRightlist)
                    {
                        if(auditFlowRight.ProcessIdentifier == flowDetail.ReceiveProcessIdentifier)
                        {

                            List<Role> roles = (from a in await _userRoleRepository.GetAllListAsync(p => p.UserId == auditFlowRight.UserId)
                                                join b in await _roleRepository.GetAllListAsync() on a.RoleId equals b.Id
                                                select b).ToList();

                            List<string> roleNames = (from a in roles.Select(p => p.Name).Distinct() select a).ToList();
                            //删除其他角色查看权限
                            if (roleNames.Contains(StaticRoleNames.Host.FinanceAdmin) || roleNames.Contains(StaticRoleNames.Host.BomConsultant))
                            {
                                await _auditFlowRightRepository.HardDeleteAsync(auditFlowRight);
                                continue;
                            }
                            AuditFlowDetail auditFlowDetail = new();
                            auditFlowDetail.AuditFlowId = flowDetail.AuditFlowId;
                            auditFlowDetail.ProcessIdentifier = flowDetail.ProcessIdentifier;
                            auditFlowDetail.UserId = flowDetail.UserId;
                            auditFlowDetail.Opinion = flowDetail.Opinion;
                            auditFlowDetail.OpinionDescription = flowDetail.OpinionDescription;
                            auditFlowDetail.ReceiveProcessIdentifier = flowDetail.ReceiveProcessIdentifier;

                            //删除其他角色查看权限end
                            if (auditFlowRight.ProcessIdentifier == AuditFlowConsts.AF_TRAuditMKT || auditFlowRight.ProcessIdentifier == AuditFlowConsts.AF_TRAuditR_D
                                || auditFlowRight.ProcessIdentifier == AuditFlowConsts.AF_ElecLossRateInput || auditFlowRight.ProcessIdentifier == AuditFlowConsts.AF_StructLossRateInput
                                || auditFlowRight.ProcessIdentifier == AuditFlowConsts.AF_ManHourImport || auditFlowRight.ProcessIdentifier == AuditFlowConsts.AF_LogisticsCostInput
                                || auditFlowRight.ProcessIdentifier == AuditFlowConsts.AF_NreInputEmc || auditFlowRight.ProcessIdentifier == AuditFlowConsts.AF_NreInputMould
                                || auditFlowRight.ProcessIdentifier == AuditFlowConsts.AF_NreInputTest || auditFlowRight.ProcessIdentifier == AuditFlowConsts.AF_NreInputGage
                                ||auditFlowRight.ProcessIdentifier== AuditFlowConsts.AF_StructBomPriceAudit || auditFlowRight.ProcessIdentifier == AuditFlowConsts.AF_ElecBomPriceAudit)
                            {
                                if (roles.Count > 0  && roleNames.Contains(StaticRoleNames.Host.ProjectManager))
                                {
                                    await _auditFlowRightRepository.HardDeleteAsync(auditFlowRight);
                                }
                                else
                                {
                                    auditFlowRight.RightType = RIGHTTYPE.Edit;
                                    auditFlowRight.IsRetype = true;
                                    auditFlowRight.Remark = auditFlowDetail.OpinionDescription;
                                    await _auditFlowRightRepository.UpdateAsync(auditFlowRight);
                                    //更新流程跳转
                                    auditFlowDetail.ReceiverId = auditFlowRight.UserId;
                                    await _auditFlowDetailRepository.InsertAsync(auditFlowDetail);

                                    await SendEmailToUserTest(auditFlowRight.AuditFlowId, auditFlowRight.ProcessIdentifier, auditFlowRight.UserId);
                                }
                            }
                            else
                            {
                                auditFlowRight.RightType = RIGHTTYPE.Edit;
                                auditFlowRight.IsRetype = true;
                                auditFlowRight.Remark = auditFlowDetail.OpinionDescription;
                                await _auditFlowRightRepository.UpdateAsync(auditFlowRight);
                                //更新流程跳转
                                auditFlowDetail.ReceiverId = auditFlowRight.UserId;
                                await _auditFlowDetailRepository.InsertAsync(auditFlowDetail);

                                await SendEmailToUserTest(auditFlowRight.AuditFlowId, auditFlowRight.ProcessIdentifier, auditFlowRight.UserId);
                            }
                        }
                        else
                        {
                            await _auditFlowRightRepository.HardDeleteAsync(auditFlowRight);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FriendlyException("拒绝流程处理" + ex.Message);
            }
            return returnId;
        }

        /// <summary>
        /// 流转明细表插入到数据库中
        /// </summary>
        /// <param name="flowDetail"></param>
        /// <returns></returns>
        private async Task<ReturnDto> InsertDetailInfo(AuditFlowDetail flowDetail)
        {
            ReturnDto returnDto = new();
            try
            {
                AuditFlowDetail tempAuditFlowDetail = new()
                {
                    AuditFlowId = flowDetail.AuditFlowId,
                    ProcessIdentifier = flowDetail.ProcessIdentifier,
                    UserId = flowDetail.UserId,
                    Opinion = flowDetail.Opinion,
                    OpinionDescription = flowDetail.OpinionDescription,
                    ReceiveProcessIdentifier = flowDetail.ReceiveProcessIdentifier,
                    ReceiverId = flowDetail.ReceiverId,
                };
                await _auditFlowDetailRepository.InsertAsync(tempAuditFlowDetail);
            }
            catch (Exception ex)
            {
                throw new FriendlyException("更新明细表" + ex.Message);
            }
            return returnDto;
        }

        /// <summary>
        /// 已完成的流程更新权限表（改成只读，同时把可以查看的用户ID添加到权限表中）
        /// </summary>
        /// <param name="flowRight"></param>
        /// <param name="managerReadList"></param>
        /// <returns></returns>
        private async Task<List<AuditFlowRight>> InsertRightInfoAfterFinished(AuditFlowRight flowRight, List<AuditFlowRight> managerReadList)
        {
            try
            {
                //已完成流程要设置成查看权限
                AuditFlowRight auditFlowRight;
                var auditFlowRightList = await _auditFlowRightRepository.GetAllListAsync(p => p.AuditFlowId == flowRight.AuditFlowId && p.ProcessIdentifier == flowRight.ProcessIdentifier && p.UserId == flowRight.UserId);
                if (auditFlowRightList.Count > 0)
                {
                    auditFlowRight = auditFlowRightList.FirstOrDefault();
                }
                else
                {
                    auditFlowRight = new();
                    auditFlowRight.AuditFlowId = flowRight.AuditFlowId;
                    auditFlowRight.ProcessIdentifier = flowRight.ProcessIdentifier;
                    auditFlowRight.UserId = flowRight.UserId;
                }
                auditFlowRight.RightType = RIGHTTYPE.ReadOnly;
                auditFlowRight.IsRetype = false;
                await _auditFlowRightRepository.InsertOrUpdateAsync(auditFlowRight);

                //根据需求调整已完成流程对应的查看权限用户插入权限表中
                if (flowRight.ProcessIdentifier == AuditFlowConsts.AF_RequirementInput || flowRight.ProcessIdentifier == AuditFlowConsts.AF_ArchiveEnd
                    || flowRight.ProcessIdentifier == AuditFlowConsts.AF_TRAuditMKT || flowRight.ProcessIdentifier == AuditFlowConsts.AF_TRAuditR_D
                    || flowRight.ProcessIdentifier == AuditFlowConsts.AF_StructBomAudit || flowRight.ProcessIdentifier == AuditFlowConsts.AF_ElectronicBomAudit
                    || flowRight.ProcessIdentifier == AuditFlowConsts.AF_ElecBomPriceAudit || flowRight.ProcessIdentifier == AuditFlowConsts.AF_StructBomPriceAudit
                    || flowRight.ProcessIdentifier == AuditFlowConsts.AF_ElecLossRateInput || flowRight.ProcessIdentifier == AuditFlowConsts.AF_StructLossRateInput
                    || flowRight.ProcessIdentifier == AuditFlowConsts.AF_ManHourImport || flowRight.ProcessIdentifier == AuditFlowConsts.AF_LogisticsCostInput
                    || flowRight.ProcessIdentifier == AuditFlowConsts.AF_NreInputEmc || flowRight.ProcessIdentifier == AuditFlowConsts.AF_NreInputMould
                    || flowRight.ProcessIdentifier == AuditFlowConsts.AF_NreInputTest || flowRight.ProcessIdentifier == AuditFlowConsts.AF_NreInputGage)
                {
                    AuditFlowRight readAuditFlowRight;
                    long userId = _projectManager == 0 ? (await _priceEvaluationRepository.SingleAsync(p => p.AuditFlowId == flowRight.AuditFlowId)).ProjectManager : _projectManager;
                    if(managerReadList == null)
                    {
                        managerReadList = await _auditFlowRightRepository.GetAllListAsync(p => p.AuditFlowId == flowRight.AuditFlowId && p.ProcessIdentifier == flowRight.ProcessIdentifier && p.UserId == userId);
                        if (managerReadList.Count > 0)
                        {
                            readAuditFlowRight = managerReadList.FirstOrDefault();
                        }
                        else
                        {
                            readAuditFlowRight = new();
                            readAuditFlowRight.AuditFlowId = flowRight.AuditFlowId;
                            readAuditFlowRight.ProcessIdentifier = flowRight.ProcessIdentifier;
                            readAuditFlowRight.UserId = userId;
                            readAuditFlowRight.RightType = RIGHTTYPE.ReadOnly;
                            readAuditFlowRight.IsRetype = false;
                            await _auditFlowRightRepository.InsertOrUpdateAsync(readAuditFlowRight);
                            managerReadList.Add(readAuditFlowRight);
                        }
                    }
                }
            }
            catch
            {
                throw new FriendlyException("已完成流程更新权限表时获取找不到角色ID！");
            }
            return managerReadList;
        }

        /// <summary>
        /// 插入其他权限
        /// </summary>
        /// <param name="flowRight"></param>
        /// <returns></returns>
        private async Task InsertOtherRightInfoAfterFinished(AuditFlowRight flowRight)
        {
            //已完成流程设置成财务部数据管理员可查看
            AuditFlowRight auditFlowRightAdmin;
            var financeAdminList = await this.GetTheUserByRole(Host.FinanceAdmin);
            foreach (var financeAdmin in financeAdminList)
            {
                var auditFlowRightAdminList = await _auditFlowRightRepository.GetAllListAsync(p => p.AuditFlowId == flowRight.AuditFlowId && p.ProcessIdentifier == flowRight.ProcessIdentifier && p.UserId == financeAdmin.Id);
                if (auditFlowRightAdminList.Count > 0)
                {
                    auditFlowRightAdmin = auditFlowRightAdminList.FirstOrDefault();
                }
                else
                {
                    auditFlowRightAdmin = new();
                    auditFlowRightAdmin.AuditFlowId = flowRight.AuditFlowId;
                    auditFlowRightAdmin.ProcessIdentifier = flowRight.ProcessIdentifier;
                    auditFlowRightAdmin.UserId = financeAdmin.Id;
                    auditFlowRightAdmin.RightType = RIGHTTYPE.ReadOnly;
                    auditFlowRightAdmin.IsRetype = false;
                    await _auditFlowRightRepository.InsertAsync(auditFlowRightAdmin);
                }
            }
            //BOM插入后增加BOM查阅者权限
            if (flowRight.ProcessIdentifier == AuditFlowConsts.AF_StructBomImport || flowRight.ProcessIdentifier == AuditFlowConsts.AF_ElectronicBomImport)
            {
                AuditFlowRight auditFlowRightBom;
                var bomConsultantId = (await this.GetTheUserByRole(Host.BomConsultant)).FirstOrDefault().Id;
                var auditFlowRightBomList = await _auditFlowRightRepository.GetAllListAsync(p => p.AuditFlowId == flowRight.AuditFlowId && p.ProcessIdentifier == flowRight.ProcessIdentifier && p.UserId == bomConsultantId);
                if (auditFlowRightBomList.Count > 0)
                {
                    auditFlowRightBom = auditFlowRightBomList.FirstOrDefault();
                }
                else
                {
                    auditFlowRightBom = new();
                    auditFlowRightBom.AuditFlowId = flowRight.AuditFlowId;
                    auditFlowRightBom.ProcessIdentifier = flowRight.ProcessIdentifier;
                    auditFlowRightBom.UserId = bomConsultantId;
                    auditFlowRightBom.RightType = RIGHTTYPE.ReadOnly;
                    auditFlowRightBom.IsRetype = false;
                    await _auditFlowRightRepository.InsertAsync(auditFlowRightBom);
                }
                
            }
        }
        /// <summary>
        /// 插入指定权限
        /// </summary>
        /// <param name="flowRight"></param>
        /// <returns></returns>
        public async Task InsertAssignJurisdiction(AuditFlowRight flowRight, string host)
        {         
            var financeAdminList = await this.GetTheUserByRole(Host.ElectronicsPriceAuditor);

            //d查阅者权限
            if (flowRight.ProcessIdentifier == AuditFlowConsts.AF_ElecBomPriceAudit || flowRight.ProcessIdentifier == AuditFlowConsts.AF_StructBomPriceAudit)
            {
                AuditFlowRight auditFlowRightBom;

                List<User> userList = await this.GetTheUserByRole(host);
                foreach (var usr in userList)
                {
                    var auditFlowRightBomList = await _auditFlowRightRepository.GetAllListAsync(p => p.AuditFlowId == flowRight.AuditFlowId && p.ProcessIdentifier == flowRight.ProcessIdentifier && p.UserId == usr.Id);
                    if (auditFlowRightBomList.Count > 0)
                    {
                        auditFlowRightBom = auditFlowRightBomList.FirstOrDefault();
                    }
                    else
                    {
                        auditFlowRightBom = new();
                        auditFlowRightBom.AuditFlowId = flowRight.AuditFlowId;
                        auditFlowRightBom.ProcessIdentifier = flowRight.ProcessIdentifier;
                        auditFlowRightBom.UserId = usr.Id;
                        auditFlowRightBom.RightType = RIGHTTYPE.Edit;
                        auditFlowRightBom.IsRetype = false;
                        await _auditFlowRightRepository.InsertAsync(auditFlowRightBom);
                    }
                }
            }
        }
        /// <summary>
        /// 删除指定权限
        /// </summary>
        /// <param name="flowRight"></param>
        /// <returns></returns>
        public async Task DelAssignJurisdiction(AuditFlowRight flowRight, string host)
        {
            var financeAdminList = await this.GetTheUserByRole(Host.ElectronicsPriceAuditor);

            //查阅者权限
            if (flowRight.ProcessIdentifier == AuditFlowConsts.AF_ElecBomPriceAudit || flowRight.ProcessIdentifier == AuditFlowConsts.AF_StructBomPriceAudit)
            {
                AuditFlowRight auditFlowRightBom;

                List<User> userList = await this.GetTheUserByRole(host);
                foreach (var usr in userList)
                {
                    var auditFlowRightBomList = await _auditFlowRightRepository.GetAllListAsync(p => p.AuditFlowId == flowRight.AuditFlowId && p.ProcessIdentifier == flowRight.ProcessIdentifier && p.UserId == usr.Id);
                    foreach (var auditFlow in auditFlowRightBomList)
                    {
                        await _auditFlowRightRepository.HardDeleteAsync(auditFlow);
                    }
                }
            }
        }
        /// <summary>
        /// 有新流程进入时更新权限表（插入可以编辑的新流程）
        /// </summary>
        /// <param name="flowDetail"></param>
        /// <param name="isAllFinished"></param>
        /// <returns></returns>
        private async Task<ReturnDto> InsertRightInfoAfterCurrent(AuditFlowDetail flowDetail, bool isAllFinished)
        {
            ReturnDto returnDto = new();
            try
            {
                if(isAllFinished)
                {
                    //接收流程要设置可编辑或者查看
                    AuditFlowRight recvAuditFlowRight;
                    var recvAuditFlowRightList = await _auditFlowRightRepository.GetAllListAsync(p => p.AuditFlowId == flowDetail.AuditFlowId && p.ProcessIdentifier == flowDetail.ReceiveProcessIdentifier && p.UserId == flowDetail.ReceiverId);
                    if (recvAuditFlowRightList.Count > 0)
                    {
                        recvAuditFlowRight = recvAuditFlowRightList.FirstOrDefault();
                    }
                    else
                    {
                        recvAuditFlowRight = new();
                        recvAuditFlowRight.AuditFlowId = flowDetail.AuditFlowId;
                        recvAuditFlowRight.ProcessIdentifier = flowDetail.ReceiveProcessIdentifier;
                        recvAuditFlowRight.UserId = flowDetail.ReceiverId;
                    }
                    //如果是归档，则直接转成已办
                    if (recvAuditFlowRight.ProcessIdentifier == AuditFlowConsts.AF_ArchiveEnd)
                    {
                        recvAuditFlowRight.RightType = RIGHTTYPE.ReadOnly;
                        long userId = _projectManager == 0 ? (await _priceEvaluationRepository.SingleAsync(p => p.AuditFlowId == flowDetail.AuditFlowId)).ProjectManager : _projectManager;
                        await _auditFlowRightRepository.InsertOrUpdateAsync(new AuditFlowRight()
                        {
                            AuditFlowId = flowDetail.AuditFlowId,
                            ProcessIdentifier = flowDetail.ReceiveProcessIdentifier,
                            UserId = userId,
                            RightType = RIGHTTYPE.ReadOnly,
                            IsRetype = false
                        });
                    }
                    else
                    {
                        recvAuditFlowRight.RightType = RIGHTTYPE.Edit;
                        if (recvAuditFlowRight.ProcessIdentifier == AuditFlowConsts.AF_QuoteApproval)
                        {
                            await _auditFlowRightRepository.InsertOrUpdateAsync(new AuditFlowRight()
                            {
                                AuditFlowId = flowDetail.AuditFlowId,
                                ProcessIdentifier = flowDetail.ReceiveProcessIdentifier,
                                UserId = (await this.GetTheUserByRole(Host.FinanceProductCostInputter)).FirstOrDefault().Id,
                                RightType = RIGHTTYPE.ReadOnly,
                                IsRetype = false
                            });
                        }
                    }
                    recvAuditFlowRight.IsRetype = false;
                    recvAuditFlowRight.Remark = flowDetail.OpinionDescription;
                    await _auditFlowRightRepository.InsertOrUpdateAsync(recvAuditFlowRight);

                    await SendEmailToUserTest(recvAuditFlowRight.AuditFlowId, recvAuditFlowRight.ProcessIdentifier, recvAuditFlowRight.UserId);
                }
            }
            catch
            {
                throw new FriendlyException("当前流程更新权限表时获取找不到角色ID！");
            }
            return returnDto;
        }

        /// <summary>
        ///更新明细表和权限表
        /// </summary>
        /// <param name="processInfo"></param>
        /// <param name="flowDetail"></param>
        /// <param name="isAllFinished"></param>
        private async Task<ReturnDto> UpdateNextUserInfo(AuditCurrentProcess processInfo, AuditFlowDetail flowDetail, bool isAllFinished)
        {
            ReturnDto returnDto = new();
            try
            {
                if (processInfo != null)
                {
                    //使用流程明细表里的用户和接收者来控制权限，用户可以查看，接收者根据后面权限字段做判断
                    if (processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_PMInput || processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_NreInputOther
                        || processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_PriceBoardAudit)
                    {
                        flowDetail.ReceiverId = _projectManager == 0 ? (await _priceEvaluationRepository.SingleAsync(p => p.AuditFlowId == processInfo.AuditFlowId)).ProjectManager : _projectManager;

                        returnDto = await this.InsertDetailInfo(flowDetail);
                        returnDto = await this.InsertRightInfoAfterCurrent(flowDetail, isAllFinished);

                    }
                    else if (processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_TRAuditMKT)
                    {
                        flowDetail.ReceiverId = (await this.GetTheUserByRole(Host.MarketTRAuditor)).FirstOrDefault().Id;

                        returnDto = await this.InsertDetailInfo(flowDetail);
                        returnDto = await this.InsertRightInfoAfterCurrent(flowDetail, isAllFinished);
                    }
                    else if (processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_TRAuditR_D)
                    {
                        flowDetail.ReceiverId = (await this.GetTheUserByRole(Host.R_D_TRAuditor)).FirstOrDefault().Id;

                        returnDto = await this.InsertDetailInfo(flowDetail);
                        returnDto = await this.InsertRightInfoAfterCurrent(flowDetail, isAllFinished);
                    }
                    else if (processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_StructBomImport)
                    {
                        var structureEngineers = (await _userInputInfoRepository.SingleAsync(p => p.AuditFlowId == processInfo.AuditFlowId)).StructureEngineerId;

                        flowDetail.ReceiverId = structureEngineers;

                        returnDto = await this.InsertDetailInfo(flowDetail);
                        returnDto = await this.InsertRightInfoAfterCurrent(flowDetail, isAllFinished);
                    }
                    else if (processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_StructBomAudit)
                    {
                        flowDetail.ReceiverId = (await this.GetTheUserByRole(Host.StructuralBomAuditor)).FirstOrDefault().Id;

                        returnDto = await this.InsertDetailInfo(flowDetail);
                        returnDto = await this.InsertRightInfoAfterCurrent(flowDetail, isAllFinished);
                    }
                    else if (processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_StructPriceInput || processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_NreInputMould)
                    {
                        List<User> userList = await this.GetTheUserByRole(Host.StructuralPriceInputter);

                        foreach (User user in userList)
                        {
                            flowDetail.ReceiverId = user.Id;

                            returnDto = await this.InsertDetailInfo(flowDetail);
                            returnDto = await this.InsertRightInfoAfterCurrent(flowDetail, isAllFinished);
                        }
                    }
                    else if (processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_ElectronicBomImport || processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_NreInputEmc)
                    {
                        var electronicEngineers = (await _userInputInfoRepository.SingleAsync(p => p.AuditFlowId == processInfo.AuditFlowId)).ElectronicEngineerId;
                        flowDetail.ReceiverId = electronicEngineers;

                        returnDto = await this.InsertDetailInfo(flowDetail);
                        returnDto = await this.InsertRightInfoAfterCurrent(flowDetail, isAllFinished);
                    }
                    else if (processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_ElectronicBomAudit)
                    {
                        flowDetail.ReceiverId = (await this.GetTheUserByRole(Host.ElectronicsBomAuditor)).FirstOrDefault().Id;
                        returnDto = await this.InsertDetailInfo(flowDetail);
                        returnDto = await this.InsertRightInfoAfterCurrent(flowDetail, isAllFinished);
                    }
                    else if (processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_ElectronicPriceInput)
                    {
                        List<User> userList = await this.GetTheUserByRole(Host.ElectronicsPriceInputter);

                        foreach (User user in userList)
                        {
                            flowDetail.ReceiverId = user.Id;

                            returnDto = await this.InsertDetailInfo(flowDetail);
                            returnDto = await this.InsertRightInfoAfterCurrent(flowDetail, isAllFinished);
                        }
                    }
                    else if (processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_ElecBomPriceAudit)
                    {
                        flowDetail.ReceiverId = (await this.GetTheUserByRole(Host.ElectronicsPriceAuditor)).FirstOrDefault().Id;

                        returnDto = await this.InsertDetailInfo(flowDetail);
                        returnDto = await this.InsertRightInfoAfterCurrent(flowDetail, isAllFinished);
                    }
                    else if (processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_StructBomPriceAudit)
                    {
                        flowDetail.ReceiverId = (await this.GetTheUserByRole(Host.StructuralPriceAuditor)).FirstOrDefault().Id;

                        returnDto = await this.InsertDetailInfo(flowDetail);
                        returnDto = await this.InsertRightInfoAfterCurrent(flowDetail, isAllFinished);
                    }
                    else if (processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_ElecLossRateInput || processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_StructLossRateInput)
                    {
                        var engineers = (await _userInputInfoRepository.SingleAsync(p => p.AuditFlowId == processInfo.AuditFlowId)).EngineerLossRateId;
                        flowDetail.ReceiverId = engineers;

                        returnDto = await this.InsertDetailInfo(flowDetail);
                        returnDto = await this.InsertRightInfoAfterCurrent(flowDetail, isAllFinished);
                    }
                    else if (processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_ManHourImport)
                    {
                        var engineers = (await _userInputInfoRepository.SingleAsync(p => p.AuditFlowId == processInfo.AuditFlowId)).EngineerWorkHourId;
                        flowDetail.ReceiverId = engineers;

                        returnDto = await this.InsertDetailInfo(flowDetail);
                        returnDto = await this.InsertRightInfoAfterCurrent(flowDetail, isAllFinished);
                    }
                    else if (processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_ProductionCostInput)
                    {
                        flowDetail.ReceiverId = (await this.GetTheUserByRole(Host.FinanceProductCostInputter)).FirstOrDefault().Id;

                        returnDto = await this.InsertDetailInfo(flowDetail);
                        returnDto = await this.InsertRightInfoAfterCurrent(flowDetail, isAllFinished);
                    }
                    else if (processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_LogisticsCostInput)
                    {
                        var engineers = (await _userInputInfoRepository.SingleAsync(p => p.AuditFlowId == processInfo.AuditFlowId)).ProductManageId;
                        flowDetail.ReceiverId = engineers;

                        returnDto = await this.InsertDetailInfo(flowDetail);
                        returnDto = await this.InsertRightInfoAfterCurrent(flowDetail, isAllFinished);
                    }
                    else if (processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_NreInputTest)
                    {
                        var engineers = (await _userInputInfoRepository.SingleAsync(p => p.AuditFlowId == processInfo.AuditFlowId)).QualityBenchId;
                        flowDetail.ReceiverId = engineers;

                        returnDto = await this.InsertDetailInfo(flowDetail);
                        returnDto = await this.InsertRightInfoAfterCurrent(flowDetail, isAllFinished);
                    }
                    else if (processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_NreInputGage)
                    {
                        var engineers = (await _userInputInfoRepository.SingleAsync(p => p.AuditFlowId == processInfo.AuditFlowId)).QualityToolId;
                        flowDetail.ReceiverId = engineers;

                        returnDto = await this.InsertDetailInfo(flowDetail);
                        returnDto = await this.InsertRightInfoAfterCurrent(flowDetail, isAllFinished);
                    }
                    else if (processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_TradeApproval)
                    {
                        flowDetail.ReceiverId = (await this.GetTheUserByRole(Host.TradeComplianceAuditor)).FirstOrDefault().Id;

                        returnDto = await this.InsertDetailInfo(flowDetail);
                        returnDto = await this.InsertRightInfoAfterCurrent(flowDetail, isAllFinished);
                    }
                    else if (processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_ProjectPriceAudit)
                    {
                        var projectAuditor = (await _userInputInfoRepository.SingleAsync(p => p.AuditFlowId == processInfo.AuditFlowId)).ProjectAuditorId;
                        if(projectAuditor == 0)
                        {//如果项目审核员没有指定，则选择角色指定人员，兼容旧版本流程
                            flowDetail.ReceiverId = (await this.GetTheUserByRole(Host.ProjectPriceAuditor)).FirstOrDefault().Id;
                        }
                        else
                        {
                            flowDetail.ReceiverId = projectAuditor;
                        }
                        
                        returnDto = await this.InsertDetailInfo(flowDetail);
                        returnDto = await this.InsertRightInfoAfterCurrent(flowDetail, isAllFinished);
                    }
                    else if (processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_FinancePriceAudit)
                    {
                        flowDetail.ReceiverId = (await this.GetTheUserByRole(Host.FinancePriceAuditor)).FirstOrDefault().Id;

                        returnDto = await this.InsertDetailInfo(flowDetail);
                        returnDto = await this.InsertRightInfoAfterCurrent(flowDetail, isAllFinished);
                    }
                    else if (processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_CostCheckNreFactor || processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_ArchiveEnd)
                    {
                        //拟稿人工号获取
                        var drafterNumber = (await _priceEvaluationRepository.SingleAsync(p => p.AuditFlowId == processInfo.AuditFlowId)).DrafterNumber;
                        flowDetail.ReceiverId = (await _userRepository.SingleAsync(p => p.Number == drafterNumber)).Id;

                        returnDto = await this.InsertDetailInfo(flowDetail);
                        returnDto = await this.InsertRightInfoAfterCurrent(flowDetail, isAllFinished);
                    }
                    else if (processInfo.FlowProcessIdentifier == AuditFlowConsts.AF_QuoteApproval)
                    {
                        flowDetail.ReceiverId = (await this.GetTheUserByRole(Host.GeneralManager)).FirstOrDefault().Id;

                        returnDto = await this.InsertDetailInfo(flowDetail);
                        returnDto = await this.InsertRightInfoAfterCurrent(flowDetail, isAllFinished);
                    }
                }
            }
            catch
            {
                throw new FriendlyException("获取相应流程角色信息失败！");
            }
            return returnDto;
        }
        private async Task<List<User>> GetTheUserByRole(string roleName)
        {

            List<User> userList = (from a in await _roleRepository.GetAllListAsync(p => p.Name.Equals(roleName))
                                   join b in await _userRoleRepository.GetAllListAsync() on a.Id equals b.RoleId
                                   join c in await _userRepository.GetAllListAsync() on b.UserId equals c.Id
                                   select new User
                                   {
                                       Id = c.Id
                                   }).ToList();
            if (userList.Count > 0)
            {
                return userList;
            }
            else
            {
                throw new FriendlyException("找不到角色：" + roleName);
            }
        }
        /// <summary>
        /// 设置核价流程有效
        /// </summary>
        /// <param name="auditFlowValidDto"></param>
        /// <returns></returns>
        public async virtual Task<ReturnDto> SetAuditFlowValid(AuditFlowValidDto auditFlowValidDto)
        {
            ReturnDto returnDto = new();
            var auditFlowInfos = await _auditFlowRepository.GetAllListAsync(p => p.Id == auditFlowValidDto.AuditFlowId);

            if (auditFlowInfos.Count > 0)
            {
                var auditFlow = auditFlowInfos.FirstOrDefault();
                auditFlow.IsValid = auditFlowValidDto.IsValid;
                await _auditFlowRepository.UpdateAsync(auditFlow);
            }
            else
            {
                throw new FriendlyException("获取流程表信息失败，查不到对应流程！");
            }
            return returnDto;
        }

        /// <summary>
        /// 获取审批流程主表记录信息
        /// </summary>
        public async virtual Task<AuditFlowRetDto> GetAuditFlowInfoById(long flowId)
        {
            AuditFlowRetDto auditFlowRetDto = new AuditFlowRetDto();
            var auditFlowInfos = await _auditFlowRepository.GetAllListAsync(p => p.Id == flowId);
            if (auditFlowInfos.Count > 0)
            {
                auditFlowRetDto.AuditFlowList = new();
                auditFlowRetDto.AuditFlowList.Add(auditFlowInfos.FirstOrDefault());
            }
            else
            {
                throw new FriendlyException("获取流程表信息失败，查不到对应流程！");
            }
            return auditFlowRetDto;
        }

        /// <summary>
        /// 获取所有流程信息
        /// </summary>
        /// <returns></returns>
        public async virtual Task<List<FlowProcessDto>> GetFlowProcessInfo()
        {
            var flowProcessList = await _flowProcessRepository.GetAllListAsync();
            List<FlowProcessDto> flowProcessListDto = ObjectMapper.Map<List<FlowProcessDto>>(flowProcessList);
            return flowProcessListDto;
        }

        /// <summary>
        /// 获取所有流程跳转信息
        /// </summary>
        /// <returns></returns>
        public async virtual Task<List<FlowJumpInfoDto>> GetFlowJumpInfo()
        {
            var flowJumpInfoList = await _flowJumpInfoRepository.GetAllListAsync();
            List<FlowJumpInfoDto> flowJumpInfoListDto = ObjectMapper.Map<List<FlowJumpInfoDto>>(flowJumpInfoList);
            return flowJumpInfoListDto;
        }

        /// <summary>
        /// 获取流程删除记录表的数据
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        public async virtual Task<List<AuditFlowDelete>> GetAuditFlowDeleteList(long? auditFlowId)
        {
            if(auditFlowId == null)
            {
                var auditFlowDelete = await _auditFlowDeleteRepository.GetAllListAsync();
                return auditFlowDelete;
            }
            else
            {
                var auditFlowDelete = await _auditFlowDeleteRepository.GetAllListAsync(p=>p.AuditFlowId == auditFlowId);
                if(auditFlowDelete.Count > 0)
                {
                    return auditFlowDelete;
                }
                else
                {
                    throw new FriendlyException("没有该流程ID的删除记录！");
                }
            }
        }

        /// <summary>
        /// 删除指定流程Id的流程
        /// </summary>
        /// <param name="auditFlowDeleteDto"></param>
        /// <returns></returns>
        public async virtual Task DeleteAuditFlowById(AuditFlowDeleteDto auditFlowDeleteDto)
        {
            AuditFlowDelete auditFlowDelete = new();
            auditFlowDelete.AuditFlowId = auditFlowDeleteDto.AuditFlowId;
            auditFlowDelete.DeleteReason = auditFlowDeleteDto.DeleteReason;

            PriceEvaluation priceEvaluation = await _priceEvaluationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowDeleteDto.AuditFlowId);
            if (priceEvaluation != null)
            {
                auditFlowDelete.AuditFlowName = priceEvaluation.Title;
                User user = await _userRepository.FirstOrDefaultAsync(p=>p.Name.Equals(priceEvaluation.Drafter));
                if(user != null)
                {
                    auditFlowDelete.AuditFlowUserId = user.Id;
                }
                if(AbpSession.UserId != null)
                {
                    auditFlowDelete.AuditFlowDeleterId = (long)AbpSession.UserId;
                }
            }

            var auditFlowList = await _auditFlowRepository.GetAllListAsync(p=>p.Id == auditFlowDeleteDto.AuditFlowId);
            if(auditFlowList.Count > 0)
            {
                auditFlowDelete.AuditFlowVersion = auditFlowList.FirstOrDefault().QuoteVersion;
                await _auditFlowRepository.HardDeleteAsync(auditFlowList.FirstOrDefault());
            }
            else
            {
                throw new FriendlyException("流程已删除！");
            }
            
            var auditFlowCurrentList = await _auditCurrentProcessRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowDeleteDto.AuditFlowId);
            foreach(var auditFlowCurrent in auditFlowCurrentList)
            {
                await _auditCurrentProcessRepository.HardDeleteAsync(auditFlowCurrent);
            }

            var auditFlowFinishedList = await _auditFinishedProcessRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowDeleteDto.AuditFlowId);
            foreach (var auditFinished in auditFlowFinishedList)
            {
                await _auditFinishedProcessRepository.HardDeleteAsync(auditFinished);
            }

            var auditFlowDetailList = await _auditFlowDetailRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowDeleteDto.AuditFlowId);
            foreach (var auditFlowDetail in auditFlowDetailList)
            {
                await _auditFlowDetailRepository.HardDeleteAsync(auditFlowDetail);
            }

            var auditFlowRightList = await _auditFlowRightRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowDeleteDto.AuditFlowId);
            foreach (var auditFlowRight in auditFlowRightList)
            {
                await _auditFlowRightRepository.HardDeleteAsync(auditFlowRight);
            }

            await _auditFlowDeleteRepository.InsertAsync(auditFlowDelete);
        }

        /// <summary>
        /// 根据用户ID删除对应的流程权限（用于用户角色变化时删除掉）
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async virtual Task DeleteFlowRightByUserId(long UserId)
        {
            //获得完成核价的流程List
            var auditFlowList = await _auditFlowRepository.GetAllListAsync(p => p.IsValid == true);
            //取完成核价流程的Id
            var auditFlowIdList = (from e in auditFlowList.Select(p => p.Id).Distinct() select e).ToList();
            //查看用户在权限表中未完成节点的节点权限
            var auditFlowRightList = await _auditFlowRightRepository.GetAllListAsync(p => p.UserId == UserId && !auditFlowIdList.Contains(p.AuditFlowId));

            foreach (var auditFlowRight in auditFlowRightList)
            {
                await _auditFlowRightRepository.HardDeleteAsync(auditFlowRight);
            }
        }

        /// <summary>
        /// 发送邮件接口
        /// </summary>
        /// <returns></returns>
        public async virtual Task SendEmailToUserTest(long flowId, string processIdentifier, long userId )
        {
            string flowTitle = null;

            var flowProcessDetail = await _flowProcessRepository.FirstOrDefaultAsync(p => p.ProcessIdentifier == processIdentifier);
            if (flowProcessDetail == null)
            {
                return;
            }
            string flowNodeName = flowProcessDetail.ProcessName;

            var userInfo = await _userRepository.FirstOrDefaultAsync(p => p.Id == userId);
            if (userInfo == null)
            {
                return;
            }
            string emailAddr = userInfo.EmailAddress;

            var priceEvaluations = await _priceEvaluationRepository.GetAllListAsync(p => p.AuditFlowId == flowId);
            if (priceEvaluations.Count > 0)
            {
                flowTitle = priceEvaluations.FirstOrDefault().Title;
            }
            else
            {
                throw new FriendlyException("找不到对应核价需求信息！");
            }
            long projectManagerId = _projectManager == 0 ? priceEvaluations.FirstOrDefault().ProjectManager : _projectManager;
            User usrInfo = await _userRepository.FirstOrDefaultAsync(p => p.Id == projectManagerId);
            if (usrInfo == null)
            {
                return;
            }
            string quoteType = priceEvaluations.FirstOrDefault().QuotationType;
            string quoteTypeName = _financeDictionaryDetailRepository.FirstOrDefault(p => p.Id == quoteType).DisplayName; 
            
            var emailInfoList = await _noticeEmailInfoRepository.GetAllListAsync();
            SendEmail email = new SendEmail();
            string loginIp = email.GetLoginAddr();
            string loginAddr = "http://" + (loginIp.Equals(FinanceConsts.AliServer_In_IP) ? FinanceConsts.AliServer_Out_IP : loginIp) + ":8080/login";
            string emailBody = "核价报价提醒：您有新的工作流（" + flowNodeName + "，项目经理：" + usrInfo.Name + "，报价形式：" + quoteTypeName + "）需要完成（" + "<a href=\"" + loginAddr + "\" >系统地址</a>" + "）";
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
            Task.Run(async () => {
                await email.SendEmailToUser(loginIp.Equals(FinanceConsts.AliServer_In_IP), flowTitle, emailBody, emailAddr, emailInfoList.Count == 0? null: emailInfoList.FirstOrDefault());
            });
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
        }

        /// <summary>
        /// 获取系统中通知邮件信息
        /// </summary>
        /// <returns></returns>
        public async virtual Task<EmailDto> GetEmailInfo()
        {
            var emailInfoList = await _noticeEmailInfoRepository.FirstOrDefaultAsync(p=>!p.IsDeleted);
            
            return ObjectMapper.Map<EmailDto>(emailInfoList); 
        
        }

        /// <summary>
        /// 修改系统通知邮箱相关信息
        /// </summary>
        /// <param name="emailDto"></param>
        /// <returns></returns>
        public async virtual Task ChangeEmailInfo(EmailDto emailDto)
        {
            var emailInfoList = await _noticeEmailInfoRepository.FirstOrDefaultAsync(p=>p.Id.Equals(emailDto.Id));
            if (emailInfoList is not null)
            {              
                emailInfoList.EmailPassword = emailDto.EmailPassword;
                emailInfoList.MaintainerEmail = emailDto.MaintainerEmail;
                await _noticeEmailInfoRepository.UpdateAsync(emailInfoList);
            }
            else
            {
                NoticeEmailInfo noticeEmailInfo = new()
                {
                    EmailAddress = FinanceConsts.MailFrom_Sunny,
                    EmailPassword = emailDto.EmailPassword,
                    MaintainerEmail = emailDto.MaintainerEmail
                };
                await _noticeEmailInfoRepository.InsertAsync(noticeEmailInfo);
            }
        }
    }
}
