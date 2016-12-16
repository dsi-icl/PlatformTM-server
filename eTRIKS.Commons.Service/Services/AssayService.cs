using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel;

namespace eTRIKS.Commons.Service.Services
{
    public class AssayService
    {
       
        private readonly IRepository<Biosample, int> _bioSampleRepository;
        private readonly IRepository<Project, int> _projectRepository;
        private readonly IRepository<Assay, int> _assayRepository;
        private readonly DatasetService _datasetService;
        private readonly IServiceUoW _dataContext;

        public AssayService(IServiceUoW uoW, DatasetService datasetService)
        {
            _dataContext = uoW;
            _bioSampleRepository = uoW.GetRepository<Biosample, int>();
            _projectRepository = uoW.GetRepository<Project, int>();
            _assayRepository = uoW.GetRepository<Assay, int>();
            _datasetService = datasetService;
        }

        /**
        * Assay Methods
        * */

        public AssayDTO GetAssay(int assayId)
        {
            var assay = _assayRepository.FindSingle(
                d => d.Id.Equals(assayId),
                new List<string>(){
                        "Datasets.Domain",
                        "TechnologyType",
                        "TechnologyPlatform",
                        "MeasurementType",
                        "DesignType"
                }
            );

            var assayDTO = new AssayDTO();
            assayDTO.Name = assay.Name;
            assayDTO.Id = assay.Id;
            assayDTO.ProjectId = assay.ProjectId;

            assayDTO.Type = assay.MeasurementType?.Id;
            assayDTO.Technology = assay.TechnologyType?.Id;
            assayDTO.Platform = assay.TechnologyPlatform?.Id;
            assayDTO.Design = assay.DesignType?.Id;


            foreach (var dst in assay.Datasets.Select(ds => _datasetService.GetActivityDatasetDTO(ds.Id)))
            {
                //TODO: convert to enums or CVterms
                if (dst.Class == "Sample Annotations")
                    assayDTO.SamplesDataset = dst;
                if (dst.Class == "Assay Observations")
                    assayDTO.ObservationsDataset = dst;
                if (dst.Class == "Feature Annotations")
                    assayDTO.FeaturesDataset = dst;
            }
            return assayDTO;
        }

        public AssayDTO AddAssay(AssayDTO assayDto)
        {
            var assay = new Assay();
            var project = _projectRepository.FindSingle(d => d.Accession
                .Equals(assayDto.ProjectAcc));

            assay.Name = assayDto.Name;
            assay.ProjectId = project.Id;
            assay.TechnologyPlatformId = assayDto.Platform;
            assay.TechnologyTypeId = assayDto.Technology;
            //assay.DesignType = getCVterm(assayDto.AssayDesignType);
            assay.MeasurementTypeId = assayDto.Type;

            if (assayDto.SamplesDataset != null) assayDto.SamplesDataset.ProjectId = project.Id;
            if (assayDto.FeaturesDataset != null) assayDto.FeaturesDataset.ProjectId = project.Id;
            if (assayDto.ObservationsDataset != null) assayDto.ObservationsDataset.ProjectId = project.Id;

            var BSdataset = _datasetService.CreateDataset(assayDto.SamplesDataset);
            if (BSdataset != null)
                assay.Datasets.Add(BSdataset);

            var FEdataset = _datasetService.CreateDataset(assayDto.FeaturesDataset);
            if (FEdataset != null)
                assay.Datasets.Add(FEdataset);

            var OBdataset = _datasetService.CreateDataset(assayDto.ObservationsDataset);
            if (OBdataset != null)
                assay.Datasets.Add(OBdataset);

            assay = _assayRepository.Insert(assay);


            if (_dataContext.Save().Equals("CREATED"))
            {
                assayDto.Id = assay.Id;
                return assayDto;
            }
            return null;
        }

        public string UpdateAssay(AssayDTO assayDTO, int assayId)
        {
            Assay assayToUpdate = _assayRepository.Get(assayId);


            assayToUpdate.Name = assayDTO.Name;
            assayToUpdate.TechnologyPlatformId = assayDTO.Platform;
            assayToUpdate.TechnologyTypeId = assayDTO.Technology;
            //assay.DesignType = getCVterm(assayDto.AssayDesignType);
            assayToUpdate.MeasurementTypeId = assayDTO.Type;

            foreach (var datasetDto in assayDTO.Datasets)
            {

                if (datasetDto == null) continue;
                datasetDto.ProjectId = assayDTO.ProjectId;
                if (datasetDto.isNew)
                {
                    var dataset = _datasetService.CreateDataset(datasetDto);
                    assayToUpdate.Datasets.Add(dataset);
                    _assayRepository.Update(assayToUpdate);
                }
                else
                    _datasetService.UpdateDataset(datasetDto);
            }
            return _dataContext.Save();
        }

        

        public Hashtable GetSamplesDataPerAssay(int assayId)
        {
            var samples = new List<Biosample>();
            samples = _bioSampleRepository.FindAll
                (bs => bs.AssayId.Equals(assayId), 
                new List<string>()
                {
                    "Study",
                    "Subject",
                    //"CollectionStudyDay"
                }).ToList();

            List<Hashtable> sample_table = new List<Hashtable>();
            HashSet<string> SCs = new HashSet<string>() { "subjectId", "studyId", "sampleId", "studyDay#" };

            foreach (Biosample sample in samples)
            {
                Hashtable ht = new Hashtable();
                ht.Add("subjectId", sample.Subject != null ? sample.Subject.UniqueSubjectId : "missing");
                ht.Add("studyId", sample.Study.Name);
                ht.Add("sampleId", sample.BiosampleStudyId);
                //ht.Add("studyDay#", sample.CollectionStudyDay?.Number);
                sample_table.Add(ht);
            }
            Hashtable returnObject = new Hashtable();
            returnObject.Add("header", SCs);
            returnObject.Add("data", sample_table);

            return returnObject;

        }

        //public void addPA(int assayId, int fileId)
        //{

        //    //var fileService = new FileService(_dataContext);

        //    var dataFile = _dataFileRepository.Get(fileId);

        //    var filePath = dataFile.Path + "\\" + dataFile.FileName;
        //    //var filePath = "temp\\" + fileName;
        //    var dataTable = _fileService.ReadOriginalFile(filePath);

        //    var PA = new PlatformAnnotation();
        //    PA.Name = "CRC305ABC";
        //    PA.Accession = "D-PA-AGI-01";
        //    foreach (DataRow row in dataTable.Rows)
        //    {
        //        var fa = new FeatureAnnotation();
        //        fa.Name = row["ProbeName"].ToString();
        //        fa.FeatureId = row["ID"].ToString();
                
        //        foreach (DataColumn column in dataTable.Columns)
        //        {
        //            if (column.ColumnName == "ProbeName" || column.ColumnName == "ID")
        //                continue;
        //            var nv = new NV() {Name = column.ColumnName, Value = row[column].ToString()};

        //            fa.Properties.Add(nv);
        //        }
        //        PA.FeatureAnnotations.Add(fa);
        //    }
        //    _parepository.Insert(PA);

        //    var assay = _assayRepository.Get(assayId);
        //    assay.PlatformAnnotationId = PA.Name;
        //    _assayRepository.Update(assay);
        //    _dataContext.Save();
        //}







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
