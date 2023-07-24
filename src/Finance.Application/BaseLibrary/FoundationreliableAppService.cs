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
    public class FoundationreliableAppService : ApplicationService
    {
        private readonly IRepository<Foundationreliable, long> _foundationreliableRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationreliableRepository"></param>
        public FoundationreliableAppService(
            IRepository<Foundationreliable, long> foundationreliableRepository)
        {
            _foundationreliableRepository = foundationreliableRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationreliableDto> GetAsyncById(long id)
        {
            Foundationreliable entity = await _foundationreliableRepository.GetAsync(id);

            return ObjectMapper.Map<Foundationreliable, FoundationreliableDto>(entity, new FoundationreliableDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationreliableDto>> GetListAsync(GetFoundationreliablesInput input)
        {
            // 设置查询条件
            var query = this._foundationreliableRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<Foundationreliable>, List<FoundationreliableDto>>(list, new List<FoundationreliableDto>());
            // 数据返回
            return new PagedResultDto<FoundationreliableDto>(totalCount, dtos);
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<FoundationreliableDto>> GetListAllByNameAsync(GetFoundationreliablesInput input)
        {
            // 设置查询条件
            var query = this._foundationreliableRepository.GetAll().Where(t => t.IsDeleted == false);
            if (!string.IsNullOrEmpty(input.Name))
            {
                query = query.Where(t => t.Name.Contains(input.Name));
            }
            if (!string.IsNullOrEmpty(input.Classification))
            {
                query = query.Where(t => t.Classification.Contains(input.Classification));
            }
            // 查询数据
            var list = query.ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<Foundationreliable>, List<FoundationreliableDto>>(list, new List<FoundationreliableDto>());
            // 数据返回
            return dtos;
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationreliableDto> GetEditorAsyncById(long id)
        {
            Foundationreliable entity = await _foundationreliableRepository.GetAsync(id);

            return ObjectMapper.Map<Foundationreliable, FoundationreliableDto>(entity, new FoundationreliableDto());
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationreliableDto> CreateAsync(FoundationreliableDto input)
        {
            var entity = ObjectMapper.Map<FoundationreliableDto, Foundationreliable>(input, new Foundationreliable());
            var maxId = this._foundationreliableRepository.GetAll().Max(t => t.Id);
            input.CreationTime = DateTime.Now;
            input.Id = maxId + 1;
            if (AbpSession.UserId != null)
            {
                input.CreatorUserId = AbpSession.UserId.Value;
            }
            entity = await _foundationreliableRepository.InsertAsync(entity);
            return ObjectMapper.Map<Foundationreliable, FoundationreliableDto>(entity, new FoundationreliableDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationreliableDto> UpdateAsync(FoundationreliableDto input)
        {
            Foundationreliable entity = await _foundationreliableRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map<FoundationreliableDto, Foundationreliable>(input, entity);
            entity.LastModificationTime = DateTime.Now;
            if (AbpSession.UserId != null)
            {
                entity.LastModifierUserId = AbpSession.UserId.Value;
            }
            entity = await _foundationreliableRepository.UpdateAsync(entity);
            return ObjectMapper.Map<Foundationreliable, FoundationreliableDto>(entity, new FoundationreliableDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationreliableRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
