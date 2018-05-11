using System;
using System.Collections.Generic;
using System.Linq;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.DatasetModel.SDTM;
using PlatformTM.Core.Domain.Model.DesignElements;
using PlatformTM.Core.Domain.Model.Timing;
using PlatformTM.Core.Domain.Model.Users.Datasets;
using PlatformTM.Core.Domain.Model.Users.Queries;
using PlatformTM.Services.DTOs.Export;

namespace PlatformTM.Services.Services
{
    public class CheckoutService
    {
        private readonly IServiceUoW _dataContext;
        private readonly IRepository<CombinedQuery, Guid> _combinedQueryRepository;
		private readonly IRepository<ExportFile, Guid> _exportFileRepository;
		private readonly IRepository<AnalysisDataset, Guid> _analysisDatasetRepository;
		private readonly QueryService _queryService;

        public CheckoutService(IServiceUoW uoW, QueryService queryService)
        {
            _dataContext = uoW;
            _combinedQueryRepository = uoW.GetRepository<CombinedQuery, Guid>();
			_exportFileRepository = uoW.GetRepository<ExportFile, Guid>();
			_analysisDatasetRepository = uoW.GetRepository<AnalysisDataset, Guid>();
            _queryService = queryService;
        }

        /*
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
        public List<ExportFileDefinition> CreateCheckoutDatasets(string queryIdStr, string userId)
        {
            Guid queryId;
            if (!Guid.TryParse(queryIdStr, out queryId))
                return null;
            var query = _combinedQueryRepository.Get(queryId);
            var projectId = query.ProjectId;
			//At this stage result shuold be ExportFiles
            //When and if the user chooses to save these files then a new analysis dataset is created and a descriptor for each column is created the association between each file of the analysis dataset and the 

            //Create Pheno dataset for all selected clinical and subject observations
            var phenoDataset = CreateOrFindSubjectClinicalDataset(query, userId);

			//var analysisDataset = new AnalysisDataset();

            var checkoutDatasets = new List<ExportFileDefinition> {phenoDataset};

            //Creates a subject-to-sample mapping dataset for each  assay
            foreach (var assayPanel in query.AssayPanels)
            {
                //THIS IS MAKING THE ASSUMPTION THAT FILTERS ON ONE ASSAY CHARACTERISTICS
                //WILL NOT
                //BE PROPAGATED TO THE OTHER ASSAY PANELS IN THE SAME QUERY
                var singleAssayCombinedQuery = query.AssayPanels.Count > 1 
                    ? _queryService.CreateSingleAssayCombinedQuery(query, assayPanel) 
                    : query;
               
                var assaySampleDataset = CreateOrFindAssaySampleDataset(assayPanel, singleAssayCombinedQuery.Id, userId, projectId);
                checkoutDatasets.Add(assaySampleDataset);
            }
            
            //Creates assay data dataset for each assay 
            foreach (var AssayPanel in query.AssayPanels)
            {
                var singleAssayCombinedQuery = query.AssayPanels.Count > 1
                    ? _queryService.CreateSingleAssayCombinedQuery(query, AssayPanel)
                    : query;


                var assayPanelDataset = CreateOrFindAssayPanelDataset(AssayPanel, singleAssayCombinedQuery.Id, userId, projectId);
                checkoutDatasets.Add(assayPanelDataset);
            }

            return checkoutDatasets;
        }
        */

		public List<ExportFile> GetCheckoutResults(string queryIdStr, string userId)
        {
            Guid queryId;
            if (!Guid.TryParse(queryIdStr, out queryId))
                return null;
            var query = _combinedQueryRepository.Get(queryId);
            var projectId = query.ProjectId;
            var exportData = _queryService.GetQueryResult(query.Id);
			var exportFiles = new List<ExportFile>();

            //WHAT IF NO SUBJECT CHARACTERSITICS ADDED?
            if (query.SubjectCharacteristics.Any() || query.DesignElements.Any())
            {
                var phenoFile = new ExportFile
                {
					Id = Guid.NewGuid(),
                    OwnerId = userId,
                    ProjectId = query.ProjectId,
                    ContentType = "PHENO",
                    Name = "Subjects",
					QueryId = query.Id.ToString()
                };
				phenoFile.SubjectCount = exportData.Subjects.Count;
				_exportFileRepository.Insert(phenoFile);
				exportFiles.Add(phenoFile);
            }


            foreach (var assayPanel in query.AssayPanels)
            {
                //THIS IS MAKING THE ASSUMPTION THAT FILTERS ON ONE ASSAY CHARACTERISTICS
                //WILL NOT
                //BE PROPAGATED TO THE OTHER ASSAY PANELS IN THE SAME QUERY
                var singleAssayCombinedQuery = query.AssayPanels.Count > 1
                    ? _queryService.CreateSingleAssayCombinedQuery(query, assayPanel)
                    : query;
                exportData = _queryService.GetQueryResult(singleAssayCombinedQuery.Id);


                var sampleCR = new ExportFile
                {
					Id = Guid.NewGuid(),
                    OwnerId = userId,
                    ProjectId = query.ProjectId,
                    ContentType = "BIOSAMPLES",
                    Name = assayPanel.AssayName + " Samples",
					QueryId = singleAssayCombinedQuery.Id.ToString()
                };


                sampleCR.SubjectCount = exportData.Subjects.Count;
                sampleCR.SampleCount = exportData.Samples.Count;
				_exportFileRepository.Insert(sampleCR);
				exportFiles.Add(sampleCR);

				var assayExportFile = new ExportFile
				{
					Id = Guid.NewGuid(),
					OwnerId = userId,
					ProjectId = query.ProjectId,
					ContentType = "ASSAY",
					Name = assayPanel.AssayName + " Data File",
					QueryId = singleAssayCombinedQuery.Id.ToString()
				};
				assayExportFile.SubjectCount = exportData.Subjects.Count;
				assayExportFile.SampleCount = exportData.Samples.Count;
				_exportFileRepository.Insert(assayExportFile);
         
				exportFiles.Add(assayExportFile);
            }

			return exportFiles;

        }

		public AnalysisDatasetDTO SaveToDataset(AnalysisDatasetDTO datasetDTO, string userId)
        {
        	AnalysisDataset analysisDataset = new AnalysisDataset()
			{
				Name = datasetDTO.Name,
				Description = datasetDTO.Description,
				Id = Guid.NewGuid(),
				ProjectId = datasetDTO.ProjectId,
				OwnerId = datasetDTO.OwnerId = userId,
				FileIds = datasetDTO.Files.Select(f=>f.Id).ToList(),
				Tags = datasetDTO.Tags
			};
			_analysisDatasetRepository.Insert(analysisDataset);
            return null;
        } 
        
		//private ExportFileDefinition CreateOrFindSubjectClinicalDataset(CombinedQuery query, string userId)
  //      {
  //          var phenoDataset = _userDatasetRepository.FindSingle(
  //              d => d.ProjectId == query.ProjectId
  //                   && d.QueryId == query.Id
  //                   && d.ContentType == "PHENO"
  //                   && d.OwnerId == userId);
  //          if (phenoDataset != null)
  //              return phenoDataset;
            
  //          phenoDataset = new ExportFileDefinition
  //          {
  //              Id = Guid.NewGuid(),
  //              OwnerId = userId,
  //              ProjectId = query.ProjectId,
  //              ContentType = "PHENO",
  //              Name = "Subjects",
  //              QueryId = query.Id,
  //              FileStatus = 0,
  //          };

  //          //ADD SUBJECTID &  STUDYID DATAFIELD
  //          phenoDataset.Fields.Add(CreateSubjectIdField());
  //          phenoDataset.Fields.Add(CreateStudyIdField());

  //          //ADD DESIGN ELEMENT FIELDS (STUDY, VISIT, ARM...etc)
  //          phenoDataset.Fields.AddRange(query.DesignElements.Select(qObj => new DatasetField()
  //          {
  //              QueryObject = qObj,
  //              QueryObjectType = qObj.QueryFor, 
  //              ColumnHeader = qObj.QueryObjectName
  //          }));

  //          //ADD SUBJECT CHARACTERISTICS (AGE, RACE, SEX ...etc) 
  //          phenoDataset.Fields.AddRange(query.SubjectCharacteristics.Select(qObj => new DatasetField()
  //          {
  //              QueryObject = qObj,
  //              QueryObjectType = nameof(SubjectCharacteristic),
  //              ColumnHeader = qObj.QueryObjectName//.ObservationName
  //          })); 

  //          //ADD CLINICAL OBSERVATIONS
  //          foreach(var co in query.ClinicalObservations){
  //              phenoDataset.Fields.Add(new DatasetField()
  //              {
  //                  QueryObject = co,
  //                  QueryObjectType = nameof(SdtmRow),
  //                  ColumnHeader = co.ObservationName
  //              });

  //              if(co.HasLongitudinalData){
  //                  phenoDataset.Fields.Add(new DatasetField(){
  //                      QueryObject = new Query(){QueryFor = nameof(Visit),QueryFrom = nameof(SdtmRow), QuerySelectProperty = "Name"},
  //                      QueryObjectType = nameof(Visit),
  //                      ColumnHeader = "visit"
                        
  //                  });
  //              }

  //              if(co.HasTPT){
  //                  phenoDataset.Fields.Add(new DatasetField()
  //                 {
  //                      QueryObject = new Query() { QueryFor = nameof(SdtmRow.CollectionStudyTimePoint), QueryFrom = nameof(SdtmRow), QuerySelectProperty = "Name" },
  //                      QueryObjectType = nameof(RelativeTimePoint),
  //                      ColumnHeader = "timepoint"

  //                 }); 
  //              }
  //          }


  //          //phenoDataset.Fields.AddRange(query.ClinicalObservations.Select(qObj => new DatasetField()
  //          //{
  //          //    QueryObject = qObj,
  //          //    QueryObjectType = nameof(SdtmRow),
  //          //    ColumnHeader = qObj.ObservationName
  //          //}));



  //          //ADD GROUPED CLINICAL OBSERVATIONS
  //          phenoDataset.Fields.AddRange(query.GroupedObservations.Select(gObs => new DatasetField()
  //          {
  //              QueryObject = gObs,
  //              QueryObjectType = nameof(SdtmRow),
  //              ColumnHeader = gObs.ObservationName
  //          }));

  //          var exportData = _queryService.GetQueryResult(query.Id);
  //          phenoDataset.SubjectCount = exportData.Subjects.Count;
            

  //          _userDatasetRepository.Insert(phenoDataset);
  //          //_dataContext.Save();
  //          return phenoDataset;
  //      }

  //      private ExportFileDefinition CreateOrFindAssaySampleDataset(AssayPanelQuery assayPanelQuery, Guid combinedQueryId, string userId, int projectId)
  //      {
  //          var assaySampleDataset = _userDatasetRepository.FindSingle(
  //              d => d.ProjectId == projectId
  //                   && d.QueryId == combinedQueryId
  //                   && d.ContentType == "BIOSAMPLES"
  //                   && d.OwnerId == userId);
  //          //TODO:should add AssayId???

  //          if (assaySampleDataset != null)
  //              return assaySampleDataset;

  //          // This is for the subject to sample mapping
  //          assaySampleDataset = new ExportFileDefinition
  //          {
  //              Id = Guid.NewGuid(),
  //              OwnerId = userId,
  //              ProjectId = projectId,
  //              ContentType = "BIOSAMPLES",
  //              Name = assayPanelQuery.AssayName + " Samples",
  //              QueryId = combinedQueryId,
                
  //              FileStatus = 0
  //          };
  //          //CREATE DATAFIELDS

  //          // 1. ADD SubjectId Field
  //          assaySampleDataset.Fields.Add(CreateSubjectIdField());
  //          // 2.ADD sample Id Field
  //          assaySampleDataset.Fields.Add(CreateSampleIdField());
  //          // 3. ADD Study Id
  //          assaySampleDataset.Fields.Add(CreateStudyIdField());

  //          // 4. ADD  Sample characteristics
  //          assaySampleDataset.Fields.AddRange(assayPanelQuery.SampleQueries.Select(qObj => new DatasetField()
  //          {
  //              QueryObjectType = nameof(SampleCharacteristic),
  //              QueryObject = qObj,
  //              ColumnHeader = qObj.QueryObjectName
  //          }));

  //          var exportData = _queryService.GetQueryResult(combinedQueryId);
  //          assaySampleDataset.SubjectCount = exportData.Subjects.Count;
  //          assaySampleDataset.SampleCount = exportData.Samples.Count;

  //          _userDatasetRepository.Insert(assaySampleDataset);
  //          //_dataContext.Save();
  //          return assaySampleDataset;

  //      }

  //      private ExportFileDefinition CreateOrFindAssayPanelDataset(AssayPanelQuery assayPanelQuery, Guid combinedQueryId, string userId, int projectId)
  //      {
  //          var assayPanelDataset = _userDatasetRepository.FindSingle(
  //              d => d.ProjectId == projectId
  //                   && d.QueryId == combinedQueryId
  //                   && d.ContentType == "ASSAY"
  //                   && d.OwnerId == userId);
  //          //TODO:should add AssayId???

  //          if (assayPanelDataset != null)
  //              return assayPanelDataset;

  //          assayPanelDataset = new ExportFileDefinition
  //          {
  //              Id = Guid.NewGuid(),
  //              OwnerId = userId,
  //              ProjectId = projectId,
  //              ContentType = "ASSAY",
  //              Name = assayPanelQuery.AssayName + " Data Matrix",
  //              QueryId = combinedQueryId,
  //              FileStatus = 0
  //          };

  //          var exportData = _queryService.GetQueryResult(combinedQueryId);
  //          assayPanelDataset.SubjectCount = exportData.Subjects.Count;
  //          assayPanelDataset.SampleCount = exportData.Samples.Count;

  //          _userDatasetRepository.Insert(assayPanelDataset);
  //          //_dataContext.Save();
  //          return assayPanelDataset;
  //      }
       
		//private DatasetField CreateSubjectIdField()
        //{
        //    return new DatasetField()
        //    {
        //        FieldName = "Subject[UniqueId]",
        //        QueryObject =
        //            new Query()
        //            {
        //                QueryFrom = nameof(HumanSubject),
        //                QueryFor = nameof(HumanSubject.UniqueSubjectId),
        //                QuerySelectProperty = nameof(HumanSubject.UniqueSubjectId)
        //            },
        //        ColumnHeader = "subjectid",
        //        ColumnHeaderIsEditable = false
        //    };
        //}

        //private DatasetField CreateSampleIdField()
        //{
        //    return new DatasetField()
        //    {
        //        FieldName = "Sample[SampleId]",
        //        QueryObject =
        //            new Query()
        //            {
        //                QueryFrom = nameof(Biosample),
        //                QueryFor = nameof(Biosample.BiosampleStudyId),
        //                QuerySelectProperty = nameof(Biosample.BiosampleStudyId)
        //            },
        //        //QueryObjectType = nameof(Biosample),
        //        ColumnHeader = "sampleid",
        //        ColumnHeaderIsEditable = false
        //    };
        //}

        //private DatasetField CreateStudyIdField()
        //{
        //    return new DatasetField()
        //    {
        //        FieldName = "Study[Name]",
        //        QueryObject =
        //            new Query()
        //            {
        //                QueryFrom = nameof(HumanSubject),
        //                QueryFor = nameof(HumanSubject.Study),
        //                QuerySelectProperty = nameof(Study.Name)
        //            },
        //        //QueryObjectType = nameof(Biosample),
        //        ColumnHeader = "StudyName",
        //        ColumnHeaderIsEditable = false
        //    };
        //}

        
    }
}
