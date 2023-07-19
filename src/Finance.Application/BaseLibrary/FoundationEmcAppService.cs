using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 管理
    /// </summary>
    public class FoundationEmcAppService : ApplicationService
    {
        private readonly IRepository<FoundationEmc, long> _foundationEmcRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationEmcRepository"></param>
        public FoundationEmcAppService(
            IRepository<FoundationEmc, long> foundationEmcRepository)
        {
            _foundationEmcRepository = foundationEmcRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationEmcDto> GetAsyncById(long id)
        {
            FoundationEmc entity = await _foundationEmcRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationEmc, FoundationEmcDto>(entity,new FoundationEmcDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationEmcDto>> GetListAsync(GetFoundationEmcsInput input)
        {
            // 设置查询条件
            var query = this._foundationEmcRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationEmc>, List<FoundationEmcDto>>(list, new List<FoundationEmcDto>());
            // 数据返回
            return new PagedResultDto<FoundationEmcDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationEmcDto> GetEditorAsyncById(long id)
        {
            FoundationEmc entity = await _foundationEmcRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationEmc, FoundationEmcDto>(entity,new FoundationEmcDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationEmcDto> CreateAsync(FoundationEmcDto input)
        {
            var entity = ObjectMapper.Map<FoundationEmcDto, FoundationEmc>(input,new FoundationEmc());
            entity = await _foundationEmcRepository.InsertAsync(entity);
            return ObjectMapper.Map<FoundationEmc, FoundationEmcDto>(entity,new FoundationEmcDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationEmcDto> UpdateAsync(FoundationEmcDto input)
        {
            FoundationEmc entity = await _foundationEmcRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _foundationEmcRepository.UpdateAsync(entity);
            return ObjectMapper.Map<FoundationEmc, FoundationEmcDto>(entity,new FoundationEmcDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationEmcRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
