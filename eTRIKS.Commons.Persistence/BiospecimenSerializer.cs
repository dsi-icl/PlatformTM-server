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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Persistence
{
    class BiospecimenSerializer : SerializerBase<Biospecimen>, IBsonDocumentSerializer, IBsonIdProvider
    {
        public static Dictionary<string, BsonSerializationInfo> DynamicMappers = new Dictionary<string, BsonSerializationInfo>();
        public override Biospecimen Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            var reader = (BsonReader)context.Reader;
            Biospecimen biospecimen = new Biospecimen();
            
            while (reader.ReadBsonType() != BsonType.EndOfDocument)
            {
                String fieldName = reader.ReadName();

                if (fieldName.EndsWith("TPT"))
                {
                    if (biospecimen.ObsStudyTimePoint == null)
                        biospecimen.ObsStudyTimePoint = new RelativeTimePoint();
                    biospecimen.ObsStudyTimePoint.Name = reader.ReadString();
                }
                else if (fieldName.EndsWith("TPTNUM"))
                {
                    if (biospecimen.ObsStudyTimePoint == null)
                        biospecimen.ObsStudyTimePoint = new RelativeTimePoint();
                    biospecimen.ObsStudyTimePoint.Number = Int32.Parse(reader.ReadString());
                }
                else if (fieldName.EndsWith("TPTREF"))
                {
                    if (biospecimen.ObsStudyTimePoint == null)
                        biospecimen.ObsStudyTimePoint = new RelativeTimePoint();
                    if (biospecimen.ObsStudyTimePoint.ReferenceTimePoint == null)
                        biospecimen.ObsStudyTimePoint.ReferenceTimePoint = new AbsoluteTimePoint();
                    biospecimen.ObsStudyTimePoint.ReferenceTimePoint.Name = reader.ReadString();
                }
                else if (fieldName.EndsWith("RFTDTC"))
                {
                    if (biospecimen.ObsStudyTimePoint == null)
                        biospecimen.ObsStudyTimePoint = new RelativeTimePoint();
                    if (biospecimen.ObsStudyTimePoint.ReferenceTimePoint == null)
                        biospecimen.ObsStudyTimePoint.ReferenceTimePoint = new AbsoluteTimePoint();
                    ((AbsoluteTimePoint)biospecimen.ObsStudyTimePoint.ReferenceTimePoint).DateTime = DateTime.Parse(reader.ReadString());
                }
                else if (fieldName.EndsWith("DTC"))
                {
                    if (biospecimen.ObDateTime == null)
                        biospecimen.ObDateTime = new AbsoluteTimePoint();
                    string dt = reader.ReadString();
                    DateTime DT;
                    if (DateTime.TryParse(dt, out DT))
                        biospecimen.ObDateTime.DateTime = DT;
                }
                
                else if (fieldName.EndsWith("DY"))
                {
                    if (biospecimen.ObsStudyDay == null)
                        biospecimen.ObsStudyDay = new RelativeTimePoint();
                    biospecimen.ObsStudyDay.Number = Int32.Parse(reader.ReadString());
                }
                else

                switch (fieldName)
                    {
                        case "_id":
                            biospecimen.Id = reader.ReadBinaryData().AsGuid;
                            break;
                        case "STUDYID":
                            biospecimen.StudyId = reader.ReadString();
                            break;
                        case "DOMAIN":
                            biospecimen.DomainCode = reader.ReadString();
                            break;
                        case "USUBJID":
                            biospecimen.SubjectId = reader.ReadString();
                            break;
                        case "BSREFID":
                            biospecimen.SampleId= reader.ReadString();
                            break;
                        case "BSGRPID":
                            biospecimen.AssayId = Int32.Parse(reader.ReadString());
                            break;
                        case "VISIT":
                            biospecimen.Visit = reader.ReadString();
                            break;
                        case "VISITNUM":
                            biospecimen.VisitNum = Int32.Parse(reader.ReadString());
                            break;
                        default:
                            biospecimen.Characteristics.Add(fieldName, reader.ReadString());
                            break;
                    }
            }
            context.Reader.ReadEndDocument();


            return biospecimen;
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Biospecimen value)
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
