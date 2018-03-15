using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Services.DTOs.Explorer;

namespace PlatformTM.Services.Services
{
    public class CacheService
    {
        private readonly IServiceUoW _dataServiceUnit;
        private readonly ICacheRepository<ClinicalExplorerDTO> _cacheRepository;
        private readonly DataExplorerService _explorerService;
        public CacheService(IServiceUoW uoW, DataExplorerService explorerService)
        {
            _dataServiceUnit = uoW;
            _cacheRepository = uoW.GetCacheRepository<ClinicalExplorerDTO>();
            _explorerService = explorerService;
        }

        public  ClinicalExplorerDTO GetClinicalTreeDTO<TEntity>(Expression<Func<ClinicalExplorerDTO, bool>> filter)
        {
            return _cacheRepository.GetFromCache(filter);
        }

        public async Task GenerateAndCacheClinicalDTO(int projectId)
        {
            _cacheRepository.RemoveFromCache(c=> c.ProjectId==projectId);
            var cTree = await _explorerService.GetClinicalObsTree(projectId);
            cTree.Id = Guid.NewGuid();
            _cacheRepository.Save(cTree);
        }
    }
}
