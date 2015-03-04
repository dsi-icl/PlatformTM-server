using eTRIKS.Commons.Core.Domain.Interfaces;
using System;
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

        public void getObservationsData(String studyId, String domain, List<string> observations)
        {
            // Simulate input (START)
            List<RecordItem> where = new List<RecordItem>();
            RecordItem ri = new RecordItem();
            ri.fieldName = "DOMAIN";
            ri.value = domain; // VS
            where.Add(ri);

            //List<string> observation = new List<string>();
            //observation.Add("BMI");
            //observation.Add("HEIGHT");
            //observation.Add("WEIGHT");

            List<string> filter = new List<string>();
            filter.Add(domain + "ORRES");
            filter.Add("USUBJID");
            filter.Add(domain + "TESTCD");
            // Simulate input (END)

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
        }

        public void getObservationsDataTemp()
        {
            // Simulate input (START)
            string code = "VS";
            List<RecordItem> where = new List<RecordItem>();
            RecordItem ri = new RecordItem();
            ri.fieldName = "DOMAIN";
            ri.value   = code;
            where.Add(ri);

            List<string> observation = new List<string>();
            observation.Add("BMI");
            observation.Add("HEIGHT");
            observation.Add("WEIGHT");

            List<string> filter = new List<string>();
            filter.Add(code+"ORRES");
            filter.Add("USUBJID");
            filter.Add(code+"TESTCD");
            // Simulate input (END)

            MongoDbDataRepository mongoDataService = new MongoDbDataRepository();
            NoSQLRecordSet recordSet = null;
            recordSet = mongoDataService.getNoSQLRecordsForGraph(where, observation, filter, code);

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
                for (int k = 0; k < singleRecord.Count(); k=k+2)
                {
                    obs.Add(singleRecord[k], singleRecord[k+1]);
                }
                observation_list.Add(obs);       
            }

        }
    }
}
