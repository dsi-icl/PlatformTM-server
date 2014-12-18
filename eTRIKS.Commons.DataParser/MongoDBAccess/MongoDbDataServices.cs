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
    class MongoDbDataServices
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
            string status = "ERROR";
            MongoDatabase dbETriks = GetDatabase();
            MongoCollection<BsonDocument> eTriksCollection = dbETriks.GetCollection<BsonDocument>("dataStream_temp");
            BsonDocument eTricksDatarecord = new BsonDocument();

            for (int i = 0; i < record.recordItem.Count; i++)
            {
                eTricksDatarecord.Add(record.recordItem[i].fieldName, record.recordItem[i].value);
            }
            try
            {
                eTriksCollection.Insert(eTricksDatarecord);
                status = "RECORD(s) SUCCESSFULLY INSERTED";
            }
            catch (Exception e) { return status; }
            return status;

        }

        public void loadDataForControlledDataset(NoSQLRecordForControlledDataset rec)
        {
            MongoDatabase dbETriks = GetDatabase();
            MongoCollection<BsonDocument> eTriksCollection = dbETriks.GetCollection<BsonDocument>("dataStream_temp");
            BsonDocument eTricksDatarecord = new BsonDocument {
                { "StudyId", rec.studyId},
                { "ItemGroup", rec.itemGroup }
                };

            //Get the Dynamic fields
            for (int i = 0; i < rec.recordItem.Count; i++)
            {
                eTricksDatarecord.Add(rec.recordItem[i].fieldName, rec.recordItem[i].value);
            }

            try
            {
                eTriksCollection.Insert(eTricksDatarecord);
            }
            catch (Exception e) { }
        }

        public List<NoSQLRecordForControlledDataset> getNoSQLRecord(string field, string value)
        {
            List<NoSQLRecordForControlledDataset> records = new List<NoSQLRecordForControlledDataset>();
            MongoDatabase dbETriks = GetDatabase();
            var query = Query.And(Query.EQ(field, BsonValue.Create(value)));
            var eTRIKSRecords = dbETriks.GetCollection("dataStream_temp").Find(query);
            DataSet ds = new DataSet();

            foreach (var rec in eTRIKSRecords)
            {
                NoSQLRecordForControlledDataset noSQLRec = new NoSQLRecordForControlledDataset();
                noSQLRec.studyId = rec.GetElement("StudyId").ToString();
                noSQLRec.itemGroup = rec.GetElement("ItemGroup").ToString();

                for (int i = 2; i < rec.ElementCount; i++)
                {
                    RecordItem ri = new RecordItem();
                    ri.fieldName = rec.GetElement(i).Name.ToString();
                    ri.value = rec.GetElement(i).Value.ToString();
                    noSQLRec.recordItem.Add(ri);
                }
                records.Add(noSQLRec);
            }

            return records;
        }
    }



    //The NOSQL classes are located here until its moved to the data model
    class NoSQLRecord
    {
        public List<RecordItem> recordItem = new List<RecordItem>();
    }

    class NoSQLRecordForControlledDataset
    {
        public string studyId { get; set; }
        public string itemGroup { get; set; }
        public List<RecordItem> recordItem = new List<RecordItem>();
    }

    class RecordItem
    {
        public string fieldName { get; set; }
        public string value { get; set; }
    }
}
