using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.Data;
using eTRIKS.Commons.DataAccess.MongoDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eTRIKS.Commons.Service.Services.HelperService;
using Observation = eTRIKS.Commons.Core.Domain.Model.Observation;
using eTRIKS.Commons.Core.Domain.Model.Data.SDTM;

namespace eTRIKS.Commons.Service.Services
{
    public class ObservationService
    {
        private readonly IServiceUoW _dataContext;
        private readonly IRepository<Activity, int> _activityRepository;
         private readonly IRepository<Observation, int> _ObservationRepository;
        private readonly IRepository<VariableDefinition, int> _variableRepository;
        private readonly IRepository<Dataset, int> _datasetRepository;
        //private readonly IRepository<ObjectOfObservation, int> _O3Repository;
        private readonly IRepository<PropertyDescriptor, int> _ObsDescriptorRepository;
        private readonly IRepository<ObservationDescriptor, int> _ObsDescriptionRepository;

        //private IRepository<SubjectCharacteristic, int> subjCharacteristicsRepository;

        //MongoDB Repositories
        private IRepository<HumanSubject, string> subjectRepository;

        private IRepository<Study, int> _studyRepository;


        public ObservationService(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _activityRepository = uoW.GetRepository<Activity, int>();
            _ObservationRepository = uoW.GetRepository<Observation, int>();
            _datasetRepository = uoW.GetRepository<Dataset, int>();
            //_subjObsRepository = uoW.GetRepository<SubjectObservation,Guid>();
            //mongoRepository = uoW.GetRepository<SubjectObservation, Guid>();

            _variableRepository = uoW.GetRepository<VariableDefinition, int>();
            //subjCharacteristicsRepository = uoW.GetRepository<SubjectCharacteristic, int>();
            subjectRepository = uoW.GetRepository<HumanSubject, string>();

            _studyRepository = uoW.GetRepository<Study, int>();
            //_O3Repository = uoW.GetRepository<ObjectOfObservation, int>();
            _ObsDescriptionRepository = uoW.GetRepository<ObservationDescriptor, int>();
            _ObsDescriptorRepository = uoW.GetRepository<PropertyDescriptor, int>();
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
        /*
        public async Task loadObservations(string studyId)
        {
			
            MongoDbDataRepository mongoDataService = new MongoDbDataRepository();

            //studyId should essentially be projectId
            //Now Load observations shuold be changed to add unique observations but add to study_observations each time an observation is used in a study

            //TODO: Change Activity-Study relationship to many-to-many
            List<Activity> activity_list = _activityRepository.FindAll(
               d => d.ProjectId.Equals(studyId),
                 new List<Expression<Func<Activity, object>>>(){
                        d => d.Datasets.Select(t => t.Domain), 
                        d=> d.Datasets.Select(t=>t.Variables),
                        d => d.Datasets.Select(t => t.Domain),
                        d => d.Project,
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

                    //if (ds.Domain.Code.Equals("DM"))
                    //{
                    //    loadSubjectCharacteristics(studyId, ds, studies);
                    //    continue;
                    //}

                    qualifiers = ds.Variables
                        .Select(l => l.VariableDefinition)
                        .Where(v => v.RoleId == "CL-Role-T-3" || v.RoleId == "CL-Role-T-8")
                        .ToList();

                    timings = ds.Variables
                        .Select(l => l.VariableDefinition)
                        .Where(v => v.RoleId == "CL-Role-T-6")
                        .ToList();

                    topic = ds.Variables
                        .Select(l => l.VariableDefinition).First(v => v.RoleId == "CL-Role-T-2");

                    controlledTerm = ds.Variables
                        .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == ds.Domain.Code+"DECOD");

                    if(obsClass.Equals("findings")){

                        controlledTerm = ds.Variables
                            .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == ds.Domain.Code+"TEST");
                        //topic = ds.Variables
                        //                    .Select(l => l.VariableDefinition)
                        //                    .Where(v => v.Name == ds.Domain.Code + "TESTCD")
                        //                    .FirstOrDefault();
                        obsName = controlledTerm.Name;
                        controlledTermName = topic.Name;
                        defaultQualifier = null;//getVarDef(ds.Domain.Code + "ORRES");
                    }

                    if (obsClass.Equals("events"))
                        defaultQualifier = null;//getVarDef(ds.Domain.Code + "SEV");
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
         */

        /*
        public void loadSubjectCharacteristics(Dataset ds)
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
                sc.Studies.Add(ds.Activity.Study);
                sc.isSubjCharacteristic = true;
                _ObservationRepository.Insert(sc);
            }
            _dataContext.Save();

            //List<SdtmEntity> subjectData = await _sdtmRepository.FindAllAsync(
             //       bs => bs.DatasetId.Equals(datasetId));

        }
         */

        /*
        public void getObservationInventory(string projectId)
        {
            List<Hashtable> table = new List<Hashtable>();
            Hashtable ht;
            string studyId = "STD-BVS-01";
            List<Observation> studyObservations =
                _ObservationRepository.FindAll(
                    x => x.Studies.Select(s => s.Accession).Contains(studyId),
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
        */
        private VariableDefinition getVarDef(string name, int projectId)
        {
            return _variableRepository.FindSingle(d => d.Name.Equals(name) && d.ProjectId.Equals(projectId));
        }
        
        public async Task<bool> loadDatasetObservations(Dataset ds,int dataFileId)
        {
            VariableDefinition controlledTerm = null;
            VariableDefinition defaultQualifier = null;
            string obsName;
            string obsClass = ds.Domain.Class.ToLower();
            if (obsClass.Equals("relationship"))
                return true;
            //Record qualifiers & Result qualifiers
            List<VariableDefinition> qualifiers = ds.Variables
                .Select(l => l.VariableDefinition)
                .Where(v => v.RoleId == "CL-Role-T-3" || v.RoleId == "CL-Role-T-8")
                .ToList();

            List<VariableDefinition> timings = ds.Variables
                .Select(l => l.VariableDefinition)
                .Where(v => v.RoleId == "CL-Role-T-6")
                .ToList();

            VariableDefinition topic = ds.Variables
                .Select(l => l.VariableDefinition).First(v => v.RoleId == "CL-Role-T-2");

            controlledTerm = ds.Variables
                .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == ds.Domain.Code + "DECOD");

            if (obsClass.Equals("findings"))
            {

                controlledTerm = ds.Variables
                    .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == ds.Domain.Code + "TEST");
                //topic = ds.Variables
                //                    .Select(l => l.VariableDefinition)
                //                    .Where(v => v.Name == ds.Domain.Code + "TESTCD")
                //                    .FirstOrDefault();
                obsName = controlledTerm.Name;
                defaultQualifier = getVarDef(ds.Domain.Code + "ORRES",ds.Activity.ProjectId);
            }

            if (obsClass.Equals("events"))
            {
                if(ds.Domain.Code.Equals("AE"))
                    defaultQualifier = getVarDef(ds.Domain.Code + "SEV", ds.Activity.ProjectId);
                else
                {
                    defaultQualifier = getVarDef(ds.Domain.Code + "OCCUR", ds.Activity.ProjectId);
                }
            }
                
            
            obsName = topic.Name;

            //Formulate Query
            var filterFields = new Dictionary<string, object>
            {
                {"DOMAIN", ds.Domain.Code},
                {"DBPROJECTACC", ds.Activity.Project.Accession},
                {"DBDATASETID",ds.Id},
                {"DBDATAFILEID",dataFileId}
            };

            var groupFields = new Dictionary<string, string>
            {
                {"Name", "$" + obsName},


                // {"Name", "$" + controlledTerm.Name}, ///FOR AEs only skip AETERM and only use DECOD
                                                      ///
                                                      /// 
               
                {"Group", "$" + ds.Domain.Code + "CAT"},
                {"Subgroup", "$" + ds.Domain.Code + "SCAT"}
            };
            if(controlledTerm!=null)
                groupFields.Add("ControlledTermStr", "$" + controlledTerm.Name);
            //End of formulate Query

            MongoDbDataRepository mongoDataService = new MongoDbDataRepository();
            List<Observation> observations = await mongoDataService.getGroupedNoSQLrecords(filterFields, groupFields);

            List<Observation> curr_observations =
                _ObservationRepository.FindAll(o => o.ProjectId == ds.Activity.ProjectId && o.DomainCode.Equals(ds.Domain.Code)).ToList();

            List<string> obsKeys = new List<string>();
            foreach (var currObservation in curr_observations)
            {
                obsKeys.Add(currObservation.Name+currObservation.Group+currObservation.Class);
            }
                
                observations = observations.Where(o => o.Group != "").ToList();

            foreach (Observation obs in observations)
            {

                string key = obs.Name + obs.Group + ds.Domain.Class;
                if(obsKeys.Contains(key))
                    continue;
                obs.Class = ds.Domain.Class;
                obs.DomainCode = ds.Domain.Code;
                obs.DomainName = ds.Domain.Name;
                obs.TopicVariable = topic;
                obs.Qualifiers = qualifiers;
                obs.Timings = timings;

                
               // obs.ControlledTermStr = controlledTerm;
 
                obs.DefaultQualifier = defaultQualifier;
                //TODO: CHECK THAT
                //obs.Studies.AddRange(ds.Activity.Project.Studies);
                obs.ProjectId = ds.Activity.ProjectId;
                //obs.DatafileId = dataFileId;
                //obs.DatasetId = ds.Id;
                _ObservationRepository.Insert(obs);
            }

            return _dataContext.Save().Equals("CREATED");
        }

        public void DeleteDatasetObservations(int DatasetId, int DatafileId)
        {
            _ObservationRepository.DeleteMany(o => o.DatasetId == DatasetId && o.DatafileId == DatafileId );
        }
        /*
        public void LoadObservations(Dataset dataset, SdtmEntityDescriptor sdtmEntityDescriptor, List<SdtmEntity> sdtmRowList)
        {

            //Retrieve all Observtion Descriptors for this domain and this project if previously a dataset of the same domain has been loaded?
            var obsDescriptionList = _ObsDescriptionRepository.FindAll(
                s => s.ProjectId.Equals(dataset.Activity.ProjectId) && s.DomainId.Equals(dataset.DomainId),
                new List<Expression<Func<ObservationDescriptor, object>>>() { od => od.O3 }).ToList();

            var O3List = new List<ObjectOfObservation>();
            if (obsDescriptionList.Count != 0)
                O3List.AddRange(obsDescriptionList.Select(observationDescription => observationDescription.O3));

            #region Create O3 and ObservationDescription
            foreach (var sdtmRow in sdtmRowList)
            {
                //O3 name is not enough to uniquely identify the observation in place
                //e.g. white blood cells with two categories Heamatology and Chemistry
                //TODO: if an observation was previoulsy loaded via a dataset that had no group for it
                //e.g. white blood cells that was meant to be heamtology but not explicitly mentioned
                //then the same white blood cells is found but with a group, it will be loaded as a different O3
                //In any case, the CVterm of whitebloodcells for heamatology should be different from whitebloodcell chemsitry
                //ACTUALLY TOPIC variable SHOULD not be white blood cells but rather the TEST itself which in this case would be
                //Heamtatology test
                string O3 = sdtmRow.TopicVariable; //THIS IS --TESTCD AND --TERM FOR NOW
                ObjectOfObservation O3obj;
                ObservationDescriptor obsDescriptor;



                if (sdtmRow.Group != null && sdtmRow.Subgroup != null)
                    O3obj = O3List.Find(o3 => o3.Name.Equals(O3)
                                              && o3.ProjectId.Equals(dataset.Activity.ProjectId)
                                              && o3.Domain.Code.Equals(sdtmRow.DomainCode)
                                              && o3.Group.Equals(sdtmRow.Group)
                                              && o3.Subgroup.Equals(sdtmRow.Subgroup));
                else if (sdtmRow.Group != null)
                {
                    O3obj = O3List.Find(o3 => o3.Name.Equals(O3)
                                              && o3.ProjectId.Equals(dataset.Activity.ProjectId)
                                              && o3.Domain.Code.Equals(sdtmRow.DomainCode)
                                              && o3.Group.Equals(sdtmRow.Group));
                }
                else
                    O3obj = O3List.Find(o3 => o3.Name.Equals(O3)
                                              && o3.ProjectId.Equals(dataset.Activity.ProjectId)
                                              && o3.Domain.Code.Equals(sdtmRow.DomainCode));
                if (O3obj != null) continue;

                //Create new O3
                O3obj = new ObjectOfObservation();
                O3obj.Name = sdtmRow.TopicVariable; //EOS
                O3obj.Synonym = sdtmRow.TopicVariableSynonym ?? null; //Eosinophils
                O3obj.ProjectId = dataset.Activity.ProjectId; //id for P-BVS
                O3obj.DomainId = dataset.DomainId; //id for D-SDTM-LB
                O3obj.CVTermStr = sdtmRow.TopicVariableControlledTerm ?? null; //TEMP //Value of DECOD or LOINC
                //O3obj.CVtermId = lookupCVterm(sdtmRow.TopicVariableControlledTerm);
                O3obj.Group = sdtmRow.Group; //Hematology
                O3obj.Subgroup = sdtmRow.Subgroup; //value of SCAT
                //_O3Repository.Insert(O3obj);
                O3List.Add(O3obj);

                //Create ObservationDescriptor
                obsDescriptor = new ObservationDescriptor();
                obsDescriptor.O3 = O3obj;
                obsDescriptor.DomainId = dataset.DomainId;
                obsDescriptor.ProjectId = dataset.Activity.ProjectId;

                _ObsDescriptionRepository.Insert(obsDescriptor);

            }
            _dataContext.Save();
            #endregion

            obsDescriptionList = _ObsDescriptionRepository.FindAll(
                s => s.ProjectId.Equals(dataset.Activity.ProjectId) && s.DomainId.Equals(dataset.DomainId),
                new List<Expression<Func<ObservationDescriptor, object>>>() { od => od.O3 }).ToList();

            //NOW WITH IDS TO PASS ON TO MONGO 
            O3List = new List<ObjectOfObservation>();
            O3List.AddRange(obsDescriptionList.Select(observationDescription => observationDescription.O3));

            //ALL descriptors mapped from the SDTM entity descriptor
            var descriptorsList = LoadObsDescriptors(sdtmEntityDescriptor, dataset.Activity.ProjectId);

            foreach (var sdtmRow in sdtmRowList)
            {
                //WARNING
                //depending ON THE STRING EQUALITY HERE TO IDENTIFY O3 ACROSS DATASETS AND ACROSS STUDIES OF THE SAME PROJECT
                //SHOULD REALLY USE THE CONTROLLED TERM INSTEAD (I.E. DECODE / LOINC)

                //1- GET O3
                ObjectOfObservation O3obj;
                string O3 = sdtmRow.TopicVariable; //THIS IS --TESTCD AND --TERM FOR NOW
                if (sdtmRow.Group != null && sdtmRow.Subgroup != null)
                    O3obj = O3List.Find(o3 => o3.Name.Equals(O3)
                                              && o3.ProjectId.Equals(dataset.Activity.ProjectId)
                                              && o3.Domain.Code.Equals(sdtmRow.DomainCode)
                                              && o3.Group.Equals(sdtmRow.Group)
                                              && o3.Subgroup.Equals(sdtmRow.Subgroup));
                else if (sdtmRow.Group != null)
                {
                    O3obj = O3List.Find(o3 => o3.Name.Equals(O3)
                                              && o3.ProjectId.Equals(dataset.Activity.ProjectId)
                                              && o3.Domain.Code.Equals(sdtmRow.DomainCode)
                                              && o3.Group.Equals(sdtmRow.Group));
                }
                else
                    O3obj = O3List.Find(o3 => o3.Name.Equals(O3)
                                              && o3.ProjectId.Equals(dataset.Activity.ProjectId)
                                              && o3.Domain.Code.Equals(sdtmRow.DomainCode));


                //2- Get its observationDefinition
                var obsDescription = obsDescriptionList.Find(od => od.ObjectOfObservationId.Equals(O3obj.Id));

                //3- Create new observation
                Study study = _studyRepository.FindSingle(s => s.Name.Equals(sdtmRow.StudyId));
                var observation = new Observation();
                observation.O3Id = O3obj.Id;
                observation.ObjectOfObservation = O3obj.Name;
                observation.Study = sdtmRow.StudyId;
                observation.DBStudyId = study != null ? study.Id : 0;
                observation.SO2Id = sdtmRow.USubjId;
                observation.SO2Type = "SUBJECT";


                #region 3.1 Add Default property value

                //THE DEFAULT DESCRIPTOR IS NOT A NATIVE DEFAULT DESCRIPTOR PRE-SPECIFIED , 
                //IT'S just a regular descriptor which could take on any qualifier value
                //e.g. OCCUR, ORRES, STRESC, STRESN
                // and only at the time of creating the obsevation first time the observationDefinition.DefaultQualifier will be set to one of the qualifiers
                //based on ... Cannot be based on information pre-assigned to the SDTMentity descriptor since that applies to the dataset as a whole and not on a row 
                //basis (will not solve the situation where some observations will have their defualt set to STRESN and others set to STRESC)
                // so at the time of observation creation first time based on the following rules set its observationDefinition.DefaultQualifier
                // if domain is findings and if there's a value for STRESN then this is the default, 
                //else if there's a value for STRESC then this is default else ORRES is the default
                //this equality is going to be achieved 


                VariableDefinition defQVariable = null; //the variable that will be read from the sdtm row as the default
                PropertyDescriptor defDescriptor; //= obsDescription.DefaultPropertyDescriptor; //the matching descriptor and set it to observationDescription

                if (dataset.Domain.Class.ToLower().Equals("findings"))
                {
                    //var defVar = sdtmEntityDescriptor.ResultVariables.Find(rv => rv.Name.Equals("STRESN"));
                    if (sdtmRow.ResultQualifiers["STRESN"] != null)
                    {
                        defQVariable = sdtmEntityDescriptor.ResultVariables.Find(rv => rv.Name.Equals("STRESN"));
                        defDescriptor = descriptorsList.Find(d => d.Name.Equals("STRES"));
                    }
                    else if (sdtmRow.ResultQualifiers["STRESC"] != null)
                    {
                        defQVariable = sdtmEntityDescriptor.ResultVariables.Find(rv => rv.Name.Equals("STRESC"));
                        defDescriptor = descriptorsList.Find(d => d.Name.Equals("STRES"));
                    }
                    else //if (sdtmRow.ResultQualifiers["ORRES"] != null)
                    {
                        defQVariable = sdtmEntityDescriptor.ResultVariables.Find(rv => rv.Name.Equals("ORRES"));
                        defDescriptor = descriptorsList.Find(d => d.Name.Equals("ORRES"));
                    }
                }
                else if (sdtmEntityDescriptor.DomainCode.Equals("AE"))
                {
                    defDescriptor = descriptorsList.Find(d => d.Name.Equals("OCCUR"));
                    //there is no defQvariable in this case as we will make it up it doesn not occur in the original dataset
                }
                else
                {
                    defQVariable = sdtmEntityDescriptor.DefaultQualifier;
                    defDescriptor = descriptorsList.Find(d => d.Name.Equals(defQVariable.Name));
                }

                obsDescription.DefaultPropertyDescriptor = defDescriptor;

                //TODO: THIS COULD EITHER BE CVTERMS SO BOTH VARIABLE DEFINITION AND OBSERVATION DESCRIPTOR WOULD SHARE THE SAME CVTERM
                //todo: CHANGE ALL VARIABLE NAMES TO ENUMS


                if (sdtmEntityDescriptor.DomainCode.Equals("AE"))
                {
                    CategoricalValue propValue = new CategoricalValue();
                    propValue.Property = defDescriptor;
                    propValue.Value = "Y";
                }
                else if (defDescriptor.ValueType == ObsValueType.Numerical)
                {
                    NumericalValue propertyValue = new NumericalValue();
                    propertyValue.Value = float.Parse(sdtmRow.ResultQualifiers[defQVariable.Name]);
                    //propertyValue.Unit = 
                    propertyValue.Property = defDescriptor;
                    observation.DefaultObservedProperty = propertyValue;
                }
                else
                {
                    CategoricalValue propValue = new CategoricalValue();
                    propValue.Property = defDescriptor;
                    propValue.Value = sdtmRow.ResultQualifiers[defQVariable.Name];
                }
                #endregion

                ///////////////////////////////////////////////////////////////////////////////////////////////

                #region 3.2 Add other qualifiers

                foreach (var qualifier in sdtmRow.Qualifiers)
                {
                    var key = qualifier.Key.Substring(2);
                    //GET THE MATCHING VARIABLE FROM the sdtm desriptor using names qualifier.key == 
                    //then use its name 
                    if (qualifier.Value != null)
                    {
                        //ADD descriptor to obsDescription if not previously added
                        //HOW IS THE LINK BETWEEN THE VALUE IN THE SDTM ENTITY AND THE DESCRIPTOR IS ESTABLISHED
                        //CURRENTLY IT IS USING THE SAME NAME FOR THE DESCRIPTOR AS THE NAME OF THE VARIABLE WITHOUT THE PREFIX

                        PropertyDescriptor descriptor = obsDescription.ObservedPropertyDescriptors.Find(d => d.Name.Equals(key));
                        if (descriptor == null)
                        {
                            descriptor = descriptorsList.Find(d => d.Name.Equals(key));
                            obsDescription.ObservedPropertyDescriptors.Add(descriptor);
                        }
                        //ADD new observation instance

                        //if(descriptor.ValueType.Equals(ObsValueType.Categorical))
                        //TODO: for now not checking on property value type and creating categorical value for all other than result for findings
                        observation.ObservedProperties.Add(new CategoricalValue()
                        {
                            Value = qualifier.Value,
                            Property = descriptor,

                            //CVTerm = 
                        });



                    }
                }

                #endregion

                ///////////////////////////////////////////////////////////////////////////////////////////////

                #region 4 Add Temporal Qualifiers

                #endregion

                #region 5 Add TimeSeries Descriptors

                #endregion
            }
        }
        */
        internal List<PropertyDescriptor> LoadObsDescriptors(SdtmRowDescriptor sdtmEntityDescriptor, int projectId)
        {

            var descriptorList = new List<PropertyDescriptor>();

           // SetDescriptorTypes(sdtmEntityDescriptor);

            //IF DOMAIN.CLASS which can be identified from sdtmEntityDescriptor
            //THEN cREATE descriptor for MATCHIN CLASS
            //BUT then how will this be linked with the variable name in the data file?

            #region HACK
            //TODO: chage this to enum 

            //A HACK TO ADD OCCURENCE descriptor for adverse events which would not be in the standard
            if (sdtmEntityDescriptor.DomainCode.Equals("AE"))
            {
                var descriptor = _ObsDescriptorRepository.FindSingle(d => d.Name.Equals("OCCUR"));
                //o => o.Type == DescriptorType.
                //&& o.Domain.Equals(sdtmEntityDescriptor.DomainCode));

                if (descriptor == null)
                {
                    descriptor = PropertyDescriptorFactory.CreateAEdefDescriptor();
                    _ObsDescriptorRepository.Insert(descriptor);
                }
                descriptorList.Add(descriptor);
            }

            //A HACK BECAUSE I DONT WANT TO HAVE TWO STANDARD RESULT DESCRIPTORS AS IT IS THE CASE WITH SDTM VARAIBLES
            if (sdtmEntityDescriptor.Class.Equals("Findings"))
            {
                var descriptorST = _ObsDescriptorRepository.FindSingle(d => d.Name.Equals("STRES"));
                if (descriptorST == null)
                {
                    descriptorST = PropertyDescriptorFactory.CreateStandardResDescriptor();
                    _ObsDescriptorRepository.Insert(descriptorST);
                }
                descriptorList.Add(descriptorST);
            }
            #endregion

            foreach (var variable in sdtmEntityDescriptor.QualifierVariables.Union(sdtmEntityDescriptor.ResultVariables))
            {
                //todo: SHOULD BE ABLE TO DIFFERENTIATE HERE THE DIFFERENT SUBCLASSES OF SDTM RECORD QUALIFIERS
                //TODO: INTO FEATURE_QUALIFIERS, DEFAULT_OBSERVED_PROPERTY, OBSERVED_PROPERTIES, SUBJECT_QUALIFIERS AND SAMPLE_QUALIFIERS
                //TODO: should find equality via CVterm not name text
                //TODO: CVterm should be assigned to each variable definition which shuold then be an EQUIVALENT CVterm used for the descriptor
                var descriptor = _ObsDescriptorRepository.FindSingle(o => o.Name.Equals(variable.Name.Substring(2)));
                if (descriptor == null)
                {
                    //FOR NOW the Descriptor is CREATED FROM the variable information
                    descriptor = new PropertyDescriptor();
                    descriptor.Name = variable.Name.Substring(2);
                    //SOME KIND OF A METHOD HERE THAT RETURNS THE DESCRIPTOR TYPE THIS VARIABLE MATCHES
                    //descriptor.Type = variable.DescriptorType; //THIS IS SUPPOSED TO HAVE NOW THE NEW SUBCLASSES //DescriptorType.ObservedPropertyDescriptor;
                    descriptor.Description = variable.Label;
                    descriptor.ProjectId = projectId;

                    //_ObsDescriptorRepository.Insert(descriptor);
                }
                descriptorList.Add(descriptor);
            }

            //_dataContext.Save();

            //TODO: consider doing the same for other qualifiers such as timings and setting variable qualifiers to qualifiers

            return descriptorList;
        }
        /*
        private void SetDescriptorTypes(SdtmEntityDescriptor sdtmEntityDescriptor)
        {
            List<string> FindingsFeatureDescriptors = new List<string>()
            {
                "STAT","REASND","NAM","METHOD","RUNID","LEAD","LLOQ","ULOQ"
            };

            List<string> FindingsObservedPropertyDescriptors = new List<string>()
            {
                "ORRES","STRESC","STRESN","RESCAT","XFN","TOXGR","SEV"
            };

            List<string> FindingsSampleDescriptors = new List<string>()
            {
                "SPEC","SPCCND","SPCUFL"
            };
            List<string> FindingsSubjectDescriptors = new List<string>()
            {
                "POS","BODSYS","LOC","CSTATE","FAST"
            };

            if (sdtmEntityDescriptor.Class.Equals("Findings"))
            {
                //iterate over all varaibels and set their descriptor type which would match the same
                //descriptor type on the observation descriptor
                foreach (var variable in sdtmEntityDescriptor.QualifierVariables.Union(sdtmEntityDescriptor.ResultVariables))
                {
                    if (FindingsFeatureDescriptors.Contains(variable.Name))
                        variable.DescriptorType = DescriptorType.FeatureDescriptor;
                    if (FindingsSampleDescriptors.Contains(variable.Name))
                        variable.DescriptorType = DescriptorType.SampleDescriptor;
                    if (FindingsSubjectDescriptors.Contains(variable.Name))
                        variable.DescriptorType = DescriptorType.SubjectDescriptor;
                    if (FindingsObservedPropertyDescriptors.Contains(variable.Name))
                        variable.DescriptorType = DescriptorType.ObservedPropertyDescriptor;
                }
            }



        }
        */
    }
}
