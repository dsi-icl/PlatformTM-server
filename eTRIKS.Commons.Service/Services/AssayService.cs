using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IServiceUoW _dataContext;

        public AssayService(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _assayRepository = uoW.GetRepository<Assay, int>();
        }

        public Assay AddAssay(ActivityDTO assayDto)
        {
            var assay = new Assay();
            //assay.AssayPlatform = assayDto.;
            //assay.AssayTechnology =;
            assay.Name = assayDto.Name;
            assay.StudyId = assayDto.StudyID;
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
