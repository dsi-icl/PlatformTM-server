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
                                                   string code,
                                                   bool compound)
        {
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
                    // Check if test is done by using the STAT field
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
                    // For observations with compound fields e.g. DIABP, TEMP
                    if (compound)
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
                    }
                    combinedList.Add(sublist);
                }
            }
            return combinedList;
        }

        public List<List<RecordItem>> groupNOSQLData(List<List<RecordItem>> combinedList, bool compound)
        {
            if (compound)
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
            else
                return
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

            List<string> observation_compund = new List<string>();
            observation_compund.Add("DIABP");

            //For observations e.g. BMI, HEIGHT and WEIGHT
            List<string> filter_groupBy = new List<string>();
            filter_groupBy.Add(code + "ORRES");
            filter_groupBy.Add("USUBJID");
            filter_groupBy.Add(code + "TESTCD");
            filter_groupBy.Add(code + "STAT");

            //For observations e.g. DIABP, TEMP
            List<string> filter_compund_groupBy = new List<string>();
            filter_compund_groupBy.Add(code + "ORRES");
            filter_compund_groupBy.Add("USUBJID");
            filter_compund_groupBy.Add(code + "TESTCD");
            filter_compund_groupBy.Add(code + "STAT");
            filter_compund_groupBy.Add("VISIT");
            filter_compund_groupBy.Add(code + "DY");
            filter_compund_groupBy.Add(code + "TPT");
            // Simulate input (END)

            List<List<RecordItem>> combinedList_compound = getNOSQLData(where, observation_compund, filter_compund_groupBy, code, true);
          //  List<List<RecordItem>> groupedList_compound = groupNOSQLData(combinedList_compound, true);
            List<List<RecordItem>> groupedList_compound = combinedList_compound;

            List<List<RecordItem>> combinedList = getNOSQLData(where, observation, filter_groupBy, code, false);
            List<List<RecordItem>> groupedList = groupNOSQLData(combinedList, false);
            

            // Format data (remove field names) and input to Hash table
            List<Hashtable> observation_list = new List<Hashtable>();
            Hashtable obs;

            for (int i = 0; i < groupedList.Count(); i++)
            {
                obs = new Hashtable();
                List<string> singleRecord = new List<string>();
                singleRecord.Add("USUBJID");
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

            for (int i = 0; i < groupedList_compound.Count(); i++)
            {
                obs = new Hashtable();
                List<string> singleRecord = new List<string>();
                
                for (int j = 0; j < groupedList_compound[i].Count(); j++)
                {
                    obs.Add(groupedList_compound[i][j].fieldName, groupedList_compound[i][j].value);
                }
                observation_list.Add(obs);
            }
            return observation_list;
        }
    }
}
