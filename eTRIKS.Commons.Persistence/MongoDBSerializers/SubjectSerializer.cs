using eTRIKS.Commons.Core.Domain.Model;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace eTRIKS.Commons.Persistence
{
    class SubjectSerializer : SerializerBase<HumanSubject>, IBsonDocumentSerializer, IBsonIdProvider
    {
        public static Dictionary<string, BsonSerializationInfo> DynamicMappers = new Dictionary<string, BsonSerializationInfo>();
        public override HumanSubject Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            var reader = (BsonReader)context.Reader;
            HumanSubject subject = new HumanSubject();
            
            while (reader.ReadBsonType() != BsonType.EndOfDocument)
            {
                String fieldName = reader.ReadName();
                DateTime dt;
                switch (fieldName)
                    {
                        case "_id":
                            //subject.Id = reader.ReadBinaryData().AsGuid;
                            break;
                        case "STUDYID":
                            //subject.StudyId = reader.ReadString();
                            break;
                        case "DOMAIN":
                            reader.ReadString();
                            break;
                        case "USUBJID":
                            subject.UniqueSubjectId = reader.ReadString();
                            break;
                        case "SITEID":
                            //subject.Site= reader.ReadString();
                            break;
                        case "ARMCD":
                            subject.ArmCode = reader.ReadString();
                            break;
                        case "RFSTDTC":
                            string dateStr_s = reader.ReadString();
                            if (dateStr_s != null)
                            {
                                if(Regex.IsMatch(dateStr_s, "(((0|1)[1-9]|2[1-9]|3[0-1])\\/(0[1-9]|1[0-2])\\/((19|20)\\d\\d))$"))
                                    dt= DateTime.ParseExact(dateStr_s, "dd/MM/yyyy", null);
                                else
                                    dt =  DateTime.Parse(dateStr_s);
                                //subject.StudyStartDate = dt;
                            }
                            break;
                        case "RFENDTC":
                            string dateStr_e = reader.ReadString();
                            if (dateStr_e != null)
                            {
                                if(Regex.IsMatch(dateStr_e, "(((0|1)[1-9]|2[1-9]|3[0-1])\\/(0[1-9]|1[0-2])\\/((19|20)\\d\\d))$"))
                                    dt= DateTime.ParseExact(dateStr_e, "dd/MM/yyyy", null);
                                else
                                    dt =  DateTime.Parse(dateStr_e);
                                //subject.StudyEndDate = dt;
                            }
                            break;
                        default:
                            //subject.characteristicsValues.Add(fieldName, reader.ReadString());
                            break;
                    }
            }
            context.Reader.ReadEndDocument();


            return subject;
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, HumanSubject value)
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
