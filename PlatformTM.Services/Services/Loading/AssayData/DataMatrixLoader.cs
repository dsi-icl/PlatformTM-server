using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Core.Domain.Model.ObservationModel;
using PlatformTM.Services.DTOs;
using Observation = PlatformTM.Core.Domain.Model.ObservationModel.Observation;

namespace PlatformTM.Services.Services.Loading.AssayData
{
    public class DataMatrixLoader
    {

        private IServiceUoW _dataContext;
        private readonly IRepository<Observation, Guid> _observationRepository;
        private readonly IRepository<Biosample, int> _biosampleRepository;
        private readonly IRepository<DataFile, int> _dataFileRepository;
        private readonly IRepository<Dataset, int> _datasetRepository;
        private readonly FileService _fileService;


        public DataMatrixLoader(IServiceUoW uoW, FileService fileService) 
        {
            _dataContext = uoW;
            _observationRepository = uoW.GetRepository<Observation, Guid>();
            _biosampleRepository = uoW.GetRepository<Biosample, int>();
           _dataFileRepository = uoW.GetRepository<DataFile, int>();
            _datasetRepository = uoW.GetRepository<Dataset, int>();
           _fileService = fileService;
        }

        private Dataset GetActivityDataset(int datasetId)
        {
            Dataset ds = _datasetRepository.FindSingle(
                d => d.Id.Equals(datasetId),
                new List<string>()
                {
                    "Variables.VariableDefinition",
                    "DataFiles",
                    "Activity.Project",
                    "Template"
                });
            return ds;
        }
        // dataset service 
        public bool LoadHDdDdata(int datasetId, int fileId/*, int referencFromHdId*/)      
        {
            _observationRepository.DeleteMany(d=> d.DatasetId == datasetId);
            var dataset = GetActivityDataset(datasetId);
            var dataFile = _dataFileRepository.Get(fileId); 
            var filePath = dataFile.Path + "\\" + dataFile.FileName;

            var dataTable = _fileService.ReadOriginalFile(filePath);

            var sampleList = _biosampleRepository.FindAll(s => s.DatasetId == datasetId).ToList();
            var scos = sampleList.ToDictionary(co => co.BiosampleStudyId);

            var obsReadyToInsert = new List<Observation>();
            
            foreach (DataRow row in dataTable.Rows)
            {
                for (int index = 0; index < dataTable.Columns.Count; index++)
                    if (index == 0) continue;
                    else
                    {
                        var column = dataTable.Columns[index];
                        var obs = new Observation();
                        // Fill an Observation
                            var value = new NumericalValue();
                            value.Value = double.Parse(row[column.ColumnName].ToString());
                            value.Property = new PropertyDescriptor();
                            {
                                value.Property.Description = dataset.Template.Description;
                                value.Property.ObsClass = dataset.Template.Class;
                                value.Property.Name = dataset.TemplateId;
                            }
                            //   value.Unit = ??
                            obs.ObservedValue = value;                                                         // Observed Value which includes in it the VALUE and its PROPERTY

                            // ****** FOR NOW WE USE THE FOLLOWING TILL WE HAVE THE FEATURE referencFromHdId ******
                            obs.SubjectOfObservationName = column.ColumnName;                                  // sample name (HERE IS THE FILE NAME)
                            //obs.SubjectOfObservationId = column.ColumnName;                                    // sampleID     (here is the file name)
                            // ****** 

                            //   obs.SubjectOfObservationName = scos[column.ColumnName].BiosampleStudyId;      // sample Name
                            //   obs.SubjectOfObservationId = scos[column.ColumnName].Id.ToString();           // sample ID
                            //   obs.StudyId = scos[column.ColumnName].StudyId;                                // study ID
                            obs.FeatureName = row[0].ToString();                                               // feature name 
                            // obs.FeatureId =;  Not implemented yet                                           // feature ID
                            obs.DatafileId = dataFile.Id;
                            obs.DatasetId = datasetId;
                            obs.ActivityId = dataset.ActivityId;
                            obs.ProjectId = dataset.Activity.ProjectId;
                            obs.Id = Guid.NewGuid();
                            
                        obsReadyToInsert.Add(obs);
                     //   observationsTotal.Add(obs);
                     
                        if (obsReadyToInsert.Count % ((dataTable.Columns.Count) - 1) == 0)
                        {
                            _observationRepository.InsertMany(obsReadyToInsert);
                            obsReadyToInsert.Clear();
                        }

                    }
               }
            if (obsReadyToInsert.Count > 0)
            {
                Debug.WriteLine("Created Observations are NOT Equal to measured values in the file please check!!");
            }

            dataFile.State = "LOADED";
            dataFile.IsLoadedToDB = true;
            _dataFileRepository.Update(dataFile);
            _dataContext.Save();
            return true;
        }
    }
}
