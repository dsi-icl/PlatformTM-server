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
    public class SubjectObsSerializer : SerializerBase<SubjectObservation>, IBsonDocumentSerializer, IBsonIdProvider
    {
        public static Dictionary<string, BsonSerializationInfo> DynamicMappers = new Dictionary<string, BsonSerializationInfo>();
        public override SubjectObservation Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            var reader = (BsonReader)context.Reader;
            SubjectObservation subjObs = new SubjectObservation();
            while (reader.ReadBsonType() != BsonType.EndOfDocument)
            {
                String fieldName = reader.ReadName();


                if (fieldName.EndsWith("TESTCD") || fieldName.EndsWith("TERM") || fieldName.EndsWith("TRT"))
                    subjObs.Name = reader.ReadString();

                if (fieldName.EndsWith("CAT"))
                    subjObs.Group = reader.ReadString();

                else if (fieldName.EndsWith("SCAT"))
                    subjObs.Subgroup = reader.ReadString();

                else if (fieldName.EndsWith("STDTC"))
                {
                    string dateStr = reader.ReadString();
                    DateTime dt;
                    if (dateStr != null)
                    {
                        try
                        {
                            dt = DateTime.Parse(dateStr);
                            if (subjObs.ObsInterval == null)
                                subjObs.ObsInterval = new TimeInterval();
                            subjObs.ObsInterval.Start = new AbsoluteTimePoint();
                            ((AbsoluteTimePoint)subjObs.ObsInterval.Start).DateTime = dt;
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.Write("Failed to parse date for " + dateStr);
                        }
                        
                       
                    }
                    
                }
                else if (fieldName.EndsWith("ENDTC"))
                {
                    string dateStr = reader.ReadString();
                    if (dateStr != null)
                    {
                        try
                        {
                            if (subjObs.ObsInterval == null)
                                subjObs.ObsInterval = new TimeInterval();
                            subjObs.ObsInterval.End = new AbsoluteTimePoint();
                            ((AbsoluteTimePoint)subjObs.ObsInterval.End).DateTime = DateTime.Parse(dateStr);
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.Write("Failed to parse date for " + dateStr);
                        }
                       
                    }
                    
                }
                else if (fieldName.EndsWith("TPT"))
                {
                    if (subjObs.ObsStudyTimePoint == null)
                        subjObs.ObsStudyTimePoint = new RelativeTimePoint();
                    subjObs.ObsStudyTimePoint.Name = reader.ReadString();
                }
                else if (fieldName.EndsWith("TPTNUM"))
                {
                    if (subjObs.ObsStudyTimePoint == null)
                        subjObs.ObsStudyTimePoint = new RelativeTimePoint();
                    subjObs.ObsStudyTimePoint.Number = Int32.Parse(reader.ReadString());
                }
                else if (fieldName.EndsWith("TPTREF"))
                {
                    if (subjObs.ObsStudyTimePoint == null)
                        subjObs.ObsStudyTimePoint = new RelativeTimePoint();
                    if (subjObs.ObsStudyTimePoint.ReferenceTimePoint == null)
                        subjObs.ObsStudyTimePoint.ReferenceTimePoint = new AbsoluteTimePoint();
                    subjObs.ObsStudyTimePoint.ReferenceTimePoint.Name = reader.ReadString();
                }
                else if (fieldName.EndsWith("RFTDTC"))
                {
                    if (subjObs.ObsStudyTimePoint == null)
                        subjObs.ObsStudyTimePoint = new RelativeTimePoint();
                    if (subjObs.ObsStudyTimePoint.ReferenceTimePoint == null)
                        subjObs.ObsStudyTimePoint.ReferenceTimePoint = new AbsoluteTimePoint();
                    ((AbsoluteTimePoint)subjObs.ObsStudyTimePoint.ReferenceTimePoint).DateTime = DateTime.Parse(reader.ReadString());
                }
                else if (fieldName.EndsWith("DTC"))
                {
                    if (subjObs.ObDateTime == null)
                        subjObs.ObDateTime = new AbsoluteTimePoint();
                    string dt = reader.ReadString();
                    DateTime DT;
                    if (DateTime.TryParse(dt, out DT))
                        subjObs.ObDateTime.DateTime = DT;
                }
                else if (fieldName.EndsWith("STDY"))
                {
                    reader.ReadString();
                    //if (subjObs.ObsInterval == null)
                    //    subjObs.ObsInterval = new TimeInterval();
                    //if(subjObs.ObsInterval.Start == null)
                    //    subjObs.ObsInterval.Start = new RelativeTimePoint();
                    //((RelativeTimePoint)subjObs.ObsInterval.Start).Number = Int32.Parse(reader.ReadString());
                }
                else if (fieldName.EndsWith("ENDY"))
                {
                    reader.ReadString();
                    //if (subjObs.ObsInterval == null)
                    //    subjObs.ObsInterval = new TimeInterval();
                    //if (subjObs.ObsInterval.End == null)
                    //    subjObs.ObsInterval.End = new RelativeTimePoint();
                    //((RelativeTimePoint)subjObs.ObsInterval.End).Number = Int32.Parse(reader.ReadString());
                }
                else if (fieldName.EndsWith("DY"))
                {
                    if (subjObs.ObsStudyDay == null)
                        subjObs.ObsStudyDay = new RelativeTimePoint();
                    subjObs.ObsStudyDay.Number = Int32.Parse(reader.ReadString());
                }
                else
                    switch (fieldName)
                    {
                        case "_id":
                            subjObs.Id = reader.ReadBinaryData().AsGuid;
                            break;
                        case "STUDYID":
                            subjObs.StudyId = reader.ReadString();
                            break;
                        case "DOMAIN":
                            subjObs.DomainCode = reader.ReadString();
                            break;
                        case "USUBJID":
                            subjObs.SubjectId = reader.ReadString();
                            break;
                        case "VISIT":
                            subjObs.Visit = reader.ReadString();
                            break;
                        case "VISITNUM":
                            subjObs.VisitNum = Int32.Parse(reader.ReadString());
                            break;
                        default:
                            subjObs.qualifiers.Add(fieldName, reader.ReadString());
                            break;
                    }


            }
            context.Reader.ReadEndDocument();


            return subjObs;
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, SubjectObservation value)
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
                case "DomainCode":
                    serializationInfo = new BsonSerializationInfo("DOMAIN", new StringSerializer(), typeof(string));
                    return true;
                case "Name":
                    serializationInfo = DynamicMappers.First(d => d.Key.Equals(memberName)).Value;
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
