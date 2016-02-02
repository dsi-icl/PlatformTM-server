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
    class MongoDocumentSerializer : SerializerBase<MongoDocument>, IBsonDocumentSerializer
    {
        //public static Dictionary<string, BsonSerializationInfo> DynamicMappers = new Dictionary<string, BsonSerializationInfo>();

        //public static SdtmEntityDefine sdtmEntityDescriptor;
        public override MongoDocument Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            //context.Reader.ReadStartDocument();
            //var reader = (BsonReader)context.Reader;
            var mongodoc = new MongoDocument();

            //while (reader.ReadBsonType() != BsonType.EndOfDocument)
            //{
            //    String fieldName = reader.ReadName();

               
            //     switch (fieldName)
            //        {
            //            case "_id":
            //                reader.ReadBinaryData();
            //                break;
            //            case "STUDYID":
            //                sdtmEntity.VerbatimStudyId = reader.ReadString();
            //                break;
            //            case "DOMAIN":
            //                sdtmEntity.DomainCode = reader.ReadString();
            //                break;
            //            case "USUBJID":
            //                sdtmEntity.USubjId = reader.ReadString();
            //                break;
            //            case "SUBJID":
            //                sdtmEntity.SubjectId = reader.ReadString();
            //                break;
            //            case "ARMCD":
            //                sdtmEntity.ArmCode = reader.ReadString();
            //                break;
            //            case "ARM":
            //                sdtmEntity.Arm = reader.ReadString();
            //                break;
            //            case "RFSTDTC":
            //                sdtmEntity.RFSTDTC = DateTime.Parse(reader.ReadString());
            //                break;
            //            case "RFENDTC":
            //                var rfedt = reader.ReadString();
            //                    DateTime rfeDT;
            //                    if (DateTime.TryParse(rfedt, out rfeDT))
            //                        sdtmEntity.RFENDTC = rfeDT;
            //                break;
            //            case "BSREFID":
            //                sdtmEntity.SampleId = reader.ReadString();
            //                break;
            //            case "DBACTIVITYID":
            //                sdtmEntity.ActivityId = reader.ReadInt32();
            //                break;
            //            case "DBSTUDYID":
            //                sdtmEntity.DBstudyId = reader.ReadInt32();
            //                break;
            //            case "DBPROJECTID":
            //                sdtmEntity.ProjectId = reader.ReadInt32();
            //                break;
            //            case "DBDATASETID":
            //                sdtmEntity.DatasetId = reader.ReadInt32();
            //                break;
            //            case "VISIT":
            //                if(sdtmEntity.Visit==null)
            //                    sdtmEntity.Visit = new Visit();
            //                sdtmEntity.Visit.Name = reader.ReadString();
            //                break;
            //            case "VISITNUM":
            //                if (sdtmEntity.Visit == null)
            //                    sdtmEntity.Visit = new Visit();
            //                 sdtmEntity.Visit.Number = Int32.Parse(reader.ReadString());
            //                break;
            //            default:
            //            {
            //                if (sdtmEntityDescriptor.QualifierVariables.Select(v => v.Name).Contains(fieldName))
            //                {
            //                    sdtmEntity.Qualifiers.Add(fieldName, reader.ReadString());
            //                }
            //                else if (sdtmEntityDescriptor.GroupDescriptors.Select(v => v.Name).Contains(fieldName))
            //                {
            //                    sdtmEntity.Groups.Add(fieldName, reader.ReadString());
            //                }
            //                else if (sdtmEntityDescriptor.SynonymVariables.Select(v => v.Name).Contains(fieldName))
            //                {
            //                    sdtmEntity.QualifierSynonyms.Add(fieldName, reader.ReadString());
            //                }
            //                else if (sdtmEntityDescriptor.VariableQualifierVariables.Select(v => v.Name).Contains(fieldName))
            //                {
            //                    sdtmEntity.QualifierQualifiers.Add(fieldName, reader.ReadString());
            //                }
            //                else if (fieldName.EndsWith("TPT"))
            //                {
            //                    if (sdtmEntity.CollectionStudyTimePoint == null)
            //                        sdtmEntity.CollectionStudyTimePoint = new RelativeTimePoint();
            //                    sdtmEntity.CollectionStudyTimePoint.Name = reader.ReadString();
            //                }
            //                else if (fieldName.EndsWith("TPTNUM"))
            //                {
            //                    if (sdtmEntity.CollectionStudyTimePoint == null)
            //                        sdtmEntity.CollectionStudyTimePoint = new RelativeTimePoint();
            //                    sdtmEntity.CollectionStudyTimePoint.Number = Int32.Parse(reader.ReadString());
            //                }
            //                else if (fieldName.EndsWith("TPTREF"))
            //                {
            //                    if (sdtmEntity.CollectionStudyTimePoint == null)
            //                        sdtmEntity.CollectionStudyTimePoint = new RelativeTimePoint();
            //                    if (sdtmEntity.CollectionStudyTimePoint.ReferenceTimePoint == null)
            //                        sdtmEntity.CollectionStudyTimePoint.ReferenceTimePoint = new AbsoluteTimePoint();
            //                    sdtmEntity.CollectionStudyTimePoint.ReferenceTimePoint.Name = reader.ReadString();
            //                }
            //                else if (fieldName.EndsWith("RFTDTC"))
            //                {
            //                    if (sdtmEntity.CollectionStudyTimePoint == null)
            //                        sdtmEntity.CollectionStudyTimePoint = new RelativeTimePoint();
            //                    if (sdtmEntity.CollectionStudyTimePoint.ReferenceTimePoint == null)
            //                        sdtmEntity.CollectionStudyTimePoint.ReferenceTimePoint = new AbsoluteTimePoint();
            //                    ((AbsoluteTimePoint)sdtmEntity.CollectionStudyTimePoint.ReferenceTimePoint).DateTime = DateTime.Parse(reader.ReadString());
            //                }
            //                else if (fieldName.EndsWith("DTC"))
            //                {
            //                    if (sdtmEntity.CollectionDateTime == null)
            //                        sdtmEntity.CollectionDateTime = new AbsoluteTimePoint();
            //                    string dt = reader.ReadString();
            //                    DateTime DT;
            //                    if (DateTime.TryParse(dt, out DT))
            //                        sdtmEntity.CollectionDateTime.DateTime = DT;
            //                }
            //                else if (fieldName.EndsWith("DY"))
            //                {
            //                    if (sdtmEntity.CollectionStudyDay == null)
            //                        sdtmEntity.CollectionStudyDay = new RelativeTimePoint();
            //                    sdtmEntity.CollectionStudyDay.Number = Int32.Parse(reader.ReadString());
            //                }
            //                else
            //                    sdtmEntity.Leftovers.Add(fieldName, reader.ReadString());
            //                break;
            //            }
            //        }

               

                   
            //}
            //context.Reader.ReadEndDocument();


            return mongodoc;
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, MongoDocument value)
        {
            var mongoDoc = value;
            var doc = new BsonDocument();
            doc.Set("_id", Guid.NewGuid());

            foreach (var field in mongoDoc.fields)
            {
                if (field.value is int)
                    doc.Add(field.Name, Convert.ToInt16(field.value));
                else
                    doc.Add(field.Name, Convert.ToString(field.value));
            }

           //context.Writer.WriteRawBsonDocument(doc.);
            BsonSerializer.Serialize(context.Writer,doc);

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
                case "DatasetId":
                    serializationInfo = new BsonSerializationInfo("DBDATASETID", new Int32Serializer(), typeof(int));
                    return true;
                case "ActivityId":
                    serializationInfo = new BsonSerializationInfo("DBACTIVITYID", new Int32Serializer(), typeof(int));
                    return true;
                default:
                    serializationInfo = null;
                    return false;
            }
        }

        //public bool GetDocumentId(object document, out object id, out Type idNominalType, out IIdGenerator idGenerator)
        //{
        //    throw new NotImplementedException();
        //}

        //public void SetDocumentId(object document, object id)
        //{
        //    throw new NotImplementedException();
        //}

    }

}
