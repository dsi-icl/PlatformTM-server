using eTRIKS.Commons.Core.Domain.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.DataAccess.MongoDB;

namespace eTRIKS.Commons.Service.Services
{
    public class DataService
    {
        public DataService(IServiceUoW uoW)
        {
            //_activityServiceUnit = uoW;
            //_activityRepository = uoW.GetRepository<Activity, int>();
            //_dataSetRepository = uoW.GetRepository<Dataset, int>();

        }

        public List<Hashtable> getObservationsData(String studyId, String domain, List<string> observations)
        {
            List<RecordItem> where = new List<RecordItem>();
            RecordItem ri_1 = new RecordItem();
            ri_1.fieldName = "DOMAIN";
            ri_1.value = domain;
            where.Add(ri_1);

            RecordItem ri_2 = new RecordItem();
            ri_2.fieldName = "STUDYID";
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

            List<List<RecordItem>> combinedList = getNOSQLData(where, observations, filter, domain);
            List<List<RecordItem>> groupedList = groupNOSQLData(combinedList);

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
                        obs.Add(groupedList[i][j].fieldName, groupedList[i][j].value);
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

        public List<Hashtable> getSubjectData(String studyId, List<string> subjCharacteristics)
        {
            // Construct the query string for NOSQL
            // e.g. ?STUDYID=CRC305A&DOMAIN=DM&AGE=*&SEX=*&RACE=*&ETHNIC=*&ARM=*
            string query = "?STUDYID=" + studyId + "&DOMAIN=DM";
            for (int i = 0; i < subjCharacteristics.Count(); i++)
            {
                query += "&"+subjCharacteristics[i] + "=*";
            }

            MongoDbDataRepository mongoDataService = new MongoDbDataRepository();
            NoSQLRecordSet recordSet = null;
            recordSet = mongoDataService.getNoSQLRecords(query);

            List<Hashtable> observation_list = new List<Hashtable>();
            Hashtable obs;
            for (int i = 0; i < recordSet.RecordSet.Count(); i++)
            {
                obs = new Hashtable();
                for (int k = 0; k < recordSet.RecordSet[i].RecordItems.Count(); k++)
                {
                    obs.Add(recordSet.RecordSet[i].RecordItems[k].fieldName, 
                            recordSet.RecordSet[i].RecordItems[k].value);
                }
                observation_list.Add(obs); 
            }
            return observation_list;
        }


        public List<List<RecordItem>> getNOSQLData(List<RecordItem> where,
                                                   List<string> observation,
                                                   List<string> filter,
                                                   string code)
        {
            MongoDbDataRepository mongoDataService = new MongoDbDataRepository();
            NoSQLRecordSet recordSet = null;
            recordSet = mongoDataService.getNoSQLRecordsForGraph(where, observation, filter, code);
            List<List<RecordItem>> combinedList = new List<List<RecordItem>>();

            for (int i = 0; i < recordSet.RecordSet.Count(); i++)
            {
                List<RecordItem> sublist = new List<RecordItem>();
                string TPT = "SCHEDULED"; // staus on if a result exists
                for (int j = 0; j < recordSet.RecordSet[i].RecordItems.Count(); j++)
                {
                    // Check if test is done by using the STAT field
                    if (recordSet.RecordSet[i].RecordItems[j].fieldName == code + "TPT")
                    {
                        TPT = recordSet.RecordSet[i].RecordItems[j].value;
                    }
                    RecordItem recordItem = new RecordItem();
                    recordItem.fieldName = recordSet.RecordSet[i].RecordItems[j].fieldName;
                    recordItem.value = recordSet.RecordSet[i].RecordItems[j].value;
                    sublist.Add(recordItem);

                }
                if (!TPT.Contains("UNSCHEDULED"))
                {
                    // Check if the DY and the TPT exist in the record if not they need to be manually added
                    // as they are required for the grouping
                    if (!sublist.Exists(item => item.fieldName.Equals(code + "DY")))
                    {
                        RecordItem recordItem = new RecordItem();
                        recordItem.fieldName = code + "DY";
                        recordItem.value = "DEFAULT";
                        sublist.Add(recordItem);
                    }
                    if (!sublist.Exists(item => item.fieldName.Equals(code + "TPT")))
                    {
                        RecordItem recordItem = new RecordItem();
                        recordItem.fieldName = code + "TPT";
                        recordItem.value = "DEFAULT";
                        sublist.Add(recordItem);
                    }
                    combinedList.Add(sublist);
                }
            }
            return combinedList;
        }

        public List<List<RecordItem>> groupNOSQLData(List<List<RecordItem>> combinedList)
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
                                        new RecordItem() { fieldName = "USUBJID", value = gss.Key.USUBJID },
                                        new RecordItem() {fieldName = "VISIT", value = gss.Key.VISIT },
                                        new RecordItem() {fieldName = "DY", value = gss.Key.DY },
                                        new RecordItem() {fieldName = "TPT", value = gss.Key.TPT }
                                    }.Concat(gss.SelectMany(x => x)).ToList()
                                ).ToList();
        }

        public List<Hashtable> getObservationsDataTemp()
        {
            // Simulate input (START)
            string code = "VS";
            List<RecordItem> where = new List<RecordItem>();
            RecordItem ri_1 = new RecordItem();
            ri_1.fieldName = "DOMAIN";
            ri_1.value = code;
            where.Add(ri_1);

            RecordItem ri_2 = new RecordItem();
            ri_2.fieldName = "STUDYID";
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

            List<List<RecordItem>> combinedList = getNOSQLData(where, observation, filter_groupBy, code);
            List<List<RecordItem>> groupedList= groupNOSQLData(combinedList);

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
                        obs.Add(groupedList[i][j].fieldName, groupedList[i][j].value);
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
