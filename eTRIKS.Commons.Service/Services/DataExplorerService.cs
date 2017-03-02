using eTRIKS.Commons.Core.Domain.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel.SDTM;
using eTRIKS.Commons.Core.Domain.Model.DesignElements;
using eTRIKS.Commons.Core.Domain.Model.Templates;
using eTRIKS.Commons.Core.Domain.Model.Timing;
using eTRIKS.Commons.Core.Domain.Model.Users;
using eTRIKS.Commons.Core.Domain.Model.Users.Datasets;
using eTRIKS.Commons.Core.Domain.Model.Users.Queries;
using eTRIKS.Commons.Service.DTOs.Explorer;


namespace eTRIKS.Commons.Service.Services
{

    public class DataExplorerService
    {
        private readonly IRepository<Observation, int> _observationRepository;
        private readonly IRepository<HumanSubject, string> _subjectRepository;
        private readonly IRepository<CharacteristicFeature, int> _characObjRepository;
        private readonly IRepository<SdtmRow, Guid> _sdtmRepository;
        private readonly IRepository<Assay, int> _assayRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<CombinedQuery, Guid> _combinedQueryRepository;
        private readonly IRepository<Biosample, int> _biosampleRepository;
        private readonly IServiceUoW _dataContext;

        private const string SubjectIdColName = "subjectId";
        private const string StudyIdColName = "studyId";
        private const string SampleIdColName = "studyId";

        public DataExplorerService(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _observationRepository = uoW.GetRepository<Observation, int>();
            _subjectRepository = uoW.GetRepository<HumanSubject, string>();
            _characObjRepository = uoW.GetRepository<CharacteristicFeature, int>();
            _sdtmRepository = uoW.GetRepository<SdtmRow, Guid>();
            _assayRepository = uoW.GetRepository<Assay, int>();
            _userRepository = uoW.GetRepository<User, Guid>();
            _combinedQueryRepository = uoW.GetRepository<CombinedQuery, Guid>();
            _biosampleRepository = uoW.GetRepository<Biosample, int>();
        }


        public CombinedQuery SaveQuery(CombinedQueryDTO cdto, string userId, int projectId)
        {
            CombinedQuery cQuery = new CombinedQuery();

            cQuery.Name = cdto.Name;
            cQuery.UserId = Guid.Parse(userId);
            cQuery.ProjectId = projectId;
            cQuery.Id = Guid.NewGuid();

            var requests = cdto.ObsRequests;

            foreach (var request in requests)
            {
                if (!request.IsMultipleObservations)
                {
                    var oq = new ObservationQuery()
                    {
                        TermName = request.O3,
                        TermId = request.O3id,
                        PropertyName = request.QO2,
                        PropertyId = request.QO2id,
                        ProjectId = request.ProjectId,

                        Group = request.Group,
                        IsOntologyEntry = request.IsOntologyEntry,
                        TermCategory = request.OntologyEntryCategoryName,

                        ObservationObjectShortName = request.O3code,
                        DataType = request.DataType,
                        FilterExactValues = request.FilterExactValues,
                        FilterRangeFrom = request.FilterRangeFrom,
                        FilterRangeTo = request.FilterRangeTo,
                        IsFiltered = request.IsFiltered
                    };
                    if (request.IsSubjectCharacteristics)
                        cQuery.SubjectCharacteristics.Add(oq);
                    if (request.IsClinicalObservations)
                        cQuery.ClinicalObservations.Add(oq);
                    if (request.IsDesignElement)
                        cQuery.DesignElements.Add(oq);
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
            return _combinedQueryRepository.Insert(cQuery);
        }

        public List<CombinedQuery> GetSavedQueries(int projectId, string userId)
        {

            List<CombinedQuery> cart = _combinedQueryRepository.FindAll(d => d.UserId == Guid.Parse(userId) && d.ProjectId == projectId).ToList();

            return cart;
        }

        public CombinedQuery GetSavedCombinedQuery(int projectId, string userId, string queryId)
        {
            //FOR VALIDATION check if user is indeed the owner of this query
            var query = _combinedQueryRepository.FindSingle(c => c.Id == Guid.Parse(queryId));
            return query.UserId == Guid.Parse(userId) ? query : null;
        }

        //************************************************************** To Be Completed *******************************************************

        //public void UpdateQueries(CombinedQueryDTO cdto, string userId, int projectId)
        //  //  public List<CombinedQuery> UpdateCart(CombinedQueryDTO cdto, int projectId, string userId)
        //{
        //    CombinedQuery cartToUpdate =
        //   _combinedQueryRepository.FindSingle(d => d.UserId == Guid.Parse(userId) && d.ProjectId == projectId && d.Name == cdto.Name);
        //        //ombinedQuery combinedQuery = new CombinedQuery();
        //        //   _combinedQuery.Update(cartToUpdate);


        //        cartToUpdate.Name = cdto.Name;
        //        //cartToUpdate.UserId = Guid.Parse(userId);
        //        //cartToUpdate.ProjectId = projectId;
        //        _combinedQueryRepository.Update(cartToUpdate);
        //    }
        //************************************************************** To Be Completed *******************************************************

        #region Observation Browser methods
        public List<ObservationRequestDTO> GetSubjectCharacteristics(int projectId)
        {
            var subjChars = new List<ObservationRequestDTO>();
            var SCs = _characObjRepository.FindAll(sco => sco.ProjectId == projectId).ToList();

            subjChars = SCs.Select(sc => new ObservationRequestDTO()
            {
                O3 = sc.FullName,
                O3id = sc.Id,
                O3code = sc.ShortName,
                DataType = sc.DataType,
                IsSubjectCharacteristics = true,
                ProjectId = projectId,
                QueryFor = nameof(HumanSubject),
                QueryBy = nameof(HumanSubject.SubjectCharacteristics),
                QuerySelectProperty = nameof(SubjectCharacteristic.CharacteristicFeatureId)

            }).ToList();

            var armSC = new ObservationRequestDTO()
            {
                O3 = nameof(Arm),
                O3code = "ARM",
                O3id = 999,
                DataType = "string",
                IsDesignElement = true,
                DesignElementType = nameof(Arm),
                QO2 = "Name",
                QueryFor = nameof(HumanSubject),
                QueryBy = nameof(HumanSubject.StudyArm),
                QuerySelectProperty = nameof(Arm.Name),
                ProjectId = projectId
            };
            var studySC = new ObservationRequestDTO()
            {
                O3 = nameof(Study),
                O3code = "STUDY",
                O3id = 888,
                DataType = "string",
                IsDesignElement = true,
                DesignElementType = nameof(Study),
                QueryFor = nameof(HumanSubject),
                QueryBy = nameof(HumanSubject.Study),
                QuerySelectProperty = nameof(Study.Name),
                QO2 = "Name",
                ProjectId = projectId
            };
            subjChars.Add(armSC);
            subjChars.Add(studySC);

            return subjChars.OrderBy(o => o.O3).ToList();
        }

        public ObservationNode GroupObservations(int projectId, List<ObservationRequestDTO> observations)
        {
            var obsRequest = new ObservationRequestDTO() { O3code = "grp_", IsEvent = true, IsClinicalObservations = true, IsMultipleObservations = true, ProjectId = projectId };


            for (int i = 0; i < observations.Count; i++)
            {
                obsRequest.O3 += observations[i].O3 + (i + 1 < observations.Count ? " & " : "");
                obsRequest.O3code += observations[i].O3variable + "(" + observations[i].O3id + ")" + (i + 1 < observations.Count ? "_" : "");
            }
            //HACK FOR AE OCCUR ASSUMING THAT ONLY AE MedDRA terms can be grouped for now
            obsRequest.QO2 = "AEOCCUR";//should be the default qualifier 
            obsRequest.QO2id = 0;//should be the default qualifier 
            obsRequest.QO2_label = "OCCURENCE";
            obsRequest.DataType = "string"; //Datatype referes to the QO2

            obsRequest.GroupedObservations = observations;


            //TODO: BIG TODO when switiching to descriptors to only add qualifiers that have values for the group
            //for now all the qualifiers are assumed to be the same for all observations in the group and listed whehter there would be data available for them or not
            int fobsid = observations.First().TermIds.First();
            var observation = _observationRepository.FindSingle(o => o.Id == fobsid,
                new List<string>() {
                    "Qualifiers.Qualifier"
                });
            var reqs = observation.Qualifiers.Select(variableDefinition => new ObservationRequestDTO()
            {

                O3 = obsRequest.O3,
                O3code = obsRequest.O3code,
                IsEvent = obsRequest.IsEvent,
                IsMultipleObservations = obsRequest.IsMultipleObservations,
                IsClinicalObservations = true,

                GroupedObservations = observations,
                ProjectId = projectId,
                Group = obsRequest.Group,

                QO2 = variableDefinition.Qualifier.Name,
                QO2id = variableDefinition.Qualifier.Id,
                DataType = variableDefinition.Qualifier.DataType,
                QO2_label = variableDefinition.Qualifier.Label,
            }).ToList();

            return new ObservationNode() { DefaultObservation = obsRequest, Qualifiers = reqs, Name = obsRequest.Name };
        }

        public async Task<IEnumerable<ClinicalDataTreeDTO>> GetClinicalObsTree(int projectId)
        {

            //TODO: will replace Observation here with ObservationDescriptor
            List<Observation> studyObservations = _observationRepository.FindAll(
                    o => o.ProjectId == projectId,
                    new List<string>()
                    {
                        "DefaultQualifier",
                        "Qualifiers.Qualifier"
                    }).ToList();


            List<ClinicalDataTreeDTO> cdTreeList = new List<ClinicalDataTreeDTO>();

            //Group by class
            var groupedByClass = studyObservations.GroupBy(ob => ob.Class);
            foreach (var classGroup in groupedByClass)
            {
                if (classGroup.Key.Equals("SpecialPurpose"))
                    continue;

                var classNode = new ClinicalDataTreeDTO();
                cdTreeList.Add(classNode);
                classNode.Class = classGroup.Key;

                //Group by domain
                var groupedByDomain = classGroup.GroupBy(x => new { domainCode = x.DomainCode, domainName = x.DomainName });
                foreach (var domainGroup in groupedByDomain)
                {
                    var domainNode = new GroupNode
                    {
                        Name = domainGroup.Key.domainName,
                        Code = domainGroup.Key.domainCode,
                        Count = domainGroup.Count(),
                        IsDomain = true
                    };
                    classNode.Domains.Add(domainNode);


                    //Group by category
                    var groupedByCategory = domainGroup.GroupBy(x => x.Group);
                    int i = 0;
                    foreach (var catGroup in groupedByCategory)
                    {
                        GroupNode groupNode = domainNode;
                        if (catGroup.Key != null)
                        {
                            groupNode = new GroupNode();
                            domainNode.Terms.Add(groupNode);
                            groupNode.Name = catGroup.Key;
                            groupNode.Code = domainNode.Code + "_cat" + ++i;
                            groupNode.Count = catGroup.Distinct().Count();
                        }

                        //***********************!!!!!!!!!!!!!!HACK!!!!!!!!!!!ALERT!!!!!!!!!!!!!!!!!!!!!!!!!**************************
                        if (domainNode.Code == "AE" && projectId == 25)
                        {
                            var AEs = await getEventsByMedDRA(projectId, catGroup.ToList(), groupNode.Code, groupNode.Name);
                            groupNode.Terms.AddRange(AEs);
                        }
                        else
                        {
                            foreach (var obs in catGroup)
                                groupNode.Terms.Add(createObsNode(obs));
                        }
                    }
                }
            }
            return cdTreeList;

        }

        public List<AssayBrowserDTO> GetProjectAssays(int projectId)
        {
            var assays = _assayRepository.FindAll(a => a.ProjectId == projectId, new List<string>(){
                    "MeasurementType",
                    "TechnologyPlatform",
                    "TechnologyType",
                }).ToList();

            if (assays.Count == 0)
                return null;

            var projectAssays = new List<AssayBrowserDTO>();
            foreach (var assay in assays)
            {
                var assayDTO = new AssayBrowserDTO()
                {
                    Id = assay.Id,
                    Type = assay.MeasurementType != null ? assay.MeasurementType.Name : "",
                    Platform = assay.TechnologyPlatform != null ? assay.TechnologyPlatform.Name : "",
                    Technology = assay.TechnologyType != null ? assay.TechnologyType.Name : "",
                    Name = assay.Name
                };

                var SCs = _characObjRepository.FindAll(sco => sco.ActivityId == assay.Id).ToList();

                assayDTO.SampleCharacteristics = SCs.Select(sc => new ObservationRequestDTO()
                {
                    O3 = sc.FullName,
                    O3id = sc.Id,
                    O3code = sc.ShortName,
                    DataType = sc.DataType,
                    IsSampleCharacteristic = true,
                    ProjectId = projectId,
                    QueryFor = nameof(Biosample),
                    QueryBy = nameof(Biosample.SampleCharacteristics),
                    QueryWhereProperty = nameof(SampleCharacteristic.CharacteristicFeatureId),
                    QuerySelectProperty = nameof(SampleCharacteristic.VerbatimValue)
                }).ToList();

                assayDTO.SampleCharacteristics.Add(new ObservationRequestDTO()
                {
                    O3 = "Collection Day",
                    O3code = "Day",
                    O3id = 454,
                    QueryFor = nameof(Biosample),
                    QueryBy = nameof(Biosample.CollectionStudyDay),
                    QuerySelectProperty = nameof(RelativeTimePoint.Number),
                    DataType = nameof(Int32),
                    IsSampleCharacteristic = true,
                    ProjectId = projectId
                    
                });
                projectAssays.Add(assayDTO);
            }
            return projectAssays;
        }
        #endregion

        #region Crossfilter data methods
        public Hashtable GetObservationsData(int projectId, List<ObservationRequestDTO> reqObservations)
        {
            var sdtmObservations = getObservations(reqObservations, projectId);

            var subjFindings = sdtmObservations.FindAll(s => s.Class.ToLower() == "findings").ToList();
            var subjEvents = sdtmObservations.FindAll(s => s.Class.ToLower() == "events").ToList();

            var findingsTable = getFindingsDataTable(subjFindings, reqObservations.Where(r => r.IsFinding).ToList());
            var eventsTable = getEventsDataTable(subjEvents, reqObservations.Where(r => r.IsEvent).ToList());

            var findingsColsList = findingsTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
            var eventsColsList = eventsTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
            var result = new Hashtable
            {
                {"findingsTbl", findingsTable.Rows},
                {"eventsTbl", eventsTable.Rows},
                {"findingsTblHeader", findingsColsList},
                {"eventsTblHeader", eventsColsList}
            };
            return result;
        }
        public DataTable GetSubjectData(int projectId, List<ObservationRequestDTO> reqObservations)
        {
            var subjects = _subjectRepository.FindAll(
                s => s.Study.ProjectId == projectId,
                new List<string>()
                {   "SubjectCharacteristics.CharacteristicFeature",
                    "StudyArm",
                    "Study"
                }).ToList();

            var subjectTable = new DataTable();

            //ADD TABLE COLUMNS
            if (reqObservations != null)
                foreach (var queryTerm in reqObservations)
                    subjectTable.Columns.Add(queryTerm.Name.ToLower());

            //ITERATE THROUGH SUBJECTS ADDING EACH REQUESTED QUERY TERM AS A COLUMN VALUE
            foreach (var subject in subjects)
            {
                var row = subjectTable.NewRow();
                row["subjectId"] = subject.UniqueSubjectId;

                if (reqObservations != null)
                    foreach (var requestDto in reqObservations)
                    {
                        if (requestDto.QueryFor != nameof(HumanSubject))
                            continue;

                        string subjProperty;

                        var queryProperty = subject.GetType().GetProperty(requestDto.QueryBy);
                        if (queryProperty != null && queryProperty.PropertyType.IsConstructedGenericType)
                        {
                            //Property is a collection
                            //Simple for now since there's only one collection in human subject i.e. subjectcharacteristics
                            subjProperty = ((IList) queryProperty.GetValue(subject)).Cast<SubjectCharacteristic>()
                                .SingleOrDefault(sc => sc.CharacteristicFeatureId == requestDto.O3id)?.VerbatimValue;
                        }
                        else
                        {
                            var properyObj = subject.GetType().GetProperty(requestDto.QueryBy)?.GetValue(subject);
                            subjProperty = properyObj?.GetType().GetProperty(requestDto.QuerySelectProperty)?.GetValue(properyObj)?.ToString();
                        }

                        //var charVal = subject.SubjectCharacteristics.SingleOrDefault(sc => sc.CharacteristicFeatureId.Equals(requestDto.O3id));
                        if (subjProperty == null) continue;
                        row[requestDto.Name] = subjProperty;
                    }

                subjectTable.Rows.Add(row);
            }
            return subjectTable;
        }
        public DataTable GetSampleDataForAssay(int assayId, List<ObservationRequestDTO> reqSampleChars)
        {
            //var samples = new List<Biosample>();
            var samples = _assayRepository.FindSingle(a => a.Id == assayId, new List<string>() { "Biosamples.Study", "Biosamples.Subject" }).Biosamples;
            //samples = _bioSampleRepository.FindAll
            //    (bs => bs.AssayId.Equals(assayId), 
            //    new List<string>()
            //    {
            //        "Study",
            //        "Subject",
            //        //"CollectionStudyDay"
            //    }).ToList();

            var sampleTable = new DataTable();
            sampleTable.Columns.Add("subjectId");
            sampleTable.Columns.Add("studyId");
            sampleTable.Columns.Add("sampleId");

            if (reqSampleChars != null)
            {
                foreach (var column in reqSampleChars)
                    sampleTable.Columns.Add(column.Name.ToLower());
            }

            foreach (var sample in samples)
            {
                var row = sampleTable.NewRow();
                row["subjectId"] = sample.Subject != null ? sample.Subject.UniqueSubjectId : "missing";
                row["studyId"] = sample.Study.Name;
                row["sampleId"] = sample.BiosampleStudyId;

                //if assay has time series include the timing dimension by default

                if (reqSampleChars != null)
                    foreach (var requestDto in reqSampleChars)
                    {
                        if (requestDto.QueryFor != nameof(Biosample))
                            continue;

                        string subjProperty;

                        var queryProperty = sample.GetType().GetProperty(requestDto.QueryBy);
                        if (queryProperty != null && queryProperty.PropertyType.IsConstructedGenericType)
                        {
                            //Property is a collection
                            //Simple for now since there's only one collection in sample i.e. subjectcharacteristics
                            subjProperty = ((IList)queryProperty.GetValue(sample)).Cast<SampleCharacteristic>()
                                .SingleOrDefault(sc => sc.CharacteristicFeatureId == requestDto.O3id)?.VerbatimValue;
                        }
                        else
                        {
                            var properyObj = sample.GetType().GetProperty(requestDto.QueryBy)?.GetValue(sample);
                            subjProperty = properyObj?.GetType().GetProperty(requestDto.QuerySelectProperty)?.GetValue(properyObj)?.ToString();
                        }
                        //var charVal = sample.SampleCharacteristics.SingleOrDefault(sc => sc.CharacteristicFeatureId.Equals(requestDto.O3id));
                        if (subjProperty == null) continue;
                        row[requestDto.Name] = subjProperty;
                    }
                sampleTable.Rows.Add(row);
            }

            return sampleTable;

        }
        #endregion

        #region Private helper methods

        private List<SdtmRow> getObservations(List<ObservationRequestDTO> obsRequests, int projectId)
        {
            List<SdtmRow> sdtmObservations = new List<SdtmRow>();

            //Retrieve rows for requested individual observations //i.e not ontology entry
            var observationsIDs = obsRequests.Where(r => !r.IsMultipleObservations).Select(o => o.O3id).ToList();
            sdtmObservations = _sdtmRepository.FindAll(s => observationsIDs.Contains(s.DBTopicId) && s.ProjectId == projectId).ToList();


            //Retrieve sdtmrows for requested OEs  (MedDRA headers)
            // var test2 = obsRequests.Where(or => or.IsMultipleObservations).
            var qTerms = obsRequests.FindAll(or => or.IsOntologyEntry)
                                        .Select(o => new { name = o.OntologyEntryCategoryName, value = o.OntologyEntryValue, group = o.Group }).Distinct().ToList();
            foreach (var qTerm in qTerms)
            {
                var observations = qTerm.group != null ?
                     _sdtmRepository.FindAll(
                         s => s.QualifierQualifiers[qTerm.name] == qTerm.value
                         && s.ProjectId == projectId
                         && s.Group == qTerm.group)
                     .ToList()
                     : _sdtmRepository.FindAll(
                         s => s.QualifierQualifiers[qTerm.name] == qTerm.value
                         && s.ProjectId == projectId)
                     .ToList();


                obsRequests
                    .FindAll(or => or.OntologyEntryValue == qTerm.value && qTerm?.group == or.Group)
                    .ForEach(oq => oq.TermIds.AddRange(observations.Select(o => o.DBTopicId).Distinct()));
                sdtmObservations.AddRange(observations);
            }

            //Retrieves sdtmrows for grouped observations
            foreach (var obsGrpReq in obsRequests.Where(or => or.IsMultipleObservations))
            {
                foreach (var or in obsGrpReq.GroupedObservations)
                {
                    var observations = or.IsOntologyEntry
                        ? _sdtmRepository.FindAll(s => s.QualifierQualifiers[or.OntologyEntryCategoryName] == or.OntologyEntryValue && s.Group == or.Group && s.ProjectId == projectId).ToList()
                        : _sdtmRepository.FindAll(s => or.O3id == s.DBTopicId && s.ProjectId == projectId).ToList();
                    if (!sdtmObservations.Contains(observations.First()))
                        sdtmObservations.AddRange(observations);
                    obsGrpReq.TermIds.AddRange(observations.Select(o => o.DBTopicId).Distinct().ToList());
                }
            }
            return sdtmObservations;
        }


        private DataTable getFindingsDataTable(List<SdtmRow> findings, IList<ObservationRequestDTO> reqObservations)
        {
            #region Build Table columns
            var datatable = new DataTable();
            datatable.Columns.Add("subjectId");
            datatable.Columns.Add("study");
            foreach (var r in reqObservations.Where(r => findings.Select(f => f.DBTopicId).Contains(r.O3id)))
                datatable.Columns.Add(r.Name);

            datatable.Columns.Add("visit");
            datatable.Columns.Add("timepoint");
            #endregion

            #region Group observations by unique key
            var findingsBySubjectVisitTime = findings //The unique combination that would make one row
                .GroupBy(ob => new
                {
                    subjId = ob.USubjId,
                    Visit = ob.VisitName,
                    StudyDay = ob.CollectionStudyDay == null ? "" : ob.CollectionStudyDay.Number.ToString(),
                    Timepoint = ob.CollectionStudyTimePoint == null ? "" : ob.CollectionStudyTimePoint.Name
                });
            #endregion

            foreach (var subjVisitTPT in findingsBySubjectVisitTime)
            {
                var row = datatable.NewRow();
                foreach (var subjObs in subjVisitTPT)//adding columns to the same row each iteration is a different observation but within each iteration
                //we could be adding more than one column per observation if requested more than one qaulifier
                {
                    row["subjectId"] = subjObs.USubjId;
                    row["study"] = subjObs.StudyId;
                    foreach (var obsreq in reqObservations.Where(r => r.O3id == subjObs.DBTopicId))
                        //TODO: assumption to be validated that visits/timepoints are study and dont differ between observations or studies of the same project
                        row[obsreq.Name] = subjObs.ResultQualifiers[obsreq.QO2];

                    row["visit"] = subjVisitTPT.Key.Visit;//subjObs.VisitName;
                    row["timepoint"] = subjVisitTPT.Key.Timepoint;//subjObs.ObsStudyTimePoint == null ? "" : subjObs.ObsStudyTimePoint.Name;
                }
                datatable.Rows.Add(row);
            }

            return datatable;
        }

        private DataTable getEventsDataTable(List<SdtmRow> events, IList<ObservationRequestDTO> reqObservations)
        {
            if (!events.Any())
                return new DataTable();

            #region Build Table columns
            var datatable = new DataTable();
            datatable.Columns.Add("subjectId");
            datatable.Columns.Add("study");

            //Assuming here that query for Adverse Events is always going to be a group query by PT (AEDECOD) and NOT by AETERM
            //CAUTION when there is another domain in Events that is NOT AE
            //foreach (var r in reqObservations.Where(r => events.Select(f => f.DBTopicId).Contains(r.O3id)))
            //datatable.Columns.Add(r.Id);

            foreach (var r2 in reqObservations.Where(r => events.Select(f => f.DBTopicId).Any(i => r.TermIds.Contains(i))))
                datatable.Columns.Add(r2.Name);

            #endregion

            #region HACK FOR AEOCCUR
            var projectId = events.First().ProjectId;
            var allSubjects = _subjectRepository.FindAll(s => s.Study.ProjectId == projectId, new List<string>() { "Study" }).ToList();
            var appendedEvents = events.ToList();
            foreach (var obsreq in reqObservations.Where(r => r.QO2 == "AEOCCUR"))
            {
                // var matchedEvents = events.FindAll(s => obsreq.TermIds.Contains(s.DBTopicId) && !s.Qualifiers.ContainsKey("AEOCCUR")).ToList();
                var matchedEvents = events.FindAll(s => obsreq.TermIds.Contains(s.DBTopicId)).ToList();

                //var matchedEvents2 = events.FindAll(s => s.QualifierQualifiers[obsreq.OntologyEntryCategoryName] == obsreq.OntologyEntryValue && !s.Qualifiers.ContainsKey("AEOCCUR")).ToList();
                //var matchedEvents = events.FindAll(s => s.QualifierQualifiers[obsreq.OntologyEntryCategoryName] == obsreq.OntologyEntryValue && s.Group == obsreq?.Group).ToList();

                if (!matchedEvents.Any()) continue;
                matchedEvents.FindAll(c => !c.Qualifiers.ContainsKey("AEOCCUR")).ForEach(m => m.Qualifiers.Add("AEOCCUR", "Y"));

                var subjectIdsEventOccured = matchedEvents.Select(e => e.USubjId).Distinct().ToList();
                var subjectsNoOccur = allSubjects.FindAll(s => !subjectIdsEventOccured.Contains(s.UniqueSubjectId));
                var noOccurEvents = new List<SdtmRow>();
                foreach (var termId in obsreq.TermIds)
                {
                    noOccurEvents.AddRange(subjectsNoOccur.Select(subj => new SdtmRow()
                    {
                        USubjId = subj.UniqueSubjectId,
                        StudyId = subj.Study.Name,
                        Topic = obsreq.O3,
                        DBTopicId = termId,//obsreq.TermIds[0],
                        Id = Guid.NewGuid(),
                        ActivityId = matchedEvents.First().ActivityId,
                        DatasetId = matchedEvents.First().DatasetId,
                        DatafileId = matchedEvents.First().DatafileId,
                        ProjectId = matchedEvents.First().ProjectId,
                        ProjectAccession = matchedEvents.First().ProjectAccession,
                        Class = matchedEvents.First().Class,
                        Group = matchedEvents.First().Group,
                        TopicControlledTerm = matchedEvents.First().TopicControlledTerm,
                        Qualifiers = new Dictionary<string, string>() { { "AEOCCUR", "N" } },
                        QualifierQualifiers = matchedEvents.First().QualifierQualifiers
                    }).ToList());
                }
                appendedEvents.AddRange(noOccurEvents);


                foreach (var row in matchedEvents)
                {
                    _sdtmRepository.Update(row);

                }
                foreach (var row in noOccurEvents)
                {
                    _sdtmRepository.Insert(row);
                }
            }

            #endregion

            //Group observations by subjectId
            var subjGroupedEvents = appendedEvents.GroupBy(ob => new { subjId = ob.USubjId });
            //Group observation requests
            var reqByO3Id = reqObservations.GroupBy(r => r.O3).ToList();

            foreach (var eventsGroup in subjGroupedEvents)
            {
                var subjectEvents = eventsGroup.Select(group => group).ToList();
                while (subjectEvents.Any())
                {
                    var row = datatable.NewRow();
                    foreach (var reqForO3 in reqByO3Id)//HEADACHE 
                    {
                        SdtmRow ev = null;
                        foreach (var obsreq in reqForO3) //AEOCCUR / AESEV
                        {
                            ev = subjectEvents.FirstOrDefault(e => obsreq.TermIds.Contains(e.DBTopicId));
                            string val = "";
                            ev?.Qualifiers.TryGetValue(obsreq.QO2, out val);
                            row[obsreq.Name] = val;
                            if (ev != null) row["subjectId"] = ev.USubjId;
                            if (ev != null) row["study"] = ev.StudyId;
                        }
                        subjectEvents.Remove(ev);
                    }
                    datatable.Rows.Add(row);
                }
            }
            datatable.Rows = datatable.Rows.OrderBy(row => row["subjectId"]).ToList();
            return datatable;
        }

        private ObservationNode createObsNode(Observation obsObject)
        {
            var node = new ObservationNode
            {
                Name = obsObject.ControlledTermStr,
                Id = obsObject.Id.ToString(),
                Code = obsObject.Name.ToLower(),
                Qualifiers = createObsRequests(obsObject),

                DefaultObservation = new ObservationRequestDTO()
                {
                    O3 = obsObject.ControlledTermStr,
                    O3id = obsObject.Id,
                    O3code = obsObject.Name.ToLower(),
                    QO2 = obsObject.DefaultQualifier.Name,
                    QO2id = obsObject.DefaultQualifier.Id,
                    DataType = obsObject.DefaultQualifier.DataType,
                    QO2_label = obsObject.DefaultQualifier.Label,
                    IsEvent = obsObject.Class.ToUpper() == "EVENTS",
                    IsFinding = obsObject.Class.ToUpper() == "FINDINGS",
                    IsClinicalObservations = true,
                    ProjectId = obsObject.ProjectId.Value
                }
            };
            if (obsObject.ControlledTermStr != null) return node;
            node.Name = obsObject.Name;
            node.DefaultObservation.O3 = obsObject.Name;
            return node;

        }

        private List<ObservationRequestDTO> createObsRequests(Observation obsObject, bool IsMultiple = false, bool IsOntologyEntry = false)
        {
            var allQualifiers = obsObject.Qualifiers.Select(q => q.Qualifier).ToList();
            allQualifiers.AddRange(obsObject.Timings.Select(a => a.Qualifier));

            var reqs = allQualifiers.Select(variableDefinition => new ObservationRequestDTO()
            {

                O3 = obsObject.Name,//obsObject.ControlledTermStr,
                O3id = obsObject.Id,
                O3code = obsObject.Name.ToLower(),
                QO2 = variableDefinition.Name,
                QO2id = variableDefinition.Id,
                DataType = variableDefinition.DataType,
                QO2_label = variableDefinition.Label,
                IsEvent = obsObject.Class.ToUpper() == "EVENTS",
                IsFinding = obsObject.Class.ToUpper() == "FINDINGS",
                IsMultipleObservations = IsMultiple,
                IsOntologyEntry = IsOntologyEntry,
                ProjectId = obsObject.ProjectId.Value,

                //TermIds = termIds,
                //Id = obsObject.Name.ToLower() + "_" + variableDefinition.Name
                //Qualifiers = obsObject.Qualifiers.Select(q => q.Name).ToList(),
                //DefaultQualifier = obsObject.DefaultQualifier.Name
            }).ToList();
            return reqs;
        }

        private async Task<List<MedDRAGroupNode>> getEventsByMedDRA(int projectId, List<Observation> observations, string gcode, string gname)
        {
            List<SdtmRow> adverseEvents =
                   await _sdtmRepository.FindAllAsync(d => d.DomainCode.Equals("AE") && d.ProjectId == projectId
                   );

            //if(group != null)
            var adverseEvents2 = adverseEvents.Where(s => observations.Select(o => o.Group).Contains(s.Group));

            List<MedDRAGroupNode> AEsByMedDRA = (
                from ae in adverseEvents2
                group ae by ae.QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AESOC")).Value into SOCs
                select new MedDRAGroupNode
                {
                    Code = gcode + "_" + SOCs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AESOCCD")).Value,
                    Name = "SO: " + SOCs.Key.ToLowerInvariant(),
                    Id = SOCs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AESOCCD")).Value,
                    Count = SOCs.Select(so => so.TopicControlledTerm).Distinct().Count(),
                    //,
                    DefaultObservation = new ObservationRequestDTO()
                    {
                        DataType = "string",
                        O3 = SOCs.Key,
                        O3code = SOCs.Key.ToLower(),//SOCs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AESOCCD")).Value,
                        O3id = int.Parse(SOCs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AESOCCD")).Value),
                        O3variable = "AESOC",
                        Group = gname,
                        ProjectId = projectId,
                        TermIds = SOCs.Select(s => s.DBTopicId).Distinct().ToList(),
                        IsEvent = true,
                        IsClinicalObservations = true,
                        IsOntologyEntry = true,
                        OntologyEntryCategoryName = "AESOCCD",
                        OntologyEntryValue = SOCs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AESOCCD")).Value
                    },
                    //Qualifiers = getObsRequests(observations.FirstOrDefault(o => o.Id == SOCs.FirstOrDefault().DBTopicId), false, true),
                    //SOCs.Select(s => s.DBTopicId).Distinct().ToList().Count,// SOCs.Select(so => so.TopicControlledTerm).Distinct().Count(),
                    Terms = (
                        from ae in SOCs
                        group ae by ae.QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEHLGT")).Value into HLGTs
                        select new MedDRAGroupNode
                        {
                            Code = gcode + "_" + HLGTs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEHLGTCD")).Value,
                            Name = "HLG: " + HLGTs.Key.ToLower(),
                            Count = HLGTs.Select(so => so.TopicControlledTerm).Distinct().Count(),//HLGTs.Select(s => s.DBTopicId).Distinct().ToList().Count(),
                            Id = HLGTs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEHLGTCD")).Value,
                            //Group = gname,
                            DefaultObservation = new ObservationRequestDTO()
                            {
                                DataType = "string",
                                O3 = HLGTs.Key,
                                O3code = HLGTs.Key.ToLower(),//HLGTs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEHLGTCD")).Value,
                                O3id = int.Parse(HLGTs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEHLGTCD")).Value),
                                O3variable = "AEHLGT",
                                Group = gname,
                                ProjectId = projectId,
                                TermIds = HLGTs.Select(s => s.DBTopicId).Distinct().ToList(),
                                IsEvent = true,
                                IsClinicalObservations = true,
                                IsOntologyEntry = true,
                                OntologyEntryCategoryName = "AEHLGTCD",
                                OntologyEntryValue = HLGTs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEHLGTCD")).Value
                            },
                            //Qualifiers = getObsRequests(observations.FirstOrDefault(o => o.Id == HLGTs.FirstOrDefault().DBTopicId), false, true),
                            Terms = (
                                from ae in HLGTs
                                group ae by ae.QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEHLT")).Value into HLTs
                                select new MedDRAGroupNode()
                                {
                                    Code = gcode + "_" + HLTs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEHLTCD")).Value,
                                    Name = "HLT: " + HLTs.Key.ToLower(),
                                    Count = HLTs.Select(so => so.TopicControlledTerm).Distinct().Count(),//HLTs.Select(s => s.DBTopicId).Distinct().ToList().Count(),
                                    Id = HLTs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEHLTCD")).Value,
                                    //Group = gname,
                                    DefaultObservation = new ObservationRequestDTO()
                                    {
                                        DataType = "string",
                                        O3 = HLTs.Key,
                                        O3code = HLTs.Key.ToLower(),//HLTs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEHLTCD")).Value,
                                        O3id = int.Parse(HLTs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEHLTCD")).Value),
                                        O3variable = "AEHLTCD",
                                        Group = gname,
                                        ProjectId = projectId,
                                        TermIds = HLTs.Select(s => s.DBTopicId).Distinct().ToList(),
                                        IsEvent = true,
                                        IsClinicalObservations = true,
                                        IsOntologyEntry = true,
                                        OntologyEntryCategoryName = "AEHLTCD",
                                        OntologyEntryValue = HLTs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEHLTCD")).Value
                                    },
                                    //Qualifiers = getObsRequests(observations.FirstOrDefault(o => o.Id == HLTs.FirstOrDefault().DBTopicId), false, true),
                                    Terms = (
                                        from ae in HLTs
                                        group ae by ae.TopicControlledTerm into PTs //ae.qualifiers.SingleOrDefault(q => q.Key.Equals("AEDECOD")).Value into PTs
                                        select createMedDRATermNode(PTs, "AEPTCD", gcode, gname)).ToList<GenericNode>()
                                    //select new ObservationNode()
                                    //{
                                    //    Id = PTs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEPTCD")).Value,
                                    //    Code = gcode + "_" + PTs.Key,
                                    //    Name = PTs.Key.ToLower(),
                                    //    DefaultObservation = new ObservationRequestDTO()
                                    //    {
                                    //        O3 = observations.FirstOrDefault(o => o.ControlledTermStr.Equals(PTs.Key)).ControlledTermStr,
                                    //        O3id = Int32.Parse(PTs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEPTCD")).Value),
                                    //        O3code = observations.FirstOrDefault(o => o.ControlledTermStr.Equals(PTs.Key)).ControlledTermStr.ToLower(),
                                    //        DataType = "string",
                                    //        QO2 = "AEOCCUR",
                                    //        QO2_label = "OCCURENCE",

                                    //        TermIds = PTs.Select(p => p.DBTopicId).Distinct().ToList(),
                                    //        IsEvent = true,
                                    //        IsClinicalObservations = true,
                                    //        IsOntologyEntry = true,
                                    //        OntologyEntryCategoryName = "AEPTCD",
                                    //        OntologyEntryValue = PTs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEPTCD")).Value
                                    //    },
                                    //    Qualifiers = getObsRequests(observations.FirstOrDefault(o => o.ControlledTermStr.Equals(PTs.Key)), false, true),

                                    //}).ToList<GenericNode>()
                                }).ToList<GenericNode>()
                        }).ToList<GenericNode>()
                }).ToList();


            return AEsByMedDRA;

        }

        private ObservationNode createMedDRATermNode(IGrouping<string, SdtmRow> PTs, string oeCategory, string gcode, string gname)
        {

            var obsId = PTs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals(oeCategory)).Value;
            var node = new ObservationNode()
            {
                Id = obsId,
                Code = gcode + "_" + PTs.Key,
                Name = PTs.Key.ToLower(),
                DefaultObservation = new ObservationRequestDTO()
                {
                    O3 = PTs.Key,
                    O3id = int.Parse(obsId),
                    O3code = PTs.Key.ToLower(),

                    DataType = "string",
                    QO2 = "AEOCCUR",
                    QO2_label = "OCCURENCE",

                    Group = gname,
                    //TermIds = PTs.Select(p => p.DBTopicId).Distinct().ToList(),
                    IsEvent = true,
                    IsClinicalObservations = true,
                    IsOntologyEntry = true,
                    OntologyEntryCategoryName = oeCategory,
                    OntologyEntryValue = obsId,
                    ProjectId = PTs.FirstOrDefault().ProjectId

                }

            };

            var observations = PTs.Select(g => g).ToList();
            var observationIds = observations.Select(o => o.DBTopicId).ToList();

            var O3s = _observationRepository.FindAll(o => observationIds.Contains(o.Id),
                    new List<string>()
                    {
                        "DefaultQualifier",
                        "Qualifiers.Qualifier",
                        "Timings.Qualifier"
                    }).ToList();

            //temp for now just do it for thefrom te first o3
            var qualifiers = O3s.FirstOrDefault().Qualifiers.Select(q => q.Qualifier).ToList();
            qualifiers.AddRange(O3s.FirstOrDefault().Timings.Select(q => q.Qualifier));

            var obsRequests = qualifiers.Select(qualifier => new ObservationRequestDTO()
            {
                O3 = node.DefaultObservation.O3,
                O3id = node.DefaultObservation.O3id,
                O3code = node.DefaultObservation.O3code,
                QO2 = qualifier.Name,
                QO2id = qualifier.Id,
                DataType = qualifier.DataType,
                QO2_label = qualifier.Label,
                Group = gname,
                IsEvent = node.DefaultObservation.IsEvent,
                IsFinding = node.DefaultObservation.IsFinding,
                IsClinicalObservations = node.DefaultObservation.IsClinicalObservations,
                IsOntologyEntry = node.DefaultObservation.IsOntologyEntry,
                OntologyEntryCategoryName = oeCategory,
                OntologyEntryValue = node.DefaultObservation.O3id.ToString(),
                ProjectId = node.DefaultObservation.ProjectId,
                TermIds = PTs.Select(p => p.DBTopicId).Distinct().ToList(),
            }).ToList();

            node.Qualifiers = obsRequests;
            return node;
        }

        private MedDRAGroupNode createMedDRAGroupNode()
        {
            var node = new MedDRAGroupNode();
            return node;
        }
        #endregion

        #region Assay Browsing methods

        #endregion


    }
}
