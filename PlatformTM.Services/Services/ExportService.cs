using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.DatasetModel.SDTM;
using PlatformTM.Core.Domain.Model.DesignElements;
using PlatformTM.Core.Domain.Model.Timing;
using PlatformTM.Core.Domain.Model.Users.Datasets;
using PlatformTM.Core.Domain.Model.Users.Queries;
using PlatformTM.Services.Configuration;
using PlatformTM.Services.DTOs;
using PlatformTM.Services.DTOs.Export;

namespace PlatformTM.Services.Services
{
    public class ExportService
    {
        private readonly IRepository<ExportFile, Guid> _exportFileRepository;
        private readonly QueryService _queryService;
        private readonly IServiceUoW _dataServiceUnit;
        private readonly string _downloadFileDirectory;
        private FileStorageSettings ConfigSettings { get; set; }

        public ExportService(IServiceUoW uoW, QueryService queryService, IOptions<FileStorageSettings> settings)
        {
            _exportFileRepository = uoW.GetRepository<ExportFile, Guid>();
            _queryService = queryService;
            _dataServiceUnit = uoW;         
            ConfigSettings = settings.Value;
            _downloadFileDirectory = ConfigSettings.DownloadFileDirectory;
        }
        


        //EXPORT FILE
        public async Task<FileInfo> ExportFile(string fileId, string userId)
        {
            var exportFile = _exportFileRepository.FindSingle(f => f.Id == Guid.Parse(fileId));
            exportFile.FileStatus = 1;
            var dnPath = Path.Combine(_getUserDirectory(userId),exportFile.Id + ".csv");
            exportFile.ExportFileURI = dnPath;

            _exportFileRepository.Update(exportFile);

            var query = _queryService.GetQuery(exportFile.QueryId);
            var exportData = _queryService.GetQueryResult(Guid.Parse(exportFile.QueryId));


            FileInfo fileInfo = null;
            DataTable dt = new DataTable();

            if (exportFile.ContentType == "ASSAY")
            {
                var fileDefinition = GetSubjectFileDefinition(query);
                fileInfo = await ExportAssayTable(exportData, fileDefinition, exportFile.ExportFileURI);
                return fileInfo;
            }
            if (exportFile.ContentType == "BIOSAMPLES")
            {
                var fileDefinition = GetSampleFileDefinition(query);
                dt = await ExportSampleTable(exportData, fileDefinition);
            }
            if (exportFile.ContentType == "PHENO")
            {
                var fileDefinition = GetSubjectFileDefinition(query);
                dt = await ExportSubjectClinicalTable(exportData, fileDefinition);
            }

            DatatableToFile(dt, exportFile.ExportFileURI);
            SetDatasetStatus(fileId, 2);
            return new FileInfo(exportFile.ExportFileURI);
        }
        
        private ExportFileDefinition GetSubjectFileDefinition(CombinedQuery query)
        {
            var phenoDataset = new ExportFileDefinition()
            {
                ProjectId = query.ProjectId,
                ContentType = "PHENO",
                Name = "Subjects",
                QueryId = query.Id
            };

            bool _hasLongitudinalData = false,_hasTpt = false;


            //ADD SUBJECTID &  STUDYID DATAFIELD
            phenoDataset.Fields.Add(CreateSubjectIdField());
            phenoDataset.Fields.Add(CreateStudyIdField());

            //ADD DESIGN ELEMENT FIELDS (STUDY, VISIT, ARM...etc)
            phenoDataset.Fields.AddRange(query.DesignElements.Select(qObj => new DatasetField()
            {
                QueryObject = qObj,
                QueryObjectType = qObj.QueryFor, 
                ColumnHeader = qObj.QueryObjectName
            }));

            //ADD SUBJECT CHARACTERISTICS (AGE, RACE, SEX ...etc) 
            phenoDataset.Fields.AddRange(query.SubjectCharacteristics.Select(qObj => new DatasetField()
            {
                QueryObject = qObj,
                QueryObjectType = nameof(SubjectCharacteristic),
                ColumnHeader = qObj.QueryObjectName//.ObservationName
            })); 

            //ADD CLINICAL OBSERVATIONS
            foreach(var co in query.ClinicalObservations){
                phenoDataset.Fields.Add(new DatasetField()
                {
                    QueryObject = co,
                    QueryObjectType = nameof(SdtmRow),
                    ColumnHeader = co.ObservationName
                });

                if (co.HasLongitudinalData)
                    _hasLongitudinalData = true;
                if (co.HasTPT)
                    _hasTpt = true;
            }

         
            //ADD GROUPED CLINICAL OBSERVATIONS
            phenoDataset.Fields.AddRange(query.GroupedObservations.Select(gObs => new DatasetField()
            {
                QueryObject = gObs,
                QueryObjectType = nameof(SdtmRow),
                ColumnHeader = gObs.ObservationName
            }));

            //ADD longitudinal data columns
            if (_hasLongitudinalData && !phenoDataset.Fields.Exists(f => f.ColumnHeader == "visit"))
            {
                phenoDataset.Fields.Add(new DatasetField()
                {
                    QueryObject = new Query() { QueryFor = nameof(Visit), QueryFrom = nameof(SdtmRow), QuerySelectProperty = "Name" },
                    QueryObjectType = nameof(Visit),
                    ColumnHeader = "visit"

                });
            }

            if (_hasTpt && !phenoDataset.Fields.Exists(f => f.ColumnHeader == "timepoint"))
            {
                phenoDataset.Fields.Add(new DatasetField()
                {
                    QueryObject = new Query() { QueryFor = nameof(SdtmRow.CollectionStudyTimePoint), QueryFrom = nameof(SdtmRow), QuerySelectProperty = "Name" },
                    QueryObjectType = nameof(RelativeTimePoint),
                    ColumnHeader = "timepoint"

                });
            }

            return phenoDataset;
        }

        private ExportFileDefinition GetSampleFileDefinition(CombinedQuery query)
        {
            AssayPanelQuery assayPanelQuery = query.AssayPanels.Single();
            // This is for the subject to sample mapping
            var assaySampleDataset = new ExportFileDefinition
            {
                ProjectId = query.ProjectId,
                ContentType = "BIOSAMPLES",
                Name = assayPanelQuery.AssayName + " Samples",
                QueryId = query.Id
            };
            //CREATE DATAFIELDS

            // 1. ADD SubjectId Field
            assaySampleDataset.Fields.Add(CreateSubjectIdField());
            // 2.ADD sample Id Field
            assaySampleDataset.Fields.Add(CreateSampleIdField());
            // 3. ADD Study Id
            assaySampleDataset.Fields.Add(CreateStudyIdField());

            // 4. ADD  Sample characteristics
            assaySampleDataset.Fields.AddRange(assayPanelQuery.SampleQueries.Select(qObj => new DatasetField()
            {
                QueryObjectType = nameof(SampleCharacteristic),
                QueryObject = qObj,
                ColumnHeader = qObj.QueryObjectName
            }));
            
            return assaySampleDataset;
        }

        private ExportFileDefinition GetAssayDataFileDefinition(CombinedQuery query)
        {         
            AssayPanelQuery assayPanelQuery = query.AssayPanels.Single();
            var assayPanelDataset = new ExportFileDefinition
            {
                ProjectId = query.ProjectId,
                ContentType = "ASSAY",
                Name = assayPanelQuery.AssayName + " Data File",
                QueryId = query.Id
            }; 
            return assayPanelDataset;
        }
       

        private async Task<DataTable> ExportSubjectClinicalTable(DataExportObject exportData, ExportFileDefinition fileDefinition) // FEATURE BY FEATURE  MAIN QUICK
        {
            return await Task.Factory.StartNew(() =>
            {

                #region Create Table Columns
                var datatable = new DataTable();
                datatable.TableName = fileDefinition.Name;

                foreach (var field in fileDefinition.Fields)
                {
                    datatable.Columns.Add(field.ColumnHeader.ToLower());
                }

                #endregion

                //var subjGroupedObservations = exportData.Observations.GroupBy(ob => new { subjId = ob.USubjId });

                var fieldsByO3Id = fileDefinition.Fields.FindAll(f => f.QueryObjectType == nameof(SdtmRow)).GroupBy(f => f.QueryObject.QueryObjectName).ToList();
                var subjPropertiesFields = fileDefinition.Fields.FindAll(f => f.QueryObjectType != nameof(SdtmRow)).ToList();

                foreach (var subject in exportData.Subjects)
                {

                    var uniqSubjectId = subject.UniqueSubjectId;
                    var subjectObservations = exportData.Observations.FindAll(o => o.USubjId == uniqSubjectId).ToList();
                    var subjectCharacteristics = exportData.SubjChars.FindAll(sc => sc.SubjectId == subject.Id).ToList();

                    var firstRow = true;
                    while (subjectObservations.Any() || firstRow)
                    {
                        var row = datatable.NewRow();
                        firstRow = false;

                        #region Design Elements
                        // row["studyid"] = subject.Study.Name;
                        #endregion

                        #region Subject Properties

                        foreach (var subjPropField in subjPropertiesFields)
                        {
                            var charVal = _queryService.GetSubjectOrSampleProperty(subject, subjPropField.QueryObject);
                            if (charVal != null)
                                row[subjPropField.ColumnHeader.ToLower()] = charVal;
                        }

                        #endregion

                        #region WRITE CLINICAL OBSERVATIONS

                        foreach (var fieldgrp in fieldsByO3Id)//HEADACHE //BMI (EVENTS AND FINDINGS TOGETHER)//NOTE .. TIMEING ARE NOT synchronized YET
                        {
                            SdtmRow obs = null;
                            foreach (var field in fieldgrp) //AEOCCUR / AESEV
                            {
                                //ONTOLOGY TERM REQUEST
                                var query = (ObservationQuery)field.QueryObject;
                                if (query.IsOntologyEntry)
                                    obs = subjectObservations.FirstOrDefault(
                                        o => o.QualifierQualifiers.ContainsKey(query.TermCategory)
                                                && ((ObservationQuery)field.QueryObject).TermId.ToString() == o.QualifierQualifiers[query.TermCategory]);

                                //GROUP OF OBSERVATIONS
                                else if (field.QueryObject.GetType() == typeof(GroupedObservationsQuery))
                                {
                                    //ASSUMPTION: GROUPS AREONLY COMPOSED OF ONTOLOGY ENTRY
                                    //ASSUMPTION: 
                                    string v;
                                    foreach (var obsQuery in ((GroupedObservationsQuery)field.QueryObject).GroupedObservations)
                                    {
                                        obs = subjectObservations.FirstOrDefault(
                                            o => o.QualifierQualifiers.TryGetValue(obsQuery.TermCategory, out v)
                                                && obsQuery.TermId.ToString() == o.QualifierQualifiers[obsQuery.TermCategory]);
                                        if (obs != null) break;
                                    }
                                }

                                //SINGLE OBSERVATION OBJECT TERM REQUEST
                                else
                                {
                                    obs = subjectObservations.FirstOrDefault(
                                    o => ((ObservationQuery)field.QueryObject).TermId == o.DBTopicId);
                                }

                                string val = "";


                                //WRITE OBSERVATION INSTANCE TO ROW
                                obs?.Qualifiers.TryGetValue(((ObservationQuery)field.QueryObject).PropertyName, out val);
                                if (val == null)
                                    obs?.ResultQualifiers.TryGetValue(((ObservationQuery)field.QueryObject).PropertyName, out val);

                                //Write 
                                //if (val == null)
                                //if (((ObservationQuery)field.QueryObject).PropertyName.ToLower().Equals("visit"))
                                //val = obs.VisitName;
                                //if (val==null && ((ObservationQuery)field.QueryObject).PropertyName.EndsWith("DY") && obs.CollectionStudyDay.Number!=null)
                                //{
                                //    val = obs.CollectionStudyDay.Number.HasValue ? obs.CollectionStudyDay.Number.ToString() : "";
                                //    //row[field.ColumnHeader.ToLower()] = val;
                                //}

                                row[field.ColumnHeader.ToLower()] = val ?? "";


                            }
                            var visitField = fileDefinition.Fields.Find(f => f.QueryObjectType == nameof(Visit));
                            if (visitField != null && obs!=null)
                            {
                                row[visitField.ColumnHeader.ToLower()] = obs?.VisitName ?? "";
                            }

                            var timepointField = fileDefinition.Fields.FirstOrDefault(f => f.QueryObject.QueryFor != null &&
                                                                                      f.QueryObject.QueryFor.Equals(nameof(SdtmRow.CollectionStudyTimePoint)));
                            if (timepointField != null && obs != null)
                                row[timepointField.ColumnHeader.ToLower()] = obs?.CollectionStudyTimePoint?.Name ?? "";

                            subjectObservations.Remove(obs);
                        }

                        #endregion

                        #region Write Timing Variables

                        #endregion


                        datatable.Rows.Add(row);
                    }
                }

                return datatable;
            });
        }

        private async Task<DataTable> ExportSampleTable(DataExportObject exportData, ExportFileDefinition fileDefinition) // FEATURE BY FEATURE  MAIN QUICK
        {
            return await Task.Factory.StartNew(() =>
            {
              
                var datatable = new DataTable();
                datatable.TableName = fileDefinition.Name;
                foreach (var field in fileDefinition.Fields)
                {
                    datatable.Columns.Add(field.ColumnHeader.ToLower());
                }

                foreach (var biosample in exportData.Samples)
                {
                    var row = datatable.NewRow();

                    row["subjectid"] = biosample.Subject.UniqueSubjectId;
                    row["sampleid"] = biosample.BiosampleStudyId;

                    foreach (var samplePropField in fileDefinition.Fields)
                    {
                        var charVal = _queryService.GetSubjectOrSampleProperty(biosample, samplePropField.QueryObject);
                        if (charVal != null)
                            row[samplePropField.ColumnHeader.ToLower()] = charVal;
                    }
                    datatable.Rows.Add(row);
                }
                return datatable;
            });
        }
        
        private async Task<FileInfo> ExportAssayTable(DataExportObject exportData, ExportFileDefinition fileDefinition, string filePath) // FEATURE BY FEATURE  MAIN QUICK
        {
            return await Task.Factory.StartNew(() =>
            {
                var projectId = fileDefinition.ProjectId;
                var activityId = exportData.Samples.First().AssayId;

                // GET sampleIds, observations, and features
                var sampleIds = exportData.Samples.OrderBy(o => o.BiosampleStudyId).Select(s => s.BiosampleStudyId).ToList();
                var assayObservations = _queryService.GetAssayObservations(projectId, activityId, sampleIds);

                var orderObservations = assayObservations.OrderBy(o => o.FeatureName).ThenBy(o => o.SubjectOfObservationName);
                //////var features = orderObservations.Select(a => a.FeatureName).Distinct().ToList();
                //////var values = orderObservations.Select(c => ((NumericalValue)c.ObservedValue).Value);

               
                var features = orderObservations.Select(a => a.FeatureName).Distinct().ToList();
                var values = orderObservations.Select(a => a.Value).ToList(); 
                var samples = orderObservations.Select(a => a.SubjectOfObservationName).Distinct().ToList();
                
                StreamWriter writer = File.CreateText(filePath);

                // First write the header (which are samples' Ids)
                writer.WriteLine("features" + "," + string.Join(",", samples));

                var sb = new StringBuilder();
                var i = 0;
                var j = 0;

                foreach (var value in values)
                {
                    i++;
                    sb.Append(value + ",");
                    if (i == exportData.Samples.Count)
                    {
                        writer.WriteLine(features[j] + "," + sb);
                        sb.Clear();
                        i = 0;
                        writer.Flush();
                        j++;
                    }
                }
               
                writer.Dispose();


                return new FileInfo(filePath);
            });
        }

        public void SetDatasetStatus(string datasetId, int status)
        {
            var exportFile = _exportFileRepository.FindSingle(d => d.Id == Guid.Parse(datasetId));
            exportFile.FileStatus = status;
            _exportFileRepository.Update(exportFile);
        }

        public int IsFileReady(string fileId)
        {
            var exportFile = _exportFileRepository.FindSingle(d => d.Id == Guid.Parse(fileId));
            int status = exportFile.FileStatus;
            return status;
        }
        
        private string DatatableToFile(DataTable dtTable, string filePath)
        {
            // datatable to string
            StringBuilder result = new StringBuilder();
            if (dtTable.Columns.Count != 0)
            {
                foreach (DataColumn col in dtTable.Columns)
                {
                    result.Append(col.ColumnName + ',');
                }
                result.Append("\r\n");
                foreach (DataRow row in dtTable.Rows)
                {
                    foreach (DataColumn column in dtTable.Columns)
                    {
                        result.Append(row[column]?.ToString() + ',');
                    }
                    result.Append("\r\n");
                }
            }
         
            StreamWriter writer = File.CreateText(filePath);    
            writer.WriteLine(result);
            writer.Flush();
            writer.Dispose();

            return filePath;
        }
        
        //GET EXPORT FILE
        public FileStream DownloadDataset(string fileId, out string filename)
        {
            var exportFile = _exportFileRepository.FindSingle(d => d.Id == Guid.Parse(fileId));
            filename = exportFile.Name;

            if (exportFile.FileStatus != 2) return null;
            var filePath = exportFile.ExportFileURI;
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            return fileStream;
        }

        private DatasetField CreateSubjectIdField()
        {
            return new DatasetField()
            {
                FieldName = "Subject[UniqueId]",
                QueryObject =
                    new Query()
                    {
                        QueryFrom = nameof(HumanSubject),
                        QueryFor = nameof(HumanSubject.UniqueSubjectId),
                        QuerySelectProperty = nameof(HumanSubject.UniqueSubjectId)
                    },
                ColumnHeader = "subjectid",
                ColumnHeaderIsEditable = false
            };
        }

        private DatasetField CreateSampleIdField()
        {
            return new DatasetField()
            {
                FieldName = "Sample[SampleId]",
                QueryObject =
                    new Query()
                    {
                        QueryFrom = nameof(Biosample),
                        QueryFor = nameof(Biosample.BiosampleStudyId),
                        QuerySelectProperty = nameof(Biosample.BiosampleStudyId)
                    },
                //QueryObjectType = nameof(Biosample),
                ColumnHeader = "sampleid",
                ColumnHeaderIsEditable = false
            };
        }

        private DatasetField CreateStudyIdField()
        {
            return new DatasetField()
            {
                FieldName = "Study[Name]",
                QueryObject =
                    new Query()
                    {
                        QueryFrom = nameof(HumanSubject),
                        QueryFor = nameof(HumanSubject.Study),
                        QuerySelectProperty = nameof(Study.Name)
                    },
                //QueryObjectType = nameof(Biosample),
                ColumnHeader = "StudyName",
                ColumnHeaderIsEditable = false
            };
        }

        private string _getUserDirectory(string userId)
        {
            string dirPath = Path.Combine(_downloadFileDirectory, "USER-" + userId);
            Directory.CreateDirectory(dirPath);
            return dirPath;
        }
    }
}