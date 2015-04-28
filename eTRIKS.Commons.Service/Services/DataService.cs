using eTRIKS.Commons.Core.Domain.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.DataAccess.MongoDB;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.DTOs;
using System.Linq.Expressions;

namespace eTRIKS.Commons.Service.Services
{
    public class DataService
    {
        private IRepository<Observation, int> _observationRepository;
        private IRepository<Activity, int> _activityRepository;
        public DataService(IServiceUoW uoW)
        {
            //_activityServiceUnit = uoW;
            _activityRepository = uoW.GetRepository<Activity, int>();
            //_dataSetRepository = uoW.GetRepository<Dataset, int>();
            _observationRepository = uoW.GetRepository<Observation, int>();
        }

        // Method for data Visulaiser
        public async Task<IEnumerable<ClinicalDataTreeDTO>> getClinicalObsTree(string studyId)
        {
            //Get data from SQL
            string role = "CL-Role-T-2";

            List<Activity> activity_list = _activityRepository.FindAll(
                d => d.StudyId.Equals(studyId),
                  new List<Expression<Func<Activity, object>>>(){
                        d => d.Datasets.Select(t => t.Domain), d=>d.Datasets.Select(t=>t.Variables),
                         d => d.Datasets.Select(t => t.Domain), d=>d.Datasets.Select(
                                                                    t=>t.Variables.Select(k=>k.VariableDefinition))
                }
            ).ToList();

            //1. Extract data for clinical tree
            List<ClinicalDataTreeRecordSummary> extractedClinicalTreeRecordList = new List<ClinicalDataTreeRecordSummary>();
            for (int i = 0; i < activity_list.Count; i++)
            {
                for (int j = 0; j < activity_list[i].Datasets.Count; j++)
                {

                    ClinicalDataTreeRecordSummary clinicalTreeRecord = new ClinicalDataTreeRecordSummary();
                    clinicalTreeRecord.Class = activity_list[i].Datasets.Select(f => f.Domain.Class).ToList()[j];
                    if (clinicalTreeRecord.Class.Equals("Relationship"))
                        continue;
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

            var groupedClinicalTreeRecordList = extractedClinicalTreeRecordList
                                                .GroupBy(u => u.Class)
                                                .Select(grp => grp.ToList())
                                                .ToList();
            // For each acitivity
            for (int i = 0; i < groupedClinicalTreeRecordList.Count(); i++)
            {
                ClinicalDataTreeDTO cdTree = new ClinicalDataTreeDTO();
                cdTree.Class = groupedClinicalTreeRecordList[i][0].Class;

                //Console.Out.WriteLine(cdTree.Class);
                // For each dataset 
                for (int j = 0; j < groupedClinicalTreeRecordList[i].Count(); j++)
                {
                    ClinicalDataTreeActivityDTO cdTreeActivity = new ClinicalDataTreeActivityDTO();
                    cdTreeActivity.Name = groupedClinicalTreeRecordList[i][j].Name;
                    cdTreeActivity.Domain = groupedClinicalTreeRecordList[i][j].Domain;
                    cdTreeActivity.code = groupedClinicalTreeRecordList[i][j].code;
                    //Console.Out.WriteLine(cdTreeActivity.code);
                    cdTree.Activities.Add(cdTreeActivity);

                    MongoDbDataRepository mongoDataService = new MongoDbDataRepository();
                    MongoDataCollection recordSet = null;

                    // Generic Grouping
                    string code = cdTreeActivity.code;
                    string variableDefinition = groupedClinicalTreeRecordList[i][j].variableDefinition;
                    //TEMP HACK

                    string testName = variableDefinition.TrimEnd('C', 'D');
                    string queryString = "?DOMAIN=" + code + "&" + variableDefinition + "=*&" + code + "CAT=*&" + code + "SCAT=*&" + testName + "=*";
                    recordSet = await mongoDataService.getNoSQLRecords(queryString);

                    if (recordSet.documents.Any())
                    {
                        bool categoryExist = mongoDataService.checkForFieldInNoSQL(recordSet, code + "CAT");
                        bool subCategoryExist = mongoDataService.checkForFieldInNoSQL(recordSet, code + "SCAT");

                        if (categoryExist)
                        {
                            if (subCategoryExist)
                            {
                                var filteredRecordSet = recordSet.documents.Select(u =>
                                               new
                                               {
                                                   code = u.fields[0].value,
                                                   category = u.fields[1].value,
                                                   subCategory = u.fields[2].value
                                               }).Distinct();
                                var groupedRecordSet = filteredRecordSet.GroupBy(k => new { k.category, k.subCategory }).
                                                        Select(grp => grp.ToList()).ToList();
                                // ************* To be Completed *****************

                            }
                            else
                            {
                                var filteredRecordSet = recordSet.documents
                                                        .Select(u =>
                                                            new
                                                            {
                                                                code = u.fields[0].value,
                                                                name = u.fields[1].value,
                                                                category = u.fields[2].value
                                                            })
                                                        .Distinct();

                                var groupedRecordSet = filteredRecordSet
                                                        .GroupBy(k => k.category)
                                                        .Select(grp => grp.ToList())
                                                        .ToList();

                                for (int k = 0; k < groupedRecordSet.Count(); k++)
                                {
                                    ObservationGroup obsGrp = new ObservationGroup();
                                    obsGrp.Name = groupedRecordSet[k][0].category;
                                    obsGrp.Code = code + "-CAT" + k;

                                    for (int m = 0; m < groupedRecordSet[k].Count(); m++)
                                    {
                                        ObservationDTO observation = new ObservationDTO();
                                        observation.Name = groupedRecordSet[k][m].name.ToLower();
                                        observation.Code = groupedRecordSet[k][m].code.ToLower();
                                        obsGrp.Observations.Add(observation);
                                    }
                                    cdTreeActivity.Observations.Add(obsGrp);

                                }
                            }
                        }
                        else
                        {
                            var filteredRecordSet = recordSet.documents.Select(u => new
                            {
                                code = u.fields[0].value,
                                name = u.fields.ElementAtOrDefault(1) != null ? u.fields[1].value : u.fields[0].value
                            }).Distinct().ToList();
                            for (int g = 0; g < filteredRecordSet.Count(); g++)
                            {
                                ObservationDTO observation = new ObservationDTO();
                                observation.Name = filteredRecordSet[g].name.ToLower();
                                observation.Code = filteredRecordSet[g].code.ToLower();
                                cdTreeActivity.Observations.Add(observation);
                            }
                        }
                    }
                }
                cdTreeList.Add(cdTree);
            }
            return cdTreeList;
        }

        public List<Hashtable> getObservationsData(string studyId, List<int> observationsIDs)
        {
            studyId = "CRC305C";
            List<Hashtable> dataMatrix = new List<Hashtable>();
            Dictionary<string, Hashtable> dataHT = new Dictionary<string, Hashtable>();
            
            List<Observation> observations = _observationRepository.FindAll(
                           o =>  observationsIDs.Contains(o.Id)
             ).ToList();
            
            //the returned data structure is a big martrix of observations doesnt matter which domain they come from
            //the domain is important to organize the observations 
            //get observations from sql matching the ids from UI
            //for each observation get 
            //SHOULD i have a mongo collection for each class of observations?
            //then I have POCO for each class of observations?

            //Need to define the structure of observations and theier qualifiers exchanged from UI
            //for findings will return ORRES by default for now
            //for events will return a Y/N by default for now 
            //ASSUMPTION ... USERS will not choose qualifiers for the selected observations. 
            //ONLY values of default qualifiers will be returned. i.e. cannot show AE severity

            //List<Observation> observation_list = new List<Observation>{ 
            //    new Observation{
            //        Name="BMI",
            //        DomainCode = "VS",
            //        TopicVariable = new VariableDefinition{
            //            Name="VSTESTCD"
            //        },
            //        DefaultQualifier = new VariableDefinition{
            //            Name="VSORRES"
            //        }
            //    },
            //    new Observation{
            //        Name="HEIGHT",
            //        DomainCode = "VS",
            //        TopicVariable = new VariableDefinition{
            //            Name="VSTESTCD"
            //        },
            //        DefaultQualifier = new VariableDefinition{
            //            Name="VSORRES"
            //        }
            //    },
            //    new Observation{
            //        Name="WEIGHT",
            //        DomainCode = "VS",
            //        TopicVariable = new VariableDefinition{
            //            Name="VSTESTCD"
            //        },
            //        DefaultQualifier = new VariableDefinition{
            //            Name="VSORRES"
            //        }
            //    },
            //    new Observation{
            //        Name="TEMP",
            //        DomainCode = "VS",
            //        TopicVariable = new VariableDefinition{
            //            Name="VSTESTCD"
            //        },
            //        DefaultQualifier = new VariableDefinition{
            //            Name="VSORRES"
            //        }
            //    }
            //};



            foreach(Observation observation in observations){
                string observationName = observation.Name ;
                string queryVariable = observation.TopicVariable.Name;

                //if observation has timing include them, but how to distinguish between
                //dates(VSSTDTC) and timing events (e.g. VISIT) 
                //need a way to find if a certain observation for a subject could be repeated according to a timing event
                List<string> returnVariables = new List<string>();
                returnVariables.Add(observation.DefaultQualifier.Name);//would be replaced by user selected qualfiers
                returnVariables.AddRange(observation.Timings.Select(t => t.Name));
                
                


                //MongoDbDataRepository mongoDataService = new MongoDbDataRepository();
                //List<SubjObservationTemp> subjObservations = mongoDataService.test(studyId,queryVariable, observationName, returnVariables);
                

                //var groupedObservations = subjObservations
                //    .GroupBy(ob => new { ob.subjId, ob.visitNo, ob.day, ob.timepoint });
                //foreach (var group in groupedObservations)
                //{
                //    string key = group.Key.ToString();
                //    Hashtable ht ;
                //    bool isnew = false;
                //    if (dataHT.ContainsKey(key))
                //        ht = dataHT[key];
                //    else{
                //         ht = new Hashtable();
                //         dataHT.Add(key, ht);
                //         isnew = true;
                //    }
                       
                //    foreach (var subjob in group)
                //    {
                //        if (isnew)
                //        {
                //            ht.Add("SubjectId", subjob.subjId);
                //            ht.Add("Visit", subjob.visitNo);
                //            ht.Add("Timepoint", subjob.timepoint);
                //            ht.Add("Day", subjob.day);
                //            isnew = false;
                //        }
                //        ht.Add(subjob.test, subjob.value);
                //    }
                //}


            }
            List<Hashtable> data = dataHT.Values.ToList();



            return data;
 

            
        }
        
        public List<Hashtable> getObservationsData(String studyId, String domain, List<string> observations)
        {
            //How this will change:
            //observaations will be alist of observation IDs
            //observations is a map of observation id and a list of qualifiers
            //each observation has a default qualifier (e.g. findings obs default to ORRES, events obs default to a present/absent)

            List<MongoField> where = new List<MongoField>();
            MongoField ri_1 = new MongoField();
            ri_1.Name = "DOMAIN";
            ri_1.value = domain;
            where.Add(ri_1);

            MongoField ri_2 = new MongoField();
            ri_2.Name = "STUDYID";
            ri_2.value = studyId;
            where.Add(ri_2);

            //TEMP 
            observations = observations.ConvertAll(d => d.ToUpper());

            // Fields needed for the graphs
            List<string> filter = new List<string>();
            filter.Add(domain + "ORRES");
            filter.Add("USUBJID");
            filter.Add(domain + "TESTCD");
            filter.Add("VISIT");
            filter.Add(domain + "DY");
            filter.Add(domain + "TPT");

            List<List<MongoField>> combinedList = getNOSQLData(where, observations, filter, domain);
            List<List<MongoField>> groupedList = groupNOSQLData(combinedList);

            // Format data (remove field names) and input to Hash table
            List<Hashtable> observation_list = new List<Hashtable>();
            Hashtable obs;

            for (int i = 0; i < groupedList.Count(); i++)
            {
                obs = new Hashtable();

                for (int j = 0; j < groupedList[i].Count(); j++)
                {
                    // Check if the field is in the observation list if so flatten the record 
                    // e.g. BMI = 24
                    if (observations.Contains(groupedList[i][j].value))
                    {
                        obs.Add(groupedList[i][j].value, groupedList[i][j + 1].value);
                        j++;
                    }
                    else
                    {
                        obs.Add(groupedList[i][j].Name, groupedList[i][j].value);
                    }
                }
                for (int k = 0; k < observations.Count(); k++)
                {
                    if (!obs.Contains(observations[k]))
                    {
                        obs.Add(observations[k], null);
                    }
                }
                observation_list.Add(obs);
            }
            return observation_list;
        }

        public async Task<List<Hashtable>> getSubjectData(String studyId, List<string> subjCharacteristics)
        {
			// Construct the query string for NOSQL
            // e.g. ?STUDYID=CRC305A&DOMAIN=DM&AGE=*&SEX=*&RACE=*&ETHNIC=*&ARM=*
            //TEMP 
            subjCharacteristics = subjCharacteristics.ConvertAll(d => d.ToUpper());

            
            string query = "?STUDYID=" + studyId + "&DOMAIN=DM";
            for (int i = 0; i < subjCharacteristics.Count(); i++)
            {
                query += "&"+subjCharacteristics[i] + "=*";
            }

            MongoDbDataRepository mongoDataService = new MongoDbDataRepository();
            MongoDataCollection recordSet = null;
            recordSet = await mongoDataService.getNoSQLRecords(query);

            List<Hashtable> observation_list = new List<Hashtable>();
            Hashtable obs;
            for (int i = 0; i < recordSet.documents.Count(); i++)
            {
                obs = new Hashtable();
                for (int k = 0; k < recordSet.documents[i].fields.Count(); k++)
                {
                    obs.Add(recordSet.documents[i].fields[k].Name, 
                            recordSet.documents[i].fields[k].value);
                }
                observation_list.Add(obs); 
            }
            return observation_list;
        }


        public List<List<MongoField>> getNOSQLData(List<MongoField> where,
                                                   List<string> observation,
                                                   List<string> filter,
                                                   string code)
        {
            MongoDbDataRepository mongoDataService = new MongoDbDataRepository();
            MongoDataCollection collection = null;
            collection = mongoDataService.getNoSQLRecordsForGraph(where, observation, filter, code);
            List<List<MongoField>> combinedList = new List<List<MongoField>>();

            for (int i = 0; i < collection.documents.Count(); i++)
            {
                List<MongoField> sublist = new List<MongoField>();
                string TPT = "SCHEDULED"; // staus on if a result exists
                for (int j = 0; j < collection.documents[i].fields.Count(); j++)
                {
                    // Check if test is done by using the STAT field
                    if (collection.documents[i].fields[j].Name == code + "TPT")
                    {
                        TPT = collection.documents[i].fields[j].value;
                    }
                    MongoField field = new MongoField();
                    field.Name = collection.documents[i].fields[j].Name;
                    field.value = collection.documents[i].fields[j].value;
                    sublist.Add(field);

                }
                if (!TPT.Contains("UNSCHEDULED"))
                {
                    // Check if the DY and the TPT exist in the record if not they need to be manually added
                    // as they are required for the grouping
                    if (!sublist.Exists(item => item.Name.Equals(code + "DY")))
                    {
                        MongoField field = new MongoField();
                        field.Name = code + "DY";
                        field.value = null;
                        sublist.Add(field);
                    }
                    if (!sublist.Exists(item => item.Name.Equals(code + "TPT")))
                    {
                        MongoField field = new MongoField();
                        field.Name = code + "TPT";
                        field.value = null;
                        sublist.Add(field);
                    }
                    combinedList.Add(sublist);
                }
            }
            return combinedList;
        }

        public List<List<MongoField>> groupNOSQLData(List<List<MongoField>> combinedList)
        {
            //Group by the data based on SubjectId, Vist, Date
            return
                                (
                                    from records in combinedList
                                    from subject in records.Take(1)
                                    let USUBJID = subject.value
                                    let VISIT = records.ToArray().ElementAt(3).value
                                    let DY = records.ToArray().ElementAt(4).value
                                    let TPT = records.ToArray().ElementAt(5).value
                                    let Subjects = records.Skip(1).Take(2)
                                    group Subjects by new
                                    {
                                        USUBJID,
                                        VISIT,
                                        DY,
                                        TPT
                                    }
                                        into gss
                                        select new[]
                                    {
                                        new MongoField() {Name = "USUBJID", value = gss.Key.USUBJID },
                                        new MongoField() {Name = "VISIT", value = gss.Key.VISIT },
                                        new MongoField() {Name = "DY", value = gss.Key.DY },
                                        new MongoField() {Name = "TPT", value = gss.Key.TPT }
                                    }.Concat(gss.SelectMany(x => x)).ToList()
                                ).ToList();
        }

        public List<Hashtable> getObservationsDataTemp()
        {
            // Simulate input (START)
            string code = "VS";
            List<MongoField> where = new List<MongoField>();
            MongoField ri_1 = new MongoField();
            ri_1.Name = "DOMAIN";
            ri_1.value = code;
            where.Add(ri_1);

            MongoField ri_2 = new MongoField();
            ri_2.Name = "STUDYID";
            ri_2.value = "CRC305C";
            where.Add(ri_2);

            List<string> observation = new List<string>();
            observation.Add("BMI");
            observation.Add("HEIGHT");
            observation.Add("WEIGHT");
            observation.Add("DIABP");
            // Simulate input (END)

            //Fields For observations
            List<string> filter_groupBy = new List<string>();
            filter_groupBy.Add(code + "ORRES");
            filter_groupBy.Add("USUBJID");
            filter_groupBy.Add(code + "TESTCD");
            filter_groupBy.Add("VISIT");
            filter_groupBy.Add(code + "DY");
            filter_groupBy.Add(code + "TPT");

            List<List<MongoField>> combinedList = getNOSQLData(where, observation, filter_groupBy, code);
            List<List<MongoField>> groupedList= groupNOSQLData(combinedList);

            // Format data (remove field names) and input to Hash table
            List<Hashtable> observation_list = new List<Hashtable>();
            Hashtable obs;

            for (int i = 0; i < groupedList.Count(); i++)
            {
                obs = new Hashtable();

                for (int j = 0; j < groupedList[i].Count(); j++)
                {
                    if (observation.Contains(groupedList[i][j].value))
                    {
                        obs.Add(groupedList[i][j].value, groupedList[i][j + 1].value);
                        j++;
                    }
                    else
                    {
                        obs.Add(groupedList[i][j].Name, groupedList[i][j].value);
                    }
                }
                for (int k = 0; k < observation.Count(); k++)
                {
                    if (!obs.Contains(observation[k]))
                    {
                        obs.Add(observation[k], null);
                    }
                }
                observation_list.Add(obs);
            }
            return observation_list;
        }
    }
}
