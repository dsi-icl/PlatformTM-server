using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using eTRIKS.Commons.DataAccess.MongoDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Service.Services
{
    public class ObservationService
    {
        private IServiceUoW _dataContext;
        private IRepository<Activity, int> _activityRepository;
        private IRepository<Observation, int> _ObservationRepository;
        private IRepository<VariableDefinition, int> _variableRepository;
        //private IRepository<SubjectCharacteristic, int> subjCharacteristicsRepository;

        //MongoDB Repositories
        private IRepository<Subject, Guid> subjectRepository;

        private IRepository<Study, String> studyRepository; 
        
        public ObservationService(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _activityRepository = uoW.GetRepository<Activity, int>();
            _ObservationRepository = uoW.GetRepository<Observation, int>();
            //_subjObsRepository = uoW.GetRepository<SubjectObservation,Guid>();
            //mongoRepository = uoW.GetRepository<SubjectObservation, Guid>();

            _variableRepository = uoW.GetRepository<VariableDefinition, int>();
            //subjCharacteristicsRepository = uoW.GetRepository<SubjectCharacteristic, int>();
            subjectRepository = uoW.GetRepository<Subject, Guid>();

            studyRepository = uoW.GetRepository<Study, string>();
        }

        /***
         * this method CREATES the SQL Observation Objects By combining Variable definitions from
         * SQL and Values from the NoSQL subject Observations (the instances of observations)
         * these observations will be the source to :
         * - build the clinical tree (no need for the aggregated query)
         * - use to query subjectObservations (data) for data plots
         * This method then assumes data is already loaded in the NoSQL data
         * OR we could imagine an input with observation values WITHOUT havind the data
         * */
        public async Task loadObservations(string studyId)
        {
            
            MongoDbDataRepository mongoDataService = new MongoDbDataRepository();

            //studyId should essentially be projectId
            //Now Load observations shuold be changed to add unique observations but add to study_observations each time an observation is used in a study

            //TODO: Change Activity-Study relationship to many-to-many
            List<Activity> activity_list = _activityRepository.FindAll(
               d => d.StudyId.Equals(studyId),
                 new List<Expression<Func<Activity, object>>>(){
                        d => d.Datasets.Select(t => t.Domain), 
                        d=> d.Datasets.Select(t=>t.Variables),
                        d => d.Datasets.Select(t => t.Domain),
                        d => d.Study,
                        d=>d.Datasets.Select(t=>t.Variables.Select(k=>k.VariableDefinition))
                }).ToList();

            //TEMP
            List<Study> studies = studyRepository.FindAll(p => p.ProjectId.Equals(1)).ToList();

            //List<Activity> activity_list = _activityRepository.FindAll(
            //   d => d.Studies.Select(p => p.)Equals(studyId),
            //     new List<Expression<Func<Activity, object>>>(){
            //            d => d.Datasets.Select(t => t.Domain), 
            //            d=> d.Datasets.Select(t=>t.Variables),
            //            d => d.Datasets.Select(t => t.Domain),
            //            d => d.Study,
            //            d=>d.Datasets.Select(t=>t.Variables.Select(k=>k.VariableDefinition))
            //    }).ToList();
            

            foreach (Activity activity in activity_list)
            {
                foreach (Dataset ds in activity.Datasets)
                {
                    List<VariableDefinition> qualifiers;
                    List<VariableDefinition> timings;
                    VariableDefinition topic;
                    VariableDefinition controlledTerm = null;
                    VariableDefinition defaultQualifier = null;
                    string obsName, controlledTermName, obsClass;

                    obsClass = ds.Domain.Class.ToLower();


                    if (obsClass.Equals("relationship") || ds.Domain.Code.Equals("VS")
                        || ds.Domain.Code.Equals("LB") || ds.Domain.Code.Equals("DM"))
                       continue;

                    if (ds.Domain.Code.Equals("DM"))
                    {
                        loadSubjectCharacteristics(studyId, ds, studies);
                        continue;
                    }

                    qualifiers = ds.Variables
                        .Select(l => l.VariableDefinition)
                        .Where(v => v.RoleId == "CL-Role-T-3" || v.RoleId == "CL-Role-T-8")
                        .ToList();

                    timings = ds.Variables
                        .Select(l => l.VariableDefinition)
                        .Where(v => v.RoleId == "CL-Role-T-6")
                        .ToList();

                    topic = ds.Variables
                        .Select(l => l.VariableDefinition)
                        .Where(v => v.RoleId == "CL-Role-T-2")
                        .First();

                    controlledTerm = ds.Variables
                        .Select(l => l.VariableDefinition)
                        .Where(v => v.Name == ds.Domain.Code+"DECOD")
                        .FirstOrDefault();

                    if(obsClass.Equals("findings")){

                        controlledTerm = ds.Variables
                                            .Select(l => l.VariableDefinition)
                                            .Where(v => v.Name == ds.Domain.Code+"TEST")
                                            .FirstOrDefault();
                        //topic = ds.Variables
                        //                    .Select(l => l.VariableDefinition)
                        //                    .Where(v => v.Name == ds.Domain.Code + "TESTCD")
                        //                    .FirstOrDefault();
                        obsName = controlledTerm.Name;
                        controlledTermName = topic.Name;
                        defaultQualifier = getVarDef(ds.Domain.Code + "ORRES");
                    }

                    if(obsClass.Equals("events"))
                        defaultQualifier = getVarDef(ds.Domain.Code + "SEV");
                    obsName = topic.Name;
                    controlledTermName = controlledTerm.Name;
 

                    Dictionary<string, string> filterFields = new Dictionary<string, string>();
                    filterFields.Add("DOMAIN", ds.Domain.Code);

                    //TODO add studyIds for this project not to mix data with other projects
                    //filterFields.Add("STUDYID", studyId);

                    Dictionary<string, string> groupFields = new Dictionary<string, string>();
                    groupFields.Add("Name", "$"+ obsName);
                    groupFields.Add("ControlledTermStr", "$" + controlledTerm.Name);
                    groupFields.Add("Group", "$" + ds.Domain.Code + "CAT");
                    groupFields.Add("Subgroup", "$" + ds.Domain.Code + "SCAT");
                    //groupFields.Add("Studies", "$" + ds.Domain.Code + "SCAT");


                    List<Observation> observations = await mongoDataService.getGroupedNoSQLrecords(filterFields, groupFields);
                    //List<SubjectObservation> subjObs =  _subjObsRepository.FindAll(x => x.DomainCode == ds.Domain.Code).ToList();
                    //var subjectObservations = _subjObsRepository.AggregateAsync<string,string>(
                    //                                    x => x.DomainCode == x.DomainCode,
                    //                                    x => new { x.Name, x.Group, x.Subgroup }.ToString(),
                    //                                    g => new { _id = g.Key }.ToString()

                    //                                    );
                   // people.ToString();
                   // subjObs.GroupBy(x => new { x.Name, x.Group, x.Subgroup });

                    foreach (Observation obs in observations)
                    {
                        obs.Class = ds.Domain.Class;
                        obs.DomainCode = ds.Domain.Code;
                        obs.DomainName = ds.Domain.Name;
                        obs.TopicVariable = topic;
                        obs.Qualifiers = qualifiers;
                        obs.Timings = timings;
                        //obs.ControlledTerm = controlledTerm;
                        if (obsClass.Equals("findings"))
                            obs.DefaultQualifier = defaultQualifier;

                        if (obsClass.Equals("events"))
                            obs.DefaultQualifier = defaultQualifier;
                        //obs.Studies.Add(activity.Study);
                        obs.Studies.AddRange(studies);
                        _ObservationRepository.Insert(obs);
                    }
                    _dataContext.Save();                       
                }
                
            }

            
        }

        public void loadSubjectCharacteristics(string projectId, Dataset ds, List<Study> studies)
        {
            //If dataset is SC or SUPPDM ... then I need to read values from the Data file SC or SUPPDM from SQL to pivot them
            //Dictionary<string, string> filterFields = new Dictionary<string, string>();
            //filterFields.Add("DOMAIN", ds.Domain.Code);
            ////filterFields.Add("STUDYID", studyId);

            //Dictionary<string, string> groupFields = new Dictionary<string, string>();
            //groupFields.Add("Name", "$" + obsName);
            //groupFields.Add("ControlledTermStr", "$" + controlledTerm.Name);
            //groupFields.Add("Group", "$" + ds.Domain.Code + "CAT");
            //groupFields.Add("Subgroup", "$" + ds.Domain.Code + "SCAT");

            //List<Subject> subjects = await subjectRepository.GetAllAsync();

            //If dataset is DM I dont need to query NoSQL (DM data) to get AGE, RACE ...etc since they already are variables
            //which I have in the template definition.
            List<VariableDefinition> qualifiers = ds.Variables
                        .Select(l => l.VariableDefinition)
                        .Where(v => v.RoleId == "CL-Role-T-3")
                        .ToList();
            foreach (var variable in qualifiers)
            {
                Observation sc = new Observation();
                sc.Name = variable.Label;
                sc.TopicVariable = variable;
                sc.DefaultQualifier = variable;
                sc.ControlledTermStr = variable.Name;
                sc.DomainCode = ds.Domain.Code;
                sc.Class = ds.Domain.Class;
                sc.DomainName = ds.Domain.Name;
                sc.Studies.AddRange(studies);
                sc.isSubjCharacteristic = true;
                _ObservationRepository.Insert(sc);
            }
            _dataContext.Save();

        }

        public void getObservationInventory(string projectId)
        {
            List<Hashtable> table = new List<Hashtable>();
            Hashtable ht;
            string studyId = "STD-BVS-01";
            List<Observation> studyObservations =
                _ObservationRepository.FindAll(
                    x => x.Studies.Select(s => s.Id).Contains(studyId),
                    new List<Expression<Func<Observation, object>>>()
                    {
                        d=>d.TopicVariable,
                        d => d.Timings,
                        d => d.Qualifiers,
                        d => d.DefaultQualifier
                    }
                    ).ToList();

            foreach (var obs in studyObservations)
            {
                //ht = new Hashtable();
                //ht.Add("Name", obs.Name);
                Debug.WriteLine("Name: "+obs.Name);
                foreach (var qualifier in obs.Qualifiers)
                {
                    Debug.WriteLine("Qualifier: " + qualifier.Label);
                }
                foreach (var timingVar in obs.Timings)
                {
                    Debug.WriteLine("Timing Variable: " + timingVar.Label);
                }
                Debug.WriteLine("**********************");
            }
        }
        private VariableDefinition getVarDef(string name)
        {
            return _variableRepository.FindSingle(d => d.Name.Equals(name));
        }
    }
}
