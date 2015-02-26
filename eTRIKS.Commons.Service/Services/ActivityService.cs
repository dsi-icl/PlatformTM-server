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
        private IRepository<Activity, int> _activityRepository;
        private IRepository<Dataset, int> _dataSetRepository;
        private IRepository<VariableDefinition, int> _variableDefinition;
        private IServiceUoW _activityServiceUnit;


        public ActivityService(IServiceUoW uoW)
        {
            _activityServiceUnit = uoW;
            _activityRepository = uoW.GetRepository<Activity, int>();
            _dataSetRepository = uoW.GetRepository<Dataset, int>();
            _variableDefinition =  uoW.GetRepository<VariableDefinition, int>();
        }

        private static readonly Expression<Func<Activity, ActivityDTO>> AsBookDto =
        x => new ActivityDTO
        {
            Name = x.Name,
            StudyID = x.StudyId,
            datasets = x.Datasets.Select(m => new DatasetBriefDTO
            {
                Name = m.Domain.Name,
                Id = m.OID
            }).ToList()
        };

        public Activity addActivity(ActivityDTO activityDTO)
        {
            Activity activity = new Activity();
            activity.Name = activityDTO.Name;
            activity.StudyId = activityDTO.StudyID;

            _activityRepository.Insert(activity);
            if (_activityServiceUnit.Save().Equals("CREATED")){
                return activity;
            }
            return null;
        }

        public bool checkExist(int activityId)
        {
            Activity activity = new Activity();
            activity =_activityRepository.GetById(activityId);
            if (activity == null)
            {
                return false;
            }
            return true;
        }


        public ActivityDTO getActivityDTOById(int activityId)
        {
            Activity activity = _activityRepository.GetSingle(
                d => d.OID.Equals(activityId),
                new List<Expression<Func<Activity, object>>>(){
                        d => d.Datasets.Select(t => t.Domain)
                }
            );

            ActivityDTO activityDTO = new ActivityDTO();
            activityDTO.Name = activity.Name;
            activityDTO.Id = activity.OID;
            activityDTO.StudyID = activity.StudyId;

            foreach (var ds in activity.Datasets){
                DatasetBriefDTO dst = new DatasetBriefDTO();
                dst.Name = ds.Domain.Name;
                dst.Id = ds.OID;
                activityDTO.datasets.Add(dst);
            }
            return activityDTO;
        }


        // Method for data Visulaiser
        public IEnumerable<ClinicalDataTreeDTO> getActivityData(string studyId)
        {
            //&& d.Name.Equals(role))
            string role = "CL-Role-T-2";

            IEnumerable<Activity> activity = _activityRepository.Get(
                d => d.StudyId.Equals(studyId),
                  new List<Expression<Func<Activity, object>>>(){
                        d => d.Datasets.Select(t => t.Domain), d=>d.Datasets.Select(t=>t.Variables),
                         d => d.Datasets.Select(t => t.Domain), d=>d.Datasets.Select(
                                                                    t=>t.Variables.Select(k=>k.VariableDefinition))
                }
            );


            List<Activity> activity_list = activity.ToList();

            //1. Extract data for clinical tree
            List<ClinicalDataTreeRecordSummary> extractedClinicalTreeRecordList = new List<ClinicalDataTreeRecordSummary>();
            for (int i = 0; i < activity_list.Count; i++)
            {
                for (int j = 0; j < activity_list[i].Datasets.Count; j++)
                {
                    ClinicalDataTreeRecordSummary clinicalTreeRecord = new ClinicalDataTreeRecordSummary();
                    clinicalTreeRecord.Class = activity_list[i].Datasets.Select(f => f.Domain.Class).ToList()[j];
                    clinicalTreeRecord.Name = activity_list[i].Name.ToString();
                    clinicalTreeRecord.Domain = activity_list[i].Datasets.Select(k => k.Domain.Name).ToList()[j];
                    clinicalTreeRecord.code = activity_list[i].Datasets.Select(f => f.Domain.Code).ToList()[j];

                    List<VariableDefinition> datasetList = activity_list[i].Datasets.Select(g => g.Variables.Select(l => l.VariableDefinition))
                                                                    .ToList()[0].ToList();
                    for (int k = 0; k < datasetList.Count(); k++)
                    {
                        if (datasetList[k].RoleId.ToString() == role)
                        {
                            clinicalTreeRecord.variableDefinition = datasetList[k].Name.ToString();
                        }
                    }
                    extractedClinicalTreeRecordList.Add(clinicalTreeRecord);
                }
            }

            //2.  Group extractedClinicalTreeRecordList on attribute class
            List<ClinicalDataTreeDTO> cdTreeList = new List<ClinicalDataTreeDTO>();

            var groupedClinicalTreeRecordList = extractedClinicalTreeRecordList.GroupBy(u => u.Class).Select(grp => grp.ToList()).ToList();
            for (int i = 0; i < groupedClinicalTreeRecordList.Count(); i++)
            {
                ClinicalDataTreeDTO cdTree = new ClinicalDataTreeDTO();
                cdTree.Class = groupedClinicalTreeRecordList[i][0].Class;
                for (int j = 0; j < groupedClinicalTreeRecordList[i].Count(); j++)
                {
                    ClinicalDataTreeActivityDTO cdTreeActivity = new ClinicalDataTreeActivityDTO();
                    cdTreeActivity.Name = groupedClinicalTreeRecordList[i][j].Name;
                    cdTreeActivity.Domain = groupedClinicalTreeRecordList[i][j].Domain;
                    cdTreeActivity.code = groupedClinicalTreeRecordList[i][j].code;
                    cdTree.Activities.Add(cdTreeActivity);

                    MongoDbDataRepository mongoDataService = new MongoDbDataRepository();
                    NoSQLRecordSet recordSet = null;
                    
                    //3.  IF Code = LB then get category data from NOSQL
                    if (cdTreeActivity.code == "LB")
                    {
                        string queryString = "?DOMAIN=" + cdTreeActivity.code + "&" + groupedClinicalTreeRecordList[i][j].variableDefinition + "=*&LBCAT=*";
                        recordSet = mongoDataService.getNoSQLRecords(queryString);
                        var filteredRecordSet = recordSet.RecordSet.Select(u => new { code =  u.RecordItems[0].value, category= u.RecordItems[1].value}).Distinct();
                        var groupedRecordSet = filteredRecordSet.GroupBy(k => k.category).Select(grp => grp.ToList()).ToList();

                        string code = "LB-CAT";
                        for (int k = 0; k < groupedRecordSet.Count(); k++)
                        {
                            Observation observation = new Observation();
                            observation.Name = groupedRecordSet[k][0].category;
                            observation.code = code + k;
                            List<string> observationList = new List<string>();
                            for (int m = 0; m < groupedRecordSet[k].Count(); m++)
                            {
                               observationList.Add(groupedRecordSet[k][m].code);
                            }
                            observation.ObservationList = observationList;
                            cdTreeActivity.Observations.Add(observation); 
                        }
                    }
                    // 4. For Code = VS or AE category are not used
                    else
                    {
                        string queryString = "?DOMAIN=" + cdTreeActivity.code + "&" + groupedClinicalTreeRecordList[i][j].variableDefinition + "=*";
                        // Call NOSQL to get list
                        recordSet = mongoDataService.getDistinctNoSQLRecords(queryString);
                        Observation observation = new Observation();
                        observation.code = "TEST";
                        List<string> observationList = new List<string>();
                        for (int g = 0; g < recordSet.RecordSet[0].RecordItems.Count; g++)
                        {
                            observationList.Add(recordSet.RecordSet[0].RecordItems[g].value);
                        }
                        observation.ObservationList = observationList;
                        cdTreeActivity.Observations.Add(observation); 
                    } 
                }
                cdTreeList.Add(cdTree);
            }
            return cdTreeList;
        }

        public IEnumerable<ActivityDTO> getStudyActivities(string studyId)
        {
            IEnumerable<Activity> activities;

            // Typed lambda expression for Select() method. 
            activities = _activityRepository.Get(
                    d => d.StudyId.Equals(studyId),
                    new List<Expression<Func<Activity, object>>>(){
                        d => d.Datasets.Select(t => t.Domain)
                    }
                );
            return activities.Select(p => new ActivityDTO
            {
                Name = p.Name,
                Id = p.OID,
                StudyID = p.StudyId,
                datasets = p.Datasets.Select(m => new DatasetBriefDTO
                {
                    Name = m.Domain.Name,
                    Id = m.OID
                }).ToList()
            }).ToList();
        }

        public string updateActivity(ActivityDTO activityDTO, int activityId)
        {
            Activity activityToUpdate = _activityRepository.GetById(activityId);

            activityToUpdate.Name = activityDTO.Name;
            return _activityServiceUnit.Save();
        }
    }
}
