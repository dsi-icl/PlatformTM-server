using eTRIKS.Commons.Core.Domain.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.DTOs;
using System.Linq.Expressions;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel.SDTM;
using eTRIKS.Commons.Core.Domain.Model.Templates;

namespace eTRIKS.Commons.Service.Services
{
    
    public class DataExplorerService
    {
        private readonly IRepository<Observation, int> _observationRepository;
        private readonly IRepository<HumanSubject, string> _subjectRepository;
        private readonly IRepository<CharacteristicObject, int> _characObjRepository;
        private readonly IRepository<SdtmRow, Guid> _sdtmRepository;
        private readonly IRepository<Assay, int> _assayRepository;


        private readonly IServiceUoW _dataContext;
        public DataExplorerService(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _observationRepository = uoW.GetRepository<Observation, int>();
            _subjectRepository = uoW.GetRepository<HumanSubject, string>();
            _characObjRepository = uoW.GetRepository<CharacteristicObject, int>();
            _sdtmRepository = uoW.GetRepository<SdtmRow, Guid>();
            _assayRepository = uoW.GetRepository<Assay, int>();


        }

        #region Observation Browser methods
        public List<ObservationRequestDTO> GetSubjectCharacteristics(int projectId)
        {
            var subjChars = new List<ObservationRequestDTO>();
            var SCs = _characObjRepository.FindAll(sco=>sco.ProjectId == projectId).ToList();

            subjChars = SCs.Select(sc => new ObservationRequestDTO()
            {
                O3 = sc.FullName,
                O3id = sc.Id,
                O3code = sc.ShortName,
                DataType = sc.DataType
            }).ToList();

            var armSC = new ObservationRequestDTO() {O3 = "Arm", O3code = "ARM", O3id = 999, DataType = "string"};
            var studySC = new ObservationRequestDTO() { O3 = "Study", O3code = "STUDY", O3id = 888, DataType = "string"};
            subjChars.Add(armSC);
            subjChars.Add(studySC);

            return subjChars.OrderBy(o => o.O3).ToList();
        }

        public ObservationNode GroupObservations(int projectId, List<ObservationRequestDTO> observations)
        {
            var obsRequest = new ObservationRequestDTO();
            obsRequest.DataType = "string";
            obsRequest.IsEvent = true;
            obsRequest.IsFinding = false;
            obsRequest.IsMultipleObservations = true;
            obsRequest.O3code = "grp_";
            obsRequest.QO2 = "AEOCCUR";
            obsRequest.QO2_label = "OCCURENCE";
            obsRequest.TermIds = new List<int>();
            
            for (int i=0; i<observations.Count;i++)
            {
                obsRequest.O3 += observations[i].O3 + (i + 1 < observations.Count ? "," : "");
                obsRequest.TermIds.AddRange(observations[i].TermIds);
                obsRequest.O3code += observations[i].O3variable + "(" + observations[i].O3id + ")" + (i + 1 < observations.Count ? "_" : "");
                obsRequest.O3id += observations[i].O3id;
            }
            int fobsid = obsRequest.TermIds[0];
            var observation = _observationRepository.FindSingle(o => o.Id == fobsid, 
                new List<string>() {
                    "Qualifiers.Qualifier"
                });
            var reqs = observation.Qualifiers.Select(variableDefinition => new ObservationRequestDTO()
            {

                O3 = obsRequest.O3,//obsObject.ControlledTermStr,
                O3id = obsRequest.O3id,
                O3code = obsRequest.O3code,
                IsEvent = obsRequest.IsEvent,
                IsFinding = obsRequest.IsFinding,
                IsMultipleObservations = obsRequest.IsMultipleObservations,
                TermIds = obsRequest.TermIds,

                QO2 = variableDefinition.Qualifier.Name,
                QO2id = variableDefinition.Qualifier.Id,
                DataType = variableDefinition.Qualifier.DataType,
                QO2_label = variableDefinition.Qualifier.Label,
            }).ToList();

            return new ObservationNode() {DefaultObservation=obsRequest,Qualifiers = reqs, Id=int.Parse(obsRequest.Id), Name=obsRequest.Name };
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
                var groupedByDomain = classGroup.GroupBy(x => new {domainCode = x.DomainCode,domainName = x.DomainName });
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
                        if(domainNode.Code == "AE" && projectId == 25)
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
        #endregion

        #region Crossfilter data methods
        public Hashtable GetObservationsData(int projectId, List<ObservationRequestDTO> reqObservations)
        {
            List<int> observationsIDs = reqObservations.Where(r=>r.IsMultipleObservations==false).Select(o => o.O3id).ToList();

            List<int> groupedObservationsIDs = reqObservations.Where(r => r.IsMultipleObservations == true).SelectMany(o => o.TermIds).ToList();

            //Retrieve rows for requested individual observations
            List<SdtmRow> sdtmObservations = _sdtmRepository.FindAll(s => observationsIDs.Contains(s.DBTopicId)).ToList();

            //Retrieve rows for requested group observations
            List<SdtmRow> sdtmObservations2 = _sdtmRepository.FindAll(s => groupedObservationsIDs.Contains(s.DBTopicId)).ToList();

            sdtmObservations = sdtmObservations.Union(sdtmObservations2).ToList();

            //&& s.Qualifiers?[s.DomainCode + "STAT"] ? == ""
            var subjFindings = sdtmObservations.FindAll(s => s.Class.ToLower() == "findings" ).ToList();
            var subjEvents = sdtmObservations.FindAll(s => s.Class.ToLower() == "events").ToList();

            var findingsTable = getFindingsDataTable(subjFindings, reqObservations.Where(r=>r.IsFinding==true));
            var eventsTable = getEventsDataTable(subjEvents, reqObservations.Where(r=>r.IsEvent==true));
           
            var findingsColsList = (from dc in findingsTable.Columns.Cast<DataColumn>()
                                    select dc.ColumnName).ToList();
            var eventsColsList = (from dc in eventsTable.Columns.Cast<DataColumn>()
                                  select dc.ColumnName).ToList();
            var result = new Hashtable
            {
                {"findingsTbl", findingsTable.Rows},
                {"eventsTbl", eventsTable.Rows},
                {"findingsTblHeader", findingsColsList},
                {"eventsTblHeader", eventsColsList}
            };
            return result;
        }
        public Hashtable GetSubjectData(int projectId, List<ObservationRequestDTO> reqObservations)
        {
            List<HumanSubject> subjects = _subjectRepository.FindAll(
                s => s.Study.ProjectId == projectId,
                new List<string>()
                {   "SubjectCharacteristics.CharacteristicObject",
                    "Study"
                }).ToList();


            List<Hashtable> subject_table = new List<Hashtable>();
            HashSet<string> SCs = new HashSet<string>(); //{ "arm", "siteid", "study" };


            foreach (HumanSubject subject in subjects)
            {
                Hashtable ht = new Hashtable();
                ht.Add("subjectId", subject.UniqueSubjectId);
                
                //if (subject.SubjectCharacteristics.SingleOrDefault(sc => sc.CharacteristicObject.ShortName.ToUpper().Equals("SEX")) != null)
                //{
                //    SCs.Add("sex"); ht.Add("sex", subject.SubjectCharacteristics.SingleOrDefault(sc => sc.CharacteristicObject.ShortName.ToUpper().Equals("SEX")).VerbatimValue);
                //}
                //if (subject.SubjectCharacteristics.SingleOrDefault(sc=> sc.CharacteristicObject.ShortName.ToUpper().Equals("AGE")) != null)
                //{
                //    SCs.Add("age"); ht.Add("age", subject.SubjectCharacteristics.SingleOrDefault(sc => sc.CharacteristicObject.ShortName.ToUpper().Equals("AGE")).VerbatimValue);
                //}
                //if (subject.SubjectCharacteristics.SingleOrDefault(sc => sc.CharacteristicObject.ShortName.ToUpper().Equals("SITEID")) != null)
                //{
                //    SCs.Add("siteid"); ht.Add("siteid", subject.SubjectCharacteristics.SingleOrDefault(sc => sc.CharacteristicObject.ShortName.ToUpper().Equals("SITEID")).VerbatimValue);
                //}
                if (subject.Study.Name != null)
                {
                    SCs.Add("study"); ht.Add("study", subject.Study.Name);
                }
                if (subject.ArmCode != null)
                {
                    SCs.Add("arm"); ht.Add("arm", subject.Arm);
                }


                if (reqObservations != null)
                    foreach (var requestDto in reqObservations)
                    {
                        var charVal = subject.SubjectCharacteristics.SingleOrDefault(sc => sc.CharacteristicObjectId.Equals(requestDto.O3id));
                        if (charVal == null) continue;
                        ht.Add(requestDto.O3code, charVal.VerbatimValue);
                        SCs.Add(requestDto.O3code.ToLower());
                    }

                subject_table.Add(ht);
            }

            Hashtable returnObject = new Hashtable();
            returnObject.Add("header", SCs);
            returnObject.Add("data", subject_table);

            return returnObject;
        }
        #endregion

        #region Private helper methods
        private DataTable getFindingsDataTable(IEnumerable<SdtmRow> findings, IEnumerable<ObservationRequestDTO> reqObservations)
        {
            #region Build Table columns
            var datatable = new DataTable();
            datatable.Columns.Add("subjectId");
            datatable.Columns.Add("study");
            foreach (var r in reqObservations.Where(r => findings.Select(f=>f.DBTopicId).Contains(r.O3id)))
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
        private DataTable getEventsDataTable(IEnumerable<SdtmRow> events, IEnumerable<ObservationRequestDTO> reqObservations)
        {

            if (events.Count() == 0)
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

            var eventsBySubjectEventId = events
                .GroupBy(ob => new
                {
                    subjId = ob.USubjId,
                    eventId = ob.DBTopicId
                    //studyDay = ob.CollectionStudyDay == null ? "" : ob.CollectionStudyDay.Number.ToString(),
                    //startDay = ob.StudyDayInterval?.Start?.Number.ToString() ?? "",
                    //endDay = ob.StudyDayInterval?.End?.Number.ToString() ?? ""
                }).ToList();
            var projectId = events.First().ProjectId;
            var allSubjects = _subjectRepository.FindAll(s => s.Study.ProjectId == projectId, 
                new List<string>()
                {
                   "Study"
                }).ToList();

            //HACK FOR AEOCCUR
            //var presentSubjs = eventsBySubject.Select(s => s.Key.subjId).ToList();
            //var missingSubjs = allSubjects.FindAll(s => !presentSubjs.Contains(s.UniqueSubjectId));

            var appendedEvents = events.ToList();
            foreach (var obsreq in reqObservations.Where(r => r.QO2 == "AEOCCUR"))
            {

                var presentSubjects = eventsBySubjectEventId.Where((s => obsreq.TermIds.Contains(s.Key.eventId))).Select(e=>e.Key.subjId);
                var missingSubjects = allSubjects.FindAll(s => !presentSubjects.Contains(s.UniqueSubjectId));

                //foreach(var subjevent in events)
                //{
                //    if(subjevent.USubjId)
                //}
                //foreach (var subj in presentSubjects)
                foreach (var subj in missingSubjects)
                {
                    
                        var sdtmrow = new SdtmRow()
                        {
                            USubjId = subj.UniqueSubjectId,
                            StudyId = subj.Study.Name,
                            DBTopicId = obsreq.TermIds[0]
                            //Qualifiers = new Dictionary<string, string>() { "AEOCCUR","Y"}
                        };

                    appendedEvents.Add(sdtmrow);
                }
            }

             var eventsBySubject = appendedEvents
                .GroupBy(ob => new
                {
                    subjId = ob.USubjId,
                    //studyDay = ob.CollectionStudyDay == null ? "" : ob.CollectionStudyDay.Number.ToString(),
                    //startDay = ob.StudyDayInterval?.Start?.Number.ToString() ?? "",
                    //endDay = ob.StudyDayInterval?.End?.Number.ToString() ?? ""
                });
            //TODO: if default qualifier used dont include square brackets
            //TODO: assumption to be validated that visits/timepoints are study and dont differ between observations or studies of the same project

            foreach (var eventSubj in eventsBySubject)
            {
                var row = datatable.NewRow();
                foreach (var subjObs in eventSubj)
                    //adding columns to the same row each iteration is a different observation but within each iteration
                    //we could be adding more than one column per observation if requested more than one qaulifier
                {
                    row["subjectId"] = subjObs.USubjId;
                    row["study"] = subjObs.StudyId;


                    //Adding single observations 
                    foreach (var obsreq in reqObservations.Where(r => !r.IsMultipleObservations &&  r.O3id == subjObs.DBTopicId  ))
                    {
                        //TODO: assumption to be validated that visits/timepoints are study and dont differ between observations or studies of the same project
                        string val = "";
                        subjObs.Qualifiers.TryGetValue(obsreq.QO2, out val);
                        row[obsreq.Name] = val;// subjObs.Qualifiers.tr?[obsreq.QO2];
                    }

                    //Adding group observations
                    foreach (var obsreq in reqObservations.Where(r => r.IsMultipleObservations && r.TermIds.Contains(subjObs.DBTopicId)))
                    {

                        if (obsreq.QO2 == "AEOCCUR")
                        {
                            string p = "";
                            if(subjObs.Qualifiers.TryGetValue("AESEV", out p))

                                row[obsreq.Name] = "Y";// subjObs.Qualifiers.tr?[obsreq.QO2];
                            else
                                row[obsreq.Name] = "N";
                        }
                        else
                        {
                            //TODO: assumption to be validated that visits/timepoints are study and dont differ between observations or studies of the same project
                            string val = "";
                            subjObs.Qualifiers.TryGetValue(obsreq.QO2, out val);

                            row[obsreq.Name] = val;// subjObs.Qualifiers.tr?[obsreq.QO2];
                        }
                        
                    }

                    //if (subjObs.ObsInterval!=null && subjObs.ObsInterval.Start!=null)
                    //row["start date"] = eventSubjDay.Key.startDay;// "";//subjObs.ObsInterval.Start.Name;
                    //row["end date"] = eventSubjDay.Key.endDay;//"";//subjObs.ObsInterval.End;
                    //row["study day"] = eventSubjDay.Key.studyDay;//subjObs.ObsStudyDay == null ? "" : subjObs.ObsStudyDay.Name;
                }
                datatable.Rows.Add(row);
            }

            return datatable;
        }
        private ObservationNode createObsNode(Observation obsObject)
        {
            var node = new ObservationNode
            {
                Name = obsObject.ControlledTermStr,
                Id = obsObject.Id,
                Code = obsObject.Name.ToLower(),
                Qualifiers = getObsRequests(obsObject),
                //Qualifiers = obsObject.Qualifiers.Select(q => q.Name).ToList(),
                //DefaultQualifier = obsObject.DefaultQualifier.Name
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
                    IsFinding = obsObject.Class.ToUpper() == "FINDINGS"
                    //Id = obsObject.Name.ToLower() + "_" + obsObject.DefaultQualifier.Name
                }
            };
            if (obsObject.ControlledTermStr == null)
            {
                node.Name = obsObject.Name;
                node.DefaultObservation.O3 = obsObject.Name;
            }
            return node;

        }
        private async Task<List<MedDRAGroupNode>> getEventsByMedDRA(int projectId, List<Observation> observations, string gcode, string gname)
        {
            List<SdtmRow> adverseEvents =
                   await _sdtmRepository.FindAllAsync(d => d.DomainCode.Equals("AE") && d.ProjectId == projectId
                   );
            //&& d.ProjectId==projectId

            //if(group != null)
            var adverseEvents2 = adverseEvents.Where(s => observations.Select(o => o.Group).Contains(s.Group));

            //TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;


            //title = textInfo.ToTitleCase(title); //War And Peace
            //var AEsByMedDRA = getAEsByMedDRA(adverseEvents);
            //GroupNode obsCatGrp = new GroupNode();
            List<MedDRAGroupNode> AEsByMedDRA = (
                from ae in adverseEvents2
                    // where ae.qualifier.name = AESOC
                group ae by ae.QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AESOC")).Value into SOCs
                select new MedDRAGroupNode
                {
                    Variable = "AESOC",
                    Code = gcode + "_" + SOCs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AESOCCD")).Value,
                    Name = "SO: " + SOCs.Key.ToLowerInvariant(),
                    GroupTerm = SOCs.Key.ToLower(),
                    Id = int.Parse(SOCs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AESOCCD")).Value),
                    TermIds = SOCs.Select(s => s.DBTopicId).Distinct().ToList(),
                    TermNames = SOCs.Select(s=>s.TopicControlledTerm).Distinct().ToList(),
                    Count = SOCs.Select(so => so.TopicControlledTerm).Distinct().Count(),
                    Group = gname,
                    DefaultObservation = new ObservationRequestDTO()
                    {
                        DataType = "string",
                        IsMultipleObservations = true,
                        O3 = SOCs.Key,
                        O3code = SOCs.Key.ToLower(),//SOCs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AESOCCD")).Value,
                        O3id = int.Parse(SOCs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AESOCCD")).Value),
                        O3variable = "AESOC",
                        QO2 = "AEOCCUR",
                        QO2_label = "OCCURENCE",
                        TermIds = SOCs.Select(s => s.DBTopicId).Distinct().ToList(),
                        IsEvent = true


                    },
                    Qualifiers = getObsRequests(observations.FirstOrDefault(o => o.Id == SOCs.FirstOrDefault().DBTopicId), true, SOCs.Select(p => p.DBTopicId).Distinct().ToList()),
                    //SOCs.Select(s => s.DBTopicId).Distinct().ToList().Count,// SOCs.Select(so => so.TopicControlledTerm).Distinct().Count(),
                    Terms = (
                        from ae in SOCs
                        group ae by ae.QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEHLGT")).Value into HLGTs
                        select new MedDRAGroupNode
                        {
                            Variable = "AEHLGT",
                            Code = gcode + "_" + HLGTs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEHLGTCD")).Value,
                            GroupTerm = HLGTs.Key.ToLower(),
                            Name = "HLG: " + HLGTs.Key.ToLower(),
                            Count = HLGTs.Select(so => so.TopicControlledTerm).Distinct().Count(),//HLGTs.Select(s => s.DBTopicId).Distinct().ToList().Count(),
                            TermIds = HLGTs.Select(s => s.DBTopicId).Distinct().ToList(),
                            TermNames = HLGTs.Select(s => s.TopicControlledTerm).Distinct().ToList(),
                            Id = int.Parse(HLGTs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEHLGTCD")).Value),
                            Group = gname,
                            DefaultObservation = new ObservationRequestDTO()
                            {
                                DataType = "string",
                                IsMultipleObservations = true,
                                O3 = HLGTs.Key,
                                O3code = HLGTs.Key.ToLower(),//HLGTs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEHLGTCD")).Value,
                                O3id = int.Parse(HLGTs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEHLGTCD")).Value),
                                O3variable = "AEHLGT",
                                QO2 = "AESEV",
                                QO2_label = "Severity",
                                TermIds = HLGTs.Select(s => s.DBTopicId).Distinct().ToList(),
                                IsEvent = true

                            },
                            Qualifiers = getObsRequests(observations.FirstOrDefault(o => o.Id == HLGTs.FirstOrDefault().DBTopicId), true, HLGTs.Select(p => p.DBTopicId).Distinct().ToList()),
                            //TermIds = (from ae in HLGTs
                            //           select observations.Find(o => o.ControlledTermStr.ToLower().Equals(ae.Name.ToLower())).Id).ToList<int>(),
                            Terms = (
                                from ae in HLGTs
                                group ae by ae.QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEHLT")).Value into HLTs
                                select new MedDRAGroupNode()
                                {
                                    Variable = "AEHLT",
                                    Code = gcode + "_" + HLTs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEHLTCD")).Value,
                                    GroupTerm = HLTs.Key.ToLower(),
                                    Name = "HLT: " + HLTs.Key.ToLower(),
                                    Count = HLTs.Select(so => so.TopicControlledTerm).Distinct().Count(),//HLTs.Select(s => s.DBTopicId).Distinct().ToList().Count(),
                                    TermIds = HLTs.Select(s => s.DBTopicId).Distinct().ToList(),
                                    TermNames = HLTs.Select(s => s.TopicControlledTerm).Distinct().ToList(),
                                    Id = int.Parse(HLTs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEHLTCD")).Value),
                                    Group = gname,
                                    DefaultObservation = new ObservationRequestDTO()
                                    {
                                        DataType = "string",
                                        IsMultipleObservations = true,
                                        O3 = HLTs.Key,
                                        O3code = HLTs.Key.ToLower(),//HLTs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEHLTCD")).Value,
                                        O3id = int.Parse(HLTs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEHLTCD")).Value),
                                        O3variable = "AEHLTCD",
                                        QO2 = "AESEV",
                                        QO2_label = "Severity",
                                        TermIds = HLTs.Select(s => s.DBTopicId).Distinct().ToList(),
                                        IsEvent = true

                                    },
                                    Qualifiers = getObsRequests(observations.FirstOrDefault(o => o.Id == HLTs.FirstOrDefault().DBTopicId), true, HLTs.Select(p => p.DBTopicId).Distinct().ToList()),
                                    Terms = (
                                        from ae in HLTs
                                        group ae by ae.TopicControlledTerm into PTs //ae.qualifiers.SingleOrDefault(q => q.Key.Equals("AEDECOD")).Value into PTs
                                        select new MedDRATermNode()
                                        {
                                            Variable = "AEDECOD",
                                            //Id = observations.FirstOrDefault(o => o.ControlledTermStr.Equals(PTs.Key)).Id, /// this is the first AETERM id in terms having same PTterm
                                            Id = Int32.Parse(PTs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEPTCD")).Value),
                                            Code = gcode + "_" + PTs.Key,
                                            //GroupTerm = PTs.Key.ToLower(),
                                            Name = PTs.Key.ToLower(),
                                            //TermIds = HLTs.Select(s => s.DBTopicId).Distinct().ToList(),
                                            //IsSelectable = true
                                            DefaultObservation = new ObservationRequestDTO()
                                            {
                                                O3 = observations.FirstOrDefault(o => o.ControlledTermStr.Equals(PTs.Key)).ControlledTermStr,
                                                //TODO: RIGHT THERE THIS IS A PROBLEM
                                                //an O3 saved in the 
                                                //This should be changed to id of the controlledTermId and NOT the Id of the Observation (i.e. the AETERM)
                                                O3id = observations.FirstOrDefault(o => o.ControlledTermStr.Equals(PTs.Key)).Id,
                                                //O3id = Int32.Parse(PTs.FirstOrDefault().QualifierQualifiers.SingleOrDefault(q => q.Key.Equals("AEPTCD")).Value),
                                                O3code = observations.FirstOrDefault(o => o.ControlledTermStr.Equals(PTs.Key)).ControlledTermStr.ToLower(),
                                                QO2 = "AEOCCUR",
                                                O3variable = "AEDECOD",
                                                QO2_label = "OCCURENCE",
                                                IsMultipleObservations = true,
                                                TermIds = PTs.Select(p => p.DBTopicId).Distinct().ToList(),
                                                IsEvent = true,
                                                //QO2id = obsObject.DefaultQualifier.Id,
                                                //Id = obsObject.Name.ToLower() + "_" + obsObject.DefaultQualifier.Name
                                                DataType = "string"//observations.FirstOrDefault(o=>o.ControlledTermStr.Equals(PTs.Key)).
                                            },
                                            Qualifiers = getObsRequests(observations.FirstOrDefault(o => o.ControlledTermStr.Equals(PTs.Key)),true, PTs.Select(p => p.DBTopicId).Distinct().ToList()),
                                            //Count = PTs.Count(),
                                            //TermIds = (from ae in PTs
                                            //           select observations.Find(o => o.Name.Equals(ae.Name)).Id).ToList<int>()
                                            //Terms = (
                                            //    //from ae in PTs
                                            //    //group ae by ae.qualifiers.SingleOrDefault(q => q.Key.Equals("AETERM")).Value into TERMs
                                            //    //select new MedDRAGroupNode
                                            //    //{
                                            //    //    Variable = "AETERM",
                                            //    //    Code = "T_" + TERMs.FirstOrDefault().qualifiers.SingleOrDefault(q => q.Key.Equals("AEPTCD")).Value,
                                            //    //    GroupTerm = TERMs.Key,
                                            //    //    Name = TERMs.Key,
                                            //    //    Terms = (
                                            //        from ae in PTs //TERMs
                                            //        select new MedDRATermNode { 
                                            //            Name = ae.Name,
                                            //            Code = observations.FirstOrDefault(o => o.Name.Equals(ae.Name)).ControlledTermStr.ToLower(),
                                            //            Id = observations.FirstOrDefault(o => o.Name.Equals(ae.Name)).Id }
                                            //).Distinct().ToList<GenericNode>()
                                            //}
                                            //).ToList<GenericNode>()
                                        }).ToList<GenericNode>()
                                }).ToList<GenericNode>()
                        }).ToList<GenericNode>()
                }).ToList();


            return AEsByMedDRA;

        }
        private List<ObservationRequestDTO> getObsRequests(Observation obsObject, bool IsMultiple=false, List<int> termIds = null)
        {
            var reqs = obsObject.Qualifiers.Select(variableDefinition => new ObservationRequestDTO()
            {

                O3 = obsObject.Name,//obsObject.ControlledTermStr,
                O3id = obsObject.Id,
                O3code = obsObject.Name.ToLower(),
                QO2 = variableDefinition.Qualifier.Name,
                QO2id = variableDefinition.Qualifier.Id,
                DataType = variableDefinition.Qualifier.DataType,
                QO2_label = variableDefinition.Qualifier.Label,
                IsEvent = obsObject.Class.ToUpper() == "EVENTS",
                IsFinding = obsObject.Class.ToUpper() == "FINDINGS",
                IsMultipleObservations = IsMultiple,
                TermIds = termIds,
                //Id = obsObject.Name.ToLower() + "_" + variableDefinition.Name
                //Qualifiers = obsObject.Qualifiers.Select(q => q.Name).ToList(),
                //DefaultQualifier = obsObject.DefaultQualifier.Name
            }).ToList();

            return reqs;
        }
        #endregion

        #region Assay Browsing methods
        public List<AssayDTO> GetProjectAssays(int projectId)
        {
            List<Assay> assays = _assayRepository.FindAll(a => a.ProjectId == projectId,
                new List<string>()
                {
                    "MeasurementType",
                    "TechnologyPlatform",
                    "TechnologyType"
                }).ToList();

            if (assays.Count == 0)
                return null;
            return assays.Select(p => new AssayDTO()
            {
                Id = p.Id,
                Type = p.MeasurementType != null ? p.MeasurementType.Name : "",
                Platform = p.TechnologyPlatform != null ? p.TechnologyPlatform.Name : "",
                Technology = p.TechnologyType != null ? p.TechnologyType.Name : "",
                Name = p.Name
            }).ToList();
        }
        #endregion


    }
}
