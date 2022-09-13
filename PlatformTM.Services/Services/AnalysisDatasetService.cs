using System;
using System.Collections.Generic;
using System.Linq;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.Users.Datasets;
using PlatformTM.Models.DTOs;
using PlatformTM.Models.DTOs.Export;

namespace PlatformTM.Models.Services
{
    public class AnalysisDatasetService
    {
		private readonly IRepository<AnalysisDataset, Guid> _analysisDatasetRepository;
		private readonly IRepository<ExportFile, Guid> _exportFileRepository;
        private readonly IServiceUoW _unitOfWork;

		public AnalysisDatasetService(IServiceUoW uoW)
		{
			_unitOfWork = uoW;
			_analysisDatasetRepository = uoW.GetRepository<AnalysisDataset, Guid>();
			_exportFileRepository = uoW.GetRepository<ExportFile, Guid>();		
		}

		public List<AnalysisDatasetDTO> GetUserDatasets(string userId)
        {
			List<AnalysisDataset> datasets = _analysisDatasetRepository.FindAll(
                d => d.OwnerId == userId).ToList();
			return datasets.Select(WriteDTO).ToList();
        }

		public AnalysisDatasetDTO GetUserDataset(string datasetId, string userId)
        {
			var userDataset = _analysisDatasetRepository.FindSingle(d => d.Id == Guid.Parse(datasetId));
            var dto = WriteDTO(userDataset);
            return dto;
        }
        
		private AnalysisDatasetDTO WriteDTO(AnalysisDataset d)
		{
			List<ExportFile> files = new List<ExportFile>();
			foreach(var fid in d.FileIds){
				files.Add(_exportFileRepository.FindSingle(a => a.Id==Guid.Parse(fid)));
			}
			var dto = new AnalysisDatasetDTO()
			{
				Id = d.Id,
				Name = d.Name,
				Description = d.Description,
				OwnerId = d.OwnerId,
				ProjectId = d.ProjectId,
				Files = files.Select(f => new ExportFileDTO()
				{
					Id = f.Id.ToString(),
					FileStatus = f.FileStatus,
					ContentType = f.ContentType,
					Name = f.Name,
					SampleCount = f.SampleCount,
					SubjectCount = f.SubjectCount,
					QueryId = f.QueryId.ToString()
				}).ToList()
			};
			return dto;
		}

		public void UpdateDataset(AnalysisDatasetDTO dataset, string userId)
        {
            //check that the owner of this dataset is the caller
            //var dataset = ReadDTO(dto);
			var datasetToUpdate = _analysisDatasetRepository.FindSingle(d => d.Id == dataset.Id);
            //datasetToUpdate.LastModified = DateTime.Today.ToString("f");
            //datasetToUpdate.Description = dataset.Description;
            datasetToUpdate.Name = dataset.Name;
			_analysisDatasetRepository.Update(datasetToUpdate);
        }

		public void DeleteDataset(string datasetId)
        {
			_analysisDatasetRepository.DeleteMany(d => d.Id == Guid.Parse(datasetId));
        }      
    }
}
