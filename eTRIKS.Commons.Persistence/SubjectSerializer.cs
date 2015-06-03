using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.Timing;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Persistence
{
    class SubjectSerializer : SerializerBase<Subject>, IBsonDocumentSerializer, IBsonIdProvider
    {
        public static Dictionary<string, BsonSerializationInfo> DynamicMappers = new Dictionary<string, BsonSerializationInfo>();
        public override Subject Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            var reader = (BsonReader)context.Reader;
            Subject subject = new Subject();
            
            while (reader.ReadBsonType() != BsonType.EndOfDocument)
            {
                String fieldName = reader.ReadName();
                switch (fieldName)
                    {
                        case "_id":
                            subject.Id = reader.ReadBinaryData().AsGuid;
                            break;
                        case "STUDYID":
                            subject.StudyId = reader.ReadString();
                            break;
                        case "DOMAIN":
                            reader.ReadString();
                            break;
                        case "USUBJID":
                            subject.SubjId = reader.ReadString();
                            break;
                        case "SITEID":
                            subject.Site= reader.ReadString();
                            break;
                        case "ARMCD":
                            subject.Arm = reader.ReadString();
                            break;
                        case "RFSTDTC":
                            string dateStr_s = reader.ReadString();
                            DateTime dt; if (dateStr_s != null)
                            subject.StudyStartDate = DateTime.Parse(dateStr_s);
                            break;
                        case "RFENDTC":
                            string dateStr_e = reader.ReadString();
                            if (dateStr_e != null)
                            subject.StudyStartDate = DateTime.Parse(dateStr_e);
                            break;
                        default:
                            subject.characteristicsValues.Add(fieldName, reader.ReadString());
                            break;
                    }
            }
            context.Reader.ReadEndDocument();


            return subject;
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Subject value)
        {
            var subjObs = value;
        //    BsonDocument document = new
        //{
        //   _id = _id,
        //    CreateDate = LogDate.Ticks
        //};
        //var bdoc = document.ToBsonDocument();
           // context.Writer.WriteRawBsonDocument bdoc, options);

        }

        public bool TryGetMemberSerializationInfo(string memberName, out BsonSerializationInfo serializationInfo)
        {
            switch (memberName)
            {
                case "SubjectId":
                    serializationInfo = new BsonSerializationInfo("_id", new ObjectIdSerializer(), typeof(ObjectId));
                    return true;
                case "StudyId":
                    serializationInfo = new BsonSerializationInfo("STUDYID", new StringSerializer(), typeof(string));
                    return true;
                case "DomainCode":
                    serializationInfo = new BsonSerializationInfo("DOMAIN", new StringSerializer(), typeof(string));
                    return true;
                default:
                    serializationInfo = null;
                    return false;
            }
        }

        public bool GetDocumentId(object document, out object id, out Type idNominalType, out IIdGenerator idGenerator)
        {
            throw new NotImplementedException();
        }

        public void SetDocumentId(object document, object id)
        {
            throw new NotImplementedException();
        }

    }

}
