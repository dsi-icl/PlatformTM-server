using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;

namespace eTRIKS.Commons.Service.Services
{
    public class AssayService
    {
        private readonly IRepository<Assay, int> _assayRepository;
        private readonly IRepository<Dictionary, string> _dictionaryRepository;
        private readonly IRepository<Biosample, int> _bioSampleRepository;

        private readonly IServiceUoW _dataContext;

        public AssayService(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _assayRepository = uoW.GetRepository<Assay, int>();
            _bioSampleRepository = uoW.GetRepository<Biosample, int>();
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
                AssayId = p.Id,
                Measurement = p.MeasurementType!=null?p.MeasurementType.Name:"",
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
                ht.Add("studyDay#", sample.CollectionStudyDay.Number);
                sample_table.Add(ht);
            }
            Hashtable returnObject = new Hashtable();
            returnObject.Add("header", SCs);
            returnObject.Add("data", sample_table);

            return returnObject;

        }


        public Assay AddAssay(ActivityDTO assayDto)
        {
            var assay = new Assay();
            //assay.AssayPlatform = assayDto.;
            //assay.AssayTechnology =;
            assay.Name = assayDto.Name;
            assay.ProjectId = assayDto.ProjectId;
            assay.TechnologyPlatformId = assayDto.AssayTechnologyPlatform;
            assay.TechnologyTypeId = assayDto.AssayTechnology;
            //assay.DesignType = getCVterm(assayDto.AssayDesignType);
            assay.MeasurementTypeId = assayDto.AssayMeasurementType;

            //var biospecimens = new Dataset();
            //var bioentities = new Dataset();

            //assay.Datasets.Add(bioentities);
            //assay.Datasets.Add(biospecimens);
            _assayRepository.Insert(assay);
            _dataContext.Save();
            return assay;
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
