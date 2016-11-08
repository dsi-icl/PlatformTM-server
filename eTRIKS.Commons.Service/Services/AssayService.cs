using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using System.Collections;

namespace eTRIKS.Commons.Service.Services
{
    public class AssayService
    {
        private readonly IRepository<Assay, int> _assayRepository;
        private readonly IRepository<Dictionary, string> _dictionaryRepository;
        private readonly IRepository<Biosample, int> _bioSampleRepository;
        private readonly DatasetService _datasetService;
        private readonly IRepository<Project, int> _projectRepository;
        private readonly IRepository<PlatformAnnotation, Guid> _parepository;
        private readonly IRepository<DataFile, int> _dataFileRepository;



        private readonly IServiceUoW _dataContext;

        public AssayService(IServiceUoW uoW, DatasetService datasetService)
        {
            _dataContext = uoW;
            _assayRepository = uoW.GetRepository<Assay, int>();
            _bioSampleRepository = uoW.GetRepository<Biosample, int>();
            _projectRepository = uoW.GetRepository<Project, int>();
            _parepository = uoW.GetRepository<PlatformAnnotation, Guid>();
            _dataFileRepository = uoW.GetRepository<DataFile, int>();
            _datasetService = datasetService;
        }

        public List<AssayDTO> GetProjectAssays(string projectAcc)
        {
            List<Assay> assays = _assayRepository.FindAll(a => a.Project.Accession.Equals(projectAcc),
                new List<Expression<Func<Assay, object>>>()
                {
                    a => a.MeasurementType,
                    a => a.TechnologyPlatform,
                    a => a.TechnologyType
                }).ToList();

            if (assays.Count == 0)
                return null;
            return assays.Select(p => new AssayDTO()
            {
                Id = p.Id,
                Type = p.MeasurementType!=null?p.MeasurementType.Name:"",
                Platform = p.TechnologyPlatform!=null?p.TechnologyPlatform.Name:"",
                Technology = p.TechnologyType!=null?p.TechnologyType.Name:"",
                Name = p.Name
            }).ToList();
        }

        public async Task<Hashtable> GetSamplesDataPerAssay(string projectId, int assayId)
        {
            var samples = new List<Biosample>();
            samples = _bioSampleRepository.FindAll
                (bs => bs.AssayId.Equals(assayId), new List<Expression<Func<Biosample, object>>>()
                {
                    d => d.Study,
                    d =>d.Subject,
                    d => d.CollectionStudyDay
                }).ToList();

            List<Hashtable> sample_table = new List<Hashtable>();
            HashSet<string> SCs = new HashSet<string>() { "subjectId", "studyId", "sampleId", "studyDay#" };

            foreach (Biosample sample in samples)
            {
                Hashtable ht = new Hashtable();
                ht.Add("subjectId", sample.Subject != null ? sample.Subject.UniqueSubjectId : "missing");
                ht.Add("studyId", sample.Study.Name);
                ht.Add("sampleId", sample.BiosampleStudyId);
                ht.Add("studyDay#", sample.CollectionStudyDay?.Number);
                sample_table.Add(ht);
            }
            Hashtable returnObject = new Hashtable();
            returnObject.Add("header", SCs);
            returnObject.Add("data", sample_table);

            return returnObject;

        }

        public void addPA(int assayId, int fileId)
        {

            var fileService = new FileService(_dataContext);

            var dataFile = _dataFileRepository.Get(fileId);

            var filePath = dataFile.Path + "\\" + dataFile.FileName;
            //var filePath = "temp\\" + fileName;
            var dataTable = fileService.ReadOriginalFile(filePath);

            var PA = new PlatformAnnotation();
            PA.Name = "CRC305ABC";
            PA.Accession = "D-PA-AGI-01";
            foreach (DataRow row in dataTable.Rows)
            {
                var fa = new FeatureAnnotation();
                fa.Name = row["ProbeName"].ToString();
                fa.FeatureId = row["ID"].ToString();
                
                foreach (DataColumn column in dataTable.Columns)
                {
                    if (column.ColumnName == "ProbeName" || column.ColumnName == "ID")
                        continue;
                    var nv = new NV() {Name = column.ColumnName, Value = row[column].ToString()};

                    fa.Properties.Add(nv);
                }
                PA.FeatureAnnotations.Add(fa);
            }
            _parepository.Insert(PA);

            var assay = _assayRepository.Get(assayId);
            assay.PlatformAnnotationId = PA.Name;
            _assayRepository.Update(assay);
            _dataContext.Save();
        }







        //TEMP
        //public void addAssayCVterms()
        //{
        //    Dictionary dict = new Dictionary();
        //    dict.Id = "CL-ASYTT";
        //    dict.Name = "ASSAY TECHNOLOGY TYPES";
        //    dict.Definition = "Terms used for describing Assay Technology Types";

        //    CVterm cvTerm = new CVterm();
        //    cvTerm.Id = dict.Id + "-T-1"  ;
        //    cvTerm.Synonyms = null;
        //    cvTerm.Name = "";
        //    cvTerm.Definition = FK;
        //    cvTerm.DictionaryId = dictionaryId;
        //    cvTerm.XrefId = null;
        //    cvTerm.IsUserSpecified = false;
        //    _templateService.addCVTerm(cvTerm);

        //}
    }
}
