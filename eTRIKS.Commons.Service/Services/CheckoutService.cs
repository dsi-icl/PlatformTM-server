using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel.SDTM;
using eTRIKS.Commons.Core.Domain.Model.Users.Datasets;
using eTRIKS.Commons.Core.Domain.Model.Users.Queries;
using eTRIKS.Commons.Service.DTOs;

namespace eTRIKS.Commons.Service.Services
{
    public class CheckoutService
    {
        private readonly IServiceUoW _dataContext;
        private readonly IRepository<CombinedQuery, Guid> _combinedQueryRepository;
        private readonly IRepository<UserDataset, Guid> _userDatasetRepository;

        private readonly QueryService _queryService;


        public CheckoutService(IServiceUoW uoW, QueryService queryService)
        {
            _dataContext = uoW;
            _combinedQueryRepository = uoW.GetRepository<CombinedQuery, Guid>();
            _userDatasetRepository = uoW.GetRepository<UserDataset, Guid>();
            _queryService = queryService;
        }

        /// <summary>
        /// Generates Datasets to save and download from the user saved query.
        /// All subject and clinical observations requested in the query are exported to one dataset (pheno)
        /// If the query contains queries for assay data, two datasets are created for each requested assay panel
        /// A sample metadata dataset including subject to sample mapping as well as any other sample characteristics
        /// requested in the query, and an "assay data" dataset containing the observed measurements.
        /// </summary>
        /// <param name="queryIdStr"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserDataset> CreateCheckoutDatasets(string queryIdStr, string userId)
        {
            Guid queryId;
            if (!Guid.TryParse(queryIdStr, out queryId))
                return null;
            var query = _combinedQueryRepository.Get(queryId);
            var projectId = query.ProjectId;


            //Create Pheno dataset for all selected clinical and subject observations
            var phenoDataset = CreateSubjectClinicalDataset(query, userId);

            var checkoutDatasets = new List<UserDataset> {phenoDataset};

            //Creates a subject-to-sample mapping dataset for each  assay
            foreach (var assayPanel in query.AssayPanels)
            {
                //THIS IS MAKING THE ASSUMPTION THAT FILTERS ON ONE ASSAY CHARACTERISTICS
                //WILL NOT
                //BE PROPAGATED TO THE OTHER ASSAY PANELS IN THE SAME QUERY
                var singleAssayCombinedQuery = query.AssayPanels.Count > 1 
                    ? _queryService.CreateSingleAssayCombinedQuery(query, assayPanel) 
                    : query;
               
                var assaySampleDataset = CreateAssaySampleDataset(assayPanel, singleAssayCombinedQuery.Id, userId, projectId);
                checkoutDatasets.Add(assaySampleDataset);
            }
            
            //Creates assay data dataset for each assay 
            foreach (var AssayPanel in query.AssayPanels)
            {
                var singleAssayCombinedQuery = query.AssayPanels.Count > 1
                    ? _queryService.CreateSingleAssayCombinedQuery(query, AssayPanel)
                    : query;


                var assayPanelDataset = CreateAssayPanelDataset(AssayPanel, singleAssayCombinedQuery.Id, userId, projectId);
                checkoutDatasets.Add(assayPanelDataset);
            }

            return checkoutDatasets;
        }

        private UserDataset CreateSubjectClinicalDataset(CombinedQuery query, string userId)
        {
            var phenoDataset = new UserDataset
            {
                Id = Guid.NewGuid(),
                OwnerId = userId,
                ProjectId = query.ProjectId,
                Type = "PHENO",
                Name = "Subject Metadata",
                QueryId = query.Id
            };

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
            phenoDataset.Fields.AddRange(query.ClinicalObservations.Select(qObj => new DatasetField()
            {
                QueryObject = qObj,
                QueryObjectType = nameof(SdtmRow),
                ColumnHeader = qObj.ObservationName
            }));

            //ADD GROUPED CLINICAL OBSERVATIONS
            phenoDataset.Fields.AddRange(query.GroupedObservations.Select(gObs => new DatasetField()
            {
                QueryObject = gObs,
                QueryObjectType = nameof(SdtmRow),
                ColumnHeader = gObs.ObservationName
            }));

            var exportData = _queryService.GetQueryResult(query.Id);
            phenoDataset.SubjectCount = exportData.Subjects.Count;
            

            _userDatasetRepository.Insert(phenoDataset);
            _dataContext.Save();
            return phenoDataset;
        }

        private UserDataset CreateAssaySampleDataset(AssayPanelQuery assayPanelQuery, Guid combinedQueryId, string userId, int projectId)
        {
            // This is for the subject to sample mapping
            var assaySampleDataset = new UserDataset
            {
                Id = Guid.NewGuid(),
                OwnerId = userId,
                ProjectId = projectId,
                Type = "BIOSAMPLES",
                Name = assayPanelQuery.AssayName + " Sample Metadata",
                QueryId = combinedQueryId
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

            var exportData = _queryService.GetQueryResult(combinedQueryId);
            assaySampleDataset.SubjectCount = exportData.Subjects.Count;
            assaySampleDataset.SampleCount = exportData.Samples.Count;

            _userDatasetRepository.Insert(assaySampleDataset);
            _dataContext.Save();
            return assaySampleDataset;

        }

        private UserDataset CreateAssayPanelDataset(AssayPanelQuery assayPanelQuery, Guid combinedQueryId, string userId, int projectId)
        {
            // TODO for HD datasets
            var assayPanelDataset = new UserDataset
            {
                Id = Guid.NewGuid(),
                OwnerId = userId,
                ProjectId = projectId,
                Type = "ASSAY",
                Name = assayPanelQuery.AssayName + " Assay",
                QueryId = combinedQueryId
            };


            var exportData = _queryService.GetQueryResult(combinedQueryId);
            assayPanelDataset.SubjectCount = exportData.Subjects.Count;
            assayPanelDataset.SampleCount = exportData.Samples.Count;

            _userDatasetRepository.Insert(assayPanelDataset);
            _dataContext.Save();
            return assayPanelDataset;
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
    }
}
