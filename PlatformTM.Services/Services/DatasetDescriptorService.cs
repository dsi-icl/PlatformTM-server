using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS.DatasetDescriptorTypes;
using PlatformTM.Core.Domain.Model.Templates;
using PlatformTM.Models.DTOs;
using PlatformTM.Services.DTOs;

namespace PlatformTM.Models.Services
{
    public class DatasetDescriptorService
    {
        private readonly IServiceUoW _dataServiceUnit;
        private readonly IRepository<Dataset, int> _datasetRepository;
        private readonly IRepository<VariableDefinition, int> _variableDefinitionRepository;

        private readonly IRepository<ObservationDatasetDescriptor, Guid> _DatasetDescriptorRepository;

        private readonly FileService _fileService;
        public DatasetDescriptorService(IServiceUoW uoW, CacheService cacheService, FileService fileService)
        {
            _dataServiceUnit = uoW;
            _datasetRepository = uoW.GetRepository<Dataset, int>();
            _variableDefinitionRepository = uoW.GetRepository<VariableDefinition, int>();
            _DatasetDescriptorRepository = uoW.GetRepository<ObservationDatasetDescriptor, Guid>();

            _fileService = fileService;
        }

        /// <summary>
        /// Retrieves dataset for selected activity including Variable_Defs
        /// </summary>
        /// <param name="datasetId"></param>
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

        /// <summary>
        /// Retrieves datasetDTO for selected activity including a union of VarDefs and TemplateVariables from the relevant Domain
        /// //TODO: This method should change once users are allowed to add their own Variables
        /// In this case VarDEFs should take precedence and the issue of adding CVterms to VarDEFs shuold be settled
        /// </summary>
        /// <param name="datasetId"></param>
        /// <returns></returns>
        public DatasetDTO GetActivityDatasetDTO(int datasetId)
        {
            DatasetDTO dto = new DatasetDTO();
            Dataset ds = _datasetRepository.FindSingle(
                d => d.Id.Equals(datasetId),
                new List<string>()
                {
                    "Variables.VariableDefinition",
                    "Template.Fields.ControlledVocabulary.Xref.DB",
                    "Activity"
                });

            dto.Id = ds.Id; //Set DatasetDTO id to Dataset.Id (int)
            dto.Class = ds.Template.Class;
            dto.Description = ds.Template.Description;
            dto.Name = ds.Template.Domain;
            dto.DomainId = ds.Template.Id;
            dto.Structure = ds.Template.Structure;
            dto.Code = ds.Template.Code;
            dto.ProjectId = ds.Activity.ProjectId;


            //ds.Domain.Variables.Where(v=> !v.IsGeneric)
            foreach (DatasetTemplateField vt in ds.Template.Fields.OrderBy(v => v.Order))
            //foreach (var vt in ds.Variables)
            {
                DatasetVariableDTO dv = new DatasetVariableDTO();
                dv.Name = vt.Name;
                dv.Description = vt.Description;
                dv.Label = vt.Label;
                dv.Accession = vt.Id;
                dv.DataType = vt.DataType;
                dv.IsCurated = true;
                dv.RoleId = vt.RoleId;

                if (vt.ControlledVocabulary != null)
                {
                    dv.DictionaryName = vt.ControlledVocabulary.Name;
                    dv.DictionaryDefinition = vt.ControlledVocabulary.Definition;
                    dv.DictionaryXrefURL = vt.ControlledVocabulary.Xref.DB.UrlPrefix +
                                           vt.ControlledVocabulary.Xref.Accession;
                }

                var vr = ds.Variables.SingleOrDefault(v => v.VariableDefinition.Name.Equals(vt.Name));
                if (vr != null)
                {
                    dv.IsRequired = vr.IsRequired;
                    dv.KeySequence = vr.KeySequence;
                    dv.OrderNumber = vt.Order;
                    dv.Id = vr.VariableDefinitionId;
                    dv.isSelected = true;
                }
                dto.Variables.Add(dv);
            }


            foreach (VariableReference vr in ds.Variables)
            {

                if (!dto.Variables.Exists(v => v.Accession.Equals(vr.VariableDefinition.Accession)))
                {
                    DatasetVariableDTO dv = new DatasetVariableDTO();
                    dv.Name = vr.VariableDefinition.Name;
                    dv.Description = vr.VariableDefinition.Description;
                    dv.Label = vr.VariableDefinition.Label;
                    dv.Accession = vr.VariableDefinition.Accession;
                    dv.DataType = vr.VariableDefinition.DataType;
                    dv.IsCurated = vr.VariableDefinition.IsCurated;
                    dv.IsComputed = vr.VariableDefinition.IsComputed ?? false;
                    dv.RoleId = vr.VariableDefinition.RoleId;

                    dv.IsRequired = vr.IsRequired;
                    dv.KeySequence = vr.KeySequence;
                    dv.OrderNumber = ds.Template.Fields.Count+1;
                    dv.Id = vr.VariableDefinitionId;
                    dv.isSelected = true;

                    dto.Variables.Add(dv);
                }


            }

            return dto;
        }

        public Dataset addDataset(DatasetDTO datasetDTO)
        {
            //1. Fields for Dataset
            var dataset = CreateDataset(datasetDTO);

            _datasetRepository.Insert(dataset);
            return _dataServiceUnit.Save().Equals("CREATED") ? dataset : null;
        }

        //public DatasetDescriptor CreateDatasetDescriptor(DatasetDTO dto)
        //{
        //    if(dto.Code == "observation")
        //    {
        //        var c = new ObservationDatasetDescriptor(dto.Name);
        //        foreach (var variableDto in dto.Variables)
        //        {
        //            if(variableDto.RoleId == "ObservedPropertyValue")
        //            {
        //                c.ObservedPropertyFields.Add(null);
        //            } 
        //        }
        //    }
        //    return null;
        //}

        public Dataset CreateDataset(DatasetDTO datasetDTO)
        {
            if (datasetDTO == null)
                return null;
            var dataset = new Dataset { ActivityId = datasetDTO.ActivityId, TemplateId = datasetDTO.DomainId };


            // Get any exisiting variable definitions for that study
            List<VariableDefinition> variableDefsOfStudy = getVariableDefinitionsOfStudy(datasetDTO.ProjectId).ToList();

            foreach (var variableDto in datasetDTO.Variables.Where(variableDto => variableDto.isSelected))
            {

                //Compare newly added Variable to previously added VarDefs using accession string 
                //since no Id for Variable created yet
                var varDef = variableDefsOfStudy.SingleOrDefault(
                    d => d.Accession.Equals(variableDto.Accession));
                if (varDef == null)
                {
                    varDef = new VariableDefinition();
                    varDef.Accession = variableDto.Accession;
                    varDef.Name = variableDto.Name;
                    varDef.Label = variableDto.Label;
                    varDef.Description = variableDto.Description;
                    varDef.DataType = variableDto.DataType;
                    varDef.IsCurated = variableDto.IsCurated;
                    varDef.RoleId = variableDto.RoleId;
                    varDef.ProjectId = datasetDTO.ProjectId;
                    //varDef.

                    varDef.VariableTypeStr = variableDto.varType;

                    if (variableDto.IsComputed)
                    {
                        varDef.IsComputed = true;
                        varDef.ComputedVarExpression = variableDto.ExpressionList.Select(t => t.val).Aggregate((i, j) => i + ',' + j);
                    }

                }
                //3. Fields for varRefList
                VariableReference varRef = new VariableReference();
                varRef.OrderNumber = variableDto.OrderNumber;
                varRef.IsRequired = variableDto.IsRequired;
                varRef.KeySequence = variableDto.KeySequence;
                varRef.VariableDefinition = varDef;
                dataset.Variables.Add(varRef);
            }
            return dataset;
        }

        public string UpdateDataset(DatasetDTO datasetDTO)
        {
            Dataset datasetToUpdate = GetActivityDataset(datasetDTO.Id);
            datasetToUpdate.State = datasetDTO.State;

            List<VariableDefinition> variableDefsOfStudy = getVariableDefinitionsOfStudy(datasetDTO.ProjectId).ToList();

            var datasetVarsToUpdate = new HashSet<string>(
                datasetToUpdate.Variables.Select(c => c.VariableDefinition.Name));

            foreach (var variableDto in datasetDTO.Variables)
            {
                if (variableDto.isSelected)
                {
                    //TODO: NOTE NO UPDATE IS ASSUMED other than adding or removing the whole variable
                    //WILL HAVE tO CHANGE WHEN UPDATING CONTROLLED VOCAB
                    if (!datasetVarsToUpdate.Contains(variableDto.Name))
                    {
                        //add VarDEF and addVarREF
                        VariableDefinition varDef;
                        varDef = variableDefsOfStudy.SingleOrDefault(d => d.Accession.Equals(variableDto.Accession));
                        if (varDef == null)
                        {
                            varDef = new VariableDefinition();
                            varDef.Accession = variableDto.Accession;
                            varDef.Name = variableDto.Name;
                            varDef.Label = variableDto.Label;
                            varDef.Description = variableDto.Description;
                            varDef.IsComputed = variableDto.IsComputed;
                            varDef.DataType = variableDto.DataType;
                            varDef.IsCurated = variableDto.IsCurated;
                            varDef.RoleId = variableDto.RoleId;
                            varDef.ProjectId = datasetDTO.ProjectId;
                            varDef.VariableTypeStr = variableDto.varType;

                            if (variableDto.IsComputed)
                            {
                                varDef.IsComputed = true;

                                varDef.ComputedVarExpression = variableDto.ExpressionList.Select(t => t.val).Aggregate((i, j) => i + ',' + j);
                            }
                        }

                        VariableReference varRef = new VariableReference();
                        varRef.OrderNumber = variableDto.OrderNumber;
                        varRef.IsRequired = variableDto.IsRequired;
                        varRef.KeySequence = variableDto.KeySequence;
                        varRef.VariableDefinition = varDef;
                        datasetToUpdate.Variables.Add(varRef);
                    }
                    datasetToUpdate.Variables.First(v=>v.VariableDefinition.Name==variableDto.Name).OrderNumber = variableDto.OrderNumber;
                }
                else if (datasetVarsToUpdate.Contains(variableDto.Name))
                {
                    //remove variable from dataset
                    VariableReference var =
                        datasetToUpdate.Variables.Single(v => v.VariableDefinitionId.Equals(variableDto.Id));
                    datasetToUpdate.Variables.Remove(var);
                }
            }

            _datasetRepository.Update(datasetToUpdate);

            return _dataServiceUnit.Save();
        }

        //TODO: move to Repository 
        public IEnumerable<VariableDefinition> getVariableDefinitionsOfStudy(int studyId)
        {
            return _variableDefinitionRepository.FindAll(d => d.ProjectId.Equals(studyId));
        }

        

        public DatasetDescriptor GetUploadedDescriptor(int projectId, string filename)
        {
            string fullpath = Path.Combine(_fileService.GetFullPath(projectId), "temp", filename);

            //string fileName = "WeatherForecast.json";
            string jsonString = File.ReadAllText(fullpath);
            var guid = Guid.NewGuid();
            
            
           // var options = new JsonSerializerOptions { WriteIndented = true, MaxDepth = 10,  IgnoreNullValues= true };
            ObservationDatasetDescriptor oDD = JsonSerializer.Deserialize<ObservationDatasetDescriptor>(jsonString)!;

            var oDD_dto = new DatasetDescriptorDTO(oDD);

            return oDD;

        }

        public DatasetDescriptor AddDescriptor(ObservationDatasetDescriptor dd, int projectId)
        {
            
            if (dd == null)
                return null;

            dd.ProjectId = projectId;
            dd.Id = Guid.NewGuid();


            _DatasetDescriptorRepository.Insert(dd);
            return _dataServiceUnit.Save().Equals("CREATED") ? dd : null;
            
        }

        public DatasetDescriptor GetDatasetDescriptor(string descriptorId)
        {
            var dd = _DatasetDescriptorRepository.FindSingle(d => d.Id == Guid.Parse(descriptorId));
            //var dto = new DatasetDescriptorDTO(dd);
            return dd;
        }

        public List<DatasetDescriptorDTO> GetDatasetDescriptors(int projectId)
        {
            List<ObservationDatasetDescriptor> descriptors = _DatasetDescriptorRepository.FindAll(
                d => d.ProjectId == projectId).ToList();
            return descriptors.Select(s=> new DatasetDescriptorDTO(s)).ToList();
        }

        public void UpdateDescriptor(ObservationDatasetDescriptor descriptor, int projectId)
        {
            var studyToUpdate = _DatasetDescriptorRepository.Get(descriptor.Id);

            //check that the owner of this dataset is the caller
            //var dataset = ReadDTO(dto);
            //var datasetToUpdate = _DatasetDescriptorRepository.FindSingle(d => d.Id == dataset.Id);
            //datasetToUpdate.LastModified = DateTime.Today.ToString("f");
            //datasetToUpdate.Description = dataset.Description;

            _DatasetDescriptorRepository.Update(descriptor);
        }


    }
}
