using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.DataParser.MongoDBAccess
{
    public class MongoDbDataServices
    {
        public MongoDatabase GetDatabase()
        {
            // Create server settings to pass connection string, timeout, etc.
            MongoServerSettings settings = new MongoServerSettings();
            //IC Cloud
            settings.Server = new MongoServerAddress("146.169.32.143", 27020);
            return MongoServer.Create(settings).GetDatabase("eTRIKS");
        }

        // The Generic loader
        public string loadDataGeneric(NoSQLRecord record)
        {
            MongoDatabase dbETriks = GetDatabase();
            MongoCollection<BsonDocument> eTriksCollection = dbETriks.GetCollection<BsonDocument>("dataStream_temp");
            BsonDocument eTricksDatarecord = new BsonDocument();

            for (int i = 0; i < record.RecordItems.Count; i++)
            {
                eTricksDatarecord.Add(record.RecordItems[i].fieldName, record.RecordItems[i].value);
            }
            try
            {
                eTriksCollection.Insert(eTricksDatarecord);
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
        public string updateDataGeneric(List<NoSQLRecord> record)
        {
            MongoDatabase dbETriks = GetDatabase();
            var eTRIKSRecords = dbETriks.GetCollection("dataStream_temp");

            QueryDocument query = new QueryDocument();
            for (int i = 0; i < record[0].RecordItems.Count; i++)
            {
                query.Add(record[0].RecordItems[i].fieldName, record[0].RecordItems[i].value);
            }

            UpdateBuilder update = new UpdateBuilder();
            for (int i = 0; i < record[1].RecordItems.Count; i++)
            {
                update = Update.Set(record[1].RecordItems[i].fieldName, record[1].RecordItems[i].value);
            }

            try
            {
                var result = eTRIKSRecords.FindAndModify(query, null, update, true);
                return "RECORD UPDATED";
            }

            catch (Exception e)
            {
                while (e.InnerException != null)
                    e = e.InnerException;
                return e.Message;
            }
        }

        // The Generic Delete
        public string deleteDataGeneric(NoSQLRecord record)
        {
            MongoDatabase dbETriks = GetDatabase();
            var eTRIKSRecords = dbETriks.GetCollection("dataStream_temp");

            QueryDocument query = new QueryDocument();
            for (int i = 0; i < record.RecordItems.Count; i++)
            {
                query.Add(record.RecordItems[i].fieldName, record.RecordItems[i].value);
            }
            try
            {
                eTRIKSRecords.Remove(query);
                return "RECORD DELETED";
            }
            catch (Exception e)
            {
                while (e.InnerException != null)
                    e = e.InnerException;
                return e.Message;
            }
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

        public List<NoSQLRecord> getNoSQLRecord(string field, string value)
        {
            List<NoSQLRecord> records = new List<NoSQLRecord>();
            MongoDatabase dbETriks = GetDatabase();
            var query = Query.And(Query.EQ(field, BsonValue.Create(value)));
            var eTRIKSRecords = dbETriks.GetCollection("dataStream_temp").Find(query);
            DataSet ds = new DataSet();

            foreach (var rec in eTRIKSRecords)
            {
                NoSQLRecord noSQLRec = new NoSQLRecord();
                for (int i = 0; i < rec.ElementCount; i++)
                {
                    RecordItem ri = new RecordItem();
                    ri.fieldName = rec.GetElement(i).Name.ToString();
                    ri.value = rec.GetElement(i).Value.ToString();
                    noSQLRec.RecordItems.Add(ri);
                }
                records.Add(noSQLRec);
            }
            return records;
        }
    }



    //The NOSQL classes are located here until its moved to the data model
    public class NoSQLRecord
    {
        public List<RecordItem> RecordItems = new List<RecordItem>();
    }

    public class NoSQLRecordForControlledDataset
    {
        public string studyId { get; set; }
        public string itemGroup { get; set; }
        public List<RecordItem> recordItem = new List<RecordItem>();
    }

    public class RecordItem
    {
        public string fieldName { get; set; }
        public string value { get; set; }
    }
}
