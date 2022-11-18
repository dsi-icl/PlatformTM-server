using System.Collections.Generic;
using System.Linq;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Models.DTOs;

namespace PlatformTM.Models.Services
{
    public class AssayService
    {
       
        private readonly IRepository<Biosample, int> _bioSampleRepository;
        private readonly IRepository<Project, int> _projectRepository;
        private readonly IRepository<Assay, int> _assayRepository;
        private readonly DatasetDescriptorService _datasetDescriptorService;
        private readonly IServiceUoW _dataContext;

        public AssayService(IServiceUoW uoW, DatasetDescriptorService datasetService)
        {
            _dataContext = uoW;
            _bioSampleRepository = uoW.GetRepository<Biosample, int>();
            _projectRepository = uoW.GetRepository<Project, int>();
            _assayRepository = uoW.GetRepository<Assay, int>();
            _datasetDescriptorService = datasetService;
        }

        /**
        * Assay Methods
        * */

        //public AssayDTO GetAssay(int assayId)
        //{
        //    var assay = _assayRepository.FindSingle(
        //        d => d.Id.Equals(assayId),
        //        new List<string>(){
        //                "Datasets.Template",
        //                "TechnologyType",
        //                "TechnologyPlatform",
        //                "MeasurementType",
        //                "DesignType",
        //                "Study"
        //        }
        //    );

        //    var assayDTO = new AssayDTO();
        //    assayDTO.Name = assay.Name;
        //    assayDTO.Id = assay.Id;
        //    assayDTO.ProjectId = assay.Study.ProjectId;

        //    assayDTO.Type = assay.MeasurementType?.Id;
        //    assayDTO.Technology = assay.TechnologyType?.Id;
        //    assayDTO.Platform = assay.TechnologyPlatform?.Id;
        //    assayDTO.Design = assay.DesignType?.Id;


        //    foreach (var dst in assay.Datasets.Select(ds => _datasetDescriptorService.GetActivityDatasetDTO(ds.Id)))
        //    {
        //        //TODO: convert to enums or CVterms
        //        if (dst.Class == "Assay Samples")
        //            assayDTO.SamplesDataset = dst;
        //        if (dst.Class == "Assay Observations")
        //            assayDTO.ObservationsDataset = dst;
        //        if (dst.Class == "Assay Features")
        //            assayDTO.FeaturesDataset = dst;
        //    }
        //    return assayDTO;
        //}

        public AssayDTO AddAssay(AssayDTO assayDto)
        {
            
            var assay = new Assay();
            //var project = _projectRepository.FindSingle(d => d.Accession
            //    .Equals(assayDto.ProjectAcc));

            assay.Name = assayDto.Name;
            //assay.ProjectId = assayDto.ProjectId;
            assay.TechnologyPlatformId = assayDto.Platform;
            assay.TechnologyTypeId = assayDto.Technology;
            //assay.DesignType = getCVterm(assayDto.AssayDesignType);
            assay.MeasurementTypeId = assayDto.Type;

            if (assayDto.SamplesDataset != null) assayDto.SamplesDataset.ProjectId = assayDto.ProjectId;
            if (assayDto.FeaturesDataset != null) assayDto.FeaturesDataset.ProjectId = assayDto.ProjectId;
            if (assayDto.ObservationsDataset != null) assayDto.ObservationsDataset.ProjectId = assayDto.ProjectId;

            //var BSdataset = _datasetDescriptorService.CreateDataset(assayDto.SamplesDataset);
            //if (BSdataset != null)
            //    assay.Datasets.Add(BSdataset);

            //var FEdataset = _datasetDescriptorService.CreateDataset(assayDto.FeaturesDataset);
            //if (FEdataset != null)
            //    assay.Datasets.Add(FEdataset);

            //var OBdataset = _datasetDescriptorService.CreateDataset(assayDto.ObservationsDataset);
            //if (OBdataset != null)
            //    assay.Datasets.Add(OBdataset);

            assay = _assayRepository.Insert(assay);


            if (_dataContext.Save().Equals("CREATED"))
            {
                assayDto.Id = assay.Id;
                return assayDto;
            }
            return null;
        }

        //public string UpdateAssay(AssayDTO assayDTO, int assayId)
        //{
            //Assay assayToUpdate = _assayRepository.Get(assayId);


            //assayToUpdate.Name = assayDTO.Name;
            //assayToUpdate.TechnologyPlatformId = assayDTO.Platform;
            //assayToUpdate.TechnologyTypeId = assayDTO.Technology;
            ////assay.DesignType = getCVterm(assayDto.AssayDesignType);
            //assayToUpdate.MeasurementTypeId = assayDTO.Type;

            //assayDTO.Datasets.Clear();
            //assayDTO.Datasets.Add(assayDTO.FeaturesDataset);
            //assayDTO.Datasets.Add(assayDTO.SamplesDataset);
            //assayDTO.Datasets.Add(assayDTO.ObservationsDataset);


            //foreach (var datasetDto in assayDTO.Datasets)
            //{

            //    if (datasetDto == null) continue;
            //    datasetDto.ProjectId = assayDTO.ProjectId;
            //    if (datasetDto.IsNew)
            //    {
            //        var dataset = _datasetDescriptorService.CreateDataset(datasetDto);
            //        //assayToUpdate.Datasets.Add(dataset);
            //        _assayRepository.Update(assayToUpdate);
            //    }
            //    else
            //        _datasetDescriptorService.UpdateDataset(datasetDto);
            //}
            //return _dataContext.Save();
        //}

        

        

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
        //public Hashtable GetSamplesDataPerAssay(int assayId)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
