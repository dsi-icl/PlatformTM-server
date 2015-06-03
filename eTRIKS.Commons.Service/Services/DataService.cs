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
        public async Task<List<Hashtable>> getObservationsData(string studyId, List<int> observationsIDs)
        {
            //studyId = "CRC305C";
            List<Hashtable> dataMatrix = new List<Hashtable>();
            
            //observationsIDs = new List<int>();
            //observationsIDs.Add(942);
            //observationsIDs.Add(948);

            Dictionary<string, List<int>> requestObservations = new Dictionary<string, List<int>>();
            //observationsIDs = requestObservations.Values.;
            
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



            //Query for observation data from Mongo
            List<SubjectObservation> subjObservations = new List<SubjectObservation>();
            List<SubjectObservation> subjAdverseEvents = new List<SubjectObservation>();

            foreach(Observation observation in observations){
                string obsName = observation.Name ;
                string fieldName = observation.TopicVariable.Name;
                _dataContext.AddClassMap(fieldName, "Name");
                List<SubjectObservation> observationData =
                    await _subObservationRepository.FindAllAsync(d => d.Name.Equals(obsName));

                if (observation.DomainCode.Equals("AE"))
                    subjAdverseEvents.AddRange(observationData);
                else
                    subjObservations.AddRange(observationData);
            }

            var adverseEventsBySubject = subjAdverseEvents
                .GroupBy(ob => ob.SubjectId);

            var adverseEventsByAE = subjAdverseEvents
                .GroupBy(ob => ob.Name);

            //Group data by subject, visit and timepoint
            var groupedBySubject = subjObservations
                .GroupBy(ob => new { 
                    subjId = ob.SubjectId,
                    Visit = ob.Visit,
                    //Day = ob.ObsStudyDay == null ? -999 : ob.ObsStudyDay.Number,
                    Timepoint = ob.ObsStudyTimePoint == null ? "" : ob.ObsStudyTimePoint.Name
                    });

            //Create a datamatrix to sendback to crossfilter
            //One row per subject per visit per timepoint

            try
            {
                foreach (var group in groupedBySubject)
                {
                    string key = group.Key.ToString();
                    string subjId = group.Key.subjId;

                    Hashtable ht = new Hashtable();
                    //adding Findings
                    foreach (var subjObs in group)
                    {
                        //check if default qualifier is avaialable
                        //if( observations.Find(o => o.Name.Equals(subjObs.Name)).DefaultQualifierId != null)

                        string defVariable = observations.Find(o => o.Name.Equals(subjObs.Name)).DefaultQualifier.Name;
                        if (!ht.ContainsKey("SubjectId")) ht.Add("SubjectId", subjObs.SubjectId);
                        ht.Add(subjObs.Name, subjObs.qualifiers.SingleOrDefault(q => q.Key.Equals(defVariable)).Value);
                        if (!ht.ContainsKey("Visit")) ht.Add("Visit", subjObs.Visit);
                        if (!ht.ContainsKey("Timepoint")) ht.Add("Timepoint", subjObs.ObsStudyTimePoint == null ? "" : subjObs.ObsStudyTimePoint.Name);
                    }

                    //
                    if (subjAdverseEvents.Count != 0)
                    {
                        var subjAE = adverseEventsBySubject.SingleOrDefault(a => a.Key.Equals(subjId)); //this will contain a list of AEs
                        if(subjAE != null)
                            //The current subject has values for the adverse events queried
                            
                            foreach (var ae in subjAE)
                            {
                                if(requestObservations.Keys.Contains(ae.Name))
                                    if (!ht.ContainsKey(ae.Name)) ht.Add(ae.Name, "Yes");
                                    else {
                                        foreach (var reqObs in requestObservations)
                                        {
                                            string obs = reqObs.Key;
                                            List<int> obsIds = reqObs.Value;
                                            int subjObsId = observations.SingleOrDefault(
                                                o => o.Name.Equals(ae.Name)).Id;
                                            if (obsIds.Contains(subjObsId))
                                            {
                                                if (!ht.ContainsKey(obs)) ht.Add(obs, "Yes");
                                            }

                                        }

                                    }
                                
                               
                                
                            }
                    }
                    

        
                    foreach (var obs in observations)
                    {
                        if (!ht.ContainsKey(obs.Name))
                            if(obs.DomainCode.Equals("AE"))
                                 ht.Add(obs.Name, "NO");
                            else
                                ht.Add(obs.Name, null);
                    }

                    dataMatrix.Add(ht);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error while generating data matrix: "+ e.Message);
            }
            

            return dataMatrix;
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
                    GroupTerm = SOCs.Key,
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
                            GroupTerm = HLGTs.Key,
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
                                    GroupTerm = HLTs.Key,
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
                                            GroupTerm = PTs.Key,
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
                ht.Add("arm", subjData.Arm);
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
