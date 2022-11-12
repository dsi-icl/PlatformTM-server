using System;
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
        private IServiceUoW uoW;
        private readonly IRepository<PrimaryDataset, int> _pdsRepository;
        public PrimaryDatasetService(IServiceUoW _uoW)
        {
            uoW = _uoW;
            _pdsRepository = uoW.GetRepository<PrimaryDataset, int>();
        }

        public void ImportDataToPDS(DataFile dataFile, DatasetDescriptor datasetDescriptor)
        {

        }

        public List<PrimaryDatasetDTO> GetPrimaryDatasetsForProject(int studyId)
        {
            IEnumerable<PrimaryDataset> primaryDatasets;

            primaryDatasets = _pdsRepository.FindAll(d => d.StudyId == studyId, new List<string>() { "Study" });

            return primaryDatasets.Select(d=> new PrimaryDatasetDTO()
            {
                Title = d.Title,
                Description = d.Description,
                ProjectName = d.Study.Project.Name,
                StudyName = d.Study.Name,
                StudyAccronym = d.Study.Accession,
                DatasetType = d.Descriptor.DatasetType.ToString()
            }).ToList();
        }

        public PrimaryDatasetDTO GetPrimaryDatasetInfo(int datasetId)
        {

        }

        public void AddPrimaryDatasetInfo()
        {

        }

        public void UpdatePrimaryDatasetInfo()
        {

        }
        //public List<DatasetVM> GetDatasets(int projectId)
        //{
        //    IEnumerable<Activity> Activities;
        //    Activities = _activityRepository.FindAll(
        //        d => (d.ProjectId == projectId && (d is Activity || d is SubjectRecording)),
        //            new List<string>(){
        //                "Datasets.Template",
        //                "Datasets.DataFiles.Datafile"
        //            }
        //        ).ToList();
        //    var datasets = Activities.SelectMany(a => a.Datasets).Select(d => new DatasetVM()
        //    {
        //        Id = d.Id,
        //        Name = d.Template.Domain,
        //        Files = d.DataFiles.Select(df => new FileVM()
        //        {
        //            Id = df.DatafileId,
        //            FileName = df.Datafile.FileName,
        //            DataType = df.Datafile.DataType,
        //            DateLastModified = df.Datafile.LastModified
        //        }).ToList()
        //    }).ToList();

        //    return datasets;
        //}
    }


}
