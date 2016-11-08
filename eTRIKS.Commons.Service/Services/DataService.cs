using eTRIKS.Commons.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.DTOs;
using System.Linq.Expressions;
using eTRIKS.Commons.Core.Domain.Model.Templates;
using System.Collections;

namespace eTRIKS.Commons.Service.Services
{
    public class DataService
    {
        private readonly IRepository<Study, int> _studyRepository;
        private readonly IRepository<Observation, int> _observationRepository;
        private readonly IRepository<HumanSubject, string> _subjectRepository;
        private readonly IRepository<CharacteristicObject, int> _characObjRepository;
        private IRepository<Activity, int> _activityRepository;
        private readonly IRepository<SubjectObservation, Guid> _subObservationRepository;
        private readonly IRepository<Biosample, int> _biospecimenRepository;
        private readonly IRepository<DomainTemplate, string> _domainRepository;
        private readonly IRepository<Assay, int> _assayRepository;
        private readonly IRepository<Biosample, int> _bioSampleRepository;
        private readonly IServiceUoW _dataContext;
        public DataService(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _activityRepository = uoW.GetRepository<Activity, int>();
            _studyRepository = uoW.GetRepository<Study, int>();
            _observationRepository = uoW.GetRepository<Observation, int>();
            _subObservationRepository = uoW.GetRepository<SubjectObservation, Guid>();
            _domainRepository = uoW.GetRepository<DomainTemplate, string>();
            _subjectRepository = uoW.GetRepository<HumanSubject, string>();
            _assayRepository = uoW.GetRepository<Assay, int>();
            _biospecimenRepository = uoW.GetRepository<Biosample, int>();
            _characObjRepository = uoW.GetRepository<CharacteristicObject, int>();
            _bioSampleRepository = uoW.GetRepository<Biosample, int>();
        }

        #region Observation Browser methods
        public List<ObservationRequestDTO> GetSubjectCharacteristics(string projectId)
        {
            var subjChars = new List<ObservationRequestDTO>();
            var SCs = _characObjRepository.FindAll(sco=>sco.Project.Accession.Equals(projectId)).ToList();

            subjChars = SCs.Select(sc => new ObservationRequestDTO()
            {
                O3 = sc.FullName,
                O3id = sc.Id,
                O3code = sc.ShortName
            }).ToList();

            var armSC = new ObservationRequestDTO() {O3 = "Arm", O3code = "ARM", O3id = 999};
            var studySC = new ObservationRequestDTO() { O3 = "Study", O3code = "STUDY", O3id = 888 };
            subjChars.Add(armSC);
            subjChars.Add(studySC);

            return subjChars.OrderBy(o => o.O3).ToList();
        }


        public async Task<IEnumerable<ClinicalDataTreeDTO>> GetClinicalObsTree(string projectAccession)
        {

            //TODO: will replace Observation here with ObservationDescriptor
            List<Observation> studyObservations;// =
            //_observationRepository.FindAll(
            //    x => x.Studies.Select(s => s.Project.Accession).Contains(projectAccession),
            //    new List<Expression<Func<Observation, object>>>(){
            //            d => d.Studies.Select(s => s.Project),
            //            d => d.DefaultQualifier,
            //            d => d.Qualifiers
            //        }
            //    ).ToList();

           // if (studyObservations.Count == 0)
           // {
                studyObservations =_observationRepository.FindAll(o => o.Project.Accession.Equals(projectAccession),
                new List<Expression<Func<Observation, object>>>(){
                        d => d.Studies.Select(s => s.Project),
                        d => d.DefaultQualifier,
                        d => d.Qualifiers
                    }).ToList();
            //}

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
                var groupedByDomain = classGroup.GroupBy(x => x.DomainCode);
                foreach (var domainGroup in groupedByDomain)
                {
                    var domain = _domainRepository.FindSingle(d => d.Code.Equals(domainGroup.Key));
                    var domainNode = new DomainNode
                    {
                        Name = domain.Name,
                        Domain = domainGroup.Key,
                        code = domainGroup.Key,
                        Count = domainGroup.Count()
                    };
                    classNode.Domains.Add(domainNode);


                    //if (domainNode.code.Equals("AE"))
                    //{
                    //    var AEs = await getEventsByMedDRA(1, studyObservations);

                    //    domainNode.Terms.AddRange(AEs);
                    //    continue;
                    //}


                    //Group by category
                    var groupedByCategory = domainGroup.GroupBy(x => x.Group);
                    int i = 0;
                    foreach (var catGroup in groupedByCategory)
                    {
                        if (catGroup.Key == null)
                        {
                            if (!domainNode.code.Equals("AE"))
                            {
                                foreach (var obs in catGroup)
                                    domainNode.Terms.Add(createObsNode(obs));
                            }
                            else
                            {
                                //TODO: HARD CODED PROJECT ID getProjectId
                                var AEs = await getEventsByMedDRA(projectAccession, catGroup.ToList(), catGroup.Key);
                                domainNode.Terms.AddRange(AEs);
                            }
                        }
                        else
                        {
                            GroupNode obsCatGrp = new GroupNode();
                            domainNode.Terms.Add(obsCatGrp);
                            obsCatGrp.Name = catGroup.Key;
                            obsCatGrp.Code = domainNode.code + "_cat" + ++i;
                            if (!domainNode.code.Equals("AE"))
                            {
                                foreach (var obs in catGroup)
                                    obsCatGrp.Terms.Add(createObsNode(obs));
                            }
                            else
                            {
                                //TODO: HARD CODED PROJECT ID getProjectId
                                var AEs = await getEventsByMedDRA(projectAccession, catGroup.ToList(), obsCatGrp.Code);
                                obsCatGrp.Terms.AddRange(AEs);
                            }
                        }
                    }
                }
            }
            return cdTreeList;

        }
        #endregion

        #region Crossfilter data methods
        //New method using observationIds, observations coming from UI shuold have observationIds in SQL database
        public async Task<Hashtable> getObservationsData(string projectAcc, List<ObservationRequestDTO> reqObservations)
        {
            //studyId = "CRC305C";
            List<Hashtable> dataMatrix = new List<Hashtable>();


            //Dictionary<string, List<int>> requestObservations = null;
            //    requestObservations =
            //        reqObservations.Cast<DictionaryEntry>()
            //        .ToDictionary(
            //        kvp => (string)kvp.Key, 
            //        kvp => (List<int>)kvp.Value.ToString()
            //            .Trim(new char[]{'[',']'})
            //            .Split(',')
            //            .Select(n => Convert.ToInt32(n)).ToList<int>());

            //List<int> observationsIDs = requestObservations.Values.Cast<List<int>>().SelectMany(o => o).ToList();

            List<int> observationsIDs = reqObservations.Select(o => o.O3id).ToList();
            //Get observation info from SQL
            List<Observation> observations = _observationRepository.FindAll(
                           o => observationsIDs.Contains(o.Id) && o.Project.Accession.Equals(projectAcc),

                           new List<Expression<Func<Observation, object>>>()
                           {
                               d=>d.TopicVariable,
                               d => d.Timings,
                               d => d.DefaultQualifier,
                               d => d.Qualifiers,
                               
                           }
             ).ToList();

            //This will be replaced by call to SQL for studies of a particular project.
            //var studyIds = new List<string> { "CRC305A", "CRC305B", "CRC305C", "CRC305D" };

            var subjFindings = new List<SubjectObservation>();
            var subjEvents = new List<SubjectObservation>();

            #region Query Mongo for Subject Observations data
            foreach (var observation in observations)
            {
                string obsName = observation.Name;
                string fieldName = observation.TopicVariable.Name;
                if (observation.ControlledTermStr == null)
                    observation.ControlledTermStr = observation.Name;
                if (observation.DomainCode.Equals("AE") && observation.ControlledTermStr != null)
                {
                    obsName = observation.ControlledTermStr;
                    fieldName = observation.DomainCode + "DECOD";
                }
                string obsGroupName = observation.Group;
                string groupFieldName = observation.DomainCode + "CAT";
                string subgroupFieldName = observation.DomainCode + "SCAT";

                _dataContext.AddClassMap(fieldName, "Name");
                //TODO: search for subject observations in Mongo by observation Id instead of names
                //if (obsGroupName != null)
                //{
                //    _dataContext.AddClassMap(groupFieldName, "Group");
                //    //add cateogry and add class ... to make the name of the observation unique
                //    observationData =
                //        await _subObservationRepository.FindAllAsync(
                //            d => d.Name.Equals(obsName) /*CRC305D-GENT001-1001*/);
                //}
                //else
                //{

                //TODO: either add projectId to Mongo Records or query for project studies ids from SQL and include them in the query for mongo
                List<SubjectObservation> observationData = await _subObservationRepository.FindAllAsync(
                    d => d.Name.Equals(obsName) && d.ProjectAcc.Equals(projectAcc));
                    //TEMP
                   // d => studyIds.Contains(d.StudyId));
                //}
                if (observation.Class.ToUpper().Equals("EVENTS"))
                {
                    subjEvents.AddRange(observationData);
                }

                else
                    subjFindings.AddRange(observationData);
            }
            #endregion


            #region get Subject Data and organize all Observations by Subject

            //List<Subject> subjectData =
            //       await _subjectRepository.FindAllAsync(
            //       s => studyIds.Contains(s.StudyId) && s.DomainCode.Equals("DM"));

            ////Group data by subject, visit and timepoint
            //var FindingsBySubject = subjObservations
            //    .GroupBy(ob => new { 
            //        subjId = ob.SubjectId,
            //        Visit = ob.Visit,
            //        //Day = ob.ObsStudyDay == null ? -999 : ob.ObsStudyDay.Number,
            //        Timepoint = ob.ObsStudyTimePoint == null ? "" : ob.ObsStudyTimePoint.Name
            //        });

            //var adverseEventsBySubject = subjAdverseEvents
            //    .GroupBy(ob => ob.SubjectId);
            #endregion

            //DataTable datamatrix = new DataTable();
            //datamatrix.Columns.Add("subjectId");
            //datamatrix.Columns.Add("study");
            //foreach(var obsreq in reqObservations)
            //{
            //    //datamatrix.Columns.Add(obsreq.O3 + "[" + obsreq.QO2 + "]");
            //    datamatrix.Columns.Add(obsreq.Id);
            //}
            //datamatrix.Columns.Add("visit");
            //datamatrix.Columns.Add("timepoint");

            //Create a datamatrix to sendback to crossfilter
            //One row per subject per visit per timepoint

            //when there are subject observations, they are grouped by subject id and used to reflect the subset
            //of subjects that have these observations selected(usually it will be the whole set of subjects, since
            //these lab tests of vital signs are collected for all subjects)

            //I then need to add to the subjectObservaation rows, info from the subjects (site, study and arms)

            //If there are no subjectobservations selected (i.e. only adverse events), I need to query all subjects and iterate 
            //through these instead of the subjecobservations

            //Will iterate through subjects as a top level, then for each subject get subject observation from subObsbySubj then
            //aes from aesbysubject
            //Dictionary<string, List<Hashtable>> subjectToObservationsPerVisitAndTime = new Dictionary<string, List<Hashtable>>();
            var findingsTable = getFindingsDataTable(subjFindings, observations, reqObservations);
            var eventsTable = getEventsDataTable(subjEvents, observations, reqObservations);
            //            #region Create the data matrix
            //            try
            //            {
            //                foreach (Subject subject in subjectData)
            //                {
            //                    //List<Hashtable> rows = new List<Hashtable>();
            //                    //subjectToObservationsPerVisitAndTime.Add(subject.SubjId, rows);

            //                    #region Add Findings
            //                    //ADD FINDINGS
            //                    if (FindingsBySubject.Any(o => o.Key.subjId.Equals(subject.SubjId)))
            //                    {
            //                        //Subject has findings
            //                        var subjFindings = FindingsBySubject.Where(o => o.Key.subjId.Equals(subject.SubjId));

            //                        foreach (var groupOfSubjFindings in subjFindings)//Visit,TimePoint
            //                        {
            //                            //Hashtable ht = new Hashtable(); //creating a new row for each visit/timepoint instance 
            //                            DataRow row = datamatrix.NewRow();

            //                            //adding Findings in the same visit and timepoint to the same row 
            //                            foreach (var subjObs in groupOfSubjFindings)
            //                            {
            //                                row["subjectId"] = subjObs.SubjectId;
            //                                row["study"] = subjObs.StudyId;
            //                                foreach (var obsreq in reqObservations)
            //                                {
            //                                    //TODO: if default qualifier used dont include square brackets
            //                                    //TODO: assumption to be validated that visits/timepoints are study and dont differ between observations or studies of the same project

            //                                    row[obsreq.Id] =
            //                                        subjObs.qualifiers.SingleOrDefault(q => q.Key.Equals(obsreq.QO2)).Value;
            //                                }
            //                                row["visit"] = subjObs.Visit;
            //                                row["timepoint"] = subjObs.ObsStudyTimePoint == null ? "" : subjObs.ObsStudyTimePoint.Name;


            //                                //if (!ht.ContainsKey("subjectId")) ht.Add("subjectId", subject.SubjId);
            //                                ////TODO: should not depend on string equality between observation and subjectObservation

            //                                //string defVariable = observations.Find(o => o.Name.Equals(subjObs.Name)).DefaultQualifier.Name;
            //                                //ht.Add(subjObs.Name.ToLower() + "_" + defVariable, subjObs.qualifiers.SingleOrDefault(q => q.Key.Equals(defVariable)).Value);



            //                                //if (!ht.ContainsKey("visit")) ht.Add("visit", subjObs.Visit);
            //                                //if (!ht.ContainsKey("timepoint")) ht.Add("timepoint", subjObs.ObsStudyTimePoint == null ? "" : subjObs.ObsStudyTimePoint.Name);
            //                                ////if (!ht.ContainsKey("study")) ht.Add("study", subjObs.StudyId);

            //                            }
            //                            datamatrix.Rows.Add(row);
            //                            //rows.Add(ht);
            //                        }
            //                    }
            //                    #endregion

            //                    #region Add Adverse Events
            //                    //ADD Adverse Events
            //                    if (subjAdverseEvents.Count != 0)
            //                    {
            //                        var subjAEs = adverseEventsBySubject.SingleOrDefault(a => a.Key.Equals(subject.SubjId)); //this will contain a list of AEs
            //                        if (subjAEs != null)
            //                            //The current subject has values for the adverse events queried

            //                            foreach (var subjAE in subjAEs)
            //                            {
            //                                //List<Hashtable> hts;
            //                                string aeName = subjAE.Name.ToLower();
            //                                //if (requestObservations.Keys.Contains(aeName))
            //                                //{
            //                                //    #region An Adverse Event Observation
            //                                //    //This is an individual AE Term selected by the user
            //                                //    if (rows.Count > 0)
            //                                //    {
            //                                //        //rows for this subject have been created before for a number of findings
            //                                //        foreach (var ht in rows)
            //                                //        {
            //                                //            if (!ht.ContainsKey(aeName)) ht.Add(aeName, "Yes");
            //                                //            if (!ht.ContainsKey(aeName + " sev"))
            //                                //            {
            //                                //                string defVariable = observations.Find(o => o.Name.Equals(subjAE.Name)).DefaultQualifier.Name;
            //                                //                ht.Add(aeName + " sev", subjAE.qualifiers.SingleOrDefault(q => q.Key.Equals(defVariable)).Value);

            //                                //            }

            //                                //        }
            //                                //    }
            //                                //    else
            //                                //    {
            //                                //        //no rows for this subject has been created before (in the case of no findings selected)

            //                                //        Hashtable ht = new Hashtable();
            //                                //        ht.Add(aeName, "Yes");
            //                                //        if (!ht.ContainsKey(aeName + " sev"))
            //                                //        {
            //                                //            string defVariable = observations.Find(o => o.Name.Equals(subjAE.Name)).DefaultQualifier.Name;
            //                                //            ht.Add(aeName + " sev", subjAE.qualifiers.SingleOrDefault(q => q.Key.Equals(defVariable)).Value);

            //                                //        }
            //                                //        ht.Add("subjectId", subject.SubjId);
            //                                //        rows.Add(ht);
            //                                //    }


            //                                //}
            //                                //    #endregion
            //                                //else
            //                                //{
            //                                //    #region Not selected by the user, but could be an AE belonging to a medDRA group selected by the user
            //                                //    foreach (var reqObs in requestObservations)
            //                                //    {
            //                                //        string reqObsName = reqObs.Key.ToLower();
            //                                //        List<int> reqObsIds = reqObs.Value;

            //                                //        //Get the Observation Id of the current ae
            //                                //        //If it is in the list of ids of this requestedObsName (i.e. MedDRA group)
            //                                //        //then add the group to ht
            //                                //        int subjObsId = observations.SingleOrDefault(
            //                                //            o => o.Name.ToLower().Equals(aeName)).Id;
            //                                //        if (reqObsIds.Contains(subjObsId))
            //                                //        {
            //                                //            if (rows.Count != 0)
            //                                //            {
            //                                //                foreach (var ht in rows)
            //                                //                {
            //                                //                    if (!ht.ContainsKey(reqObsName)) ht.Add(reqObsName, "Yes");
            //                                //                }
            //                                //            }
            //                                //            else
            //                                //            {
            //                                //                Hashtable ht = new Hashtable();
            //                                //                ht.Add(reqObsName, "Yes");
            //                                //                rows.Add(ht);
            //                                //            }

            //                                //        }

            //                                //    }
            //                                //    #endregion
            //                                //}

            //                            }
            //                    }
            //                    #endregion

            //                    //if (rows.Count == 0)
            //                    //{
            //                    //    Hashtable ht = new Hashtable();
            //                    //    rows.Add(ht);
            //                    //}

            //                    #region Fill in nulls and Nos
            //                    //foreach (var ht in rows)
            //                    //{
            //                    //    if (!ht.ContainsKey("subjectId")) ht.Add("subjectId", subject.SubjId);
            //                    //    //if (!ht.ContainsKey("arm")) ht.Add("arm", subject.ArmCode);
            //                    //    //if (!ht.ContainsKey("site")) ht.Add("site", subject.Site);
            //                    //    foreach (string obsReq in requestObservations.Keys)
            //                    //    {
            //                    //        if (!ht.ContainsKey(obsReq.ToLower()))
            //                    //        {
            //                    //            //If obs was requested and was not added to the ht so far
            //                    //            var obs = observations.SingleOrDefault(o => o.Name.ToLower().Equals(obsReq.ToLower()));

            //                    //            if (obs != null)
            //                    //            {
            //                    //                //This is an individual observation AE or Findings
            //                    //                var obsName = obs.Name.ToLower();
            //                    //                if (obs.DomainCode.Equals("AE"))
            //                    //                    ht.Add(obsName, "NO");
            //                    //                else
            //                    //                    ht.Add(obsName, null);
            //                    //            }
            //                    //            else
            //                    //                //This is a group and this subject has no occurence for all its descendent observations
            //                    //                ht.Add(obsReq.ToLower(), "NO");
            //                    //        }
            //                    //    }
            //                    //}

            //                    #endregion


            //                    //Think about not adding hts that have no values for any of the requested observations
            //                    //dataMatrix.AddRange(rows);

            //                }

            //                //foreach (var group in FindingsBySubject)
            //                //{
            //                //    string key = group.Key.ToString();
            //                //    string subjId = group.Key.subjId;

            //                //    Hashtable ht = new Hashtable();
            //                //    //adding Findings
            //                //    foreach (var subjObs in group)
            //                //    {
            //                //        //check if default qualifier is avaialable
            //                //        //if( observations.Find(o => o.Name.Equals(subjObs.Name)).DefaultQualifierId != null)

            //                //        string defVariable = observations.Find(o => o.Name.Equals(subjObs.Name)).DefaultQualifier.Name;
            //                //        if (!ht.ContainsKey("SubjectId")) ht.Add("SubjectId", subjObs.SubjectId);
            //                //        ht.Add(subjObs.Name.ToLower(), subjObs.qualifiers.SingleOrDefault(q => q.Key.Equals(defVariable)).Value);
            //                //        if (!ht.ContainsKey("Visit")) ht.Add("Visit", subjObs.Visit);
            //                //        if (!ht.ContainsKey("Timepoint")) ht.Add("Timepoint", subjObs.ObsStudyTimePoint == null ? "" : subjObs.ObsStudyTimePoint.Name);
            //                //        if (!ht.ContainsKey("study")) ht.Add("study", subjObs.StudyId);
            //                //        if (!ht.ContainsKey("arm")) ht.Add("arm", subjO);
            //                //    }

            //                //    //
            //                //    if (subjAdverseEvents.Count != 0)
            //                //    {
            //                //        var subjAE = adverseEventsBySubject.SingleOrDefault(a => a.Key.Equals(subjId)); //this will contain a list of AEs
            //                //        if(subjAE != null)
            //                //            //The current subject has values for the adverse events queried

            //                //            foreach (var ae in subjAE)
            //                //            {
            //                //                string obsName = ae.Name.ToLower();
            //                //                if (requestObservations.Keys.Contains(obsName))
            //                //                {
            //                //                    if (!ht.ContainsKey(ae.Name)) ht.Add(obsName, "Yes");
            //                //                }
            //                //                else {
            //                //                        foreach (var reqObs in requestObservations)
            //                //                        {
            //                //                            string obs = reqObs.Key;
            //                //                            List<int> obsIds = reqObs.Value;

            //                //                            int subjObsId = observations.SingleOrDefault(
            //                //                                o => o.Name.ToLower().Equals(obsName)).Id;
            //                //                            if (obsIds.Contains(subjObsId))
            //                //                            {
            //                //                                if (!ht.ContainsKey(obs)) ht.Add(obs, "Yes");
            //                //                            }

            //                //                        }

            //                //                    }



            //                //            }
            //                //    }





            //            }
            //            catch (Exception e)
            //            {
            //                Debug.WriteLine("Error while generating data matrix: "+ e.Message);
            //            }
            //#endregion
            //List<string> columns = dataMatrix.ElementAt(0).Keys.Cast<string>().ToList();

            var findingsColsList = (from dc in findingsTable.Columns.Cast<DataColumn>()
                                    select dc.ColumnName).ToList();
            var eventsColsList = (from dc in eventsTable.Columns.Cast<DataColumn>()
                                  select dc.ColumnName).ToList();
            var result = new Hashtable
            {
                {"findingsTbl", findingsTable},
                {"eventsTbl", eventsTable},
                {"findingsTblHeader", findingsColsList},
                {"eventsTblHeader", eventsColsList}
            };
            return result;
        }
        public async Task<Hashtable> getSubjectData(string projectId, List<ObservationRequestDTO> reqObservations)
        {
            List<HumanSubject> subjects = _subjectRepository.FindAll(
                s => s.Study.Project.Accession.Equals(projectId),
                new List<Expression<Func<HumanSubject, object>>>()
                {   d=> d.SubjectCharacteristics.Select(s=>s.CharacteristicObject),
                    d=>d.Study
                }).ToList();


            List<Hashtable> subject_table = new List<Hashtable>();
            HashSet<string> SCs = new HashSet<string>(); //{ "arm", "siteid", "study" };


            foreach (HumanSubject subject in subjects)
            {
                Hashtable ht = new Hashtable();
                ht.Add("subjectId", subject.UniqueSubjectId);
                if (subject.ArmCode != null)
                {
                    SCs.Add("arm"); ht.Add("arm", subject.ArmCode);
                }

                if (subject.Study.Name != null)
                {
                    SCs.Add("study"); ht.Add("study", subject.Study.Name);
                }

                if (subject.Study.Site != null)
                {
                    SCs.Add("siteid"); ht.Add("siteid", subject.Study.Site);
                }

                if (subject.SubjectCharacteristics.SingleOrDefault(sc => sc.CharacteristicObject.ShortName.ToUpper().Equals("SEX")) != null)
                {
                    SCs.Add("sex"); ht.Add("sex", subject.SubjectCharacteristics.SingleOrDefault(sc => sc.CharacteristicObject.ShortName.ToUpper().Equals("SEX")).VerbatimValue);
                }
                if (subject.SubjectCharacteristics.SingleOrDefault(sc=> sc.CharacteristicObject.ShortName.ToUpper().Equals("AGE")) != null)
                {
                    SCs.Add("age"); ht.Add("age", subject.SubjectCharacteristics.SingleOrDefault(sc => sc.CharacteristicObject.ShortName.ToUpper().Equals("AGE")).VerbatimValue);
                }

                    
                if (reqObservations != null)
                    foreach (var requestDto in reqObservations)
                    {
                        var charVal = subject.SubjectCharacteristics.Single(sc => sc.CharacteristicObjectId.Equals(requestDto.O3id));
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
        private DataTable getFindingsDataTable(IEnumerable<SubjectObservation> findings, IEnumerable<Observation> O3s, IEnumerable<ObservationRequestDTO> reqObservations)
        {
            #region Build Table columns
            var datatable = new DataTable();
            datatable.Columns.Add("subjectId");
            datatable.Columns.Add("study");
            foreach (var obs in O3s.Where(obs => obs.Class.ToLower().Equals("findings")))
            {
                foreach (var r in reqObservations.Where(r => r.O3id.Equals(obs.Id)))
                {
                    datatable.Columns.Add(r.Id);
                }
                //datatable.Columns.AddRange(reqObservations.Where(r => r.O3id.Equals(obs.Id)).Select(f => f.Id).ToArray());
            }
            datatable.Columns.Add("visit");
            datatable.Columns.Add("timepoint");
            #endregion

            #region Group observations by unique key
            var findingsBySubjectVisitTime = findings //The unique combination that would make one row
                .GroupBy(ob => new
                {
                    subjId = ob.SubjectId,
                    Visit = ob.Visit,
                    //Day = ob.ObsStudyDay == null ? -999 : ob.ObsStudyDay.Number,
                    Timepoint = ob.ObsStudyTimePoint == null ? "" : ob.ObsStudyTimePoint.Name
                });
            #endregion

            foreach (var subjVisitTPT in findingsBySubjectVisitTime)
            {
                var row = datatable.NewRow();
                foreach (var subjObs in subjVisitTPT)//adding columns to the same row each iteration is a different observation but within each iteration
                //we could be adding more than one column per observation if requested more than one qaulifier
                {
                    row["subjectId"] = subjObs.SubjectId;
                    row["study"] = subjObs.StudyId;
                    //TODO: SHOULD ADD O3id to subjObs in MongoDB
                    foreach (var obsreq in reqObservations.Where(r => r.O3code.ToLower().Equals(subjObs.Name.ToLower()))) //WILL BREAK NOW WITH EVENTS SINCE subjObs.NAME = TERM, while reqObs.O3 = DECOD
                        //TODO: if default qualifier used dont include square brackets
                        //TODO: assumption to be validated that visits/timepoints are study and dont differ between observations or studies of the same project
                        row[obsreq.Id] = subjObs.qualifiers.SingleOrDefault(q => q.Key.Equals(obsreq.QO2)).Value;

                    row["visit"] = subjObs.Visit;
                    row["timepoint"] = subjObs.ObsStudyTimePoint == null ? "" : subjObs.ObsStudyTimePoint.Name;
                }
                datatable.Rows.Add(row);
            }

            return datatable;
        }
        private DataTable getEventsDataTable(IEnumerable<SubjectObservation> events, IEnumerable<Observation> O3s, IEnumerable<ObservationRequestDTO> reqObservations)
        {

            #region Build Table columns
            var datatable = new DataTable();
            datatable.Columns.Add("subjectId");
            datatable.Columns.Add("study");
            foreach (var obs in O3s.Where(obs => obs.Class.ToLower().Equals("events")))
            {
                foreach (var r in reqObservations.Where(r => r.O3code.ToLower().Equals(obs.ControlledTermStr.ToLower())))
                    datatable.Columns.Add(r.Id);
            }

            datatable.Columns.Add("start date");
            datatable.Columns.Add("end date");
            datatable.Columns.Add("study day");
            #endregion

            var eventsBySubjectStudyDay = events
                .GroupBy(ob => new
                {
                    subjId = ob.SubjectId,
                    studyDay = ob.ObsStudyDay
                });


            //TODO: if default qualifier used dont include square brackets
            //TODO: assumption to be validated that visits/timepoints are study and dont differ between observations or studies of the same project
            //TODO: SHOULD ADD O3id to subjObs in MongoDB
            foreach (var eventSubjDay in eventsBySubjectStudyDay)
            {
                var row = datatable.NewRow();
                foreach (var subjObs in eventSubjDay)//adding columns to the same row each iteration is a different observation but within each iteration
                //we could be adding more than one column per observation if requested more than one qaulifier
                {
                    row["subjectId"] = subjObs.SubjectId;
                    row["study"] = subjObs.StudyId;

                    var obsName = subjObs.Name != null ? subjObs.Name.ToLower() : subjObs.VerbatimName.ToLower();

                    foreach (var obsreq in reqObservations.Where(r => r.O3code.ToLower().Equals(obsName)))
                        row[obsreq.Id] = subjObs.qualifiers.SingleOrDefault(q => q.Key.Equals(obsreq.QO2)).Value;

                    //if (subjObs.ObsInterval!=null && subjObs.ObsInterval.Start!=null)
                    row["start date"] = "";//subjObs.ObsInterval.Start.Name;
                    row["end date"] = "";//subjObs.ObsInterval.End;
                    row["study day"] = subjObs.ObsStudyDay == null ? "" : subjObs.ObsStudyDay.Name;
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
        private async Task<List<MedDRAGroupNode>> getEventsByMedDRA(string projectAccession, List<Observation> observations, string gname)
        {
            List<SubjectObservation> adverseEvents =
                   await _subObservationRepository.FindAllAsync(d => d.DomainCode.Equals("AE") && d.ProjectAcc == projectAccession
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
                group ae by ae.qualifiers.SingleOrDefault(q => q.Key.Equals("AESOC")).Value into SOCs
                select new MedDRAGroupNode
                {
                    Variable = "AESOC",
                    Code = gname + "_" + SOCs.FirstOrDefault().qualifiers.SingleOrDefault(q => q.Key.Equals("AESOCCD")).Value,
                    Name = "SO: " + SOCs.Key.ToLowerInvariant(),
                    GroupTerm = SOCs.Key.ToLower(),
                    //TermIds = (from ae in SOCs
                    //           select observations.Find(o => o.ControlledTermStr.ToLower().Equals(ae.Name.ToLower())).Id).ToList<int>(),
                    Count = SOCs.Select(so => so.Name).Distinct().Count(),
                    Terms = (
                        from ae in SOCs
                        group ae by ae.qualifiers.SingleOrDefault(q => q.Key.Equals("AEHLGT")).Value into HLGTs
                        select new MedDRAGroupNode
                        {
                            Variable = "AEHLGT",
                            Code = gname + "_" + HLGTs.FirstOrDefault().qualifiers.SingleOrDefault(q => q.Key.Equals("AEHLGTCD")).Value,
                            GroupTerm = HLGTs.Key.ToLower(),
                            Name = "HLG: " + HLGTs.Key.ToLower(),
                            Count = HLGTs.Count(),
                            //TermIds = (from ae in HLGTs
                            //           select observations.Find(o => o.ControlledTermStr.ToLower().Equals(ae.Name.ToLower())).Id).ToList<int>(),
                            Terms = (
                                from ae in HLGTs
                                group ae by ae.qualifiers.SingleOrDefault(q => q.Key.Equals("AEHLT")).Value into HLTs
                                select new MedDRAGroupNode
                                {
                                    Variable = "AEHLT",
                                    Code = gname + "_" + HLTs.FirstOrDefault().qualifiers.SingleOrDefault(q => q.Key.Equals("AEHLTCD")).Value,
                                    GroupTerm = HLTs.Key.ToLower(),
                                    Name = "HLT: " + HLTs.Key.ToLower(),
                                    Count = HLTs.Count(),
                                    //TermIds = (from ae in HLTs
                                    //           select observations.Find(o => o.ControlledTermStr.ToLower().Equals(ae.Name.ToLower())).Id).ToList<int>(),
                                    Terms = (
                                        from ae in HLTs
                                        group ae by ae.StandardName into PTs //ae.qualifiers.SingleOrDefault(q => q.Key.Equals("AEDECOD")).Value into PTs
                                        select new MedDRATermNode()
                                        {
                                            Variable = "AEDECOD",
                                            //Id = observations.FirstOrDefault(o => o.ControlledTermStr.Equals(PTs.Key)).Id, /// this is the first AETERM id in terms having same PTterm
                                            Id = Int32.Parse(PTs.FirstOrDefault().qualifiers.SingleOrDefault(q => q.Key.Equals("AEPTCD")).Value),
                                            Code = gname + "_" + PTs.Key,
                                            //GroupTerm = PTs.Key.ToLower(),
                                            Name = PTs.Key.ToLower(),
                                            DefaultObservation = new ObservationRequestDTO()
                                            {
                                                O3 = observations.FirstOrDefault(o => o.ControlledTermStr.Equals(PTs.Key)).ControlledTermStr,
                                                //TODO: RIGHT THERE THIS IS A PROBLEM
                                                //an O3 saved in the 
                                                O3id = observations.FirstOrDefault(o => o.ControlledTermStr.Equals(PTs.Key)).Id,
                                                O3code = observations.FirstOrDefault(o => o.ControlledTermStr.Equals(PTs.Key)).ControlledTermStr.ToLower(),
                                                QO2 = "AEOCCUR",
                                                O3variable = "AEDECOD",
                                                //QO2id = obsObject.DefaultQualifier.Id,
                                                //Id = obsObject.Name.ToLower() + "_" + obsObject.DefaultQualifier.Name
                                            },
                                            Qualifiers = getObsRequests(observations.FirstOrDefault(o => o.ControlledTermStr.Equals(PTs.Key))),
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

        private List<ObservationRequestDTO> getObsRequests(Observation obsObject)
        {
            var reqs = obsObject.Qualifiers.Select(variableDefinition => new ObservationRequestDTO()
            {

                O3 = obsObject.Name,//obsObject.ControlledTermStr,
                O3id = obsObject.Id,
                O3code = obsObject.Name.ToLower(),
                QO2 = variableDefinition.Name,
                QO2id = variableDefinition.Id,
                //Id = obsObject.Name.ToLower() + "_" + variableDefinition.Name
                //Qualifiers = obsObject.Qualifiers.Select(q => q.Name).ToList(),
                //DefaultQualifier = obsObject.DefaultQualifier.Name
            }).ToList();

            return reqs;
        }
        #endregion



        
    }
}
