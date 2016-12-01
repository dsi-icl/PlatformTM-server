using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model.Templates;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using eTRIKS.Commons.Service.DTOs;

namespace eTRIKS.Commons.Service.Services
{
    public class TemplateService// : ITemplateService
    {

        private readonly IServiceUoW _dataServiceUnit;

        private readonly IRepository<CVterm, string> _cvTermRepository;
        private readonly IRepository<Dictionary, string> _dictionaryRepository;
        private readonly IRepository<DomainTemplate, string> _templateRepository;
        private readonly FileService _fileService;

        public TemplateService(IServiceUoW uoW, FileService fileService)
        {
            _dataServiceUnit = uoW;
            _templateRepository = uoW.GetRepository<DomainTemplate, string>();

            _cvTermRepository = uoW.GetRepository<CVterm, string>();
            _dictionaryRepository = uoW.GetRepository<Dictionary, string>();
            _fileService = fileService;
        }


        /// <summary>
        /// Retrieves Domain dataset from Template Tables for "New" datasets
        /// </summary>
        /// <param name="domainId"></param>
        public DatasetDTO GetTemplateDataset(string domainId)
        {
            DomainTemplate domainTemplate = _templateRepository.FindAll(
                d => d.Id.Equals(domainId),
                new List<string>()
                {
                   "Variables.controlledTerminology.Xref.DB"
                }
                ).FirstOrDefault<DomainTemplate>();


            //TODO: USE AutoMapper instead of this manual mapping

            if (domainTemplate == null)
                return null;

            var dto = GetDatasetDTO(domainTemplate);

            return dto;
        }

        public List<DatasetDTO> GetAllDomainTemplates()
        {
            //leaky abstraction
            //TODO: create a method in Generic Repository that takes an expression as a parameter
            // and uses it in a select method.
            var domains = _templateRepository.FindAll(
                d => d.Id.StartsWith("D-SDTM"), 
                new List<string>()
                {
                    "Variables"
                }).ToList()
                  .OrderBy(d => d.Class);
            return domains.Select(GetDatasetDTO).ToList();
        }

        public List<DatasetDTO> GetAssayFeatureTemplates()
        {
            var ds = GetTemplateDataset("D-ASSAY-FEAT");
            return new List<DatasetDTO>() { ds };
        }

        public List<DatasetDTO> GetAssaySampleTemplates()
        {
            var ds = GetTemplateDataset("D-SDTM-BS");
            return new List<DatasetDTO> { ds };
        }

        public List<DatasetDTO> GetAssayDataTemplates()
        {
            var templates = _templateRepository.FindAll(
                d => d.Class == "Assay Observations", 
                new List<string>()
                {
                    "Variables"
                });
            List<DatasetDTO> templateDTOs = new List<DatasetDTO>();
            foreach (var ds in templates)
            {
                templateDTOs.Add(GetDatasetDTO(ds));
            }
            return templateDTOs;
        }


        private DatasetDTO GetDatasetDTO(DomainTemplate domainTemplate)
        {
            DatasetDTO dto = new DatasetDTO();

            dto.Class = domainTemplate.Class;
            dto.Description = domainTemplate.Description;
            dto.Name = domainTemplate.Name;
            dto.DomainId = domainTemplate.Id;
            dto.Structure = domainTemplate.Structure;
            dto.Code = domainTemplate.Code;

            foreach (DomainVariableTemplate vt in domainTemplate.Variables.OrderBy(c => c.Order))
            {
                DatasetVariableDTO dv = new DatasetVariableDTO();
                dv.Name = vt.Name;
                dv.Description = vt.Description;
                dv.Label = vt.Label;
                dv.Accession = vt.Id;
                dv.RoleId = vt.RoleId;
                dv.DataType = vt.DataType;
                dv.UsageId = vt.UsageId;
                //if (vt.controlledTerminology != null)
                //{
                //    dv.DictionaryName = vt.controlledTerminology.Name;
                //    dv.DictionaryDefinition = vt.controlledTerminology.Definition;
                //    dv.DictionaryXrefURL = vt.controlledTerminology.Xref.DB.UrlPrefix +
                //                           vt.controlledTerminology.Xref.Accession;
                //}
                dv.IsRequired = false;
                dv.KeySequence = null;
                dv.OrderNumber = null;
                dv.IsCurated = true;
                dv.IsComputed = false;
                dv.varType = "STANDARD";

                if (dv.UsageId.Equals("CL-Compliance-T-1") || dv.UsageId.Equals("CL-Compliance-T-2"))
                {
                    dv.isSelected = true;

                    if (dv.UsageId.Equals("CL-Compliance-T-1"))
                        dv.IsRequired = true;
                }

                dto.variables.Add(dv);
            }
            return dto;

        }

        public void addDictionaryItem(Dictionary dictionaryItem)
        {
            _dictionaryRepository.Insert(dictionaryItem);
            _dataServiceUnit.Save();
        }

        public void addCVTerm(CVterm cvTerm)
        {
            _cvTermRepository.Insert(cvTerm);
            _dataServiceUnit.Save();
        }



        #region temp loader methods

        public void loadDatasetTemplate(string templateDefFilename, string varDefFilename)
        {
            //var fileservice = new FileService(_dataServiceUnit);
            var tbl1 = _fileService.ReadOriginalFile(templateDefFilename);
            //foreach (var row in tbl1.Rows)
            //{
                var dt = new DomainTemplate();
                dt.Id = tbl1.Rows[0][0].ToString();
                dt.Name = tbl1.Rows[0][4].ToString();//ds.Tables[0].Rows[i][4].ToString().Trim();
                dt.Class = tbl1.Rows[0][1].ToString();
                dt.Description = tbl1.Rows[0][6].ToString();
                dt.Code = tbl1.Rows[0][5].ToString();
                dt.Structure = tbl1.Rows[0][7].ToString();
            //}

            

            var tbl2 = _fileService.ReadOriginalFile(varDefFilename);
            foreach (DataRow row in tbl2.Rows)
            {
                var dvt = new DomainVariableTemplate();
                dvt.Id = row[0].ToString();
                dvt.Name = row[3].ToString();//ds.Tables[0].Rows[i][1].ToString().Trim();
                dvt.Label = row[4].ToString(); //ds.Tables[0].Rows[i][2].ToString().Trim();
                dvt.Description = row[5].ToString();//ds.Tables[0].Rows[i][3].ToString().Trim();
                dvt.DataType = row[8].ToString(); //ds.Tables[0].Rows[i][4].ToString().Trim();
                dvt.RoleId = getOIDOfCVterm(row[10].ToString());
                dvt.UsageId = getOIDOfCVterm(row[9].ToString());
                
                //IOUtility iou = new IOUtility();
                //dvt.controlledTerminologyId = iou.processDictionaryIdForTemplateVariable(ds.Tables[0].Rows[i][7].ToString().Trim());
                //dvt.DomainId = ds.Tables[0].Rows[i][8].ToString().Trim();
                dvt.Order = Convert.ToInt32(row[2].ToString());
                dt.Variables.Add(dvt);
            }
            _templateRepository.Insert(dt);
            _dataServiceUnit.Save();
        }

        public void loadCVterms(string DictFilename, string CVsfFilename)
        {
           // var fileservice = new FileService(_dataServiceUnit);
           // var tbl1 = fileservice.ReadOriginalFile(DictFilename);

            var tbl1 = _fileService.ReadOriginalFile(CVsfFilename);
            //foreach (DataRow row in tbl1.Rows)
            //{
            //  Dictionary dict  = new Dictionary();
            //    dict.Id = row[0].ToString();
            //    dict.Name = row[1].ToString();
            //    dict.Definition = row[2].ToString();
            //    _dictionaryRepository.Insert(dict);
            //}

            foreach (DataRow row in tbl1.Rows)
            {
                CVterm cv = new CVterm();
                cv.Id = row[0].ToString();
                cv.Name = row[1].ToString();
                cv.DictionaryId = row[2].ToString();
                _cvTermRepository.Insert(cv);
            }
            _dataServiceUnit.Save();
        }

        public string getOIDOfCVterm(string name)
        {
            if (name.Length < 1)
                return null;
            //return _cvTermRepository.GetRecords(o => o.Name.Equals(name)).First().Id;
            return _cvTermRepository.FindSingle(o => o.Name.Equals(name)).Id;
        }

        public void loadDataMatrixTemplate()
        {
            var dt = new DomainTemplate();
            dt.Id = "D-CUST-MX";
            dt.Name = "Assay data matrix";
            dt.Structure = "A two dimensional data matrix with a single measurement for each feature/sample combination";
            dt.IsRepeating = false;
            dt.Class = "Assay Observations";
            dt.Code = "MX";
            dt.Description = "A simple template for a two dimensional data matrix used for array data or any other assay data that is represented in a matrix structure, where columns/rows are sample or feature identifiers and cells are measurement values measure in a single measurement (e.g. log intensity";

            dt.Variables.Add(new DomainVariableTemplate()
            {
                Id = "V-CUST-MX-X",
                Name = "X",
                DataType = "decimal",
                Description = "Identifiers for columns of the matrix",
                Order = 1,
                UsageId = "CL-Compliance-T-1",
                RoleId = "CL-Role-T-1",
                Label = "Column Identifier"
                
            });
            dt.Variables.Add(new DomainVariableTemplate()
            {
                Id = "V-CUST-MX-Y",
                Name = "Y",
                DataType = "decimal",
                Description = "Identifiers for rows of the matrix",
                Order = 2,
                UsageId = "CL-Compliance-T-1",
                RoleId = "CL-Role-T-1",
                Label = "Row Identifier"
            });
            dt.Variables.Add(new DomainVariableTemplate()
            {
                Id = "V-CUST-MX-VAL",
                Name = "VALUE",
                DataType = "decimal",
                Description = "Measured observation value",
                Order = 3,
                UsageId = "CL-Compliance-T-1",
                RoleId = "CL-Role-T-8",
                Label = "Measured Value"
            });
            _templateRepository.Insert(dt);
            _dataServiceUnit.Save();
        }

        #endregion


    }
}
