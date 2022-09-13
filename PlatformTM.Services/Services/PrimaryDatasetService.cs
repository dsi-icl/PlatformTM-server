using System;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model.DatasetDescriptorTypes;
using PlatformTM.Core.Domain.Model.DatasetModel;

namespace PlatformTM.Models.Services
{
    public class PrimaryDatasetService
    {
        private IServiceUoW uoW;
        public PrimaryDatasetService(IServiceUoW _uoW)
        {
            uoW = _uoW;
        }

        public void ImportDataToPDS(DataFile dataFile, DatasetDescriptor datasetDescriptor)
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
