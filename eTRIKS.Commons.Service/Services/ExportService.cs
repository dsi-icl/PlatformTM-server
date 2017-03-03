using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.Templates;
using eTRIKS.Commons.Core.Domain.Model.Users.Datasets;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Core.Domain.Model.DesignElements;
using eTRIKS.Commons.Core.Domain.Model.ObservationModel;
using Observation = eTRIKS.Commons.Core.Domain.Model.Observation;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel.SDTM;
using eTRIKS.Commons.Core.Domain.Model.Users.Queries;
using MongoDB.Bson;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace eTRIKS.Commons.Service.Services
{
    public class ExportService
    {
        private readonly IRepository<HumanSubject, string> _subjectRepository;
        private readonly IRepository<DatasetTemplate, string> _domainTemplate;
        private readonly IRepository<CharacteristicFeature, int> _characObjRepository;
        private IServiceUoW _dataContext;
        private readonly IRepository<Core.Domain.Model.Observation, int> _observationRepository;
        private readonly IRepository<Visit, int> _visitRepository;
        private readonly IRepository<SubjectCharacteristic,int> _subjectCharacteristicRepository;
        private readonly IRepository<Study, int> _studyRepository;
        private readonly IRepository<Arm, string> _armRepository;
        private readonly IRepository<SdtmRow, Guid> _sdtmRepository;

        // Vahid  added start ******************************************************************************************************************
        private readonly IRepository<Biosample, int> _bioSampleRepository;
        private DataExplorerService _explorerService;  // TO FIND ASSAY
        private readonly IRepository<Assay, int> _assayRepository;  // TO FIND BIOSAMPLES 
        // also added @DataExplorerService explorerService@ to  public ExportService(IServiceUoW uoW, DataExplorerService explorerService)
        // Vahid  added finish ******************************************************************************************************************



        public ExportService(IServiceUoW uoW, DataExplorerService explorerService)
        {
            _dataContext = uoW;
            _subjectRepository = uoW.GetRepository<HumanSubject, string>();
            _observationRepository = uoW.GetRepository<Core.Domain.Model.Observation, int>();
            _characObjRepository = uoW.GetRepository<CharacteristicFeature, int>();
            _domainTemplate = uoW.GetRepository<DatasetTemplate, string>();
            _visitRepository = uoW.GetRepository<Visit, int>();
            _subjectCharacteristicRepository = uoW.GetRepository<SubjectCharacteristic, int>();
            _studyRepository = uoW.GetRepository<Study,int>();
            _armRepository = uoW.GetRepository<Arm, string>();
            _sdtmRepository = uoW.GetRepository<SdtmRow, Guid>();

            // Vahid  added start ******************************************************************************************************************
            _bioSampleRepository = uoW.GetRepository<Biosample, int>();
            _explorerService = explorerService;
            _assayRepository = uoW.GetRepository<Assay, int>();
            // Vahid  added finish ******************************************************************************************************************

        }

        #region OLD METHODS
        public List<TreeNodeDTO> GetAvailableFields(int projectId)
        {

            var roots = new List<TreeNodeDTO>();

            //1- Design Elements
            var DEnodes = createOrFindTreeNode(roots, "DEelems", "Design Elements","", true);
            //Arm Node
            var armNode = createOrFindTreeNode(DEnodes.Children, "ARM", "Arm", "string", false, typeof(Arm), "Name");
            //Study Node
            var studyNode = createOrFindTreeNode(DEnodes.Children, "STUDY", "Study", "string", false, typeof(Study), "Name");
            //Visit nodes
            var visitNode = createOrFindTreeNode(DEnodes.Children, "VISIT", "Visit", "string", true);
            var visitNameNode = createOrFindTreeNode(visitNode.Children, "vname", "Visit Name", "string", false, typeof(Visit), "Name");
            var visitStudyDayNode = createOrFindTreeNode(visitNode.Children, "vsday", "Study Day", "string", false, typeof(Visit), "StudyDay");


            //2-Study Characteristics
            var subjCharsnode = createOrFindTreeNode(roots, "subj", "Subject Characterisitcs", "", true);
            var SCs = _characObjRepository.FindAll(sco => sco.ProjectId == projectId).ToList();

            foreach (var sc in SCs)
            {
                var scnode = createOrFindTreeNode(subjCharsnode.Children, sc.Id.ToString(), sc.FullName, "", false);
                scnode.Field = new DataFieldDTO()
                {
                    FieldName = sc.ShortName,
                    Entity = typeof(SubjectCharacteristic).FullName,
                    EntityId = sc.Id, //AGE//Sex//Race...
                    Property = "ShortName",
                    DataType = sc.ShortName == "AGE" ? "integer" : "string", //TEMP till add datatype to charObj
                    IsFiltered = false,
                    //DisplayName = "SubjectCharacteristic" + " [" + sc.FullName + "]",
                };
            }


            //3- Observations
            List<Observation> studyObservations =
                   _observationRepository.FindAll(
                       o => o.ProjectId == projectId,
                       new List<string>(){
                           "Timings.Qualifier",
                           //"Studies.Project",
                           "TopicVariable",
                           "Qualifiers.Qualifier"
                       }
                       ).ToList();

            var groupedByClDmGp = studyObservations.GroupBy(ob => new { ob.Class, ob.DomainCode, ob.DomainName, ob.Group });

            int i = 0;
            foreach (var obsGrp in groupedByClDmGp)
            {
                i++;
                var classNode = createOrFindTreeNode(roots, obsGrp.Key.Class.ToUpper(), obsGrp.Key.Class, "", true);
                var domainNode = createOrFindTreeNode(classNode.Children, obsGrp.Key.DomainCode.ToUpper(), obsGrp.Key.DomainName, "", true);
                var groupNode = obsGrp.Key.Group == null ? domainNode :
                    createOrFindTreeNode(domainNode.Children, obsGrp.Key.DomainCode.ToUpper() + "_grp_" + i, obsGrp.Key.Group, "", true);

                foreach (var obs in obsGrp)
                {
                    var node = createOrFindTreeNode(groupNode.Children, obs.Id.ToString(), obs.ControlledTermStr, "", true);

                    foreach (var q in obs.Qualifiers.Select(o=>o.Qualifier))
                    {
                        var qnode = createOrFindTreeNode(node.Children, node.Id + "_" + q.Id, q.Label, q.DataType, false);
                        qnode.Field = new DataFieldDTO()
                        {
                            FieldName = obs.Name + " [" + q.Name + "]",
                            Entity = typeof(ObjectOfObservation).FullName,//if entity is always going to be SubjectObservation, then property should be
                            EntityId = obs.Id,
                            Property = q.Name,//obs.DomainCode.Equals("AE")?"AEDECOD":obs.TopicVariable.Name,//SHOULD BE O3
                            PropertyId = q.Id,
                            DataType = q.DataType,
                            IsFiltered = false,
                            //DisplayName = obs.DomainName + " - " + (obs.Group != null ? (obs.Group + " - ") : "") + obs.Name + " [" + q.Name + "]", //can ge from o3 from db

                        };
                    }
                    foreach (var q in obs.Timings.Select(t=>t.Qualifier))
                    {
                        var qnode = createOrFindTreeNode(node.Children, node.Id + "_" + q.Id, q.Label, q.DataType, false);
                        qnode.Field = new DataFieldDTO()
                        {
                            FieldName = obs.Name + " [" + q.Name + "]",
                            Entity = typeof(ObjectOfObservation).FullName,//if entity is always going to be SubjectObservation, then property should be
                            EntityId = obs.Id,
                            Property = q.Name,//obs.DomainCode.Equals("AE")?"AEDECOD":obs.TopicVariable.Name,//SHOULD BE O3
                            PropertyId = q.Id,
                            DataType = q.DataType,
                            IsFiltered = false,
                            //DisplayName = obs.DomainName + " - " + (obs.Group != null ? (obs.Group + " - ") : "") + obs.Name + " [" + q.Name + "]", //can ge from o3 from db

                        };
                    }
                }
            }



            return roots;
        }
        
        public DataFilterDTO GetFieldValueSet(int projectId, DataFieldDTO field )
        {
            //TODO: 

            //ParameterExpression parameter = Expression.Parameter(typeof(Foo), "x");
            //Expression property = Expression.Property(parameter, propertyName);
            //Expression target = Expression.Constant(inputText);
            //Expression equalsMethod = Expression.Call(property, "Equals", null, target);
            //Expression<Func<Foo, bool>> lambda =
            //   Expression.Lambda<Func<Foo, bool>>(equalsMethod, parameter);



            var filter = new DataFilterDTO();

            if (field.Entity == typeof(Study).FullName)
            {
                var studies = _studyRepository.FindAll(s => s.ProjectId == projectId);
                filter.Field = field;
                filter.ValueSet = studies.Select(s => s.Name).ToList();
            }

            else if (field.Entity == typeof(Visit).FullName)
            {
                var visits = _visitRepository.FindAll(v => v.Study.ProjectId == projectId);
                //visits.GroupBy(v=>v.Study)
                var days = visits.Select(v => v.StudyDay.Number).ToList();
                filter.Field = field;
                //filter.ValueSet = days;
            }
            else if (field.Entity == typeof(Arm).FullName)
            {
                var arms = _armRepository.FindAll(a => a.Studies.Select(s=>s.Study).All(s => s.ProjectId == projectId));//Studies.All(t=>t.Project.Accession == projectAcc));
                //visits.GroupBy(v=>v.Study)
                var vals = arms.Select(v => v.Name).ToList();
                filter.Field = field;
                filter.ValueSet = vals;
            }


            //Will get values for subj charaterisitcs from SQL
            else if (field.Entity == typeof(SubjectCharacteristic).FullName)
            {
                var subjectObservations = _subjectCharacteristicRepository.FindAll(sc => sc.CharacteristicFeatureId == field.EntityId);

                var vals = subjectObservations.Select(s => s.VerbatimValue).Distinct().ToList();
                filter.Field = field;
                filter.ValueSet = vals;

                //TODO:do datatype int
            }

            //Valuesets for Observations

            else if (field.Entity == typeof(ObjectOfObservation).FullName)
            {
                //var observation =_observationRepository.FindSingle(o => o.Id == field.EntityId, new List<Expression<Func<Observation, object>>>() { o=>o.TopicVariable });

                //_dataContext.AddClassMap(observation.TopicVariable.Name, "Name");
                //var observationData = await _subObservationRepository.FindAllAsync(
                //    d => d.Name.Equals(observation.Name) &&  d.ProjectAcc == projectAcc);
                List<SdtmRow> observationData = _sdtmRepository.FindAll(s => s.DBTopicId == field.EntityId).ToList();

                var domainCode = observationData.First().DomainCode;
                
                //    d => d.Name.Equals(observation.Name) &&  d.ProjectAcc == projectAcc);
                if (field.DataType.Equals("string"))
                {

                    HashSet<string> vals = new HashSet<string>();
                    foreach (var q in observationData)
                    {
                        string val;
                        if (q.ResultQualifiers.TryGetValue(field.Property, out val))
                            vals.Add(val);
                        else if (q.Qualifiers.TryGetValue(field.Property, out val))
                            vals.Add(val);
                    }
                    var dis = vals.Distinct().ToList();
                    dis.Remove("");
                    filter.Field = field;
                    filter.ValueSet = dis;
                }
                else if (field.DataType.Equals("DataTime"))
                {
                    throw new NotImplementedException();
                }
                else
                {
                    HashSet<double> vals = new HashSet<double>();
                    foreach (var q in observationData)
                    {
                        double val;

                        
                        if (double.TryParse(q.ResultQualifiers[field.Property], out val))
                            vals.Add(val);
                    }
                    filter.Field = field;
                    filter.Max = vals.Max();
                    filter.Min = vals.Min();
                    filter.To = vals.Max();
                    filter.From = vals.Min();
                    filter.IsNumeric = true;
                    filter.Unit = observationData.First().QualifierQualifiers[domainCode+"ORRESU"];
                }
            }

            return filter;
        }

        public Hashtable ExportDataTable(int projectId, UserDatasetDTO userDatasetDto)
        {
            UserDataset userdataset = getUserDataset(userDatasetDto);
            DataExportObject exportData =  GetExportData(projectId, userdataset);

            var dataTable = CreateDataTable(exportData,userdataset);
            var ht = getHashtable(dataTable);
            return ht;
        }

        //public async Task<List<TreeNodeDTO>> ExportDataTree(string projectAcc, List<Criterion> criteria)
        //{

        //    List<SubjectObservation> observations = await GetExportData(projectAcc, criteria);

        //    var dataTree = getDataTree(observations);

        //    return dataTree;
        //}

        #endregion

        #region Checkout Methods
        public DataExportObject GetDatasetContent(int projectId, UserDataset userDataset)
        {

            var exportData = new DataExportObject
            {
                Subjects = _subjectRepository.FindAll(
                    s => s.Study.ProjectId == projectId,
                    new List<string>() {"StudyArm", "Study"}).ToList()
            };

            //QUERY FOR CLINICAL OBSERVATIONS
            var observationQueries = userDataset.Fields.FindAll(f => f.QueryObjectType == nameof(SdtmRow)).Select(f=>f.QueryObject).ToList();
            exportData.Observations = getObservations(observationQueries);

            //if sapleF
            

            foreach (var selField in userDataset.Fields)
            {
                switch (selField.QueryObjectType)
                {
                    case nameof(SubjectCharacteristic):
                        //TODO:Need to do a separate query for dates
                        var obsQuery = selField.QueryObject;

                        //QUERY FOR SUBJECT CHARACTERISITIC (e.g. AGE)
                        var characteristics = _subjectCharacteristicRepository.FindAll(
                           sc => sc.Subject.Study.ProjectId == projectId && obsQuery.TermId == sc.CharacteristicFeatureId,
                           new List<string>() { "Subject" }).ToList();

                        //APPLY FILTERING IF FILTER PRESENT
                        if (characteristics.Any() && obsQuery.IsFiltered)
                        {
                            characteristics = (obsQuery.DataType == "string")
                                ? characteristics.FindAll(sc => obsQuery.FilterExactValues.Contains(sc.VerbatimValue))
                                : characteristics.FindAll(sc =>
                                                            int.Parse(sc.VerbatimValue) >= obsQuery.FilterRangeFrom &&
                                                            int.Parse(sc.VerbatimValue) <= obsQuery.FilterRangeTo);
                        }

                        //ADD TO EXPORT DATA 
                        exportData.SubjChars.AddRange(characteristics);
                        exportData.IsSubjectIncluded = true;
                        break;
                    //case nameof(SampleCharacteristic):
                    //    //TODO:Need to do a separate query for dates
                    //    var obsQuery = selField.QueryObject;
                    //    var characteristics = _subjectCharacteristicRepository.FindAll(
                    //       sc => sc.Subject.Study.ProjectId == projectId && obsQuery.TermId == sc.CharacteristicFeatureId,
                    //       new List<string>() { "Subject" }).ToList();

                    //    if (characteristics.Any() && obsQuery.IsFiltered)
                    //    {
                    //        characteristics = (obsQuery.DataType == "string")
                    //            ? characteristics.FindAll(sc => obsQuery.FilterExactValues.Contains(sc.VerbatimValue))
                    //            : characteristics.FindAll(sc =>
                    //                                        int.Parse(sc.VerbatimValue) >= obsQuery.FilterRangeFrom &&
                    //                                        int.Parse(sc.VerbatimValue) <= obsQuery.FilterRangeTo);
                    //    }
                    //    exportData.SubjChars.AddRange(characteristics);
                    //    exportData.IsSubjectIncluded = true;
                    //    break;




                     //  added start ******************************************************************************************************************
                        case nameof(SampleCharacteristic):
                        // you need to get all the samples for the assay and added to exportData object then access it from exportr servoive after looping over subjects to write the samples for each subject. 
                        //For now it has only two raw which are subjectID and sample id but then this need to be expanded if sample characteristics are included 

                        /*
                                                //added the property of  AssayPanels in DatasetField.cs   *********          Ibrahim said it is not necessery to use assayPanel in this method to access assayId insetad we use queryObject
                                                var AssayPanelQuery = selField.AssayPanels;
                                                var samples1 = _bioSampleRepository.FindAll(bs =>bs.Subject.Study.ProjectId == projectId && bs.AssayId == AssayPanelQuery.AssayId, new List<string>() { "Subject" }).ToList();
                         */

                        //  QQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQ What does the following actually do????? "Biosamples.Study", "Biosamples.Subject"
                        var observationQuery = selField.QueryObject;
                        observationQuery.TermId = 111;

                        var samples = _bioSampleRepository.FindAll(bs => bs.Subject.Study.ProjectId == projectId && bs.AssayId == observationQuery.TermId, new List<string>() { "Biosamples.SampleCharacteristics", "Biosamples.Subject" }).ToList();
                       
                        // If filter is used for samples then samples should be chosen accordingly
                       if (samples.Any() && observationQuery.IsFiltered)
                        {

                            //samples = (observationQuery.DataType == "string")
                            //    ? samples.FindAll(bs => observationQuery.FilterExactValues.Contains(bs.VerbatimValue))
                            //    : samples.FindAll(bs => int.Parse(bs.VerbatimValue) >= observationQuery.FilterRangeFrom && int.Parse(bs.VerbatimValue) <= observationQuery.FilterRangeTo);
                        }

                        exportData.Samples.AddRange(samples);
                      
                        //added the bool of  IsSampleIncluded in DataExportObject.cs ********* 
                        exportData.IsSampleIncluded = true;

                        break;

                    // added finish ******************************************************************************************************************
                    
                    case nameof(Arm):
                        if (selField.QueryObject.IsFiltered)
                            exportData.Arms = _armRepository.FindAll(
                                a => a.Studies.Select(s => s.Study).All(s => s.ProjectId == projectId)
                                && selField.QueryObject.FilterExactValues.Contains(a.Name)).ToList();
                        else
                            exportData.Arms = _armRepository.FindAll(
                                a => a.Studies.Select(s=>s.Study.ProjectId).Contains(projectId)).ToList();
                        break;
                    case nameof(Study):
                        var studies = _studyRepository.FindAll(
                            s => s.ProjectId == projectId
                            && (selField.QueryObject.IsFiltered && selField.QueryObject.DataType.Equals("string"))
                                ? selField.QueryObject.FilterExactValues.Contains(s.Name)
                                : s.ProjectId == projectId,
                            new List<string>() { "Subjects" }).ToList();
                        exportData.Studies = studies;
                        break;
                    case nameof(Visit):
                        var visits = _visitRepository.FindAll(
                            v => v.Study.ProjectId == projectId
                            //apply filter if present
                            ).ToList();
                        exportData.Visits = visits;
                        break;
                }
            }

            exportData.FilterAndJoin();

            return exportData;
        }

        private List<SdtmRow> getObservations(List<ObservationQuery> Queries)
        {
            List<SdtmRow> observations = new List<SdtmRow>();

            var queriesByO3Id = Queries.GroupBy(f => f.QueryObjectName).ToList();
            foreach (var sameO3queries in queriesByO3Id)
            {
                var o3q_g = sameO3queries.Select(group => group).First();
                var o3q_list = new List<ObservationQuery>();
                var obs_list = new List<SdtmRow>();

                //EXPANIDNG ALL to GROUP  TO ACCOMMODATE FOR GROUP OBSERVATIONS AS WELL IN THE SAME CODE BLOCK
                if (o3q_g.GetType().Name == nameof(GroupedObservationsQuery))
                {
                    ((GroupedObservationsQuery)o3q_g).GroupedObservations.ForEach(oq=> o3q_list.Add(oq));
                }
                else
                    o3q_list.Add(o3q_g);
                //////////////////////////////////////////////////////////////////////////////////////////////


                foreach (var o3q in o3q_list)
                {
                   obs_list.AddRange(o3q.IsOntologyEntry
                        ? _sdtmRepository.FindAll(s => s.QualifierQualifiers[o3q.TermCategory] == o3q.TermId.ToString() && s.Group == o3q.Group && s.ProjectId == o3q.ProjectId).ToList()
                        : _sdtmRepository.FindAll(s => s.DBTopicId == o3q.TermId && s.ProjectId == o3q.ProjectId).ToList());

                }

                foreach (var oq in sameO3queries) //AEOCCUR / AESEV
                {
                    if (oq.IsFiltered)
                    {
                        //HACK FOR FINDINGS SHOULD GO AWAY IN THE NEW OBSERVATION MODEL
                        obs_list.ForEach(o => o.Qualifiers = o.ResultQualifiers.Union(o.Qualifiers).ToDictionary(p => p.Key, p => p.Value));

                        string v = null;
                        obs_list = oq.DataType == "string"
                                    ? obs_list.FindAll(s => s.Qualifiers.TryGetValue(oq.PropertyName, out v) && oq.FilterExactValues.Contains(s.Qualifiers[oq.PropertyName]))
                                    : obs_list.FindAll(s => float.Parse(s.Qualifiers[oq.PropertyName]) >= oq.FilterRangeFrom &&
                                                                float.Parse(s.Qualifiers[oq.PropertyName]) <= oq.FilterRangeTo);
                    }
                }
                observations.AddRange(obs_list);
            }

            //Retrieve rows for requested individual observations //i.e not ontology entry

            //var observationsIDs = Queries.Where(r => r.GetType().Name != nameof(GroupedObservationsQuery) && !r.IsOntologyEntry).Select(o => o.TermId).ToList();
            //observations = _sdtmRepository.FindAll(s => observationsIDs.Contains(s.DBTopicId) && s.ProjectId == projectId).ToList();


            //var queries = Queries.FindAll(or => or.IsOntologyEntry); //NON GROUP ONES

            ////ASSUMPTION: GROUPED OBSERVATINS ARE ALWAYS ONTOLOGY ENTRY QUERIES!
            //var groupQueries = Queries.FindAll(q => q.GetType().Name == nameof(GroupedObservationsQuery)).Cast<GroupedObservationsQuery>();
            //var obsGroupQueries = groupQueries.SelectMany(og => og.GroupedObservations);
            //queries.AddRange(obsGroupQueries);

            //foreach (var query in queries)
            //{
            //    var obs_list = _sdtmRepository.FindAll(
            //        s => s.QualifierQualifiers[query.TermCategory] == query.TermId.ToString()
            //             && s.Group == query.Group
            //             && s.ProjectId == projectId).ToList();
            //    if (!obs_list.TrueForAll(observations.Contains))
            //        observations.AddRange(obs_list);

            //}


            //switch (query.GetType().Name)
            //{
            //    case nameof(ObservationQuery):
            //        var oq = (ObservationQuery) query;

            //        var obs_list = oq.IsOntologyEntry 
            //            ? _sdtmRepository.FindAll(s => s.QualifierQualifiers[oq.TermCategory] == oq.TermId.ToString() && s.Group == oq.Group && s.ProjectId == query.ProjectId).ToList()
            //            : _sdtmRepository.FindAll(s => s.DBTopicId == oq.TermId && s.ProjectId == query.ProjectId).ToList();

            //        //HACK FOR FINDINGS SHOULD GO AWAY IN THE NEW OBSERVATION MODEL
            //        obs_list.ForEach(o=>o.Qualifiers = o.ResultQualifiers.Union(o.Qualifiers).ToDictionary(p=>p.Key,p=>p.Value));


            //        string v = null;
            //        if (oq.IsFiltered)
            //            obs_list = oq.DataType == "string"
            //                    ? obs_list.FindAll(s => s.Qualifiers.TryGetValue(oq.PropertyName, out v) && oq.FilterExactValues.Contains(s.Qualifiers[oq.PropertyName]))
            //                    : obs_list.FindAll(s => float.Parse(s.Qualifiers[oq.PropertyName]) >= oq.FilterRangeFrom &&
            //                                                float.Parse(s.Qualifiers[oq.PropertyName]) <= oq.FilterRangeTo);
            //        observations = obs_list;
            //    break;
            //    case nameof(GroupedObservationsQuery):
            //        var groupQuery = (GroupedObservationsQuery)query;
            //        foreach (var obq in groupQuery.GroupedObservations)
            //        {
            //            var groupobservations = obq.IsOntologyEntry
            //                ? _sdtmRepository.FindAll(s => s.QualifierQualifiers[obq.TermCategory] == obq.TermId.ToString() && s.Group == obq.Group && s.ProjectId == query.ProjectId).ToList()
            //                : _sdtmRepository.FindAll(s => obq.TermId == s.DBTopicId && s.ProjectId == query.ProjectId).ToList();

            //            if (groupQuery.IsFiltered)
            //                groupobservations = obq.DataType == "string"
            //                    ? groupobservations.FindAll(
            //                        s => groupQuery.FilterExactValues.Contains(s.Qualifiers[groupQuery.PropertyName]))
            //                    : groupobservations.FindAll(
            //                        s =>
            //                            int.Parse(s.Qualifiers[groupQuery.PropertyName]) >= groupQuery.FilterRangeFrom &&
            //                            int.Parse(s.Qualifiers[groupQuery.PropertyName]) <= groupQuery.FilterRangeTo);

            //            observations.AddRange(groupobservations);
            //            //obsGrpReq.TermIds.AddRange(observations.Select(o => o.DBTopicId));
            //        }
            //        break;
            //}
            return observations.ToList();
        }

        private List<SdtmRow> applyObsFilters(List<SdtmRow> observations, List<DatasetField> fields)
        {
            var filteredObservations = new List<SdtmRow>();
            string v;

            var fieldsByO3Id = fields.GroupBy(f => f.QueryObject.QueryObjectName).ToList();
            foreach (var fieldgrp in fieldsByO3Id)
            {
                foreach (var field in fieldgrp) //AEOCCUR / AESEV
                {
                    var query = field.QueryObject;
                    if (query.IsFiltered)
                    {
                        if (query.IsOntologyEntry)
                            filteredObservations = query.DataType == "string"
                                ? observations.FindAll(
                                    o => o.QualifierQualifiers[query.TermCategory] == query.TermId.ToString()
                                    && o.Group == query.Group
                                    && o.Qualifiers.TryGetValue(query.PropertyName, out v) 
                                    && query.FilterExactValues.Contains(o.Qualifiers[query.PropertyName]))

                                : observations.FindAll(
                                    o => float.Parse(o.Qualifiers[query.PropertyName]) >= query.FilterRangeFrom
                                    && float.Parse(o.Qualifiers[query.PropertyName]) <= query.FilterRangeTo);
                           


                    }
                }
            }

            return filteredObservations;
        }



        // added start ******************************************************************************************************************
       // you need BsonTypeMapperOptions seperate two methods BsonTypeMapperOptions not get confused 
        //public DataTable GetSampleTable(DataExportObject exportData, UserDataset dataset)
        //{
        //    return null;
            
        //    //if (exportData.IsSampleIncluded)
        //    //{
        //    //    datatable.Columns.Add("subjectid");
        //    //    datatable.Columns.Add("sampleid");
        //    //    var sampleCharacsFields = dataset.Fields.FindAll(f => f.QueryObjectType == nameof(SampleCharacteristic)).ToList();

        //    //    foreach (var sample in exportData.Samples)
        //    //    {
        //    //        var row = datatable.NewRow();

        //    //        row["studyid"] = subject.Study.Name;
        //    //        row["subjectid"] = subject.UniqueSubjectId;
        //    //        row["sampleid"] = sample.BiosampleStudyId;

        //    //        foreach (var sampleCharField in sampleCharacsFields)
        //    //        {
        //    //            var charVal = sample.SampleCharacteristics.SingleOrDefault(
        //    //                            sc => sc.CharacteristicFeatureId.Equals(sampleCharField.QueryObject.TermId));
        //    //            if (charVal != null)
        //    //                row[sampleCharField.ColumnHeader.ToLower()] = charVal.VerbatimValue;

        //    //        }

        //    //        datatable.Rows.Add(row);
        //    //    }
        //    //}


        //}
        // added start ******************************************************************************************************************



        public DataTable GetDatasetTable(DataExportObject exportData, UserDataset dataset)
        {
            #region Create Table Columns
            var datatable = new DataTable();

            //datatable.Columns.Add("subjectid");
            datatable.Columns.Add("studyid");
            foreach (var field in dataset.Fields)
            {
                datatable.Columns.Add(field.ColumnHeader.ToLower());
            }

            #endregion

            /////TEMP////
            exportData.IsSampleIncluded = true;

            //var subjGroupedObservations = exportData.Observations.GroupBy(ob => new { subjId = ob.USubjId });

            var fieldsByO3Id = dataset.Fields.FindAll(f=> f.QueryObjectType == nameof(SdtmRow)).GroupBy(f => f.QueryObject.QueryObjectName).ToList();
            var subjCharacsFields = dataset.Fields.FindAll(f => f.QueryObjectType == nameof(SubjectCharacteristic)).ToList();
            
            foreach (var subject in exportData.Subjects)
            {

                // added start ******************************************************************************************************************
                if (exportData.IsSampleIncluded)
                {
                    datatable.Columns.Add("subjectid");
                    datatable.Columns.Add("sampleid");
                    var sampleCharacsFields = dataset.Fields.FindAll(f => f.QueryObjectType == nameof(SampleCharacteristic)).ToList();

                    foreach (var sample in exportData.Samples)
                    {
                        var row = datatable.NewRow();
                       
                        row["studyid"] = subject.Study.Name;
                        row["subjectid"] = subject.UniqueSubjectId;
                        row["sampleid"] = sample.BiosampleStudyId;

                        foreach (var sampleCharField in sampleCharacsFields)
                        {
                            var charVal = sample.SampleCharacteristics.SingleOrDefault(
                                            sc => sc.CharacteristicFeatureId.Equals(sampleCharField.QueryObject.TermId));
                            if (charVal != null)
                                row[sampleCharField.ColumnHeader.ToLower()] = charVal.VerbatimValue;

                        }

                        datatable.Rows.Add(row);
                    }
                }
                // added finish******************************************************************************************************************

                var uniqSubjectId = subject.UniqueSubjectId;
                var subjectObservations = exportData.Observations.FindAll(o => o.USubjId == uniqSubjectId).ToList();
                var subjectCharacteristics = exportData.SubjChars.FindAll(sc => sc.SubjectId == subject.Id).ToList();
                //var uniqSubjectId = subjectObservations.FirstOrDefault().USubjId;




                var firstRow = false;
                while (subjectObservations.Any() || firstRow )
                {
                    var row = datatable.NewRow();
                    firstRow = false;
                    
                    #region Design Elements
                    row["subjectid"] = uniqSubjectId;
                    row["studyid"] = subject.Study.Name;
                    //arm//visit//studt//
                    #endregion

                    #region Subject Characteristics

                    foreach (var subjCharField in subjCharacsFields)
                    {
                        var charVal = subjectCharacteristics.SingleOrDefault(
                                        sc => sc.CharacteristicFeatureId.Equals(subjCharField.QueryObject.TermId));
                        if (charVal != null)
                            row[subjCharField.ColumnHeader.ToLower()] = charVal.VerbatimValue;

                    }

                    #endregion

                    #region WRITE CLINICAL OBSERVATIONS

                    foreach (var fieldgrp in fieldsByO3Id)//HEADACHE //BMI (EVENTS AND FINDINGS TOGETHER)//NOTE .. TIMEING ARE NOT synchronized YET
                    {
                        SdtmRow obs = null;
                        foreach (var field in fieldgrp) //AEOCCUR / AESEV
                        {
                            //ONTOLOGY TERM REQUEST
                            if (field.QueryObject.IsOntologyEntry)
                                obs = subjectObservations.FirstOrDefault(
                               o => ((ObservationQuery)field.QueryObject).TermId.ToString() == o.QualifierQualifiers[field.QueryObject.TermCategory]);

                            //GROUP OF OBSERVATIONS
                            else if (field.QueryObject.GetType() == typeof(GroupedObservationsQuery))
                            {
                                //ASSUMPTION: GROUPS AREONLY COMPOSED OF ONTOLOGY ENTRY
                                //ASSUMPTION: 
                                string v;
                                foreach (var obsQuery in ((GroupedObservationsQuery)field.QueryObject).GroupedObservations)
                                {
                                    obs = subjectObservations.FirstOrDefault(
                                        o=> o.QualifierQualifiers.TryGetValue(obsQuery.TermCategory, out v)
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

                            

                           //WRITE OBSERVATION INSTANCE TO ROW

                           string val = "";
                           obs?.Qualifiers.TryGetValue(((ObservationQuery)field.QueryObject).PropertyName, out val);
                           if(val == null)
                               obs?.ResultQualifiers.TryGetValue(((ObservationQuery)field.QueryObject).PropertyName, out val);
                           row[field.ColumnHeader.ToLower()] = val;   
                       }
                        subjectObservations.Remove(obs);
                    }

                    #endregion

                    datatable.Rows.Add(row);
                }
            }
            return datatable;
        }

        
        #endregion

        #region Private Methods

        private DataTable CreateDataTable(DataExportObject exportData, UserDataset userDataset)
        {
            #region Build Table columns
            var datatable = new DataTable();
            //var projections = new List<string>();
            datatable.Columns.Add("SUBJECTID");
            datatable.Columns.Add("STUDY");

            // Vahid  added start ******************************************************************************************************************
            // to be editted accordingly 
            //  datatable.Columns.Add("SAMPLEID");
            // Vahid  added finish *****************************************************************************************************************



            foreach (var field in userDataset.Fields)
            {
                datatable.Columns.Add(field.FieldName.ToLower());
            }

            //if selected fields contain them or if a selected field has timeseries then show them by default
            datatable.Columns.Add("VISIT");
            datatable.Columns.Add("TIMEPOINT");
            #endregion

            #region Group observations by unique key
            var findingsBySubjectVisitTime = exportData.Observations //The unique combination that would make one row
                .GroupBy(ob => new
                {
                    subjId = ob.USubjId,
                    Visit = ob.VisitName,
                    Day = ob.CollectionStudyDay == null ? "" : ob.CollectionStudyDay.Number.ToString(),
                    Timepoint = ob.CollectionStudyTimePoint == null ? "" : ob.CollectionStudyTimePoint.Name
                });
            #endregion

            foreach (var subjVisitTPT in findingsBySubjectVisitTime)
            {
                var row = datatable.NewRow();

                row["SUBJECTID"] = subjVisitTPT.Key.subjId;
                row["VISIT"] = subjVisitTPT.Key.Visit;
                row["TIMEPOINT"] = subjVisitTPT.Key.Timepoint;
                row["STUDY"] = subjVisitTPT.FirstOrDefault()?.StudyId;

                //foreach (var field in userDataset.Fields)
                //{
                //    if (field.Entity == typeof(ObjectOfObservation).FullName)
                //        row[field.FieldName.ToLower()] = subjVisitTPT.FirstOrDefault(o=>o.DBTopicId == field.EntityId)?.ResultQualifiers.SingleOrDefault(q => q.Key.ToLower().Equals(field.Property.ToLower())).Value;
                //    //row[field.FieldName.ToLower()] = subjVisitTPT.FirstOrDefault(o => o.DBTopicId == field.EntityId)?.Qualifiers.SingleOrDefault(q => q.Key.ToLower().Equals(field.Property.ToLower())).Value;

                //    if (field.Entity == typeof(Arm).FullName)
                //        row[field.FieldName.ToLower()] = exportData.GetArmForSubject(subjVisitTPT.Key.subjId);

                //    if (field.Entity == typeof(SubjectCharacteristic).FullName)
                //        row[field.FieldName.ToLower()] =
                //            exportData.GetSubjCharacterisiticForSubject(subjVisitTPT.Key.subjId, field.EntityId);
                //}
                datatable.Rows.Add(row);
            }

            return datatable;
        }

        private DataExportObject GetExportData(int projectId, UserDataset userDataset)
        {

            DataExportObject exportData = new DataExportObject();

            //1- Query for observations
            foreach (var selField in userDataset.Fields)
            {
                var queryFields = new List<object>(); //List of all filters to query Mongo


                //if (selField.Entity == typeof(ObjectOfObservation).FullName)
                //{
                //    //var observation = _observationRepository.FindSingle(o => o.Id == selField.EntityId, new List<Expression<Func<Observation, object>>>() { o => o.TopicVariable });

                //    ////VSTESTCD, BMI
                //    //queryFields.Add(new Dictionary<string, string> { { observation.TopicVariable.Name, observation.Name } });

                //    //if (selField.IsFiltered)
                //    //{
                //    //    var filters = userDataset.Filters.FindAll(f => f.DataField.FieldName.Equals(selField.FieldName)).ToList();
                //    //    if (filters.Count != 0)
                //    //        queryFields.AddRange(filters.Select(AddFilter));
                //    //}

                //    //queryFields.Add(new Dictionary<string, string> { { "DBPROJECTACC", projectAcc } });

                //    //List<SubjectObservation> subjectObservations = await _subObservationRepository.FindAllAsync(filterFields: queryFields);
                //    //foreach (var subjectObservation in subjectObservations)
                //    //{
                //    //    subjectObservation.O3Id = selField.EntityId;
                //    //}

                //    List<SdtmRow> observationData = _sdtmRepository.FindAll(s => s.DBTopicId == selField.EntityId).ToList();
                //    exportData.Observations.AddRange(observationData);
                //    exportData.IsSubjectIncluded = true;
                //}
                //else if (selField.Entity == typeof(SubjectCharacteristic).FullName)
                //{
                //    var characteristics = _subjectCharacteristicRepository.FindAll(
                //        sc => sc.Subject.Study.ProjectId == projectId && selField.EntityId == sc.CharacteristicFeatureId,
                //        new List<string>() { "Subject" }).ToList();
                //    //APPLY filter here?
                //    exportData.SubjChars.AddRange(characteristics);
                //    exportData.IsSubjectIncluded = true;

                //}
                //else if (selField.Entity == typeof(Arm).FullName)
                //{
                //    //var arms = _armRepository.FindAll(a => a.Studies.All(s => s.Project.Accession == projectAcc)).ToList();
                //    ////Apply filter?
                //    //exportData.Arms = arms;
                //    //exportData.SubjectArms = _subjectRepository.FindAll(s => s.Study.Project.Accession == projectAcc,
                //    //    new List<Expression<Func<HumanSubject, object>>>() { s => s.StudyArm }).ToList();
                //    //exportData.StudyArms = _studyRepository.FindAll(s => s.Project.Accession == projectAcc,
                //    //    new List<Expression<Func<Study, object>>>() { s => s.Arms }).ToList();
                //    fillExportDataArms(exportData, projectId);
                //}
                //else if (selField.Entity == typeof(Study).FullName)
                //{
                //    var studies = _studyRepository.FindAll(s => s.ProjectId == projectId, new List<string>() { "Subjects" }).ToList();
                //    exportData.Studies = studies;
                //}
                //else if (selField.Entity == typeof(Visit).FullName)
                //{
                //    var visits = _visitRepository.FindAll(v => v.Study.ProjectId == projectId).ToList();
                //    exportData.Visits = visits;
                //}
            }

            //2- Do filterings
            foreach (var filter in userDataset.Filters)
            {
                //if (filter.DataField.Entity == typeof(ObjectOfObservation))
                //{
                //    if (typeof(DataFilterExact) == filter.GetType())
                //        exportData.Observations = exportData.Observations.FindAll(
                //            s => s.O3Id == filter.DataField.EntityId &&
                //            ((DataFilterExact) filter).Values.Contains(s.qualifiers[filter.DataField.Property]));

                //}
                if (filter.DataField.Entity == typeof(Study).FullName)
                {
                    exportData.Observations = exportData.Observations.FindAll(o => ((DataFilterExact)filter).Values.Contains(o.StudyId));
                }
                else if (filter.DataField.Entity == typeof(Visit).FullName)
                {
                    if (filter.DataField.Property == "StudyDay")
                        exportData.Observations = exportData.Observations.FindAll(o => o.CollectionStudyDay.Number >= ((DataFilterRange)filter).Lowerbound && o.CollectionStudyDay.Number <= ((DataFilterRange)filter).Upperbound);
                    if (filter.DataField.Property == "Name")
                        exportData.Observations = exportData.Observations.FindAll(o => ((DataFilterExact)filter).Values.Contains(o.VisitName));
                }
                else if (filter.DataField.Entity == typeof(Arm).FullName)
                {
                    if (exportData.Subjects == null)
                        fillExportDataArms(exportData, projectId);

                    exportData.Subjects = exportData.Subjects?.FindAll(s => ((DataFilterExact)filter).Values.Contains(s.StudyArm.Name));
                    exportData.Observations =
                        exportData.Observations?.FindAll(
                            o => exportData.Subjects.Select(s => s.UniqueSubjectId).Contains(o.SubjectId));
                }
                else if (filter.DataField.Entity == typeof(SubjectCharacteristic).FullName)
                {

                    if (typeof(DataFilterExact) == filter.GetType())
                    {
                        if (exportData.SubjChars == null)
                            fillSubjCharData(exportData, filter.DataField, projectId);
                        exportData.SubjChars = exportData.SubjChars.FindAll(
                        sc => sc.CharacteristicFeatureId == filter.DataField.EntityId &&
                        ((DataFilterExact)filter).Values.Contains(sc.VerbatimValue));

                        exportData.Observations =
                            exportData.Observations?.FindAll(o => exportData.SubjChars.Select(sc => sc.SubjectId).Contains(o.SubjectId));

                    }
                    else
                    {
                        //TODO:
                    }

                    //observations = observations.Join(subjectIds, o => o.SubjectId, s => s, (s, o) => s).ToList();

                }
            }

            #region old query
            ////FIELD here is refering to Mongo document field 
            //foreach (var criterion in criteria)
            //{
            //    var filterFields = new List<object>();
            //    var filterField = new Dictionary<string, object>();
            //    filterField.Add("DBPROJECTACC", projectAcc);
            //    filterFields.Add(filterField);

            //    //will be querying for subjectObservations
            //    //do the filtering first then do the projection
            //    //the thing is some filters will only apply to a particular observation such as filtering on VSORRES range x:y
            //    //this would only apply when querying for VSTEST weight for instance
            //    //so in a way selected observation fields should be the first obejct to iterate through knowing that they are observation fields
            //    //because other filters like studyId and visit would apply on all observations 
            //    //if I interate over the select observation fields then I will be retrieving all the observation information 
            //    //then if for instance a filter on Weight[VSORRES] exists an another on Weight[VSCOMPLETE] these are filters on the specific observation
            //    //to do the filters inside the observation field or separate?
            //    //to do the filtering at the DB or in the service layer

            //    // 

            //    //Filter for O3 and any other related discrete valued QO2 (e.g. VSTESTCD:"BMI", VSLOC:"x","y")
            //    //(e.g. AEDECOD:"INJECTION SITE ...", AESEV:("mo","sev"), AEPAT:("CONT")
            //    //should also have group and domain to make sure we have the right obs
            //    //this is UNTIL we search for obs by Id not by name
            //    //should expect to have LBCAT:Urinology and DOMAIN:LB
            //    foreach (var filter in criterion.ExactFilters)
            //    {
            //        filterField = new Dictionary<string, object>();
            //        //IMPLICIT AND between filters of different fields
            //        string fieldName = filter.Property;
            //        List<string> values = filter.Values;


            //        //CONSTRUCT the equivalent of a BSON $OR document for multiple filters on the same field
            //        var valueList = new List<Dictionary<string, string>>();
            //        foreach (var value in values)
            //        {
            //            var filterValue = new Dictionary<string, string> { { fieldName, value } }; //{"STUDYID":"CRC305C"}
            //            valueList.Add(filterValue);// [{"STUDYID":"CRC305C"},{"STUDYID":"CRC305A"}]
            //        }

            //        filterField.Add("$or", valueList);
            //        filterFields.Add(filterField);
            //    }

            //    //Filters for numeric fields/variables (e.g. VSORRES)
            //    //WILL NOT HOLD the same field filter for two different O3s
            //    foreach (var filter in criterion.RangeFilters)
            //    {
            //        filterField = new Dictionary<string, object>();
            //        //IMPLICIT AND between filters of different fields
            //        var fieldName = filter.Property;
            //        var range = filter.Range;


            //        //CONSTRUCT the equivalent of a BSON {field : {$gt:v,$lt:v2}}
            //        var valueRange = new Dictionary<string, string>();
            //        valueRange.Add("$gt", range.Lowerbound.ToString());
            //        valueRange.Add("$lt", range.Upperbound.ToString());
            //        filterField.Add(fieldName, valueRange);
            //        filterFields.Add(filterField);
            //    }

            //    List<SubjectObservation> subjectData = await _subObservationRepository.FindAllAsync(filterFields: filterFields);
            //    observations.AddRange(subjectData);
            //}
            #endregion

            //userDataset.Fields

            return exportData;
        }

        private void fillExportDataArms(DataExportObject exportData, int projectId)
        {
            var arms = _armRepository.FindAll(a => a.Studies.Select(s => s.Study).All(s => s.ProjectId == projectId)).ToList();
            //Apply filter?
            exportData.Arms = arms;
            exportData.Subjects = _subjectRepository.FindAll(s => s.Study.ProjectId == projectId,
                new List<string>() { "StudyArm" }).ToList();
            //exportData.StudiesWithArmsIncluded = _studyRepository.FindAll(s => s.ProjectId == projectId,
            //    new List<string>() { "Arms" }).ToList();
        }

        private void fillSubjCharData(DataExportObject exportData, DataField field, int projectId)
        {
            var characteristics = _subjectCharacteristicRepository.FindAll(
                        sc => sc.Subject.Study.ProjectId == projectId && field.EntityId == sc.CharacteristicFeatureId,
                        new List<string>() { "Subject" }).ToList();
            //APPLY filter here?
            //exportData = exportData;
            exportData.SubjChars.AddRange(characteristics);
            exportData.IsSubjectIncluded = true;

        }

        private List<TreeNodeDTO> getDataTree(List<SubjectObservation> observations)
        {
            var roots = new List<TreeNodeDTO>();
            var r = createOrFindTreeNode(roots, "Pid", "P-BVS","group",true);
            var groupedByClDmGp = observations.GroupBy(ob => new { ob.Class, ob.DomainCode, ob.DomainName});

            int i = 0;
            foreach (var obsGrp in groupedByClDmGp)
            {
                i++;

                var domain = _domainTemplate.FindSingle(d => d.Code.Equals(obsGrp.Key.DomainCode),
                    new List<string>()
                    {
                        "Fields"
                    });

                var Class = domain.Class;
                var Domain = domain.Domain;

                var classNode = createOrFindTreeNode(r.Children, Class.ToLower(), Class,"group",true);
                var domainNode = createOrFindTreeNode(classNode.Children, obsGrp.Key.DomainCode.ToUpper(), Domain,"group", true);
                //var groupNode = obsGrp.Key.Group == null ? domainNode :
                //    createOrFindTreeNode(domainNode.Children, obsGrp.Key.DomainCode.ToUpper() + "_grp_" + i, obsGrp.Key.Group, true);

                foreach (var obs in obsGrp)
                {
                    //Topic Variable
                    var topicVarNode = createOrFindTreeNode(domainNode.Children, "tpc_" + obs.DomainCode, "Topic","group", true);
                    createOrFindTreeNode(topicVarNode.Children, obs.StandardName.Replace(' ', '_'), obs.Name, "char", false);
                    
                    //Group variables
                    var groupNode = createOrFindTreeNode(domainNode.Children, "GrpQuals_" + obs.DomainCode, "Grouping Qualifier", "group", true);
                    TreeNodeDTO catNode;
                    if (obs.Group != null)
                    {
                        catNode = createOrFindTreeNode(groupNode.Children, "catQuals_" + obs.DomainCode, "Category for " + Domain, "group", true);
                        createOrFindTreeNode(catNode.Children, obs.Group.ToLower(), obs.Group,"char", false);
                    }

                    //ResultQualifiers
                    var resQualsNode = createOrFindTreeNode(domainNode.Children, "ResQuals_" + obs.DomainCode, "Result Qualifier", "group", true);
                   
                    foreach (var q in obs.qualifiers)
                    {
                        float res;
                        string type;
                        type = float.TryParse(q.Value, out res) ? "numeric" : "char";
                        var v = domain.Fields.FirstOrDefault(d => d.Name.Equals(q.Key));
                        if (v == null) continue;
                            createOrFindTreeNode(resQualsNode.Children, resQualsNode.Id + q.Key, v.Label, type,false);
                    }

                    //TimingQualfiers
                    var timeQualsNode = createOrFindTreeNode(domainNode.Children, "TimeQuals_" + obs.DomainCode, "Timing Qualifier", "group",true);

                    foreach (var q in obs.timings)
                    {
                        float res;
                        string type;
                        type = float.TryParse(q.Value, out res) ? "numeric" : "char";
                        var v = domain.Fields.FirstOrDefault(d => d.Name.Equals(q.Key));
                        createOrFindTreeNode(timeQualsNode.Children, timeQualsNode.Id + q.Key, v.Label,type, false);
                    }
                        
                    
                    //for each obs create a group node for Result qualifier, Identifier (topic,synonym...etc),timing,qualifier
                }
               
            }

            return roots;
        }
        
        private Hashtable getHashtable(DataTable sdtmTable)
        {
            var ht = new Hashtable();
            var headerList = new List<Dictionary<string, string>>();
            foreach (var col in sdtmTable.Columns.Cast<DataColumn>())
            {
                var header = new Dictionary<string, string>
                {
                    {"data", col.ColumnName.ToLower()},
                    {"title", col.ColumnName.ToUpper()}
                };
                headerList.Add(header);
            }
            ht.Add("header", headerList);
            ht.Add("data", sdtmTable);
            return ht;
        }

        private TreeNodeDTO createOrFindTreeNode(List<TreeNodeDTO> parent, string nodeId, string nodeText, string type, bool groupNode, Type entity = null, string property = null)
        {
            TreeNodeDTO node;
            if (parent != null)
            {
                node = parent.Find(r => r.Text.Equals(nodeText));
                if (node != null)
                    return node;
            }

            node = new TreeNodeDTO
            {
                Id = nodeId,
                Text = nodeText,
                Children = groupNode ? new List<TreeNodeDTO>() : null,
                IsGroup = groupNode,
                Type = type,
                Field = new DataFieldDTO()
                {
                    Entity = entity?.FullName,
                    Property = property,
                    //DisplayName = entity != null ? entity.Name + " [" + property + "]" : "",
                    FieldName = entity != null ? entity.Name + " [" + property +"]": "",
                    DataType = type,
                    IsFiltered = false,
                }
            };


            parent.Add(node);
            return node;

        }

        private UserDataset getUserDataset(UserDatasetDTO dto)
        {
            UserDataset ds = new UserDataset()
            {
                Name = dto.Name,
                Description = dto.Description,
                Tags = dto.Tags,
                OwnerId = dto.OwnerId,
                ProjectId = dto.ProjectId
            };

            
            foreach (var filterDto in dto.Filters)
            {
                
                if (filterDto.IsNumeric)
                {
                    var filter = new DataFilterRange();
                    filter.DataField = getDataField(filterDto.Field);
                    filter.Lowerbound = filterDto.From;
                    filter.Upperbound = filterDto.To;
                    ds.Filters.Add(filter);
                }
                else
                {
                    var filter = new DataFilterExact();
                    filter.DataField = getDataField(filterDto.Field);
                    filter.Values = filterDto.FilterValues;
                    ds.Filters.Add(filter);
                }
                
            }

            //foreach (var fieldDto in dto.Fields)
            //{
            //    ds.Fields.Add(getDataField(fieldDto));
            //}
            return ds;
        }

        private DataField getDataField(DataFieldDTO dto)
        {
            var field = new DataField()
            {
                Entity = dto.Entity,
                EntityId = dto.EntityId,
                Property = dto.Property,
                PropertyId = dto.PropertyId,
                DataType = dto.DataType,
                IsFiltered = dto.IsFiltered,
                FieldName = dto.FieldName
            };
            return field;
        }
        #endregion

    }
}
