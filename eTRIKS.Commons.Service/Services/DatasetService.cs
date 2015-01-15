using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.Templates;
using eTRIKS.Commons.Service.DTOs;

namespace eTRIKS.Commons.Service.Services
{
    public class DatasetService
    {
        private IServiceUoW _dataServiceUnit;
        private IRepository<Dataset, int> _datasetRepository;
        private IRepository<DomainTemplate, string> _domainRepository;
        private IRepository<Activity, string> _activityRepository;
        private IRepository<VariableDefinition, int> _variableDefinitionRepository;

        public DatasetService(IServiceUoW uoW)
        {
            _dataServiceUnit = uoW;
            //_templateRepository = uoW.GetRepository<DomainTemplate, string>();
            //_templateVariableRepository = uoW.GetRepository<DomainTemplateVariable,string>();
            _datasetRepository = uoW.GetRepository<Dataset, int>();
            _domainRepository = uoW.GetRepository<DomainTemplate, string>();
            _variableDefinitionRepository = uoW.GetRepository<VariableDefinition, int>();
        }

        /// <summary>
        /// Retrieves Domain dataset from Template Tables for "New" datasets
        /// </summary>
        /// <param name="domainId"></param>
        public DatasetDTO GetTemplateDataset(string domainId)
        {
            DomainTemplate domainTemplate = _domainRepository.Get(
                d => d.OID.Equals(domainId),
                new List<Expression<Func<DomainTemplate, object>>>(){
                    d => d.Variables.Select(c => c.controlledTerminology.Xref.DB)
                },
                null,
                null
                ).FirstOrDefault<DomainTemplate>();


            //TODO: USE AutoMapper instead of this manual mapping

            DatasetDTO dto = new DatasetDTO();

            dto.Class = domainTemplate.Class;
            dto.Description = domainTemplate.Description;
            dto.Name = domainTemplate.Name;
            dto.DomainId = domainTemplate.OID;
            dto.Structure = domainTemplate.Structure;
            List<DatasetVariableDTO> vars = new List<DatasetVariableDTO>();
            foreach (DomainVariableTemplate vt in domainTemplate.Variables)
            {
                DatasetVariableDTO dv = new DatasetVariableDTO();
                dv.Name = vt.Name;
                dv.Description = vt.Description;
                dv.Label = vt.Label;
                dv.Accession = vt.OID;
                if (vt.controlledTerminology != null)
                {
                    dv.DictionaryName = vt.controlledTerminology.Name;
                    dv.DictionaryDefinition = vt.controlledTerminology.Definition;
                    dv.DictionaryXrefURL = vt.controlledTerminology.Xref.DB.UrlPrefix+vt.controlledTerminology.Xref.Accession;
                }
                dv.IsRequired = null;
                dv.KeySequence = null;
                dv.OrderNumber = null;
                    
                vars.Add(dv);
            }
            dto.variables = vars;

            return dto;
        }


        public List<DatasetDTO> GetAllDomainTemplates()
        {
            //leaky abstraction
            //TODO: create a method in Generic Repository that takes an expression as a parameter
            // and uses it in a select method.
            return _domainRepository.GetAll().Select(AsDatasetDto).ToList();
        }

        

        /// <summary>
        /// Retrieves dataset for selected activity including Variable_Defs
        /// </summary>
        /// <param name="datasetId"></param>
        private Dataset GetActivityDataset(int datasetId)
        {
            Dataset ds = _datasetRepository.GetSingle(
                d => d.OID.Equals(datasetId),
                new List<Expression<Func<Dataset, object>>>(){
                        d => d.Variables.Select(t => t.VariableDefinition),
                });
            return ds;   
        }

        /// <summary>
        /// Retrieves datasetDTO for selected activity including a union of VarDefs and TemplateVariables from the relevant Domain
        /// //TODO: This method should change once users are allowed to add their own Variables
        //          In this case VarDEFs should take precedence and the issue of adding CVterms to VarDEFs shuold be settled
        /// </summary>
        /// <param name="datasetId"></param>
        /// <returns></returns>
        public DatasetDTO GetActivityDatasetDTO(int datasetId)
        {
            
            DatasetDTO dto = new DatasetDTO();
            Dataset ds = _datasetRepository.GetSingle(
                d => d.OID.Equals(datasetId),
                new List<Expression<Func<Dataset, object>>>(){
                        d => d.Variables.Select(t => t.VariableDefinition),
                        d => d.Domain.Variables.Select(t => t.controlledTerminology.Xref.DB)
                });

            dto.Id = ds.OID;//Set DatasetDTO id to Dataset.Id (int)
            dto.Class = ds.Domain.Class;
            dto.Description = ds.Domain.Description;
            dto.Name = ds.Domain.Name;
            dto.DomainId = ds.Domain.OID;
            dto.Structure = ds.Domain.Structure;
            dto.Code = ds.Domain.Code;

            foreach (DomainVariableTemplate vt in ds.Domain.Variables)
            //foreach (var vt in ds.Variables)
            {
                DatasetVariableDTO dv = new DatasetVariableDTO();
                dv.Name = vt.Name;
                dv.Description = vt.Description;
                dv.Label = vt.Label;
                dv.Accession = vt.OID;

                if (vt.controlledTerminology != null)
                {
                    dv.DictionaryName = vt.controlledTerminology.Name;
                    dv.DictionaryDefinition = vt.controlledTerminology.Definition;
                    dv.DictionaryXrefURL = vt.controlledTerminology.Xref.DB.UrlPrefix + vt.controlledTerminology.Xref.Accession;
                }
                dto.variables.Add(dv);
            }
            foreach (VariableReference vr in ds.Variables)
            {
                DatasetVariableDTO dv;
                dv = dto.variables.SingleOrDefault(v => v.Accession.Equals(vr.VariableDefinition.Accession));
                if (dv != null)
                {
                    dv.IsRequired = vr.IsRequired;
                    dv.KeySequence = vr.KeySequence;
                    dv.OrderNumber = vr.OrderNumber;
                    dv.Id = vr.VariableDefinitionId;
                    dv.isSelected = true;
                }
            }
            
            return dto;
        }

        public Dataset addDataset(DatasetDTO datasetDTO)
        {

            //1. Fields for Dataset
            Dataset dataset = new Dataset();
            dataset.ActivityId = datasetDTO.ActivityId;
            dataset.DomainId = datasetDTO.DomainId;

            // Get any exisiting variable definitions for that study
            List<VariableDefinition> variableDefsOfStudy = getVariableDefinitionsOfStudy(datasetDTO.StudyId).ToList();

            foreach(var VariableDTO in datasetDTO.variables)
            {
                if (VariableDTO.isSelected)
                {

                   VariableDefinition varDef;
                    //Compare newly added Variable to previously added VarDefs using accession string 
                    //since no Id for Variable created yet
                   varDef = variableDefsOfStudy.SingleOrDefault(d => d.Accession.Equals(VariableDTO.Accession));
                   if (varDef == null){
                       varDef = new VariableDefinition();
                       varDef.Accession = VariableDTO.Accession;
                       varDef.Name = VariableDTO.Name;
                       varDef.Label = VariableDTO.Label;
                       varDef.Description = VariableDTO.Description;
                       varDef.DataType = VariableDTO.DataType;
                       varDef.StudyId = datasetDTO.StudyId;
                   }
                   //3. Fields for varRefList
                   VariableReference varRef = new VariableReference();
                   varRef.OrderNumber = VariableDTO.OrderNumber;
                   varRef.IsRequired = VariableDTO.IsRequired;
                   varRef.KeySequence = VariableDTO.KeySequence;
                   varRef.VariableDefinition = varDef;
                   dataset.Variables.Add(varRef);
                }
            }

            _datasetRepository.Insert(dataset);
            if (_dataServiceUnit.Save().Equals("CREATED"))
            {
                return dataset;
            }
            return null;
        }

        public string updateDataset(DatasetDTO datasetDTO, string datasetId)
        {
            Dataset datasetToUpdate = GetActivityDataset(datasetDTO.Id);
            List<VariableDefinition> variableDefsOfStudy = getVariableDefinitionsOfStudy(datasetDTO.StudyId).ToList();

            var datasetVarsToUpdate = new HashSet<string>(
                datasetToUpdate.Variables.Select(c => c.VariableDefinition.Accession));

            foreach(var VariableDTO in datasetDTO.variables){
                if (VariableDTO.isSelected)
                {
                    //switching to VariableDTO.Accession since for newly selected variables Id is null
                    if(!datasetVarsToUpdate.Contains(VariableDTO.Accession))
                    {
                        //add VarDEF and addVarREF
                        VariableDefinition varDef;
                        varDef = variableDefsOfStudy.SingleOrDefault(d => d.Accession.Equals(VariableDTO.Accession));
                        if (varDef == null)
                        {
                            varDef = new VariableDefinition();
                            varDef.Accession = VariableDTO.Accession;
                            varDef.Name = VariableDTO.Name;
                            varDef.Label = VariableDTO.Label;
                            varDef.Description = VariableDTO.Description;
                            varDef.DataType = VariableDTO.DataType;
                            varDef.StudyId = datasetDTO.StudyId;
                        }

                        VariableReference varRef = new VariableReference();
                        varRef.OrderNumber = VariableDTO.OrderNumber;
                        varRef.IsRequired = VariableDTO.IsRequired;
                        varRef.KeySequence = VariableDTO.KeySequence;
                        varRef.VariableDefinition = varDef;
                        datasetToUpdate.Variables.Add(varRef);
                    }
                }
                else if (datasetVarsToUpdate.Contains(VariableDTO.Accession))
                {
                    //remove variable from dataset
                    VariableReference var = datasetToUpdate.Variables.Single(v => v.VariableDefinitionId.Equals(VariableDTO.Id));
                    datasetToUpdate.Variables.Remove(var);
                }
            }

            _datasetRepository.Update(datasetToUpdate);

            return _dataServiceUnit.Save();
        }

        public IEnumerable<VariableDefinition> getVariableDefinitionsOfStudy(string studyId)
        {
           return _variableDefinitionRepository.Get(d => d.StudyId.Equals(studyId));
        }

        private static readonly Expression<Func<DomainTemplate, DatasetDTO>> AsDatasetDto =
            x => new DatasetDTO
            {
                Name = x.Name,
                Class = x.Class,
                Description = x.Description,
                DomainId = x.OID,
                Structure = x.Structure,
                Code = x.Code,

            };
    
    }
}
