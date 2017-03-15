using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel.SDTM;
using eTRIKS.Commons.Core.Domain.Model.DesignElements;
using eTRIKS.Commons.Core.Domain.Model.Templates;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Core.Domain.Model.ObservationModel;

namespace eTRIKS.Commons.Service.Services.Loading.HdDataLoader
{
    public class DataMatrixLoader
    {

        private IServiceUoW _dataContext;
        private readonly IRepository<Core.Domain.Model.ObservationModel.Observation, Guid> _observationRepository;
        private readonly IRepository<Biosample, int> _biosampleRepository;
        private readonly IRepository<DataFile, int> _dataFileRepository;
        private readonly IRepository<Dataset, int> _datasetRepository;
        private readonly FileService _fileService;


        public DataMatrixLoader(IServiceUoW uoW, FileService fileService) 
        {
            _dataContext = uoW;
            _observationRepository = uoW.GetRepository<Core.Domain.Model.ObservationModel.Observation, Guid>();
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
           
            var dataset = GetActivityDataset(datasetId);
            var dataFile = _dataFileRepository.Get(fileId); 
            var filePath = dataFile.Path + "\\" + dataFile.FileName;

            var dataTable = _fileService.ReadOriginalFile(filePath);

            var sampleList = _biosampleRepository.FindAll(s => s.DatasetId == datasetId).ToList();
         //  var scos = sampleList.ToDictionary(co => co.BiosampleStudyId);

            var obsReadyToInsert = new List<Core.Domain.Model.ObservationModel.Observation>();

            foreach (DataRow row in dataTable.Rows)
            {
                for (int index = 0; index < dataTable.Columns.Count; index++)

                    if (index == 0) continue;
                    else
                    {
                        var column = dataTable.Columns[index];
                        //var PropertyDescriptor = new PropertyDescriptor();   // add property discreptor
                        //var PropertyDescriptor1 = dataset.Template.Class;
                        //var PropertyDescriptor3 = dataset.Variables.FirstOrDefault();
                        

                        var obs = new Core.Domain.Model.ObservationModel.Observation();
                        {
                            var value = new NumericalValue();
                            value.Value = float.Parse(row[column.ColumnName].ToString());
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
                            obs.SubjectOfObservationId = column.ColumnName;                                    // sampleID     (here is the file name)
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
                            
                        }

                        obsReadyToInsert.Add(obs);
                        if (obsReadyToInsert.Count % 500 == 0)
                        {
                            _observationRepository.InsertManyAsync(obsReadyToInsert);
                            _dataContext.Save();
                            obsReadyToInsert.Clear();
                        }
                    }
               }

            dataFile.State = "LOADED";
            dataFile.IsLoadedToDB = true;
            _dataFileRepository.Update(dataFile);
            _observationRepository.InsertManyAsync(obsReadyToInsert);
            _dataContext.Save();
            return true;
        }
    }
}
