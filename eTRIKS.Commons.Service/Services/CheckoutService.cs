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


        public CheckoutService(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _combinedQueryRepository = uoW.GetRepository<CombinedQuery, Guid>();
            _userDatasetRepository = uoW.GetRepository<UserDataset, Guid>();
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

            var checkoutDatasets = new List<UserDataset>();

            //Create Pheno dataset for all selected clinical and subject observations
            var phenoDataset = CreateSubjectClinicalDataset(query, userId);

            checkoutDatasets.Add(phenoDataset);

            //Creates a subject-to-sample mapping dataset for each  assay
           var projectId = query.ProjectId;

            foreach (var AssayPanel in query.AssayPanels)
            {
                var assaySampleDataset = CreateAssaySampleDataset(AssayPanel,query.Id, userId, projectId);
                checkoutDatasets.Add(assaySampleDataset);
            }

            
            //TODO Creates assay data dataset for each assay using following

            //foreach (var AssayPanel in query.AssayPanels)
            // {
            //     var assayPanelDataset = CreateAssayPanelDataset(AssayPanel, userId, projectId);
            //     checkoutDatasets.Add(assayPanelDataset);
            // }

            return checkoutDatasets;
        }

       

        

        private UserDataset CreateSubjectClinicalDataset(CombinedQuery query, string userId)
        {
            var phenoDataset = new UserDataset();
            phenoDataset.Id = Guid.NewGuid();
            phenoDataset.OwnerId = userId;
            phenoDataset.ProjectId = query.ProjectId;
            phenoDataset.Type = "PHENO";
            phenoDataset.Name = "Phenotypes";
            phenoDataset.QueryId = query.Id;
            //CREATE DATAFIELDS

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

            _userDatasetRepository.Insert(phenoDataset);
            _dataContext.Save();
            return phenoDataset;
        }

        private UserDataset CreateAssaySampleDataset(AssayPanelQuery assayPanelQuery, Guid combinedQueryId, string userId, int projectId)
        {
            // This is for the subject to sample mapping
            var assaySampleDataset = new UserDataset();
            assaySampleDataset.Id = Guid.NewGuid();
            assaySampleDataset.OwnerId = userId;
            assaySampleDataset.ProjectId = projectId;
            assaySampleDataset.Type = "AssaySamples";
            assaySampleDataset.Name = "SubjectsToSamplesMapping";
            assaySampleDataset.QueryId = combinedQueryId;
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


            _userDatasetRepository.Insert(assaySampleDataset);
            _dataContext.Save();
            return assaySampleDataset;

        }
        // Following is not completed 

        private UserDataset CreateAssayPanelDataset(AssayPanelQuery assayPanelQuery, string userId, int projectId)
        {
            // TODO for HD datasets
            var assayPanelDataset = new UserDataset();
            assayPanelDataset.Id = Guid.NewGuid();
            assayPanelDataset.OwnerId = userId;
            assayPanelDataset.ProjectId = projectId;
            assayPanelDataset.Type = "AssayPanel";
            assayPanelDataset.Name = "HDSamples";

            return null;
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
