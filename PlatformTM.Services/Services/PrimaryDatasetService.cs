﻿using System;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS.DatasetDescriptorTypes;
using PlatformTM.Core.Domain.Model.DatasetModel;
using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS;
using System.Linq;
using PlatformTM.Services.DTOs;

namespace PlatformTM.Models.Services
{
    public class PrimaryDatasetService
    {
        private IServiceUoW _dataContext;
        private readonly IRepository<PrimaryDataset, int> _pdsRepository;
        private readonly IRepository<ObservationDatasetDescriptor, Guid> _datasetDescriptorRepository;
        public PrimaryDatasetService(IServiceUoW _uoW)
        {
            _dataContext = _uoW;
            _pdsRepository = _dataContext.GetRepository<PrimaryDataset, int>();
            _datasetDescriptorRepository = _dataContext.GetRepository<ObservationDatasetDescriptor, Guid>();
        }

       

        public List<PrimaryDatasetDTO> GetPrimaryDatasetsForProject(int projectId)
        {
            IEnumerable<PrimaryDataset> primaryDatasets;
            primaryDatasets = _pdsRepository.FindAll(d => d.ProjectId == projectId, new List<string>() { "Project", "Studies"} );
            return primaryDatasets.Select(s=>WriteToDTO(s,false)).ToList();
        }

        public PrimaryDatasetDTO GetPrimaryDatasetInfo(int datasetId)
        {
            var primaryDataset = _pdsRepository.FindSingle(d => d.Id == datasetId, new List<string>() { "Project", "Studies" });

            if (primaryDataset == null)
                return null;
            return WriteToDTO(primaryDataset, true);
        }

        public PrimaryDataset AddPrimaryDatasetInfo(PrimaryDatasetDTO dto)
        {
            var newPDS = ReadFromDTO(dto, new PrimaryDataset()
            {
                ProjectId = dto.ProjectId,
                Created = DateTime.Now.ToString("D"),
                DescriptorId = Guid.Parse(dto.DescriptorId),
                Version = "v1.0"
            });

            newPDS = _pdsRepository.Insert(newPDS);
            return (_dataContext.Save().Equals("CREATED")) ? newPDS : null;
        }

        public PrimaryDataset UpdatePrimaryDatasetInfo(PrimaryDatasetDTO primaryDatasetDTO)
        {
            var primaryDataset = _pdsRepository.FindSingle(d => d.Id == primaryDatasetDTO.Id);

            primaryDataset = ReadFromDTO(primaryDatasetDTO, primaryDataset);
            _pdsRepository.Update(primaryDataset);
            return _dataContext.Save().Equals("CREATED") ? primaryDataset : null;
        }

     

        private PrimaryDatasetDTO WriteToDTO(PrimaryDataset dataset, bool addDescriptor)
        {
            PrimaryDatasetDTO datasetDTO = null;
            if (dataset != null)
            {
                datasetDTO = new PrimaryDatasetDTO()
                {
                    Id = dataset.Id,
                    Title = dataset.Title,
                    Description = dataset.Description,
                    Acronym = dataset.Acronym,
                    Domain = dataset.Domain,
                    ProjectName = dataset.Project.Name,
                    StudyNames = dataset.Studies.Select(s=>s.Description).ToList(),
                    StudyAccronyms = dataset.Studies.Select(s=> s.Name).ToList(),
                    DateCreated = dataset.Created,
                    DateModified = dataset.Modified,
                    

                    //DatasetType = dataset.Descriptor.DatasetType.ToString()
                };

                if (addDescriptor)
                {
                    var dd = _datasetDescriptorRepository.FindSingle(d => d.Id == dataset.DescriptorId);
                    var dto = new DatasetDescriptorDTO(dd);
                    datasetDTO.Descriptor = dto;
                }
            }
            return datasetDTO;
        }

        private PrimaryDataset ReadFromDTO(PrimaryDatasetDTO dto, PrimaryDataset pds)
        {
            pds.Title = dto.Title;
            pds.Description = dto.Description;
            pds.Acronym = dto.Acronym;
            pds.Domain = dto.Domain;
            pds.Modified = DateTime.Now.ToString("D");
            pds.Version = dto.Version;


            //if (dto.AssociatedDatasets != null && dto.AssociatedDatasets.Count != 0)
            //{
            //    var selectedDSids = dto.AssociatedDatasets.Where(a => a.IsSelected).ToList().Select(a => a.Id);
            //    //var unselectedDSids = dto.AssociatedDatasets.Where(a => !a.IsSelected).ToList().Select(a => a.Id);

            //    var selectedDatasets = _pdsRepository.FindAll(s => selectedDSids.Contains(s.Id));
            //    if (selectedDatasets == null)
            //        return null;

            //    if (assessment.Datasets.Count != 0)
            //        ((List<PrimaryDataset>)assessment.Datasets).Clear();

            //    ((List<PrimaryDataset>)assessment.Datasets).AddRange(selectedDatasets.ToList());
            //}
            return pds;
        }

        public List<PrimaryDatasetDTO> GetPrimaryDatasetsForStudy(int studyId)
        {
            var studyDatasets = _pdsRepository.FindAll(d => d.Studies.Any(s => s.Id == studyId)).ToList();

            return studyDatasets.Select(s => WriteToDTO(s,false)).ToList();

        }
        
    }


}
