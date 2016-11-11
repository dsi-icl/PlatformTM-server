using eTRIKS.Commons.Core.Domain.Model.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel.SDTM;
using eTRIKS.Commons.Service.DTOs;

namespace eTRIKS.Commons.Service.Services
{
    public class SDTMreader
    {
        public static List<SdtmRow> ReadSDTM(DataTable dataTable, SdtmRowDescriptor descriptor)
        {
            var SDTM = new List<SdtmRow>();
            foreach (DataRow row in dataTable.Rows)
            {
                SDTM.Add(readSDTMrow(row, dataTable, descriptor));
            }
            return SDTM;
        }

        public static SdtmRow readSDTMrow(DataRow tableRow, DataTable dataTable, SdtmRowDescriptor descriptor)
        {
            var sdtmrow = new SdtmRow();

            sdtmrow.Class = descriptor.Class;
            foreach (DataColumn column in dataTable.Columns)
            {
                var colName = column.ColumnName;
                var value = tableRow[column].ToString();

                if (colName == descriptor.StudyIdentifierVariable.Name)
                {
                    //"STUDYID":
                    sdtmrow.StudyId = value;
                }
                else if (colName == descriptor.DomainVariable.Name)
                {
                    //DOMAIN
                    sdtmrow.DomainCode = value;
                }
                else if (colName == descriptor.SubjIdVariable?.Name)
                {
                    //SUBJID
                    sdtmrow.SubjectId = value;
                }
                else if (colName == descriptor.UniqueSubjIdVariable.Name)
                {
                    //USUBJID
                    sdtmrow.USubjId = value;
                }
                else if (colName == descriptor.SampleIdVariable?.Name)
                {
                    //--REFID
                    sdtmrow.SampleId = value;
                }
                else if (colName == descriptor.TopicVariable?.Name)
                {
                    //--TESTCD // --TERM // --TRT
                    sdtmrow.Topic = value;
                }
                else if (colName == descriptor.TopicSynonymVariable?.Name)
                {
                    //--TEST // --MODIFY
                    sdtmrow.TopicSynonym = value;
                }
                else if (colName == descriptor.TopicCVtermVariable?.Name)
                {
                    //--LOINC --DECODE
                    sdtmrow.TopicControlledTerm = value;
                }
                else if (colName == descriptor.GroupVariable?.Name)
                {
                    //--CAT
                    sdtmrow.Group = value;
                }
                else if (colName == descriptor.SubgroupVariable?.Name)
                {
                    //--SCAT
                    sdtmrow.Subgroup = value;
                }
                else if (descriptor.QualifierVariables.Select(v => v.Name).Contains(colName))
                {
                    //RECORD QUALIFIERS
                    sdtmrow.Qualifiers.Add(colName, value);
                }
                else if (descriptor.ResultVariables.Select(v => v.Name).Contains(colName))
                {
                    //RESULT QUALIFIERS
                    sdtmrow.ResultQualifiers.Add(colName, value);
                }
                else if (descriptor.SynonymVariables.Select(v => v.Name).Contains(colName))
                {
                    //SYNOYMS
                    sdtmrow.QualifierSynonyms.Add(colName, value);
                }
                else if (descriptor.VariableQualifierVariables.Select(v => v.Name).Contains(colName))
                {
                    //VARIABLE QUALIFIERS
                    sdtmrow.QualifierQualifiers.Add(colName, value);
                }
                else if (descriptor.VisitNameVariable?.Name == colName)
                {
                    //VISIT
                    sdtmrow.VisitName = value;
                }
                else if (descriptor.VisitNumVariable?.Name == colName)
                {
                    //VISITNUM
                    sdtmrow.VisitNum = int.Parse(value);
                }
                else if (descriptor.VisitPlannedStudyDay?.Name == colName)
                {
                    //VISITNUM
                    sdtmrow.VisitPlannedStudyDay = int.Parse(value);
                }
                else if (descriptor.DateTimeVariable?.Name == colName)
                {
                    //DTC
                    if (sdtmrow.CollectionDateTime == null)
                        sdtmrow.CollectionDateTime = new AbsoluteTimePoint();
                    DateTime DT;
                    if (DateTime.TryParse(value, out DT))
                        sdtmrow.CollectionDateTime.DateTime = DT;
                }
                else if (descriptor.StudyDayVariable?.Name == colName)
                {
                    //--DY
                    int num;
                    if (!int.TryParse(value, out num)) continue;
                    if (sdtmrow.CollectionStudyDay == null)
                        sdtmrow.CollectionStudyDay = new RelativeTimePoint();
 
                    sdtmrow.CollectionStudyDay.Number = num;
                    sdtmrow.CollectionStudyDay.Name = null;
                }
                else if (descriptor.TimePointNameVariable?.Name == colName)
                {
                    //--TPT
                    if (sdtmrow.CollectionStudyTimePoint == null)
                        sdtmrow.CollectionStudyTimePoint = new RelativeTimePoint();
                    sdtmrow.CollectionStudyTimePoint.Name = value;
                }
                else if (descriptor.TimePointNameVariable?.Name == colName)
                {
                    //--TPTNUM
                    int num;
                    if (!int.TryParse(value, out num)) continue;
                    if (sdtmrow.CollectionStudyTimePoint == null)
                        sdtmrow.CollectionStudyTimePoint = new RelativeTimePoint();
                        sdtmrow.CollectionStudyTimePoint.Number = num;
                }
                else if (descriptor.DurationVariable?.Name == colName)
                {
                    //--DUR
                    sdtmrow.Duration = value;
                }
                else if (descriptor.StartDateTimeVariable?.Name == colName)
                {
                    //--STDTC
                    DateTime dt;
                    if (!DateTime.TryParse(value, out dt)) continue;

                    if (sdtmrow.DateTimeInterval == null)
                        sdtmrow.DateTimeInterval = new TimeInterval();
                    sdtmrow.DateTimeInterval.Start = new AbsoluteTimePoint();
                    ((AbsoluteTimePoint)sdtmrow.DateTimeInterval.Start).DateTime = dt;
                }
                else if (descriptor.EndDateTimeVariable?.Name == colName)
                {
                    //--ENDTC
                    DateTime dt;
                    if (!DateTime.TryParse(value, out dt)) continue;

                    if (sdtmrow.DateTimeInterval == null)
                        sdtmrow.DateTimeInterval = new TimeInterval();
                    sdtmrow.DateTimeInterval.End = new AbsoluteTimePoint();
                    ((AbsoluteTimePoint)sdtmrow.DateTimeInterval.End).DateTime = dt;
                }
                else if (descriptor.StartStudyDayVariable?.Name == colName)
                {
                    //--STDY
                    int val;
                    if(!int.TryParse(value, out val)) continue;

                    if (sdtmrow.StudyDayInterval == null)
                        sdtmrow.StudyDayInterval = new TimeInterval();
                    sdtmrow.StudyDayInterval.Start = new RelativeTimePoint();
                    ((RelativeTimePoint)sdtmrow.StudyDayInterval.Start).Number = val;
                }
                else if (descriptor.EndStudyDayVariable?.Name == colName)
                {
                    //--ENDY
                    int val;
                    if (!int.TryParse(value, out val)) continue;

                    if (sdtmrow.StudyDayInterval == null)
                        sdtmrow.StudyDayInterval = new TimeInterval();
                    sdtmrow.StudyDayInterval.End = new RelativeTimePoint();
                    ((RelativeTimePoint)sdtmrow.StudyDayInterval.End).Number = val;
                }
                else
                    sdtmrow.Leftovers.Add(colName, value);

            }
            return sdtmrow;
        }
    }
}
