using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using PlatformTM.Core.Domain.Model;

namespace PlatformTM.Data.MongoDBSerializers
{
    class BiospecimenSerializer : SerializerBase<Biosample>, IBsonDocumentSerializer, IBsonIdProvider
    {
        public static Dictionary<string, BsonSerializationInfo> DynamicMappers = new Dictionary<string, BsonSerializationInfo>();
        public override Biosample Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            var reader = (BsonReader)context.Reader;
            var biospecimen = new Biosample();

            //while (reader.ReadBsonType() != BsonType.EndOfDocument)
            //{
            //    String fieldName = reader.ReadName();

            //    if (fieldName.EndsWith("TPT"))
            //    {
            //        if (biospecimen.CollectionStudyTimePoint == null)
            //            biospecimen.CollectionStudyTimePoint = new RelativeTimePoint();
            //        biospecimen.CollectionStudyTimePoint.Name = reader.ReadString();
            //    }
            //    else if (fieldName.EndsWith("TPTNUM"))
            //    {
            //        if (biospecimen.CollectionStudyTimePoint == null)
            //            biospecimen.CollectionStudyTimePoint = new RelativeTimePoint();
            //        biospecimen.CollectionStudyTimePoint.Number = Int32.Parse(reader.ReadString());
            //    }
            //    else if (fieldName.EndsWith("TPTREF"))
            //    {
            //        if (biospecimen.CollectionStudyTimePoint == null)
            //            biospecimen.CollectionStudyTimePoint = new RelativeTimePoint();
            //        if (biospecimen.CollectionStudyTimePoint.ReferenceTimePoint == null)
            //            biospecimen.CollectionStudyTimePoint.ReferenceTimePoint = new AbsoluteTimePoint();
            //        biospecimen.CollectionStudyTimePoint.ReferenceTimePoint.Name = reader.ReadString();
            //    }
            //    else if (fieldName.EndsWith("RFTDTC"))
            //    {
            //        if (biospecimen.CollectionStudyTimePoint == null)
            //            biospecimen.CollectionStudyTimePoint = new RelativeTimePoint();
            //        if (biospecimen.CollectionStudyTimePoint.ReferenceTimePoint == null)
            //            biospecimen.CollectionStudyTimePoint.ReferenceTimePoint = new AbsoluteTimePoint();
            //        ((AbsoluteTimePoint)biospecimen.CollectionStudyTimePoint.ReferenceTimePoint).DateTime = DateTime.Parse(reader.ReadString());
            //    }
            //    else if (fieldName.EndsWith("DTC"))
            //    {
            //        if (biospecimen.CollectionDateTime == null)
            //            biospecimen.CollectionDateTime = new AbsoluteTimePoint();
            //        string dt = reader.ReadString();
            //        DateTime DT;
            //        if (DateTime.TryParse(dt, out DT))
            //            biospecimen.CollectionDateTime.DateTime = DT;
            //    }

            //    else if (fieldName.EndsWith("DY"))
            //    {
            //        if (biospecimen.CollectionStudyDay == null)
            //            biospecimen.CollectionStudyDay = new RelativeTimePoint();
            //        biospecimen.CollectionStudyDay.Number = Int32.Parse(reader.ReadString());
            //    }
            //    else

            //        switch (fieldName)
            //        {
            //            case "_id":
            //                // biospecimen.Id = reader.ReadBinaryData().AsGuid;
            //                break;
            //            case "STUDYID":
            //                biospecimen.StudyId = reader.ReadString();
            //                break;
            //            case "DOMAIN":
            //                //biospecimen = reader.ReadString();
            //                break;
            //            case "USUBJID":
            //                biospecimen.SubjectId = reader.ReadString();
            //                break;
            //            case "BSREFID":
            //                biospecimen.SampleId = reader.ReadString();
            //                break;
            //            case "ACTIVITYID":
            //                biospecimen.AssayId = Int32.Parse(reader.ReadString());
            //                break;
            //            case "DATASETID":
            //                biospecimen.DatasetId = Int32.Parse(reader.ReadString());
            //                break;
            //            case "VISIT":
            //                if(biospecimen.Visit==null)
            //                    biospecimen.Visit = new Visit();
            //                biospecimen.Visit.Name = reader.ReadString();
            //                break;
            //            case "VISITNUM":
            //                if (biospecimen.Visit == null)
            //                    biospecimen.Visit = new Visit();
            //                 biospecimen.Visit.Number = Int32.Parse(reader.ReadString());
            //                break;
            //            default:
            //                 biospecimen.SampleProperties.Add(new Property()
            //                 {
            //                     Name = fieldName,
            //                     Value = reader.ReadString(),
            //                     SubjectMatter = "BioSample",
            //                     DatasetDomainCode = "BS"
            //                 });
            //                break;
            //        }
            //}
            context.Reader.ReadEndDocument();


            return biospecimen;
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Biosample value)
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
                case "SampleId":
                    serializationInfo = new BsonSerializationInfo("BSREFID", new StringSerializer(), typeof(string));
                    return true;
                case "AssayId":
                    serializationInfo = new BsonSerializationInfo("BSGRPID", new Int32Serializer(), typeof(int));
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
