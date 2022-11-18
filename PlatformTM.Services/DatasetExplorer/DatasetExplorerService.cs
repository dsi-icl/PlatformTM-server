using System;
using PlatformTM.Core.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using PlatformTM.Core.Domain.Model.BMO;
using System.Collections;
using PlatformTM.Models.DTOs.Explorer;
using System.Threading.Tasks;
using System.Data;

namespace PlatformTM.Services.DatasetExplorer
{
    public class DatasetExplorerService
    {
        private readonly IRepository<Core.Domain.Model.BMO.Observation, Guid> _observationRepository;
        private readonly IRepository<Core.Domain.Model.BMO.Feature, int> _featureRepository;
        //private readonly IRepository<HumanSubject, string> _subjectRepository;
        //private readonly IRepository<CharacteristicFeature, int> _characObjRepository;
        //private readonly IRepository<SdtmRow, Guid> _sdtmRepository;
        //private readonly IRepository<Assay, int> _assayRepository;
        //private readonly ICacheRepository<ClinicalExplorerDTO> _cacheRepository;
        //private readonly IRepository<Biosample, int> _biosampleRepository;
        private readonly IServiceUoW _dataContext;
        //private readonly QueryService _queryService;

        public DatasetExplorerService(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _observationRepository = uoW.GetRepository<Core.Domain.Model.BMO.Observation, Guid>();
            _featureRepository = uoW.GetRepository<Core.Domain.Model.BMO.Feature, int>();
            //_subjectRepository = uoW.GetRepository<HumanSubject, string>();
            //_characObjRepository = uoW.GetRepository<CharacteristicFeature, int>();
            //_sdtmRepository = uoW.GetRepository<SdtmRow, Guid>();
            //_assayRepository = uoW.GetRepository<Assay, int>();
            //_biosampleRepository = uoW.GetRepository<Biosample, int>();
            //_cacheRepository = uoW.GetCacheRepository<ClinicalExplorerDTO>();
            //_queryService = queryService;

        }


        #region initMethods

        //public async Task<Hashtable> GetInitAssayData(int projectId)
        //{
        //    var assayTask = GetProjectAssays(projectId);
        //    var xfdataTask = GetAllAssaySamples(projectId);
        //    var assays = await assayTask;
        //    var XFdata = await xfdataTask;


        //    var ExplorerAssaysVM = new Hashtable();
        //    ExplorerAssaysVM.Add("assays", assays);
        //    ExplorerAssaysVM.Add("xfdata", XFdata);
        //    return ExplorerAssaysVM;

        //}

        //public async Task<SubjectExplorerDTO> GetInitSubjectData(int projectId)
        //{
        //    //var subjChars = GetSubjectCharacteristics(projectId);
        //    var xfData = await GetSubjectData(projectId);
        //    //var result = await subjChars;
        //    //SubjectExplorerDTO subjectExplorerDTO = await xfData;
        //    //result.Xfdata = subjectExplorerDTO.Xfdata;
        //    return xfData;
        //}

        #endregion


        #region Observation Browser methods


        public List<Core.Domain.Model.BMO.Feature> GetObservationPhenos(int datasetId)
        {
            var ObservedFeatures = _featureRepository
                .FindAll(f => f.DatasetId == datasetId, new List<string> { "ObservablePhenomena.ObservedProperty" });
            ClinicalExplorerDTO clinicalExplorerDTO = new ClinicalExplorerDTO();
            var classNode = new ClinicalDataTreeDTO();
            clinicalExplorerDTO.Classes.Add(classNode);
            classNode.Class = "";


            var domainNode = new GroupNode
            {
                Name = ObservedFeatures.First().Domain,
                Code = ObservedFeatures.First().Domain,
                //Count = domainGroup.Count(),
                IsDomain = true
            };
            classNode.Domains.Add(domainNode);

            var groupedBy = ObservedFeatures.GroupBy(d => d.Category);
            int i = 0;
            foreach (var group in groupedBy)
            {
                i++;
                var cgroupNode = new GroupNode();
                domainNode.Terms.Add(cgroupNode);
                cgroupNode.Name = group.Key;
                cgroupNode.Code = domainNode.Code + "_cat" + ++i;

                foreach (var feature in group)
                    cgroupNode.Terms.Add(createObsNode(feature));
            }

            return ObservedFeatures.ToList();
        }

        //private void PopulateTreeView(TreeNodeCollection nodes, string path)
        //{
        //    try
        //    {
        //        string[] dirs = Directory.GetDirectories(path);

        //        foreach (string dir in dirs)
        //        {
        //            nodes.Add(dir, Path.GetFileName(dir));
        //        }

        //        foreach (TreeNode node in nodes)
        //        {
        //            PopulateTreeView(node.Nodes, node.Name);
        //        }
        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        return;
        //    }
        //    catch
        //    {
        //        return;
        //    }
        //}

        public async Task<ClinicalExplorerDTO> GetClinicalObsTree(int datasetId)
        {
            var ObservedFeatures = _featureRepository
               .FindAll(f => f.DatasetId == datasetId, new List<string> { "ObservablePhenomena.ObservedProperty" });
            ClinicalExplorerDTO clinicalExplorerDTO = new ClinicalExplorerDTO();
            var classNode = new ClinicalDataTreeDTO();
            clinicalExplorerDTO.Classes.Add(classNode);
            classNode.Class = "";


            var domainNode = new GroupNode
            {
                Name = ObservedFeatures.First().Domain,
                Code = ObservedFeatures.First().Domain,
                //Count = domainGroup.Count(),
                IsDomain = true
            };
            classNode.Domains.Add(domainNode);

            var groupedBy = ObservedFeatures.GroupBy(d => d.Category);
            int i = 0;
            foreach (var group in groupedBy)
            {
                i++;
                var cgroupNode = new GroupNode();
                domainNode.Terms.Add(cgroupNode);
                cgroupNode.Name = group.Key;
                cgroupNode.Code = domainNode.Code + "_cat" + ++i;

                foreach (var feature in group)
                    cgroupNode.Terms.Add(createObsNode(feature));
            }

            return clinicalExplorerDTO;
        }

        public async Task<ClinicalExplorerDTO> GetClinicalObsTrees(int datasetId)
        {
            ClinicalExplorerDTO clinicalExplorerDTO = new ClinicalExplorerDTO();
            //clinicalExplorerDTO.ProjectId = projectId;



            var ObservedFeatures = _featureRepository.FindAll(f => f.DatasetId == datasetId);
            //            o => o.ProjectId == projectId,
            //            new List<string>()
            //            {
            //                "DefaultQualifier",
            //                "Qualifiers.Qualifier",
            //                "Timings.Qualifier"
            //            }).ToList();

            var classNode = new ClinicalDataTreeDTO();
            clinicalExplorerDTO.Classes.Add(classNode);
            classNode.Class = "";


            var domainNode = new GroupNode
            {
                Name = ObservedFeatures.First().Domain,
                Code = ObservedFeatures.First().Domain,
                //Count = domainGroup.Count(),
                IsDomain = true
            };
            classNode.Domains.Add(domainNode);


            var tempCategoryNodes = new Dictionary<string, GroupNode>();
            var tempSubcategoryNodes = new Dictionary<string, GroupNode>();

            var groupedByCatAndSubCat = ObservedFeatures.GroupBy(x => new { x.Category, x.Subcategory }).OrderBy(g => g.Key);
            int i = 0;
            foreach (var group in groupedByCatAndSubCat)
            {
                var currentGroupNode = domainNode;
                if (group.Key != null)
                {
                    GroupNode cgroupNode, sgroupNode;

                    GroupNode tempCatNode;
                    if (tempCategoryNodes.TryGetValue(group.Key.Category, out tempCatNode))
                    {
                        cgroupNode = tempCatNode;
                        currentGroupNode = tempCatNode;
                    }


                    else
                    {
                        cgroupNode = new GroupNode();
                        domainNode.Terms.Add(cgroupNode);
                        cgroupNode.Name = group.Key.Category;
                        cgroupNode.Code = domainNode.Code + "_cat" + ++i;
                        //groupNode.Count = catGroup.Distinct().Count();
                        tempCategoryNodes.Add(group.Key.Category, cgroupNode);
                        currentGroupNode = cgroupNode;

                    }

                    if (group.Key.Subcategory == "")
                        break;
                    GroupNode tempSubCatNode;
                    if (tempSubcategoryNodes.TryGetValue(group.Key.Category + group.Key.Subcategory, out tempSubCatNode))
                    {
                        sgroupNode = tempSubCatNode;
                        currentGroupNode = sgroupNode;
                    }


                    else
                    {
                        sgroupNode = new GroupNode();
                        cgroupNode.Terms.Add(sgroupNode);
                        sgroupNode.Name = group.Key.Subcategory;
                        sgroupNode.Code = domainNode.Code + "_scat" + ++i;
                        //groupNode.Count = catGroup.Distinct().Count();
                        tempCategoryNodes.Add(group.Key.Category + group.Key.Subcategory, sgroupNode);
                        currentGroupNode = sgroupNode;
                    }

                }
                {
                    foreach (var feature in group)
                        currentGroupNode.Terms.Add(createObsNode(feature));
                }
            }
            return clinicalExplorerDTO;

        }



        #endregion

        #region Crossfilter data methods

        //public Hashtable GetObservationsData(int projectId, List<ObservationRequestDTO> reqObservations)
        //{

        //    //GETTING THE Observations for the requested features/properties (i.e. the phenomenon)
        //    var sdtmObservations = _getObservations(reqObservations, projectId);



        //    var clinicalData = _getClinicalDataJson(sdtmObservations, reqObservations);

        //    return clinicalData;
        //}


        //private List<Observation> _getObservations(List<ObservationRequestDTO> obsRequests, int datasetId)
        //{
        //    List<Observation> subjectObservations = new List<Observation>();

        //    //Retrieve rows for requested individual observations //i.e not ontology entry
        //    //var observationsIDs = obsRequests.Where(r => !r.IsMultipleObservations).Select(o => o.O3id).ToList();
        //    //sdtmObservations = _sdtmRepository.FindAll(s => observationsIDs.Contains(s.DBTopicId) && s.ProjectId == projectId).ToList();

        //    // I will assume this phenoIds
        //    //var obsPhenomenaIds = obsRequests.Where(r => !r.IsMultipleObservations).Select(o => o.O3id).ToList();
        //    var requestedObsFeatureIds = obsRequests.Select(r => r.ObsFeatureId).ToList();

        //   var _allSubjectObservations =  _observationRepository.FindAll(o=> o.DatasetId == datasetId && requestedObsFeatureIds.Contains(o.FeatureOfInterestId)).ToList();

        //    return subjectObservations;
        //}



        private Hashtable _getClinicalDataJson(List<Core.Domain.Model.BMO.Observation> observations, IList<ObservationRequestDTO> reqObservations)
        {
            var keys = new HashSet<string>();
            var data = new List<Hashtable>();
            var result = new Hashtable {
                {"data", data},
                {"keys", keys}
            };

            if (!observations.Any())
                return result;

            keys.Add("subjectId");
            foreach (var r in reqObservations)
            {
                keys.Add(r.ObsPhenoName);
            }

            var subjectIds = observations.Select(s => s.SubjectId).Distinct().ToList();
            //assuming here that all subjectIds are in here

            foreach (var subjectId in subjectIds)
            {
                var subjectObservations = observations.FindAll(o => o.SubjectId == subjectId);
                var subjdata = new Hashtable();
                subjdata.Add("subjectId", subjectId);


                foreach (var req in reqObservations)
                {
                    var obsPheno = subjectObservations.Select(o => o.ObservedPhenomena.Where(p => p.ObservedPhenomenonId == req.ObsPhenoId));

                    var test = (NominalObsResult)obsPheno.FirstOrDefault();

                    subjdata.Add(req.ObsPhenoName, test.Value);
                }
                ;

                //if (values != null && values.Count != 0)
                //{
                //    values = values.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                //    if (values.Count != 0)
                //        subjdata.Add(req.Name, values);
                //    //subjdata.Add(req.Name, values.OrderByDescending(v => v));
                //}



                //subjdata.Add("visit", "");// = subjVisitTPT.Key.Visit;//subjObs.VisitName;
                //subjdata.Add("timepoint", "");//row["timepoint"] = subjVisitTPT.Key.Timepoint;//subjObs.ObsStudyTimePoint == null ? "" : subjObs.ObsStudyTimePoint.Name;

                data.Add(subjdata);

            }


            return result;

        }




        private ObservationNode createObsNode(Core.Domain.Model.BMO.Feature obsFeature)
        {
            var node = new ObservationNode
            {
                Name = obsFeature.Name,
                Id = obsFeature.Id.ToString(),
                Code = obsFeature.Name,
                //Id = obsObject.Id.ToString(),
                //Code = obsObject.Name.ToLower(),
                Qualifiers = createObsRequests(obsFeature),

                DefaultObservation = new Models.DTOs.Explorer.ObservationRequestDTO()
                {
                    O3 = obsFeature.Name,
                    O3id = obsFeature.Id,
                    O3code = obsFeature.Name,
                    QO2 = obsFeature.ObservablePhenomena.First().ObservedProperty.Name,
                    QO2id = obsFeature.ObservablePhenomena.First().ObservedProperty.Id,
                    DataType = "string",
                    QO2_label = obsFeature.ObservablePhenomena.First().ObservedProperty.Name,
                    IsEvent = false,
                    IsFinding = true,
                    IsClinicalObservations = true,
                    ProjectId = 1,
                    //QueryFrom = nameof(SdtmRow),
                    //QueryFor = nameof(SdtmRow), //
                    //QueryWhereProperty = nameof(SdtmRow.DBTopicId),
                    //QueryWhereValue = obsObject.Id.ToString(),
                    //HasLongitudinalData = obsObject.Timings.Any(),//WRONG...not specific to O3
                    //HasTPT = obsObject.Timings.Exists(t => t.Qualifier.Name.EndsWith("TPT"))
                    //QuerySelectProperty = nameof()
                }
            };

            //if (obsObject.ControlledTermStr != null) return node;
            //node.Name = obsObject.Name;
            //node.DefaultObservation.O3 = obsObject.Name;

            return node;

        }

        private List<Models.DTOs.Explorer.ObservationRequestDTO> createObsRequests(Core.Domain.Model.BMO.Feature obsObject, bool IsMultiple = false, bool IsOntologyEntry = false)
        {
            var allQualifiers = obsObject.ObservablePhenomena.Select(q => q.ObservedProperty).ToList();
            allQualifiers.AddRange(obsObject.FeatureProperties);

            var reqs = allQualifiers.Select(variableDefinition => new Models.DTOs.Explorer.ObservationRequestDTO()
            {

                O3 = obsObject.Name,//obsObject.ControlledTermStr,
                O3id = obsObject.Id,
                O3code = obsObject.Name,
                QO2 = variableDefinition.Name,
                QO2id = variableDefinition.Id,
                DataType = "string",
                QO2_label = variableDefinition.Name,
                IsEvent = false,
                IsFinding = true,
                IsClinicalObservations = true,
                IsMultipleObservations = IsMultiple,
                IsOntologyEntry = IsOntologyEntry,
                ProjectId = 1,
                QueryFrom = nameof(ObservablePhenomenon)

                //TermIds = termIds,
                //Id = obsObject.Name.ToLower() + "_" + variableDefinition.Name
                //Qualifiers = obsObject.Qualifiers.Select(q => q.Name).ToList(),
                //DefaultQualifier = obsObject.DefaultQualifier.Name
            }).ToList();
            return reqs;
        }


        #endregion

        public DataTable GetDatasetAsTable(int datasetId)
        {
            var observations = _observationRepository.FindAll(o => o.DatasetId == datasetId).ToList();

            return new DataTable();
        }

    }
}
