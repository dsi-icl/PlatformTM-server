/************************************************************/
/********   Created By: Dilshan Silva 12-12-2014   **********/
/******** Services to handle functions on Activity **********/
/************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.DTOs;
using System.Linq.Expressions;
using eTRIKS.Commons.DataAccess.MongoDB;


namespace eTRIKS.Commons.Service.Services
{
    public class ActivityService
    {
        private readonly IRepository<Activity, int> _activityRepository;
        private readonly IRepository<Dataset, int> _dataSetRepository;
        private readonly IRepository<VariableDefinition, int> _variableDefinition;
        private readonly IRepository<Assay, int> _assayRepository;
        private readonly IRepository<Project, int> _projectRepository;
        private readonly IServiceUoW _activityServiceUnit;
        private readonly DatasetService _datasetService;


        public ActivityService(IServiceUoW uoW, DatasetService datasetService)
        {
            _activityServiceUnit = uoW;
            _activityRepository = uoW.GetRepository<Activity, int>();
            _assayRepository = uoW.GetRepository<Assay, int>();
            _dataSetRepository = uoW.GetRepository<Dataset, int>();
            _projectRepository = uoW.GetRepository<Project, int>();
            _variableDefinition =  uoW.GetRepository<VariableDefinition, int>();
            _datasetService = datasetService;
        }



        

        //public bool checkExist(int activityId)
        //{
        //    Activity activity = new Activity();
        //    activity =_activityRepository.Get(activityId);
        //    if (activity == null)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        public ActivityDTO GetActivity(int activityId)
        {
            Activity activity = _activityRepository.FindSingle(
                d => d.Id.Equals(activityId),
                new List<Expression<Func<Activity, object>>>(){
                        d => d.Datasets.Select(t => t.Domain)
                }
            );

            ActivityDTO activityDTO = new ActivityDTO();
            activityDTO.Name = activity.Name;
            activityDTO.Id = activity.Id;
            activityDTO.ProjectId = activity.ProjectId;
            if (activity.GetType() == typeof (Assay))
                activityDTO.isAssay = true;

            foreach (var ds in activity.Datasets)
            {
                DatasetDTO dst = _datasetService.GetActivityDatasetDTO(ds.Id);
                activityDTO.datasets.Add(dst);
            }
            return activityDTO;
        }

        public ActivityDTO AddActivity(ActivityDTO activityDTO)
        {
            var project = _projectRepository.FindSingle(d => d.Accession
                .Equals(activityDTO.ProjectAcc));
            var activity = new Activity { Name = activityDTO.Name, ProjectId = project.Id };
            foreach (var datasetDto in activityDTO.datasets)
            {
                datasetDto.ProjectId = project.Id;
                var dataset = _datasetService.CreateDataset(datasetDto);
                activity.Datasets.Add(dataset);
            }


            activity = _activityRepository.Insert(activity);
            if (_activityServiceUnit.Save().Equals("CREATED"))
            {
                activityDTO.Id = activity.Id;
                return activityDTO;
            }
            return null;
        }
     
        public string UpdateActivity(ActivityDTO activityDTO, int activityId)
        {
            Activity activityToUpdate = _activityRepository.Get(activityId);

            activityToUpdate.Name = activityDTO.Name;
            foreach (var datasetDto in activityDTO.datasets)
            {
                //datasetDto.ProjectId = project.Id;
                if (datasetDto.isNew)
                {
                    var dataset = _datasetService.CreateDataset(datasetDto);
                    activityToUpdate.Datasets.Add(dataset);
                }
                else
                    _datasetService.updateDataset(datasetDto);                
            }
            return _activityServiceUnit.Save();
        }

        public IEnumerable<ActivityDTO> GetStudyActivities(string projectAccession)
        {
            IEnumerable<Activity> activities;

            // Typed lambda expression for Select() method. 
            activities = _activityRepository.FindAll(
                    d => d.Project.Accession.Equals(projectAccession),
                    new List<Expression<Func<Activity, object>>>(){
                        d => d.Datasets.Select(t => t.Domain),
                        d => d.Project
                    }
                );
            return activities.Select(p => new ActivityDTO
            {
                Name = p.Name,
                Id = p.Id,
                ProjectId = p.ProjectId,
                ProjectAcc = p.Project.Accession,
                datasets = p.Datasets.Select(m => new DatasetDTO
                {
                    Name = m.Domain.Name,
                    Id = m.Id,
                    DomainId = m.DomainId
                }).ToList()
            }).ToList();
        }

        /**
         * Assay Methods
         * */

        public AssayDTO GetAssay(int assayId)
        {
            var assay = _assayRepository.FindSingle(
                d => d.Id.Equals(assayId),
                new List<Expression<Func<Assay, object>>>(){
                        d => d.Datasets.Select(t => t.Domain),
                        d => d.TechnologyType,
                        d => d.TechnologyPlatform,
                        d => d.MeasurementType,
                        d => d.DesignType
                }
            );

            var assayDTO = new AssayDTO();
            assayDTO.Name = assay.Name;
            assayDTO.Id = assay.Id;
            assayDTO.ProjectId = assay.ProjectId;

            assayDTO.Type = assay.MeasurementType.Id;
            assayDTO.Technology = assay.TechnologyType.Id;
            assayDTO.Platform = assay.TechnologyPlatform.Id;
            assayDTO.Design = assay.DesignType.Id;


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

            if(assayDto.SamplesDataset !=null) assayDto.SamplesDataset.ProjectId = project.Id;
            if (assayDto.FeaturesDataset != null) assayDto.FeaturesDataset.ProjectId = project.Id;
            if (assayDto.ObservationsDataset != null)  assayDto.ObservationsDataset.ProjectId = project.Id;

            var BSdataset = _datasetService.CreateDataset(assayDto.SamplesDataset);
            if(BSdataset != null)
                assay.Datasets.Add(BSdataset);

            var FEdataset = _datasetService.CreateDataset(assayDto.FeaturesDataset);
            if (FEdataset != null)
                assay.Datasets.Add(FEdataset);

            var OBdataset = _datasetService.CreateDataset(assayDto.ObservationsDataset);
            if (OBdataset != null)
                assay.Datasets.Add(OBdataset);

            assay = _assayRepository.Insert(assay);
           

            if (_activityServiceUnit.Save().Equals("CREATED"))
            {
                assayDto.Id = assay.Id;
                return assayDto;
            }
            return null;
        }

        public string UpdateAssay(AssayDTO assayDTO, int activityId)
        {
            Activity activityToUpdate = _activityRepository.Get(activityId);

            activityToUpdate.Name = assayDTO.Name;

            //foreach (var datasetDto in assayDTO.datasets)
            //{
                //datasetDto.ProjectId = project.Id;

                //if (datasetDto.isNew)
                //{
                //    var dataset = _datasetService.CreateDataset(datasetDto);
                //    activityToUpdate.Datasets.Add(dataset);
                //}
                //else
                //    _datasetService.updateDataset(datasetDto);
            //}
            return _activityServiceUnit.Save();
        }
    }
}
