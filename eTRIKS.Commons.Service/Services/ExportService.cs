using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.Templates;
using eTRIKS.Commons.Core.Domain.Model.Users.Datasets;
using eTRIKS.Commons.Service.DTOs;

namespace eTRIKS.Commons.Service.Services
{
    public class ExportService
    {
        private IRepository<HumanSubject, string> _subjectRepository;
        private IRepository<DomainTemplate, string> _domainTemplate;
        private readonly IRepository<SubjectObservation, Guid> _subObservationRepository;
        private readonly IRepository<CharacteristicObject, int> _characObjRepository;
        private IServiceUoW _dataContext;
        private readonly IRepository<Observation, int> _observationRepository;

        public ExportService(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _subObservationRepository = uoW.GetRepository<SubjectObservation, Guid>();
            _subjectRepository = uoW.GetRepository<HumanSubject, string>();
            _observationRepository = uoW.GetRepository<Observation, int>();
            _characObjRepository = uoW.GetRepository<CharacteristicObject, int>();
            _domainTemplate = uoW.GetRepository<DomainTemplate, string>();
        }
        private async Task<List<SubjectObservation>> ExportDataset(string projectAcc, List<Criterion> criteria)
        {
            List<SubjectObservation> observations = new List<SubjectObservation>();
            //Prepare Query
            foreach (var criterion in criteria)
            {
                var filterFields = new List<object>();
                var filterField = new Dictionary<string, object>();
                filterField.Add("DBPROJECTACC", projectAcc);
                filterFields.Add(filterField);


                //Filter for O3 and any other related discrete valued QO2 (e.g. VSTESTCD:"BMI", VSLOC:"x","y")
                                                                        //(e.g. AEDECOD:"INJECTION SITE ...", AESEV:("mo","sev"), AEPAT:("CONT")
                                                                        //should also have group and domain to make sure we have the right obs
                                                                        //this is UNTIL we search for obs by Id not by name
                                                                        //should expect to have LBCAT:Urinology and DOMAIN:LB
                foreach (var filter in criterion.ExactFilters)
                {
                    filterField = new Dictionary<string, object>();
                    //IMPLICIT AND between filters of different fields
                    string fieldName = filter.Field;
                    List<string> values = filter.Values;


                    //CONSTRUCT the equivalent of a BSON $OR document for multiple filters on the same field
                    var valueList = new List<Dictionary<string, string>>();
                    foreach (var value in values)
                    {
                        var filterValue = new Dictionary<string, string> { { fieldName, value } }; //{"STUDYID":"CRC305C"}
                        valueList.Add(filterValue);// [{"STUDYID":"CRC305C"},{"STUDYID":"CRC305A"}]
                    }

                    filterField.Add("$or", valueList);
                    filterFields.Add(filterField);
                }

                //Filters for numeric fields/variables (e.g. VSORRES)
                //WILL NOT HOLD the same field filter for two different O3s
                foreach (var filter in criterion.RangeFilters)
                {
                    filterField = new Dictionary<string, object>();
                    //IMPLICIT AND between filters of different fields
                    var fieldName = filter.Field;
                    var range = filter.Range;


                    //CONSTRUCT the equivalent of a BSON {field : {$gt:v,$lt:v2}}
                    var valueRange = new Dictionary<string, string>();
                    valueRange.Add("$gt", range.Lowerbound.ToString());
                    valueRange.Add("$lt", range.Upperbound.ToString());
                    filterField.Add(fieldName, valueRange);
                    filterFields.Add(filterField);
                }
                List<SubjectObservation> subjectData = await _subObservationRepository.FindAllAsync(filterFields: filterFields);
                
                observations.AddRange(subjectData);
            }

            return observations;
        }

        public async Task<List<HumanSubject>>  ExportDatasets(string projectId, ExportRequestDTO dto)
        {
           // List<HumanSubject> subjectData = await ExportDataset(projectId, dto.SubjectCriteria);
           // var clinicalData = await ExportDataset(projectId, dto.ClinicalCriteria);
            //var sampleData = await ExportDataset(projectId, dto.SampleCriteria);
            return null;


        }

        public List<TreeNodeDTO> getAvailableFields(string projectAcc)
        {

            var roots = new List<TreeNodeDTO>();

            //1- Design Elements
            var DEnodes = createOrFindTreeNode(roots, "DEelems", "Design Elements","", true);
            var armNode = createOrFindTreeNode(DEnodes.Children, "ARM", "Arm", "", false);
            var siteNode = createOrFindTreeNode(DEnodes.Children, "SITE", "Site", "", false);
            var studyNode = createOrFindTreeNode(DEnodes.Children, "STUDY", "Study", "", false);
            var studyDays = createOrFindTreeNode(DEnodes.Children, "DY", "Study Day#", "", false);
            var visitNode = createOrFindTreeNode(DEnodes.Children, "VISIT", "Visit Name", "", false);

            //2-Study Characteristics
            var SubjCharsnode = createOrFindTreeNode(roots, "subj", "Subject Characterisitcs", "", true);
            var SCs = _characObjRepository.FindAll(sco => sco.Project.Accession.Equals(projectAcc)).ToList();

            foreach (var sc in SCs)
            {
                createOrFindTreeNode(SubjCharsnode.Children, sc.Id.ToString(), sc.FullName, "", false);
            }

            List<Observation> studyObservations =
           _observationRepository.FindAll(
               x => x.Studies.Select(s => s.Project.Accession).Contains(projectAcc),
               new List<Expression<Func<Observation, object>>>(){
                        d => d.Studies.Select(s => s.Project),
                        d=> d.TopicVariable,
                        //d => d.DefaultQualifier,
                        d => d.Qualifiers
                    }
               ).ToList();

            var groupedByClDmGp = studyObservations.GroupBy(ob => new {ob.Class, ob.DomainCode, ob.DomainName, ob.Group});

           
            int i = 0;
            foreach (var obsGrp in groupedByClDmGp)
            {
                i++;
                var classNode = createOrFindTreeNode(roots, obsGrp.Key.Class.ToUpper(), obsGrp.Key.Class, "", true);
                var domainNode = createOrFindTreeNode(classNode.Children, obsGrp.Key.DomainCode.ToUpper(), obsGrp.Key.DomainName, "", true);
                var groupNode = obsGrp.Key.Group == null ? domainNode :
                    createOrFindTreeNode(domainNode.Children, obsGrp.Key.DomainCode.ToUpper() + "_grp_" + i, obsGrp.Key.Group, "", true);

                foreach (var obs in obsGrp)
                    createOrFindTreeNode(groupNode.Children, obs.Id.ToString(), obs.ControlledTermStr, "", true, obs);
            }

            
            return roots;
        }

        public async Task<Field> getFieldValueSet(string projectAcc, Field field )
        {
            List<SubjectObservation> observationData;
            if (field.QO2Name.EndsWith("ORRES"))
            {
                _dataContext.AddClassMap(field.O3VarName, "Name");
                observationData = await _subObservationRepository.FindAllAsync(
                        d => d.Name.Equals(field.O3Name));
                HashSet<double> vals = new HashSet<double>();
                foreach (var q in observationData)
                {
                    double val;
                    if(double.TryParse(q.qualifiers[field.QO2Name], out val))
                        vals.Add(val);
                }
                field.ValueSet = new ValueSetDTO();
                field.ValueSet.Max= vals.Max();
                field.ValueSet.Min = vals.Min();
                field.ValueSet.To = vals.Max();
                field.ValueSet.From = vals.Min();
                field.ValueSet.IsNumeric = true;
                field.ValueSet.Unit = observationData.First().qualifiers["VSORRESU"];
            }
            else
            {
                _dataContext.AddClassMap("AEDECOD", "Name");
                observationData = await _subObservationRepository.FindAllAsync(
                        d => d.Name.Equals(field.O3Name));
                HashSet<string> vals = new HashSet<string>();
                foreach (var q in observationData)
                {
                    vals.Add(q.qualifiers[field.QO2Name]);
                }
                var dis = vals.Distinct().ToList();
                dis.Remove("");
                field.ValueSet = new ValueSetDTO();
                field.ValueSet.ValueSet = dis;
            }
            return field;


        }
        private TreeNodeDTO createOrFindTreeNode(List<TreeNodeDTO> parent, string Id, string Text, string type, bool groupNode, Observation obs = null, VariableDefinition variable = null)
        {
            TreeNodeDTO node;
            if (parent != null)
            {
                node = parent.Find(r => r.Text.Equals(Text));
                if (node != null)
                    return node;
            }

            node = new TreeNodeDTO()
            {
                Id = Id,
                Text = Text,
                Children = groupNode ? new List<TreeNodeDTO>() : null,
                IsGroup = groupNode,
                Type = type
            };

            if (variable != null)
            {
                node.Field = new Field()
                {
                    O3Name = obs.Name,
                    O3Id = obs.Id,
                    O3VarName = obs.TopicVariable.Name,
                    QO2Name = variable.Name,
                    Id = node.Id,
                    DomainCode = obs.DomainCode,
                    Domain = obs.DomainName,
                    GroupName = obs.Group,
                    GroupVarName = obs.DomainCode+"CAT"
                };
            }

            if (obs != null && variable == null)
                foreach (var qualifier in obs.Qualifiers)
                    createOrFindTreeNode(node.Children, node.Id + "_" + qualifier.Id, qualifier.Label,qualifier.DataType, false, obs, qualifier);
            

            parent.Add(node);
            return node;

        }

        public async Task<Hashtable> ExportDataTable(string projectAcc, List<Criterion> criteria)
        {
           
            List<SubjectObservation> observations = await ExportDataset(projectAcc, criteria);

            var dataTable = getDataTable(observations, criteria);
            var ht = getHashtable(dataTable);
            return ht;
        }

        public async Task<List<TreeNodeDTO>> ExportDataTree(string projectAcc, List<Criterion> criteria)
        {

            List<SubjectObservation> observations = await ExportDataset(projectAcc, criteria);

            var dataTree = getDataTree(observations);
            
            return dataTree;
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
                    new List<Expression<Func<DomainTemplate, object>>>()
                    {
                        v => v.Variables
                    });

                var Class = domain.Class;
                var Domain = domain.Name;

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
                        var v = domain.Variables.FirstOrDefault(d => d.Name.Equals(q.Key));
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
                        var v = domain.Variables.FirstOrDefault(d => d.Name.Equals(q.Key));
                        createOrFindTreeNode(timeQualsNode.Children, timeQualsNode.Id + q.Key, v.Label,type, false);
                    }
                        
                    
                    //for each obs create a group node for Result qualifier, Identifier (topic,synonym...etc),timing,qualifier
                }
               
            }

            return roots;
        }
        
        
        private DataTable getDataTable(IEnumerable<SubjectObservation> findings, List<Criterion> criteria)
        {
            #region Build Table columns
            var datatable = new DataTable();
            var projections = new List<string>();
            datatable.Columns.Add("SUBJECTID");
            datatable.Columns.Add("STUDY");

            foreach (var field in criteria.SelectMany(criterion => criterion.Projection))
            {
                datatable.Columns.Add(field.ToLower());
                projections.Add(field.ToLower());
            }

            datatable.Columns.Add("VISIT");
            datatable.Columns.Add("TIMEPOINT");
            #endregion

            #region Group observations by unique key
            var findingsBySubjectVisitTime = findings //The unique combination that would make one row
                .GroupBy(ob => new
                {
                    subjId = ob.SubjectId,
                    //Visit = ob.ObsStudyDay,
                    Day = ob.ObsStudyDay == null ? "" : ob.ObsStudyDay.Number.ToString(),
                    Timepoint = ob.ObsStudyTimePoint == null ? "" : ob.ObsStudyTimePoint.Name
                });
            #endregion

            foreach (var subjVisitTPT in findingsBySubjectVisitTime)
            {
                var row = datatable.NewRow();
                foreach (var subjObs in subjVisitTPT)//adding columns to the same row each iteration is a different observation but within each iteration
                //we could be adding more than one column per observation if requested more than one qaulifier
                {
                    row["SUBJECTID"] = subjObs.SubjectId;
                    row["STUDY"] = subjObs.StudyId;
                    foreach (var field in projections)
                    {
                        var QO2 = field.Substring(field.IndexOf("_")+1);
                        var O3 = field.Split('_')[0];
                        if (O3.ToLower().Equals(subjObs.Name.ToLower()))
                        row[field] = subjObs.qualifiers.SingleOrDefault(q => q.Key.ToLower().Equals(QO2)).Value;
                    }

                    //TODO: assumption to be validated that visits/timepoints are study and dont differ between observations or studies of the same project
                    row["VISIT"] = subjObs.Visit;
                    row["TIMEPOINT"] = subjObs.ObsStudyTimePoint == null ? "" : subjObs.ObsStudyTimePoint.Name;
                }
                datatable.Rows.Add(row);
            }
            return datatable;
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
    }
}
