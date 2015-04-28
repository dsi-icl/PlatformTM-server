using eTRIKS.Commons.Core.Domain.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
//using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace eTRIKS.Commons.DataAccess.MongoDB
{
    public class MongoDbDataRepository
    {
        public const string mongoCollectionName = "BioSpeak_Data";
        public IMongoDatabase GetDatabase()
        {
            // Create server settings to pass connection string, timeout, etc.
            //MongoServerSettings settings = new MongoServerSettings();
            //IC Cloud
            //settings.Server = new MongoServerAddress("146.169.32.143", 27020);
            //return MongoServer.Create(settings).GetDatabase("eTRIKS");
            MongoClient mongoClient = new MongoClient(ConfigurationManager.ConnectionStrings["MongoDBConnectionString"]
                                            .ConnectionString);
            return mongoClient.GetDatabase(ConfigurationManager.AppSettings["NoSQLDatabaseName"]);
        }

        // The Generic loader
        public string loadDataGeneric(MongoDocument record)
        {
            IMongoDatabase dbETriks = GetDatabase();
            IMongoCollection<BsonDocument> eTriksCollection = dbETriks.GetCollection<BsonDocument>(mongoCollectionName);
            BsonDocument eTricksDatarecord = new BsonDocument();

            for (int i = 0; i < record.fields.Count; i++)
            {
                eTricksDatarecord.Add(record.fields[i].Name, record.fields[i].value);
            }
            try
            {
                eTriksCollection.InsertOneAsync(eTricksDatarecord);
                return "RECORD(s) SUCCESSFULLY INSERTED";
            }
            catch (Exception e)
            {
                while (e.InnerException != null)
                    e = e.InnerException;
                return e.Message;
            }
        }

        // The Generic Update
        public string updateDataGeneric(NoSQLRecordForUpdate updateRecord)
        {
            IMongoDatabase dbETriks = GetDatabase();
            var eTRIKSRecords = dbETriks.GetCollection<BsonDocument>(mongoCollectionName);

            //QueryDocument query = new QueryDocument();
            //for (int i = 0; i < updateRecord.CurrentRecord.Count; i++)
            //{
            //    query.Add(updateRecord.CurrentRecord[i].Name, updateRecord.CurrentRecord[i].value);
            //}

            //UpdateBuilder update = new UpdateBuilder();
            //for (int i = 0; i < updateRecord.NewRecord.Count; i++)
            //{
            //    update = Update.Set(updateRecord.NewRecord[i].Name, updateRecord.NewRecord[i].value);
            //}

            //try
            //{
            //    var result = eTRIKSRecords.FindAndModify(query, null, update, true);
                return "RECORD UPDATED";
            //}

            //catch (Exception e)
            //{
            //    while (e.InnerException != null)
            //        e = e.InnerException;
            //    return e.Message;
            //}
        }

        // The Generic Delete
        public string deleteDataGeneric(MongoDocument record)
        {
            //IMongoDatabase dbETriks = GetDatabase();
            //var eTRIKSRecords = dbETriks.GetCollection<BsonDocument>(mongoCollectionName);

            //QueryDocument query = new QueryDocument();
            //for (int i = 0; i < record.fields.Count; i++)
            //{
            //    query.Add(record.fields[i].Name, record.fields[i].value);
            //}
            //try
            //{
            //    eTRIKSRecords.DeleteOneAsync(query);
                return "RECORD DELETED";
            //}
            //catch (Exception e)
            //{
            //    while (e.InnerException != null)
            //        e = e.InnerException;
            //    return e.Message;
            //}
        }

        //Method to check if category or subcategory exist
        public bool checkForFieldInNoSQL(MongoDataCollection recordSet, string fieldName)
        {
            for (int k = 0; k < recordSet.documents.Count(); k++)
            {
                if (recordSet.documents[k].fields.Exists(f => f.Name.Equals(fieldName)))
                {
                    return true;
                }
            }
            return false;
        }

        //public void loadDataForControlledDataset(NoSQLRecordForControlledDataset rec)
        //{
        //    MongoDatabase dbETriks = GetDatabase();
        //    MongoCollection<BsonDocument> eTriksCollection = dbETriks.GetCollection<BsonDocument>("dataStream_temp");
        //    BsonDocument eTricksDatarecord = new BsonDocument {
        //        { "StudyId", rec.studyId},
        //        { "ItemGroup", rec.itemGroup }
        //        };

        //    //Get the Dynamic fields
        //    for (int i = 0; i < rec.recordItem.Count; i++)
        //    {
        //        eTricksDatarecord.Add(rec.recordItem[i].fieldName, rec.recordItem[i].value);
        //    }

        //    try
        //    {
        //        eTriksCollection.Insert(eTricksDatarecord);
        //    }
        //    catch (Exception e) { }
        //}


        public MongoDataCollection getNoSQLRecordsForGraph(List<MongoField> where, List<string> observation, List <string> filter, string code)
        {
            var observationList = new BsonValue[observation.Count()];
            for (int i = 0; i < observation.Count(); i++)
            {
                observationList[i] = observation[i];
            }

            //QueryDocument whereList = new QueryDocument();
            //for (int i = 0; i < where.Count(); i++)
            //{
            //    whereList.Add(where[i].Name, where[i].value);
            //}

            //var query = Query.And(Query.In(code+"TESTCD", observationList), Query.And(whereList));
            //MongoDatabase dbETriks = GetDatabase();
            //var eTRIKSRecords = dbETriks.GetCollection(mongoCollectionName).Find(query).
            //                  SetFields(Fields.Include(filter.ToArray()).Exclude("_id"));

            MongoDataCollection recordSet = new MongoDataCollection();
            List<MongoDocument> records = new List<MongoDocument>();

            //foreach (var rec in eTRIKSRecords)
            //{
            //    MongoDocument noSQLRec = new MongoDocument();
            //    for (int i = 0; i < rec.ElementCount; i++)
            //    {
            //        MongoField ri = new MongoField();
            //        ri.Name = rec.GetElement(i).Name.ToString();
            //        ri.value = rec.GetElement(i).Value.ToString();
            //        noSQLRec.fields.Add(ri);
            //    }
            //    // Check if mongo record didnt have a matching column vale
            //    if (noSQLRec.fields.Count != 0)
            //    {
            //        records.Add(noSQLRec);
            //    }
            //}
            recordSet.documents = records;
            return recordSet;
        }

        public void exctractFromQueryString(string queryString, out BsonDocument query, out string[] filteredColumnList)
        {
            var parsedString = HttpUtility.HtmlDecode(queryString);
            NameValueCollection coll = HttpUtility.ParseQueryString(parsedString);
            query = new BsonDocument();
            int countConditions = 0;

            // Extract conditions from the query string
            for (int i = 0; i < coll.Count; i++)
            {
                if (coll.GetValues(coll.AllKeys[i])[0] != "*")
                {
                    query.Add(coll.AllKeys[i], coll.GetValues(coll.AllKeys[i])[0]);
                    countConditions++;
                }
                else
                {
                    break;
                }
            }
            // Extract select columns from query string
            // The * signifies the start of the column selection
            filteredColumnList = new string[coll.Count - countConditions];
            for (int j = 0; j < coll.Count - countConditions; j++)
            {
                filteredColumnList[j] = coll.AllKeys[countConditions + j];
            }
        }

        public MongoDataCollection getDistinctNoSQLRecords(string queryString)
        {
            MongoDataCollection recordSet = new MongoDataCollection();
            //List<MongoDocument> records = new List<MongoDocument>();
            //MongoDatabase dbETriks = GetDatabase();

            //QueryDocument query;
            //string[] filteredColumnList;
            //exctractFromQueryString(queryString, out query, out filteredColumnList);

            //var eTRIKSRecords = dbETriks.GetCollection(mongoCollectionName).Distinct(filteredColumnList[0],
            //                                                                query);
            //MongoDocument noSQLRec = new MongoDocument();
            //for (int i = 0; i < eTRIKSRecords.Count(); i++)
            //{
            //    MongoField ri = new MongoField();
            //    ri.Name = filteredColumnList[0];
            //    ri.value = eTRIKSRecords.ElementAt(i).ToString();
            //    noSQLRec.fields.Add(ri);
            //}
            //records.Add(noSQLRec);
            //recordSet.documents = records;
            return recordSet;
        }

        public async Task<MongoDataCollection> getNoSQLRecords(string queryString)
        {
            MongoDataCollection recordSet = new MongoDataCollection();
            List<MongoDocument> records = new List<MongoDocument>();
            IMongoDatabase dbETriks = GetDatabase();

            BsonDocument query;
            string[] filteredColumnList;
            exctractFromQueryString(queryString, out query, out filteredColumnList);
            var projection = Builders<BsonDocument>
                            .Projection
                            .Exclude("_id");
            foreach (string p in filteredColumnList)
            {
                projection.Include(p);
            }
            

            var eTRIKSRecords = await dbETriks.GetCollection<BsonDocument>(mongoCollectionName)
                                .Find(query)
                                .Project(projection).ToListAsync();
                                //.SetFields(Fields.Include(filteredColumnList).Exclude("_id"));

            foreach (var rec in eTRIKSRecords)
            {
                MongoDocument noSQLRec = new MongoDocument(); // the equivalent of a row
                for (int i = 0; i < rec.ElementCount; i++)
                {
                    MongoField ri = new MongoField(); //the equivalent of a field / column and row value
                    ri.Name = rec.GetElement(i).Name.ToString();
                    ri.value = rec.GetElement(i).Value.ToString();
                    noSQLRec.fields.Add(ri);
                }
                // Check if mongo record didnt have a matching column vale
                if (noSQLRec.fields.Count != 0)
                {
                    records.Add(noSQLRec);
                }
            }
            recordSet.documents = records;
            return recordSet;
        }

        //public List<Observation> getGroupedNoSQLrecords(IDictionary<string, string> filterFields, IDictionary<string, string> groupingFields)
        //{
        //    MongoDatabase dbETriks = GetDatabase();

        //    Dictionary<string, object> projectionFields = new Dictionary<string, object>();
        //    projectionFields.Add("_id", 0);
        //    foreach(var item in groupingFields){
        //        projectionFields.Add(item.Key, "$_id." + item.Key);
        //    }

        //    PipelineDefinition aggregateArgs = new BsonDocument[]
        //    {
                
        //                new BsonDocument ("$match",   filterFields.ToBsonDocument()),
        //                new BsonDocument ("$group", new BsonDocument{
        //                        {"_id", groupingFields.ToBsonDocument() } 
        //                    }),
        //                new BsonDocument("$project", projectionFields.ToBsonDocument())
                    
        //    };

        //    var observationRecords = dbETriks.GetCollection(mongoCollectionName)
        //                            .Aggregate(aggregateArgs)
        //                            .Select(BsonSerializer.Deserialize<Observation>).ToList();;

        //    return observationRecords;
        //}

        //public List<SubjObservationTemp> test(string studyId, string queryVariable, string observationName, List<string> returnVariables)
        //{
        //    MongoDatabase dbETriks = GetDatabase();


        //    Dictionary<string, string> pivotVariables = new Dictionary<string, string>();
        //    pivotVariables.Add("test", "$" + queryVariable);
        //    foreach (string rv in returnVariables)
        //    {
        //        pivotVariables.Add(rv, "$"+rv);
        //    }

        //    //pivotVariables.Add("visitNo", "$VISITNUM");
        //    //pivotVariables.Add("timepoint", "$" + observation.DomainCode + "TPT");
        //    //pivotVariables.Add("day", "$" + observation.DomainCode + "DY");


        //    Dictionary<string, object> projectionFields = new Dictionary<string, object>();
        //    projectionFields.Add("subjId", "$_id");
        //    projectionFields.Add("_id", 0);
        //    foreach (var pivotVar in pivotVariables)
        //    {
        //        projectionFields.Add(pivotVar.Key, "$observations." + pivotVar.Key);
        //    }

        //    AggregateArgs aggregateArgs = new AggregateArgs()
        //    {
        //        Pipeline = new[]
        //            {
        //                new BsonDocument ("$match", Query.And(Query.EQ("STUDYID",studyId),
        //                                            Query.EQ(queryVariable,new BsonString(observationName))
        //                                            ).ToBsonDocument()),// Query.In(queryVariable, new BsonArray(observationList.ToArray())).ToBsonDocument() ),
        //                new BsonDocument ("$group", new BsonDocument{
        //                        {"_id", "$USUBJID" },
        //                        {"observations", 
        //                            new BsonDocument {
        //                                {"$push", pivotVariables.ToBsonDocument()}
        //                            }   
        //                        }}),
        //                new BsonDocument("$unwind", "$observations"),
        //                new BsonDocument("$project", projectionFields.ToBsonDocument())
        //            }
        //    };

        //    var observationRecords = dbETriks.GetCollection(mongoCollectionName)
        //                            .Aggregate(aggregateArgs)
        //                            .Select(BsonSerializer.Deserialize<SubjObservationTemp>)
        //                            .ToList(); ;
        //   return observationRecords;

            

            
        //     /* new BsonDocument{
        //                      { "VSTESTCD", new BsonDocument{
        //                            {"$in", new BsonArray{"BMI","TEMP"} }}}}
        //     */
        //}
    }




 
}
