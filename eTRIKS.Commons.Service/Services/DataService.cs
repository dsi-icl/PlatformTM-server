using eTRIKS.Commons.Core.Domain.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.DataAccess.MongoDB;
using System.Collections;

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
            // Simulate input (START)
            List<RecordItem> where = new List<RecordItem>();
            RecordItem ri_1 = new RecordItem();
            ri_1.fieldName = "DOMAIN";
            ri_1.value = domain;
            where.Add(ri_1);

            RecordItem ri_2 = new RecordItem();
            ri_2.fieldName = "STUDYID";
            ri_2.value = studyId;
            where.Add(ri_2);

            //List<string> observation = new List<string>();
            //observation.Add("BMI");
            //observation.Add("HEIGHT");
            //observation.Add("WEIGHT");
            // Simulate input (END)

            // Fields needed for the graphs
            List<string> filter = new List<string>();
            filter.Add(domain + "ORRES");
            filter.Add("USUBJID");
            filter.Add(domain + "TESTCD");

            //TEMP 
            observations = observations.ConvertAll(d => d.ToUpper());


            MongoDbDataRepository mongoDataService = new MongoDbDataRepository();
            NoSQLRecordSet recordSet = null;
            recordSet = mongoDataService.getNoSQLRecordsForGraph(where, observations, filter, domain);

            List<List<RecordItem>> combinedList = new List<List<RecordItem>>();

            for (int i = 0; i < recordSet.RecordSet.Count(); i++)
            {
                List<RecordItem> sublist = new List<RecordItem>();
                for (int j = 0; j < recordSet.RecordSet[i].RecordItems.Count(); j++)
                {
                    RecordItem recordItem = new RecordItem();
                    recordItem.fieldName = recordSet.RecordSet[i].RecordItems[j].fieldName;
                    recordItem.value = recordSet.RecordSet[i].RecordItems[j].value;
                    sublist.Add(recordItem);
                }
                combinedList.Add(sublist);
            }

            // Group the data based on SubjectId
            List<List<RecordItem>> groupedList =
                                (
                                    from records in combinedList
                                    from subject in records.Take(1)
                                    let USUBJID = subject.value
                                    let Subjects = records.Skip(1)
                                    group Subjects by USUBJID into gss
                                    select new[]
                                    {
                                        new RecordItem() { fieldName = "USUBJID", value = gss.Key },
                                    }.Concat(gss.SelectMany(x => x)).ToList()
                                ).ToList();

            // Format data (remove field names) and input to Hash table
            List<Hashtable> observation_list = new List<Hashtable>();
            Hashtable obs;

            for (int i = 0; i < groupedList.Count(); i++)
            {
                obs = new Hashtable();
                List<string> singleRecord = new List<string>();
                singleRecord.Add("SubjId");
                for (int j = 0; j < groupedList[i].Count(); j++)
                {
                    singleRecord.Add(groupedList[i][j].value);
                }
                for (int k = 0; k < singleRecord.Count(); k = k + 2)
                {
                    obs.Add(singleRecord[k], singleRecord[k + 1]);
                }
                observation_list.Add(obs);
            }


            //For each observation in observations 
            // query its results column from NoSQL collection
            // IF observations was an array like this one:
            //['BMI','Height','Weight']
            ///return an object that looks like that
            /*[
               { subjId: “xxx”,  BMI: 22.5, Height: 1.8, Weight: 80 },
             * { subjId: “xxx”,  BMI: 24.2, Height: 1.71, Weight: 70 },
             * { subjId: “xxx”,  BMI: 20.5, Height: 1.85, Weight: 83 },
             * { subjId: “xxx”,  BMI: 23.2, Height: 1.7, Weight: 80 },
             * { subjId: “xxx”,  BMI: 26, Height: 1.62, Weight: 75 },
             * { subjId: “xxx”,  BMI: 21, Height: 1.73, Weight: 80 },
            ]*/

            //double[] heights = {1.8,1.71,1.85,1.7,1.62,1.73};
            //int[] weights = { 80, 70, 83, 87, 65, 91 };
            //double[] bmis = { 22.5, 24.2, 20.5, 23.2, 26, 21 };

            //List<Hashtable> observation_list = new List<Hashtable>();
            //Hashtable obs;
            //for (int i = 0; i < 6; i++)
            //{
            //    obs = new Hashtable();
            //    obs.Add("subjId", "subj_"+i);
            //    obs.Add("BMI", bmis[i]);
            //    obs.Add("Height", heights[i]);
            //    obs.Add("Weight", weights[i]);
            //    observation_list.Add(obs);
            //}

            return observation_list;

        }

        public List<Hashtable> getSubjectData(String studyId, List<string> subjCharacteristics)
        {
            //TEMP 
            subjCharacteristics = subjCharacteristics.ConvertAll(d => d.ToUpper());
            
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
            //observation.Add("BMI");
            //observation.Add("HEIGHT");
            //observation.Add("WEIGHT");
            observation.Add("DIABP");

            List<string> filter = new List<string>();
            filter.Add(code+"ORRES");
            filter.Add("USUBJID");
            filter.Add(code+"TESTCD");
            filter.Add(code + "STAT");
            filter.Add("VISIT");
            // Simulate input (END)

            MongoDbDataRepository mongoDataService = new MongoDbDataRepository();
            NoSQLRecordSet recordSet = null;
            recordSet = mongoDataService.getNoSQLRecordsForGraph(where, observation, filter, code);

            List<List<RecordItem>> combinedList = new List<List<RecordItem>>();

            for (int i = 0; i < recordSet.RecordSet.Count(); i++)
            {
                List<RecordItem> sublist = new List<RecordItem>();
                string stat = null; // staus on if a result exists
                for (int j = 0; j < recordSet.RecordSet[i].RecordItems.Count(); j++)
                {
                    if (recordSet.RecordSet[i].RecordItems[j].fieldName != code + "STAT")
                    {
                        RecordItem recordItem = new RecordItem();
                        recordItem.fieldName = recordSet.RecordSet[i].RecordItems[j].fieldName;
                        recordItem.value = recordSet.RecordSet[i].RecordItems[j].value;
                        sublist.Add(recordItem);
                    }
                    else
                    {
                        stat = recordSet.RecordSet[i].RecordItems[j].value;
                    }
                }
                if (stat != "NOT DONE")
                {
                    combinedList.Add(sublist);
                }
            }


            //Group by the data based on SubjectId, Vist, Date
            //List<List<RecordItem>> groupedList =
            //                    (
            //                        from records in combinedList
            //                        from subject in records.Take(1)
            //                        let USUBJID = subject.value
            //                        let VISIT = records.ToArray().ElementAt(3).value
            //                        let Subjects = records.Skip(1)
            //                        group Subjects by new
            //                        {
            //                            USUBJID,
            //                            VISIT
            //                        }
            //                        into gss
            //                        select new[]
            //                        {
            //                            new RecordItem() { fieldName = "USUBJID", value = gss.Key.USUBJID },
            //                            new RecordItem() {fieldName = "VISIT", value = gss.Key.VISIT }
            //                        }.Concat(gss.SelectMany(x => x)).ToList()
            //                    ).ToList();


            List<List<RecordItem>> groupedList =
                               (
                                   from records in combinedList
                                   from subject in records.Take(1)
                                   let USUBJID = subject.value
                                   let Subjects = records.Skip(1)
                                   group Subjects by USUBJID into gss
                                   select new[]
                                    {
                                        new RecordItem() { fieldName = "USUBJID", value = gss.Key },
                                    }.Concat(gss.SelectMany(x => x)).ToList()
                               ).ToList();

            // Format data (remove field names) and input to Hash table
            List<Hashtable> observation_list = new List<Hashtable>();
            Hashtable obs;

            for (int i = 0; i < groupedList.Count(); i++)
            {
                obs = new Hashtable();
                List<string> singleRecord = new List<string>();
                singleRecord.Add("SubjId");
                for (int j = 0; j < groupedList[i].Count(); j++)
                {
                    singleRecord.Add(groupedList[i][j].value);  
                }
                for (int k = 0; k < singleRecord.Count(); k=k+2)
                {
                    obs.Add(singleRecord[k], singleRecord[k+1]);
                }
                observation_list.Add(obs);       
            }
            return observation_list;
        }
    }
}
