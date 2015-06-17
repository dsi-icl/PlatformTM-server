using eTRIKS.Commons.Core.Domain.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.DataAccess.MongoDB;
using eTRIKS.Commons.DataAccess;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.DTOs;
using System.Linq.Expressions;
using MongoDB.Bson;
using eTRIKS.Commons.Core.Domain.Model.Templates;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Globalization;

namespace eTRIKS.Commons.Service.Services
{
    public class DataService
    {
        private IRepository<Project, int> _projectRepository;
        private IRepository<Observation, int> _observationRepository;
        private IRepository<Subject, Guid> _subjectRepository;
        //private IRepository<SubjectCharacteristic, int> _subjCharacterisitcRepository;
        private IRepository<Activity, int> _activityRepository;
        private IRepository<SubjectObservation, Guid> _subObservationRepository;
        private IRepository<DomainTemplate, string> _domainRepository;
        private IServiceUoW _dataContext;
        public DataService(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _activityRepository = uoW.GetRepository<Activity, int>();
            //_dataSetRepository = uoW.GetRepository<Dataset, int>();
            _observationRepository = uoW.GetRepository<Observation, int>();
            _subObservationRepository = uoW.GetRepository<SubjectObservation, Guid>();
            _domainRepository = uoW.GetRepository<DomainTemplate, string>();
            _subjectRepository = uoW.GetRepository<Subject, Guid>();
           // _subjCharacterisitcRepository = uoW.GetRepository<SubjectCharacteristic, int>();
        }


        public async Task<IEnumerable<ClinicalDataTreeDTO>> getClinicalObsTree(int projectId)
        {

            //Should change to reflect multiple studies for project
            //Either change the observations in the database to be associated with project and not study 
            //or query here for 
            string studyId = "STD-BVS-01";
            List<Observation> studyObservations =
                _observationRepository.FindAll(x => x.Studies.Select(s => s.Id).Contains(studyId)).ToList();
            //_observationRepository.FindAll(x => x.Studies.Select(s => s.ProjectId).Equals(projectId)).ToList();
            

            List<ClinicalDataTreeDTO> cdTreeList = new List<ClinicalDataTreeDTO>();

            //Group by class
            var groupedByClass = studyObservations.GroupBy(ob => ob.Class);
            foreach (var classGroup in groupedByClass)
            {
                if (classGroup.Key.Equals("SpecialPurpose"))
                    continue;
               ClinicalDataTreeDTO classNode = new ClinicalDataTreeDTO();
               cdTreeList.Add(classNode);
               classNode.Class = classGroup.Key;

                //Group by domain
               var groupedByDomain = classGroup.GroupBy(x => x.DomainCode);
               foreach (var domainGroup in groupedByDomain)
               {
                   DomainTemplate domain = _domainRepository.FindSingle(d => d.Code.Equals(domainGroup.Key));
                   DomainNode domainNode = new DomainNode();
                   domainNode.Name = domain.Name;
                   domainNode.Domain = domainGroup.Key;
                   domainNode.code = domainGroup.Key;
                   domainNode.Count = domainGroup.Count();
                   classNode.Domains.Add(domainNode);

                   
                   if (domainNode.code.Equals("AE"))
                   {
                       List<MedDRAGroupNode> AEs = await getEventsByMedDRA(projectId, studyObservations);
                       
                       domainNode.Terms.AddRange(AEs);
                       continue;
                   }
                       
                       
                   

                   //Group by category
                   var groupedByCategory = domainGroup.GroupBy(x => x.Group);
                   int i = 0;
                   foreach (var catGroup in groupedByCategory)
                   {
                       if (catGroup.Key == null)
                           foreach (var obs in catGroup)
                               domainNode.Terms.Add(createObsNode(obs));
                       else
                       {
                           GroupNode obsCatGrp = new GroupNode();
                           domainNode.Terms.Add(obsCatGrp);
                           obsCatGrp.Name = catGroup.Key;
                           obsCatGrp.Code = domainNode.code + "_cat" + ++i;
                           foreach (var obs in catGroup)
                               obsCatGrp.Terms.Add(createObsNode(obs));
                       }
                   }
               }
           }
           return cdTreeList;
                
        }

        //New method using observationIds, observations coming from UI shuold have observationIds in SQL database
        public async Task<Hashtable> getObservationsData(string projectId, Hashtable reqObservations)
        {
            //studyId = "CRC305C";
            List<Hashtable> dataMatrix = new List<Hashtable>();


            Dictionary<string, List<int>> requestObservations = null;
                requestObservations =
                    reqObservations.Cast<DictionaryEntry>()
                    .ToDictionary(
                    kvp => (string)kvp.Key, 
                    kvp => (List<int>)kvp.Value.ToString()
                        .Trim(new char[]{'[',']'})
                        .Split(',')
                        .Select(n => Convert.ToInt32(n)).ToList<int>());

            List<int> observationsIDs = requestObservations.Values.Cast<List<int>>().SelectMany(o => o).ToList();
            //Get observation info from SQL
            List<Observation> observations = _observationRepository.FindAll(
                           o => observationsIDs.Contains(o.Id),
                           new List<Expression<Func<Observation, object>>>()
                           {
                               d=>d.TopicVariable,
                               d => d.Timings,
                               d => d.DefaultQualifier
                           }
             ).ToList();

            //This will be replaced by call to SQL for studies of a particular project.
            List<string> studyIds = new List<string> { "CRC305A", "CRC305B", "CRC305C", "CRC305D" };

            List<SubjectObservation> subjObservations = new List<SubjectObservation>();
            List<SubjectObservation> subjAdverseEvents = new List<SubjectObservation>();

            #region Query Mongo for Subject Observations data
            foreach (Observation observation in observations){
                string obsName = observation.Name ;
                string obsGroupName = observation.Group;
                string fieldName = observation.TopicVariable.Name;
                string groupFieldName = observation.DomainCode + "CAT";
                List<SubjectObservation> observationData;

                _dataContext.AddClassMap(fieldName, "Name");
                //TODO: search for subject observations in Mongo by observation Id instead of names
                if (obsGroupName != null)
                {
                    _dataContext.AddClassMap(groupFieldName, "Group");
                    //add cateogry and add class ... to make the name of the observation unique
                    observationData =
                        await _subObservationRepository.FindAllAsync(
                            d => d.Name.Equals(obsName) /*CRC305D-GENT001-1001*/);
                }
                else
                {
                    observationData =
                        await _subObservationRepository.FindAllAsync(
                            d => d.Name.Equals(obsName));
                }
                if (observation.DomainCode.Equals("AE"))
                    subjAdverseEvents.AddRange(observationData);
                else
                    subjObservations.AddRange(observationData);
            }
            #endregion


            #region get Subject Data and organize all Observations by Subject        

            List<Subject> subjectData =
                   await _subjectRepository.FindAllAsync(
                   s => studyIds.Contains(s.StudyId) && s.DomainCode.Equals("DM"));

            //Group data by subject, visit and timepoint
            var FindingsBySubject = subjObservations
                .GroupBy(ob => new { 
                    subjId = ob.SubjectId,
                    Visit = ob.Visit,
                    //Day = ob.ObsStudyDay == null ? -999 : ob.ObsStudyDay.Number,
                    Timepoint = ob.ObsStudyTimePoint == null ? "" : ob.ObsStudyTimePoint.Name
                    });

            var adverseEventsBySubject = subjAdverseEvents
                .GroupBy(ob => ob.SubjectId);
            #endregion

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
            Dictionary<string, List<Hashtable>> subjectToObservationsPerVisitAndTime = new Dictionary<string, List<Hashtable>>();
            #region Create the data matrix
            try
            {
                foreach (Subject subject in subjectData)
                {
                    List<Hashtable> rows = new List<Hashtable>();
                    subjectToObservationsPerVisitAndTime.Add(subject.SubjId, rows);

                    #region Add Findings
                    //ADD FINDINGS
                    if (FindingsBySubject.Any(o => o.Key.subjId.Equals(subject.SubjId)))
                    {
                        //Subject has findings
                        var subjFindings = FindingsBySubject.Where(o => o.Key.subjId.Equals(subject.SubjId));
                        
                        foreach (var groupOfSubjFindings in subjFindings)//Visit,TimePoint
                        {
                            Hashtable ht = new Hashtable();
                            //adding Findings
                            foreach (var subjObs in groupOfSubjFindings)
                            {
                                if (!ht.ContainsKey("subjectId")) ht.Add("subjectId", subject.SubjId);
                                string defVariable = observations.Find(o => o.Name.Equals(subjObs.Name)).DefaultQualifier.Name;
                                ht.Add(subjObs.Name.ToLower(), subjObs.qualifiers.SingleOrDefault(q => q.Key.Equals(defVariable)).Value);
                                if (!ht.ContainsKey("visit")) ht.Add("visit", subjObs.Visit);
                                if (!ht.ContainsKey("timepoint")) ht.Add("timepoint", subjObs.ObsStudyTimePoint == null ? "" : subjObs.ObsStudyTimePoint.Name);
                                if (!ht.ContainsKey("study")) ht.Add("study", subjObs.StudyId);
                                
                            }
                            rows.Add(ht);
                        }
                    }
                    #endregion

                    #region Add Adverse Events
                    //ADD Adverse Events
                    if (subjAdverseEvents.Count != 0)
                    {
                        var subjAEs = adverseEventsBySubject.SingleOrDefault(a => a.Key.Equals(subject.SubjId)); //this will contain a list of AEs
                        if (subjAEs != null)
                            //The current subject has values for the adverse events queried
                            
                            foreach (var subjAE in subjAEs)
                            {
                                //List<Hashtable> hts;
                                string aeName = subjAE.Name.ToLower();
                                if (requestObservations.Keys.Contains(aeName))
                                {
                                    #region An Adverse Event Observation
                                    //This is an individual AE Term selected by the user
                                    if (rows.Count>0){
                                        //rows for this subject have been created before for a number of findings
                                        foreach (var ht in rows)
                                        {
                                            if (!ht.ContainsKey(aeName)) ht.Add(aeName, "Yes");
                                            if (!ht.ContainsKey(aeName+" sev")){
                                                string defVariable = observations.Find(o => o.Name.Equals(subjAE.Name)).DefaultQualifier.Name;
                                                ht.Add(aeName+ " sev", subjAE.qualifiers.SingleOrDefault(q => q.Key.Equals(defVariable)).Value);
                                                
                                            } 
                                                
                                        }
                                    }
                                    else
                                    {
                                       //no rows for this subject has been created before (in the case of no findings selected)
   
                                        Hashtable ht = new Hashtable();
                                        ht.Add(aeName, "Yes");
                                        if (!ht.ContainsKey(aeName + " sev"))
                                        {
                                            string defVariable = observations.Find(o => o.Name.Equals(subjAE.Name)).DefaultQualifier.Name;
                                            ht.Add(aeName + " sev", subjAE.qualifiers.SingleOrDefault(q => q.Key.Equals(defVariable)).Value);

                                        } 
                                        ht.Add("subjectId", subject.SubjId);
                                        rows.Add(ht);
                                    }
                                    
                                    
                                }
                                #endregion
                                else
                                {
                                    //Not selected by the user, but could be an AE belonging to a medDRA group selected by the user
                                    foreach (var reqObs in requestObservations)
                                    {
                                        string reqObsName = reqObs.Key.ToLower();
                                        List<int> reqObsIds = reqObs.Value;

                                        //Get the Observation Id of the current ae
                                        //If it is in the list of ids of this requestedObsName (i.e. MedDRA group)
                                        //then add the group to ht
                                        int subjObsId = observations.SingleOrDefault(
                                            o => o.Name.ToLower().Equals(aeName)).Id;
                                        if (reqObsIds.Contains(subjObsId))
                                        {
                                            if (rows.Count!=0){
                                                foreach (var ht in rows)
                                                {
                                                    if (!ht.ContainsKey(reqObsName)) ht.Add(reqObsName, "Yes");
                                                }
                                            }
                                            else{
                                                Hashtable ht = new Hashtable();
                                                ht.Add(reqObsName, "Yes");
                                                rows.Add(ht);
                                            }
                                            
                                        }

                                    }

                                }
                            }
                    }
                    #endregion

                    if (rows.Count == 0)
                    {
                        Hashtable ht = new Hashtable();
                        rows.Add(ht);
                    }
                        
                    #region Fill in nulls and Nos
                    foreach (var ht in rows)
                    {
                        if (!ht.ContainsKey("subjectId")) ht.Add("subjectId", subject.SubjId);
                        if (!ht.ContainsKey("arm")) ht.Add("arm", subject.ArmCode);
                        if (!ht.ContainsKey("site")) ht.Add("site", subject.Site);
                        foreach (string obsReq in requestObservations.Keys)
                        {
                            if (!ht.ContainsKey(obsReq.ToLower()))
                            {
                                //If obs was requested and was not added to the ht so far
                                var obs = observations.SingleOrDefault(o => o.Name.ToLower().Equals(obsReq.ToLower()));
                                
                                if (obs != null)
                                {
                                    //This is an individual observation AE or Findings
                                    var obsName = obs.Name.ToLower();
                                    if (obs.DomainCode.Equals("AE"))
                                        ht.Add(obsName, "NO");
                                    else
                                        ht.Add(obsName, null);
                                }
                                else
                                    //This is a group and this subject has no occurence for all its descendent observations
                                    ht.Add(obsReq.ToLower(), "NO");
                            }
                        }
                    }
                    
                    #endregion

                    
                    //Think about not adding hts that have no values for any of the requested observations
                    dataMatrix.AddRange(rows);

                }
                
                //foreach (var group in FindingsBySubject)
                //{
                //    string key = group.Key.ToString();
                //    string subjId = group.Key.subjId;

                //    Hashtable ht = new Hashtable();
                //    //adding Findings
                //    foreach (var subjObs in group)
                //    {
                //        //check if default qualifier is avaialable
                //        //if( observations.Find(o => o.Name.Equals(subjObs.Name)).DefaultQualifierId != null)

                //        string defVariable = observations.Find(o => o.Name.Equals(subjObs.Name)).DefaultQualifier.Name;
                //        if (!ht.ContainsKey("SubjectId")) ht.Add("SubjectId", subjObs.SubjectId);
                //        ht.Add(subjObs.Name.ToLower(), subjObs.qualifiers.SingleOrDefault(q => q.Key.Equals(defVariable)).Value);
                //        if (!ht.ContainsKey("Visit")) ht.Add("Visit", subjObs.Visit);
                //        if (!ht.ContainsKey("Timepoint")) ht.Add("Timepoint", subjObs.ObsStudyTimePoint == null ? "" : subjObs.ObsStudyTimePoint.Name);
                //        if (!ht.ContainsKey("study")) ht.Add("study", subjObs.StudyId);
                //        if (!ht.ContainsKey("arm")) ht.Add("arm", subjO);
                //    }

                //    //
                //    if (subjAdverseEvents.Count != 0)
                //    {
                //        var subjAE = adverseEventsBySubject.SingleOrDefault(a => a.Key.Equals(subjId)); //this will contain a list of AEs
                //        if(subjAE != null)
                //            //The current subject has values for the adverse events queried
                            
                //            foreach (var ae in subjAE)
                //            {
                //                string obsName = ae.Name.ToLower();
                //                if (requestObservations.Keys.Contains(obsName))
                //                {
                //                    if (!ht.ContainsKey(ae.Name)) ht.Add(obsName, "Yes");
                //                }
                //                else {
                //                        foreach (var reqObs in requestObservations)
                //                        {
                //                            string obs = reqObs.Key;
                //                            List<int> obsIds = reqObs.Value;

                //                            int subjObsId = observations.SingleOrDefault(
                //                                o => o.Name.ToLower().Equals(obsName)).Id;
                //                            if (obsIds.Contains(subjObsId))
                //                            {
                //                                if (!ht.ContainsKey(obs)) ht.Add(obs, "Yes");
                //                            }

                //                        }

                //                    }
                                
                               
                                
                //            }
                //    }



                    
                
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error while generating data matrix: "+ e.Message);
            }
#endregion
            List<string> columns = dataMatrix.ElementAt(0).Keys.Cast<string>().ToList();

            Hashtable result = new Hashtable();
            result.Add("data", dataMatrix);
            result.Add("columns", columns);
            return result;
        }

        private async Task<List<MedDRAGroupNode>> getEventsByMedDRA(int projectId, List<Observation> observations)
        {
            List<SubjectObservation> adverseEvents =
                   await _subObservationRepository.FindAllAsync(d => d.DomainCode.Equals("AE"));

            //TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            
	
            //title = textInfo.ToTitleCase(title); //War And Peace
            //var AEsByMedDRA = getAEsByMedDRA(adverseEvents);
            //GroupNode obsCatGrp = new GroupNode();
            List<MedDRAGroupNode> AEsByMedDRA = (
                from ae in adverseEvents
                group ae by ae.qualifiers.SingleOrDefault(q => q.Key.Equals("AESOC")).Value into SOCs
                select new MedDRAGroupNode
                {
                    Variable = "AESOC",
                    Code = SOCs.FirstOrDefault().qualifiers.SingleOrDefault(q => q.Key.Equals("AESOCCD")).Value,
                    Name = "SO: "+SOCs.Key.ToLowerInvariant(),
                    GroupTerm = SOCs.Key.ToLower(),
                    TermIds = (from ae in SOCs
                               select observations.Find(o => o.Name.Equals(ae.Name)).Id).ToList<int>(),
                    Count = SOCs.Count(),
                    Terms = (
                        from ae in SOCs
                        group ae by ae.qualifiers.SingleOrDefault(q => q.Key.Equals("AEHLGT")).Value into HLGTs
                        select new MedDRAGroupNode
                        {
                            Variable = "AEHLGT",
                            Code = HLGTs.FirstOrDefault().qualifiers.SingleOrDefault(q => q.Key.Equals("AEHLGTCD")).Value,
                            GroupTerm = HLGTs.Key.ToLower(),
                            Name = "HLG: "+HLGTs.Key.ToLower(),
                            Count = HLGTs.Count(),
                            TermIds = (from ae in HLGTs
                                       select observations.Find(o => o.Name.Equals(ae.Name)).Id).ToList<int>(),
                            Terms = (
                                from ae in HLGTs
                                group ae by ae.qualifiers.SingleOrDefault(q => q.Key.Equals("AEHLT")).Value into HLTs
                                select new MedDRAGroupNode
                                {
                                    Variable = "AEHLT",
                                    Code = HLTs.FirstOrDefault().qualifiers.SingleOrDefault(q => q.Key.Equals("AEHLTCD")).Value,
                                    GroupTerm = HLTs.Key.ToLower(),
                                    Name = "HLT: "+HLTs.Key.ToLower(),
                                    Count = HLTs.Count(),
                                    TermIds = (from ae in HLTs
                                               select observations.Find(o => o.Name.Equals(ae.Name)).Id).ToList<int>(),
                                    Terms = (
                                        from ae in HLTs
                                        group ae by ae.qualifiers.SingleOrDefault(q => q.Key.Equals("AEDECOD")).Value into PTs
                                        select new MedDRAGroupNode
                                        {
                                            Variable = "AEDECOD",
                                            Code = PTs.FirstOrDefault().qualifiers.SingleOrDefault(q => q.Key.Equals("AEPTCD")).Value,
                                            GroupTerm = PTs.Key.ToLower(),
                                            Name = "PT: "+PTs.Key.ToLower(),
                                            Count = PTs.Count(),
                                            TermIds = (from ae in PTs
                                                       select observations.Find(o => o.Name.Equals(ae.Name)).Id).ToList<int>(),
                                            Terms = (
                                                //from ae in PTs
                                                //group ae by ae.qualifiers.SingleOrDefault(q => q.Key.Equals("AETERM")).Value into TERMs
                                                //select new MedDRAGroupNode
                                                //{
                                                //    Variable = "AETERM",
                                                //    Code = "T_" + TERMs.FirstOrDefault().qualifiers.SingleOrDefault(q => q.Key.Equals("AEPTCD")).Value,
                                                //    GroupTerm = TERMs.Key,
                                                //    Name = TERMs.Key,
                                                //    Terms = (
                                                    from ae in PTs //TERMs
                                                    select new MedDRATermNode { 
                                                        Name = ae.Name,
                                                        Code = observations.FirstOrDefault(o => o.Name.Equals(ae.Name)).ControlledTermStr.ToLower(),
                                                        Id = observations.FirstOrDefault(o => o.Name.Equals(ae.Name)).Id }
                                            ).Distinct().ToList<GenericNode>()
                                                //}
                                                //).ToList<GenericNode>()
                                        }).ToList<GenericNode>()
                                }).ToList<GenericNode>()
                        }).ToList<GenericNode>()
                }).ToList();


            return AEsByMedDRA;

        }

        public async Task<Hashtable> getSubjectData(string projectId, List<int> subjCharacteristicIds)
        {

            List<Observation> subjCharacteristics = null;
            //Get Requested (If any) subject characteristics (Observations)
            if(subjCharacteristicIds!=null && subjCharacteristicIds.Count!=0)
            subjCharacteristics = _observationRepository.FindAll(
                           o => subjCharacteristicIds.Contains(o.Id)//,
                //new List<Expression<Func<SubjectCharacteristic, object>>>()
                //{
                //d=>d.TopicVariable,
                //d => d.Timings//,
                //d => d.DefaultQualifier
                //}
             ).ToList();


            //This will be replaced by call to SQL for studies of a particular project.
            List<string> studyIds = new List<string> { "CRC305A", "CRC305B", "CRC305C", "CRC305D" };


            // A query to Mongo
            List<Subject> subjectData =
                    await _subjectRepository.FindAllAsync(
                    s => studyIds.Contains(s.StudyId) && s.DomainCode.Equals("DM"));
           
            List<Hashtable> subject_table = new List<Hashtable>();
            HashSet<string> SCs = new HashSet<string>() { "arm", "site", "study" };
            

            Hashtable ht;
            foreach (Subject subjData in subjectData)
            {
                ht = new Hashtable();
                ht.Add("subjectId", subjData.SubjId);
                ht.Add("arm", subjData.ArmCode);
                ht.Add("study", subjData.StudyId);
                ht.Add("site", subjData.Site);
                
                if(subjCharacteristics != null)
                    foreach (Observation sc in subjCharacteristics)
                    {
                        string value = subjData.characteristicsValues.SingleOrDefault(q => q.Key.Equals(sc.ControlledTermStr)).Value;
                        ht.Add(sc.ControlledTermStr, value);
                        SCs.Add(sc.ControlledTermStr.ToLower());
                    }

                subject_table.Add(ht);
            }

            Hashtable returnObject = new Hashtable();
            returnObject.Add("scs", SCs);
            returnObject.Add("data", subject_table);

            return returnObject;
        }

        //public async Task<List<Hashtable>> getSubjectData(String studyId, List<string> subjCharacteristics)
        //{
        //    //Query 

        //    // Construct the query string for NOSQL
        //    // e.g. ?STUDYID=CRC305A&DOMAIN=DM&AGE=*&SEX=*&RACE=*&ETHNIC=*&ARM=*
        //    //TEMP 
        //    subjCharacteristics = subjCharacteristics.ConvertAll(d => d.ToUpper());


        //    string query = "?STUDYID=" + studyId + "&DOMAIN=DM";
        //    for (int i = 0; i < subjCharacteristics.Count(); i++)
        //    {
        //        query += "&" + subjCharacteristics[i] + "=*";
        //    }

        //    MongoDbDataRepository mongoDataService = new MongoDbDataRepository();
        //    MongoDataCollection recordSet = null;
        //    recordSet = await mongoDataService.getNoSQLRecords(query);

        //    //List<SubjectObservation> subjectData =
        //    //        await _subObservationRepository.FindAllAsync(d => d.DomainCode.Equals("DM") && d.StudyId.Equals(studyId));

        //    List<Hashtable> observation_list = new List<Hashtable>();
        //    Hashtable obs;
        //    for (int i = 0; i < recordSet.documents.Count(); i++)
        //    {
        //        obs = new Hashtable();
        //        for (int k = 0; k < recordSet.documents[i].fields.Count(); k++)
        //        {
        //            obs.Add(recordSet.documents[i].fields[k].Name,
        //                    recordSet.documents[i].fields[k].value);
        //        }
        //        observation_list.Add(obs);
        //    }
        //    return observation_list;
        //}
        
        //public async Task<List<Hashtable>> getObservationsData(String studyId, String domain, List<string> observations)
        //{
        //    //How this will change:
        //    //observaations will be alist of observation IDs
        //    //observations is a map of observation id and a list of qualifiers
        //    //each observation has a default qualifier (e.g. findings obs default to ORRES, events obs default to a present/absent)
        //    //TEMP 
        //    observations = observations.ConvertAll(d => d.ToUpper());

        //    // Fields needed for the graphs


        //    List<List<MongoField>> combinedList = await getNOSQLData(studyId,domain, observations, domain);
        //    List<List<MongoField>> groupedList = groupNOSQLData(combinedList);

        //    // Format data (remove field names) and input to Hash table
        //    List<Hashtable> observation_list = new List<Hashtable>();
        //    Hashtable obs;

        //    for (int i = 0; i < groupedList.Count(); i++)
        //    {
        //        obs = new Hashtable();

        //        for (int j = 0; j < groupedList[i].Count(); j++)
        //        {
        //            // Check if the field is in the observation list if so flatten the record 
        //            // e.g. BMI = 24
        //            if (observations.Contains(groupedList[i][j].value))
        //            {
        //                obs.Add(groupedList[i][j].value, groupedList[i][j + 1].value);
        //                j++;
        //            }
        //            else
        //            {
        //                obs.Add(groupedList[i][j].Name, groupedList[i][j].value);
        //            }
        //        }
        //        for (int k = 0; k < observations.Count(); k++)
        //        {
        //            if (!obs.Contains(observations[k]))
        //            {
        //                obs.Add(observations[k], null);
        //            }
        //        }
        //        observation_list.Add(obs);
        //    }
        //    return observation_list;
        //}



        private ObservationNode createObsNode(Observation obsObject)
        {
            return new ObservationNode()
            {
                Name = obsObject.ControlledTermStr,
                Id = obsObject.Id,
                Code = obsObject.Name.ToLower()
            };

        }
       
        public List<SubjCharDTO> getSubjectCharacteristics(string projectId)
        {
            List<SubjCharDTO> subjChars = new List<SubjCharDTO>();
            List<Observation> SCs =
             _observationRepository.FindAll(
                            o => o.Studies.Any(s => s.Project.Accession.Equals(projectId)) && o.isSubjCharacteristic==true
                            ).ToList();

            SubjCharDTO scdto;
            foreach (var sc in SCs)
            {
                if (sc.ControlledTermStr.Contains("ARM") || sc.ControlledTermStr.Contains("RF")
                    || sc.ControlledTermStr.Equals("SITEID"))
                    continue;
                scdto = new SubjCharDTO();
                scdto.Id = sc.Id;
                scdto.Name = sc.Name;
                scdto.Code = sc.ControlledTermStr.ToLower();
                subjChars.Add(scdto);
            }
            return subjChars;
        }
    }
}
