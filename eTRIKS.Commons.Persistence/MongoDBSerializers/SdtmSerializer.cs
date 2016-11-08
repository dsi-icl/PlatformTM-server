using eTRIKS.Commons.Core.Domain.Model.Data.SDTM;
using eTRIKS.Commons.Core.Domain.Model.Timing;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace eTRIKS.Commons.Persistence
{
    public class SdtmSerializer : SerializerBase<SdtmRow>, IBsonDocumentSerializer, IBsonIdProvider
    {
        public static Dictionary<string, BsonSerializationInfo> DynamicMappers = new Dictionary<string, BsonSerializationInfo>();

        public static SdtmRowDescriptor sdtmEntityDescriptor;
        public override SdtmRow Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            var reader = (BsonReader)context.Reader;
            var sdtmEntity = new SdtmRow();

            while (reader.ReadBsonType() != BsonType.EndOfDocument)
            {
                String fieldName = reader.ReadName();

               
                 switch (fieldName)
                    {
                        case "_id":
                            reader.ReadBinaryData();
                            break;
                        case "STUDYID":
                            sdtmEntity.StudyId = reader.ReadString();
                            break;
                        case "DOMAIN":
                            sdtmEntity.DomainCode = reader.ReadString();
                            break;
                        case "USUBJID":
                            sdtmEntity.USubjId = reader.ReadString();
                            break;
                        case "SUBJID":
                            sdtmEntity.SubjectId = reader.ReadString();
                            break;
                        case "ARMCD":
                            sdtmEntity.ArmCode = reader.ReadString();
                            break;
                        case "ARM":
                            sdtmEntity.Arm = reader.ReadString();
                            break;
                        case "RFSTDTC":
                            var refstdtc = reader.ReadString();
                            DateTime rfsDT;
                            if (DateTime.TryParse(refstdtc, out rfsDT))
                                sdtmEntity.RFSTDTC = rfsDT;
                            break;
                        case "RFENDTC":
                            var rfedt = reader.ReadString();
                            DateTime rfeDT;
                                if (DateTime.TryParse(rfedt, out rfeDT))
                                    sdtmEntity.RFENDTC = rfeDT;
                            break;
                        case "BSREFID":
                            sdtmEntity.SampleId = reader.ReadString();
                            break;
                        case "DBACTIVITYID":
                            sdtmEntity.ActivityId = reader.ReadInt32();
                            break;
                        case "DBSTUDYID":
                            sdtmEntity.DBStudyId = reader.ReadInt32();
                            break;
                        case "DBDATAFILEID":
                            sdtmEntity.DatafileId = reader.ReadInt32();
                            break;
                        case "DBPROJECTID":
                            sdtmEntity.ProjectId = reader.ReadInt32();
                            break;
                        case "DBPROJECTACC":
                            sdtmEntity.ProjectAccession = reader.ReadString();
                            break;
                        case "DBDATASETID":
                            sdtmEntity.DatasetId = reader.ReadInt32();
                            break;
                        //case "VISIT":
                        //    if(sdtmEntity.Visit==null)
                        //        sdtmEntity.Visit = new Visit();
                        //    sdtmEntity.Visit.Name = reader.ReadString();
                        //    break;
                        //case "VISITNUM":
                        //    if (sdtmEntity.Visit == null)
                        //        sdtmEntity.Visit = new Visit();
                        //    int vnum;
                        //    if (int.TryParse(reader.ReadString(), out vnum))
                        //        sdtmEntity.Visit.Number = vnum;
                        //    break;
                        default:
                        {
                            if (fieldName.EndsWith("BLFL"))
                                sdtmEntity.BaseLineFlag = reader.ReadString().ToUpper().Equals("Y");
                            else
                            if (sdtmEntityDescriptor.TopicVariable!=null && sdtmEntityDescriptor.TopicVariable.Name.Equals(fieldName))
                            {
                                sdtmEntity.Topic = reader.ReadString();

                            }
                            else if (sdtmEntityDescriptor.GroupVariable!=null && sdtmEntityDescriptor.GroupVariable.Name.Equals(fieldName))
                            {
                                sdtmEntity.Group = reader.ReadString();
                            }
                            else if (sdtmEntityDescriptor.SubgroupVariable != null && sdtmEntityDescriptor.SubgroupVariable.Name.Equals(fieldName))
                            {
                                sdtmEntity.Subgroup = reader.ReadString();
                            }
                            else if (sdtmEntityDescriptor.TopicSynonymVariable != null && sdtmEntityDescriptor.TopicSynonymVariable.Name.Equals(fieldName))
                            {
                                sdtmEntity.TopicSynonym = reader.ReadString();
                            }
                            else if (sdtmEntityDescriptor.TopicCVtermVariable != null && sdtmEntityDescriptor.TopicCVtermVariable.Name.Equals(fieldName))
                            {
                                sdtmEntity.TopicControlledTerm = reader.ReadString();
                            }
                            else if (sdtmEntityDescriptor.QualifierVariables.Select(v => v.Name).Contains(fieldName))
                            {
                                sdtmEntity.Qualifiers.Add(fieldName, reader.ReadString());
                            }
                            else if (sdtmEntityDescriptor.ResultVariables.Select(v => v.Name).Contains(fieldName))
                            {
                                sdtmEntity.ResultQualifiers.Add(fieldName, reader.ReadString());
                            }
                            //else if (sdtmEntityDescriptor.GroupDescriptors.Select(v => v.Name).Contains(fieldName))
                            //{
                            //    sdtmEntity.Groups.Add(fieldName, reader.ReadString());
                            //}
                            else if (sdtmEntityDescriptor.SynonymVariables.Select(v => v.Name).Contains(fieldName))
                            {
                                sdtmEntity.QualifierSynonyms.Add(fieldName, reader.ReadString());
                            }
                            else if (sdtmEntityDescriptor.VariableQualifierVariables.Select(v => v.Name).Contains(fieldName))
                            {
                                sdtmEntity.QualifierQualifiers.Add(fieldName, reader.ReadString());
                            }
                            else if (fieldName.EndsWith("TPT"))
                            {
                                if (sdtmEntity.CollectionStudyTimePoint == null)
                                    sdtmEntity.CollectionStudyTimePoint = new RelativeTimePoint();
                                sdtmEntity.CollectionStudyTimePoint.Name = reader.ReadString();
                            }
                            else if (fieldName.EndsWith("TPTNUM"))
                            {
                                if (sdtmEntity.CollectionStudyTimePoint == null)
                                    sdtmEntity.CollectionStudyTimePoint = new RelativeTimePoint();
                                sdtmEntity.CollectionStudyTimePoint.Number = Int32.Parse(reader.ReadString());
                            }
                            else if (fieldName.EndsWith("TPTREF"))
                            {
                                if (sdtmEntity.CollectionStudyTimePoint == null)
                                    sdtmEntity.CollectionStudyTimePoint = new RelativeTimePoint();
                                if (sdtmEntity.CollectionStudyTimePoint.ReferenceTimePoint == null)
                                    sdtmEntity.CollectionStudyTimePoint.ReferenceTimePoint = new AbsoluteTimePoint();
                                sdtmEntity.CollectionStudyTimePoint.ReferenceTimePoint.Name = reader.ReadString();
                            }
                            else if (fieldName.EndsWith("RFTDTC"))
                            {
                                if (sdtmEntity.CollectionStudyTimePoint == null)
                                    sdtmEntity.CollectionStudyTimePoint = new RelativeTimePoint();
                                if (sdtmEntity.CollectionStudyTimePoint.ReferenceTimePoint == null)
                                    sdtmEntity.CollectionStudyTimePoint.ReferenceTimePoint = new AbsoluteTimePoint();
                                ((AbsoluteTimePoint)sdtmEntity.CollectionStudyTimePoint.ReferenceTimePoint).DateTime = DateTime.Parse(reader.ReadString());
                            }
                            else if (fieldName.EndsWith("DTC"))
                            {
                                if (sdtmEntity.CollectionDateTime == null)
                                    sdtmEntity.CollectionDateTime = new AbsoluteTimePoint();
                                string dt = reader.ReadString();
                                DateTime DT;
                                if (DateTime.TryParse(dt, out DT))
                                    sdtmEntity.CollectionDateTime.DateTime = DT;
                            }
                            else if (fieldName.EndsWith("DY"))
                            {
                                if (sdtmEntity.CollectionStudyDay == null)
                                    sdtmEntity.CollectionStudyDay = new RelativeTimePoint();
                                int num;
                                if(int.TryParse(reader.ReadString(),out num))
                                    sdtmEntity.CollectionStudyDay.Number = num;
                                sdtmEntity.CollectionStudyDay.Name = null;
                            }
                            else
                                sdtmEntity.Leftovers.Add(fieldName, reader.ReadString());
                            break;
                        }
                    }

               

                   
            }
            context.Reader.ReadEndDocument();


            return sdtmEntity;
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, SdtmRow sdtmrow)
        {
           
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
                case "DatasetId":
                    serializationInfo = new BsonSerializationInfo("DBDATASETID", new Int32Serializer(), typeof(int));
                    return true;
                case "ActivityId":
                    serializationInfo = new BsonSerializationInfo("DBACTIVITYID", new Int32Serializer(), typeof(int));
                    return true;
                case "DatafileId":
                    serializationInfo = new BsonSerializationInfo("DBDATAFILEID", new Int32Serializer(), typeof(int));
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
