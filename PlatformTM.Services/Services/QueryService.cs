using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.DatasetModel.SDTM;
using PlatformTM.Core.Domain.Model.DesignElements;
using PlatformTM.Core.Domain.Model.ObservationModel;
using PlatformTM.Core.Domain.Model.Users.Queries;
using PlatformTM.Models.DTOs;
using PlatformTM.Models.DTOs.Explorer;
using Observation = PlatformTM.Core.Domain.Model.ObservationModel.Observation;

namespace PlatformTM.Models.Services
{
    public class QueryService
    {
        private readonly IRepository<CombinedQuery, Guid> _combinedQueryRepository;
        private readonly IRepository<HumanSubject, string> _subjectRepository;
        private readonly IRepository<SubjectCharacteristic, int> _subjectCharacteristicRepository;
        private readonly IRepository<SampleCharacteristic, int> _sampleCharacteristicRepository;
        private readonly IRepository<Study, int> _studyRepository;
        private readonly IRepository<Cohort, string> _armRepository;
        private readonly IRepository<SdtmRow, Guid> _sdtmRepository;
        private readonly IRepository<Visit, int> _visitRepository;
        private readonly IRepository<Biosample, int> _biosampleRepository;
        private readonly IRepository<Assay, int> _assayRepository;
        private readonly IRepository<Observation, Guid> _observationRepository;

        public QueryService(IServiceUoW uoW)
        {
            _combinedQueryRepository = uoW.GetRepository<CombinedQuery, Guid>();
            _subjectRepository = uoW.GetRepository<HumanSubject, string>();
            _subjectCharacteristicRepository = uoW.GetRepository<SubjectCharacteristic, int>();
            _sampleCharacteristicRepository = uoW.GetRepository<SampleCharacteristic, int>();
            _armRepository = uoW.GetRepository<Cohort, string>();
            _studyRepository = uoW.GetRepository<Study, int>();
            _visitRepository = uoW.GetRepository<Visit, int>();
            _sdtmRepository = uoW.GetRepository<SdtmRow, Guid>();
            _biosampleRepository = uoW.GetRepository<Biosample, int>();
            _assayRepository = uoW.GetRepository<Assay, int>();
            _observationRepository = uoW.GetRepository<Observation, Guid>();

        }

        public string GetSubjectOrSampleProperty(object subject, Query query)
        {
            if (subject.GetType() != typeof(HumanSubject) && subject.GetType() != typeof(Biosample))
                return null;

            string subjProperty;
            var queryProperty = subject.GetType().GetProperty(query.QueryFor);
            if (queryProperty != null && queryProperty.PropertyType.IsConstructedGenericType)
            {
                //Property is a collection
                //Simple for now since there's only one collection in human subject i.e. subjectcharacteristics
                var obj =((IList)queryProperty.GetValue(subject)).Cast<object>().SingleOrDefault(sc => sc.GetType().GetProperty(query.QueryWhereProperty).GetValue(sc).ToString() == query.QueryWhereValue);
                subjProperty = obj?.GetType().GetProperty(query.QuerySelectProperty).GetValue(obj).ToString();
            }
            else
            {
                //QueryFrom Subject QueryFor: Arm, QuerySelectProperty: Arm.Name
                //QueryFrom: SDTMrow, QueryFor: 
                var properyObj = subject.GetType().GetProperty(query.QueryFor)?.GetValue(subject);
                subjProperty = query.QueryFor != query.QuerySelectProperty 
                    ? properyObj?.GetType().GetProperty(query.QuerySelectProperty)?.GetValue(properyObj)?.ToString() 
                    : properyObj?.ToString();
            }
            return subjProperty;
        }

        public List<AssayDataDTO> GetAssayObservations(int projectId, int activityId, List<string> sampleIds)
        {
            var assayObservations = 
                _observationRepository.FindObservations(s =>
                    s.ProjectId == projectId && s.ActivityId == activityId && sampleIds.Contains(s.SubjectOfObservationName),
                    x => new AssayDataDTO() { FeatureName = x.FeatureName, SubjectOfObservationName = x.SubjectOfObservationName, Value = ((NumericalValue)x.ObservedValue).Value }
                  );
            return assayObservations.Cast<AssayDataDTO>().ToList();
        }
        
        public CombinedQuery SaveQuery(CombinedQueryDTO cdto, string userId)
        {
            CombinedQuery cQuery = new CombinedQuery();

            cQuery.Name = cdto.Name;
            cQuery.UserId = Guid.Parse(userId);
            cQuery.ProjectId = cdto.ProjectId;
            cQuery.Id = Guid.NewGuid();
            cQuery.IsSavedByUser = cdto.IsSavedByUser;

            var requests = cdto.ObsRequests.Union(cdto.SubjCharRequests);

            foreach (var request in requests)
            {
                if (!request.IsMultipleObservations)
                {
                    var query = GetQueryFromQueryDTO(request);
                    if (request.IsSubjectCharacteristics)
                        cQuery.SubjectCharacteristics.Add(query);
                    if (request.IsClinicalObservations)
                        cQuery.ClinicalObservations.Add((ObservationQuery)query);
                    if (request.IsDesignElement)
                        cQuery.DesignElements.Add(query);
                }
                else
                {
                    var goq = new GroupedObservationsQuery()
                    {
                        //Name = request.Name,
                        GroupedObsName = request.O3,
                        PropertyName = request.QO2,
                        PropertyLabel = request.QO2_label,
                        PropertyId = request.QO2id,
                        Group = request.Group,
                        GroupedObservations = new List<ObservationQuery>(),
                        DataType = request.DataType,
                        FilterExactValues = request.FilterExactValues,
                        FilterRangeFrom = request.FilterRangeFrom,
                        FilterRangeTo = request.FilterRangeTo,
                        IsFiltered = request.IsFiltered,
                        ProjectId = request.ProjectId
                    };
                    goq.GroupedObservations.AddRange(request.GroupedObservations.Select(obsReq => new ObservationQuery()
                    {
                        TermId = obsReq.O3id,
                        TermName = obsReq.O3,
                        PropertyId = obsReq.QO2id,
                        PropertyName = obsReq.QO2,
                        Group = obsReq.Group,
                        IsOntologyEntry = obsReq.IsOntologyEntry,
                        TermCategory = obsReq.OntologyEntryCategoryName,
                        DataType = obsReq.DataType,
                        ProjectId = obsReq.ProjectId
                    }));
                    cQuery.GroupedObservations.Add(goq);
                }
            }

            var assaypanels = cdto.AssayPanelRequests.Values.Where(ap=>ap.IsRequested);
            foreach (var assayRequest in assaypanels)
            {
                var assayPanelQuery = new AssayPanelQuery();
                assayPanelQuery.AssayId = assayRequest.AssayId;
                assayPanelQuery.AssayName = assayRequest.AssayName;

                foreach (var sampleQuery in assayRequest.SampleQuery)
                {
                    var query = GetQueryFromQueryDTO(sampleQuery);
                    assayPanelQuery.SampleQueries.Add(query);
                }
                cQuery.AssayPanels.Add(assayPanelQuery);
            }

            return _combinedQueryRepository.Insert(cQuery);
        }

        public List<CombinedQueryDTO> GetSavedQueries(string userId)
        {
            var userQueries = _combinedQueryRepository.FindAll(
                d => d.UserId == Guid.Parse(userId) && d.IsSavedByUser).ToList();
            var dtoQueries = userQueries.Select(GetcQueryDTO).ToList();
            return dtoQueries;
        }

        public List<CombinedQueryDTO> GetProjectSavedQueries(int projectId, string userId)
        {
            var userQueries = _combinedQueryRepository.FindAll(d => d.UserId == Guid.Parse(userId)
            && d.ProjectId == projectId
            && d.IsSavedByUser).ToList();
            var dtoQueries = userQueries.Select(QueryService.GetcQueryDTO).ToList();
            return dtoQueries;
        }

        public CombinedQueryDTO GetSavedCombinedQuery(string userId, string queryId)
        {
            //FOR VALIDATION check if user is indeed the owner of this query

            var query = _combinedQueryRepository.FindSingle(c => c.Id == Guid.Parse(queryId));
            var queryDto = GetcQueryDTO(query);
            if (queryDto == null) throw new ArgumentNullException(nameof(queryDto));
            return queryDto.UserId == userId ? queryDto : null;
        }

        public CombinedQueryDTO GetNewCqueryForProject(int projectId, string userId)
        {
            var dto = new CombinedQueryDTO();
            var assays = _assayRepository.FindAll(a => a.Study.Project.Id == projectId).ToList();
            dto.IsSavedByUser = false;
            dto.ProjectId = projectId;

            //if (assays.Count == 0)
            //    return null;
            foreach (var assay in assays)
            {
                var apanel = new AssayPanelDTO
                {
                    AssayId = assay.Id,
                    AssayName = assay.Name
                };
                dto.AssayPanelRequests.Add(apanel.AssayId,apanel);
                dto.UserId = userId;
            }
            return dto;
        }

        public DataExportObject GetQueryResult(Guid combinedQueryId)
        {
            var combinedQuery = _combinedQueryRepository.Get(combinedQueryId);
            var queryResult = new DataExportObject();
            var projectId = combinedQuery.ProjectId;

            //TODO: the list of included proerties should reflect the selected subject properties
            //IE if StudyArm is selected as a field, then include it, if CollectionStudyDay is included, then include it ...etc
            //INSTEAD OF INCLUDING ALL PROPERTIES WHICH WOOULD MAKE THE QUERY REALLY SLOW
            queryResult.Subjects = _subjectRepository.FindAll(
                s => s.Study.ProjectId == projectId,
                new List<string>() { "StudyArm", "Study", "SubjectCharacteristics.CharacteristicFeature" }).ToList();

            //QUERY FOR CLINICAL OBSERVATIONS
            _getObservations(combinedQuery, ref queryResult);


            #region QUERY FOR SUBJECT CHARACTERISITIC (e.g. AGE)

			HashSet<string> scFilteredSubjectIds = new HashSet<string>();
            foreach (var subjCharQuery in combinedQuery.SubjectCharacteristics)
            {
                var characteristics = _subjectCharacteristicRepository.FindAll(
                    sc =>
                        sc.Subject.Study.ProjectId == projectId &&
                        subjCharQuery.QueryWhereValue == sc.CharacteristicFeatureId.ToString(),
                    new List<string>() {"Subject"}).ToList();

                //APPLY FILTERING IF FILTER PRESENT
                if (characteristics.Any() && subjCharQuery.IsFiltered)
                {
                    characteristics = (subjCharQuery.DataType == "string")
                        ? characteristics.FindAll(sc => subjCharQuery.FilterExactValues.Contains(sc.VerbatimValue))
                        : characteristics.FindAll(sc =>
                            int.Parse(sc.VerbatimValue) >= subjCharQuery.FilterRangeFrom &&
                            int.Parse(sc.VerbatimValue) <= subjCharQuery.FilterRangeTo);


					var filteredSubjectIds = characteristics.Select(o => o.SubjectId).Distinct().ToList();
					if (!scFilteredSubjectIds.Any()) scFilteredSubjectIds.UnionWith(filteredSubjectIds);

					scFilteredSubjectIds.IntersectWith(filteredSubjectIds);
                    queryResult.SubjCharsFiltered = true;
                }

                //ADD TO EXPORT DATA 
                queryResult.SubjChars.AddRange(characteristics);
            }
			//CASCADE FILTERING OF OBERVATIONS FROM ALL FILTERS ON OBSERVATIONS
			if (queryResult.SubjCharsFiltered)
				queryResult.SubjChars = queryResult.SubjChars.FindAll(o => scFilteredSubjectIds.Contains(o.SubjectId));

            #endregion


			//IF A REQUESTED OBSERVATION HAS LONGITUDINAL DATA, AUTOMATICALLY ADD A TIMING COLUMN (ASSUMING VISIT AS DEFAULT FOR NOW) SHOULD BE LATER DECIDED BASED ON O3
			if (queryResult.HasLongitudinalData)
                combinedQuery.DesignElements.Add(new Query() { QueryFor = nameof(Visit) });

            foreach (var deQuery in combinedQuery.DesignElements)
            {
                switch (deQuery.QueryFor)
                {
                    case nameof(HumanSubject.StudyCohort):
                        if (deQuery.IsFiltered)
                            queryResult.Arms = _armRepository.FindAll(
                                a => a.Studies.Select(s => s).All(s => s.ProjectId == projectId)
                                && deQuery.FilterExactValues.Contains(a.Name)).ToList();
                        else
                            queryResult.Arms = _armRepository.FindAll(
                                a => a.Studies.Select(s => s.ProjectId).Contains(projectId)).ToList();
                        break;
                    case nameof(HumanSubject.Study):
                        if (deQuery.IsFiltered)
                            queryResult.Studies = _studyRepository.FindAll(
                                                        s => s.ProjectId == projectId
                                                        && deQuery.FilterExactValues.Contains(s.Name),
                                                        new List<string>() { "Subjects" }).ToList();
                        else
                            queryResult.Studies = _studyRepository.FindAll(s => s.ProjectId == projectId).ToList();
                        break;
                    case nameof(Visit):
                        var visits = _visitRepository.FindAll(
                            v => v.Study.ProjectId == projectId
                            //apply filter if present
                            ).ToList();
                        queryResult.Visits = visits;
                        break;
                } 
            }

            //FOR NOW SHOULD NOT BE ALLOWED UNLESS WE DECIDED TO PROPAGATE ALL ASSAY SPECIFIC FILTERS TO ALL ASSAYS IN THE COMBINED QUERY
            //if (combinedQuery.AssayPanels.Any() && combinedQuery.AssayPanels.Count > 1)
            //    return null;

            var apQuery = combinedQuery.AssayPanels.FirstOrDefault();
            if (apQuery != null)
            {
                //TODO:BRING BACK COLLECTIONSTUDYDAY
                var assaySamples =
                    _biosampleRepository.FindAll(s => s.AssayId == apQuery.AssayId,
                        new List<string>() {"Subject", "SampleCharacteristics.CharacteristicFeature", "CollectionStudyDay" }).ToList();
                queryResult.Samples = assaySamples;
                //HOW DO I FILTER THE SAMPLES BY THE QUERED PROPERTIES?
                //HOW DO I filter samples by collectionStudyDay?

                //TODO: TEMP SOLUTION FOR COLLECTION STUDY DAY
                //foreach (var sampleQuery in apQuery.SampleQueries.Where(sq=>sq.QueryFor != nameof(Biosample.SampleCharacteristics)))
                //{
                //    assaySamples.FindAll(b => 
                //    int.Parse(b.GetType().GetProperty("CollectionStudyDay").GetValue(b).properyObj?.GetType().GetProperty(query.QuerySelectProperty)?.GetValue(properyObj)?.ToString().ToString()) >= sampleQuery.FilterRangeFrom &&
                //    int.Parse(b.GetType().GetProperty("CollectionStudyDay").GetValue(b).ToString()) <= sampleQuery.FilterRangeTo
                //    );

                //}
                if (apQuery.SampleQueries.Exists(sq => sq.QueryFor == nameof(Biosample.CollectionStudyDay)))
                {
                    var dayQuery = apQuery.SampleQueries.Find(sq => sq.QueryFor == nameof(Biosample.CollectionStudyDay));
                    if (dayQuery.IsFiltered)
                        queryResult.Samples = assaySamples.FindAll(
                            s =>
                                s.CollectionStudyDay?.Number != null &&
                                dayQuery.FilterExactValues.Contains(s.CollectionStudyDay?.Number.Value.ToString()));
                }

                //QUERY AND FILTER SAMPLE CHARACTERISTICS
                foreach (var sampleQuery in apQuery.SampleQueries.Where(sq=>sq.QueryFor == nameof(Biosample.SampleCharacteristics)))
                {
                    var characteristics = _sampleCharacteristicRepository.FindAll(
                        sc =>
                            sc.Sample.Study.ProjectId == projectId &&
                            sampleQuery.QueryWhereValue == sc.CharacteristicFeatureId.ToString(),
                            new List<string>() {"Sample"}).ToList();

                    //APPLY FILTERING IF FILTER PRESENT
                    if (characteristics.Any() && sampleQuery.IsFiltered)
                    {
                        characteristics = (sampleQuery.DataType == "string")
                            ? characteristics.FindAll(sc => sampleQuery.FilterExactValues.Contains(sc.VerbatimValue))
                            : characteristics.FindAll(sc =>
                                int.Parse(sc.VerbatimValue) >= sampleQuery.FilterRangeFrom &&
                                int.Parse(sc.VerbatimValue) <= sampleQuery.FilterRangeTo);
                    }
                    //ADD TO EXPORT DATA 
                    queryResult.SampleCharacteristics.AddRange(characteristics);
                }
            }
            

            queryResult.FilterAndJoin();

            return queryResult;
        }

        private void _getObservations(CombinedQuery combinedQuery, ref DataExportObject queryResult)
        {
            List<SdtmRow> observations = new List<SdtmRow>();
            HashSet<string> ObsFilteredSubjectIds = new HashSet<string>();


        //GROUP CLINICAL OBSERVATIONS (GROUP AND INDIVIDUAL)
        var obsQueries = combinedQuery.ClinicalObservations.Union(combinedQuery.GroupedObservations).ToList();

            //GROUP CLINICAL OBSERVATIONS BY O3 TO QUERY FOR THE OBSERVATION (E.G. HEADACHE ONLY ONCE EVEN IF TWO REQUEST ARE MADE (e.g. AEOCCUR and AESEV)
            var queriesByO3Id = obsQueries.GroupBy(f => f.QueryObjectName).ToList();
            foreach (var sameO3queries in queriesByO3Id)
            {
                var o3q_g = sameO3queries.Select(group => group).First();
                var o3q_list = new List<ObservationQuery>();
               

                //EXPANIDNG ALL to GROUP  TO ACCOMMODATE FOR GROUP OBSERVATIONS AS WELL IN THE SAME CODE BLOCK
                if (o3q_g.GetType().Name == nameof(GroupedObservationsQuery))
                {
                    ((GroupedObservationsQuery)o3q_g).GroupedObservations.ForEach(oq => o3q_list.Add(oq));
                }
                else
                    o3q_list.Add(o3q_g);
                //////////////////////////////////////////////////////////////////////////////////////////////

                var obsQueryResult = new List<SdtmRow>();

                //QUERYING FOR OBSERVATIONS
                foreach (var o3q in o3q_list)
                {
                    var _observations = o3q.IsOntologyEntry
                         ? _sdtmRepository.FindAll(s => s.QualifierQualifiers[o3q.TermCategory] == o3q.TermId.ToString() && s.Group == o3q.Group && s.ProjectId == o3q.ProjectId).ToList()
                                          : _sdtmRepository.FindAll(s => s.DBTopicId == o3q.TermId && s.ProjectId == o3q.ProjectId).ToList();
                    obsQueryResult.AddRange(_observations);

                    if(_observations.Count > queryResult.Subjects.Count)
                        queryResult.HasLongitudinalData = true;
                }



                //FILTERING OBSERVATIONS
                foreach (var oq in sameO3queries) //AEOCCUR / AESEV
                {
                    if (!oq.IsFiltered) continue;

                    //HACK FOR AEOCCUR 'Y' FILTERING
                    if(oq.PropertyName != "AEOCCUR"){
                        //HACK FOR FINDINGS SHOULD GO AWAY IN THE NEW OBSERVATION MODEL
                        obsQueryResult.ForEach(o => o.Qualifiers = o.ResultQualifiers.Union(o.Qualifiers).ToDictionary(p => p.Key, p => p.Value));

                        string v = null;
                        float vn;
                        obsQueryResult = oq.DataType == "string"
                            ? obsQueryResult.FindAll(
                                s => s.Qualifiers.TryGetValue(oq.PropertyName, out v) &&
                                oq.FilterExactValues.Contains(s.Qualifiers[oq.PropertyName]))

                            : obsQueryResult.FindAll(
                                s => float.TryParse(s.Qualifiers[oq.PropertyName], out vn) &&
                                     vn >= oq.FilterRangeFrom &&
                                     vn <= oq.FilterRangeTo);
                    }

                    var filteredSubjectIds = obsQueryResult.Select(o => o.USubjId).Distinct().ToList();
                    if(oq.PropertyName == "AEOCCUR" && oq.FilterExactValues.All(f => f == "N")){
                        //Need to do A ~ B where A is al project subjects and B is the subjects with AEOCCUR=Y i.e. the ones 
                        //that are returned with the query in obsQueryResult
                        var allSubjIds = queryResult.Subjects.Select(s => s.UniqueSubjectId).ToList();
                        filteredSubjectIds = allSubjIds.Except(filteredSubjectIds).ToList();
                        //Create fake observations for subjects withOUT the AE (i.e. AEOCCUR = 'N'
                        obsQueryResult = GetFakeAEoccurN(filteredSubjectIds, obsQueryResult.First());
                    }

                    //fill it in first if first round
                    if (!ObsFilteredSubjectIds.Any()) ObsFilteredSubjectIds.UnionWith(filteredSubjectIds);

                    //maintain the intersection of all filters applies across all requested observations
                    ObsFilteredSubjectIds.IntersectWith(filteredSubjectIds);
                    queryResult.ObservationsFiltered = true;
                }
                observations.AddRange(obsQueryResult);//will be filtered later by the combined ObsFilteredSubjectIds below
            }

            //CASCADE FILTERING OF OBERVATIONS FROM ALL FILTERS ON OBSERVATIONS
            if(queryResult.ObservationsFiltered)
                observations = observations.FindAll(o => ObsFilteredSubjectIds.Contains(o.USubjId));
            queryResult.Observations = observations;
        }

        private List<SdtmRow> GetFakeAEoccurN(List<string> subjectIds,SdtmRow yAE){
            return subjectIds.Select(subj => new SdtmRow()
            {
                USubjId = subj,
                //StudyId = subj.Study.Name,
                //Topic = obsreq.O3,
                //DBTopicId = termId,//obsreq.TermIds[0],
                //Id = Guid.NewGuid(),
                ActivityId = yAE.ActivityId,
                DatasetId = yAE.DatasetId,
                DatafileId = yAE.DatafileId,
                ProjectId = yAE.ProjectId,
                ProjectAccession = yAE.ProjectAccession,
                Class = yAE.Class,
                Group = yAE.Group,
                TopicControlledTerm = yAE.TopicControlledTerm,
                QualifierQualifiers = yAE.QualifierQualifiers,
                Qualifiers = new Dictionary<string, string>() { { "AEOCCUR", "N" } }
            }).ToList();
        }
		public CombinedQuery CreateSingleAssayCombinedQuery(CombinedQuery cQuery, AssayPanelQuery apQuery)
        {
            var singleAssayCombinedQuery = new CombinedQuery
            {
                ProjectId = cQuery.ProjectId,
                ClinicalObservations = cQuery.ClinicalObservations,
                DesignElements = cQuery.DesignElements,
                GroupedObservations = cQuery.GroupedObservations,
                SubjectCharacteristics = cQuery.SubjectCharacteristics,
                UserId = cQuery.UserId,
                Id = Guid.NewGuid()
            };
            singleAssayCombinedQuery.AssayPanels.Add(apQuery);
            _combinedQueryRepository.Insert(singleAssayCombinedQuery);

            return singleAssayCombinedQuery;
        }

        public static CombinedQueryDTO GetcQueryDTO(CombinedQuery cQuery)
        {
            var dto = new CombinedQueryDTO();
            dto.Id = cQuery.Id.ToString();
            dto.Name = cQuery.Name;
            dto.UserId = cQuery.UserId.ToString();
            dto.ObsRequests = new List<ObservationRequestDTO>();
            dto.ProjectId = cQuery.ProjectId;
            foreach (var oq in cQuery.ClinicalObservations.Union(cQuery.GroupedObservations))
            {
                dto.ObsRequests.Add(GetDTOforQuery(oq));
            }
            foreach (var oq in cQuery.SubjectCharacteristics.Union(cQuery.DesignElements))
            {
                dto.SubjCharRequests.Add(GetDTOforQuery(oq));
            }
            foreach (var apq in cQuery.AssayPanels)
            {
                dto.AssayPanelRequests.Add(apq.AssayId,GetDTOforAssayPanelQuery(apq));
            }

            return dto;
        }

        public Query GetQueryFromQueryDTO(ObservationRequestDTO dto)
        {
            Query query;
            if (dto.IsClinicalObservations)
            {
                query = new ObservationQuery()
                {
                    QueryFrom = dto.QueryFrom,
                    QueryFor = dto.QueryFor,

                    //can change these to more specific observation relevant names
                    TermName = dto.O3,
                    TermId = dto.O3id,
                    PropertyName = dto.QO2,
                    PropertyId = dto.QO2id,
                    ProjectId = dto.ProjectId,

                    Group = dto.Group,
                    IsOntologyEntry = dto.IsOntologyEntry,
                    TermCategory = dto.OntologyEntryCategoryName,
                    HasLongitudinalData = dto.HasLongitudinalData,
                    HasTPT = dto.HasTPT,

                    ObservationObjectShortName = dto.O3code,
                    DataType = dto.DataType,
                    FilterExactValues = dto.FilterExactValues,
                    FilterRangeFrom = dto.FilterRangeFrom,
                    FilterRangeTo = dto.FilterRangeTo,
                    IsFiltered = dto.IsFiltered
                };
            }
            else
            {
                query = new Query()
                {
                    QueryFrom = dto.QueryFrom,
                    QueryFor = dto.QueryFor,
                    QueryWhereProperty = dto.QueryWhereProperty,
                    QueryWhereValue = dto.QueryWhereValue,
                    QuerySelectProperty = dto.QuerySelectProperty,

                    QueryObjectName = dto.O3code,//+"["+dto.QuerySelectProperty+"]",

                    DataType = dto.DataType,
                    FilterExactValues = dto.FilterExactValues,
                    FilterRangeFrom = dto.FilterRangeFrom,
                    FilterRangeTo = dto.FilterRangeTo,
                    IsFiltered = dto.IsFiltered
                };


            }
            return query;
        }

		public CombinedQuery GetQuery(string queryId){
			var query = _combinedQueryRepository.FindSingle(c => c.Id == Guid.Parse(queryId));
			return query;
		}

        private static ObservationRequestDTO GetDTOforQuery(Query query)
        {
            int o3id;
            var qdto = new ObservationRequestDTO()
            {
                O3 = query.QueryObjectName, 
                O3code = query.QueryObjectName,
                O3id = int.TryParse(query.QueryWhereValue, out o3id) ? o3id:0,
                DataType = query.DataType,
                ProjectId = query.ProjectId,
                QueryFrom = query.QueryFrom,
                QueryFor = query.QueryFor,
                QueryWhereProperty = query.QueryWhereProperty,
                QueryWhereValue = query.QueryWhereValue,
                QuerySelectProperty = query.QuerySelectProperty,


                IsFiltered = query.IsFiltered,
                FilterRangeTo = query.FilterRangeTo,
                FilterRangeFrom = query.FilterRangeFrom,
                FilterExactValues = query.FilterExactValues,
                FilterText = query.FilterText
                
            };
            if (query.GetType() == typeof(ObservationQuery) || query.GetType() == typeof(GroupedObservationsQuery))
            {
                qdto.O3 = ((ObservationQuery)query).TermName;
                qdto.O3id = ((ObservationQuery)query).TermId;
                qdto.O3code = ((ObservationQuery)query).ObservationObjectShortName?? (((ObservationQuery)query).ObservationName);
                qdto.QO2 = ((ObservationQuery) query).PropertyName;
                qdto.QO2_label = ((ObservationQuery) query).PropertyLabel;
                qdto.QO2id = ((ObservationQuery) query).PropertyId;
            }
            return qdto;
        }

        private static AssayPanelDTO GetDTOforAssayPanelQuery(AssayPanelQuery apQuery)
        {
            var apDTO = new AssayPanelDTO()
            {
               AssayId = apQuery.AssayId,
               AssayName = apQuery.AssayName
            };
            foreach (var sampleQuery in apQuery.SampleQueries)
            {
                apDTO.SampleQuery.Add(GetDTOforQuery(sampleQuery));
            }
            return apDTO;
        }
    }
}
