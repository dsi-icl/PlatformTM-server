using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using eTRIKS.Commons.DataAccess.MongoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Service.Services
{
    public class ObservationService
    {
        private IServiceUoW _dataContext;
        private IRepository<Activity, int> _activityRepository;
        private IRepository<Observation, int> _ObservationRepository;
        private IRepository<SubjectObservation, Guid> _subjObsRepository;
        private IRepository<MongoDocument, Guid> mongoRepository;
        

        //private IRepository<VariableDefinition, int> _variableRepository;
        public ObservationService(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _activityRepository = uoW.GetRepository<Activity, int>();
            _ObservationRepository = uoW.GetRepository<Observation, int>();
            _subjObsRepository = uoW.GetRepository<SubjectObservation,Guid>();
            //mongoRepository = uoW.GetRepository<SubjectObservation, Guid>();

           // _variableRepository = uoW.GetRepository<VariableDefinition, int>();
        }
        public void loadObservations(string studyId)
        {

            //MongoDbDataRepository mongoDataService = new MongoDbDataRepository();
            List<Activity> activity_list = _activityRepository.FindAll(
               d => d.StudyId.Equals(studyId),
                 new List<Expression<Func<Activity, object>>>(){
                        d => d.Datasets.Select(t => t.Domain), 
                        d=> d.Datasets.Select(t=>t.Variables),
                        d => d.Datasets.Select(t => t.Domain),
                        d => d.Study,
                        d=>d.Datasets.Select(t=>t.Variables.Select(k=>k.VariableDefinition))
                }).ToList();

            

            foreach (Activity activity in activity_list)
            {
                foreach (Dataset ds in activity.Datasets)
                {
                    List<VariableDefinition> qualifiers;
                    List<VariableDefinition> timings;
                    VariableDefinition topic;
                    VariableDefinition controlledTerm = null;
                    string obsName, controlledTermName, obsClass;

                    obsClass = ds.Domain.Class.ToLower();

                    if(obsClass.Equals("relationship"))
                       continue;

                    qualifiers = ds.Variables
                        .Select(l => l.VariableDefinition)
                        .Where(v => v.RoleId == "CL-Role-T-3")
                        .ToList();

                    timings = ds.Variables
                        .Select(l => l.VariableDefinition)
                        .Where(v => v.RoleId == "CL-Role-T-6")
                        .ToList();

                    topic = ds.Variables
                        .Select(l => l.VariableDefinition)
                        .Where(v => v.RoleId == "CL-Role-T-2")
                        .First();

                    controlledTerm = ds.Variables
                        .Select(l => l.VariableDefinition)
                        .Where(v => v.Name == ds.Domain.Code+"DECOD")
                        .FirstOrDefault();

                    if(obsClass.Equals("findings")){

                        controlledTerm = ds.Variables
                                            .Select(l => l.VariableDefinition)
                                            .Where(v => v.Name == ds.Domain.Code+"TEST")
                                            .FirstOrDefault();
                        //topic = ds.Variables
                        //                    .Select(l => l.VariableDefinition)
                        //                    .Where(v => v.Name == ds.Domain.Code + "TESTCD")
                        //                    .FirstOrDefault();
                        obsName = controlledTerm.Name;
                        controlledTermName = topic.Name;
                    }

                    obsName = topic.Name;
                    controlledTermName = controlledTerm.Name;
 

                    Dictionary<string, string> filterFields = new Dictionary<string, string>();
                    filterFields.Add("DOMAIN", ds.Domain.Code);

                    Dictionary<string, string> groupFields = new Dictionary<string, string>();
                    groupFields.Add("Name", "$"+ obsName);
                    groupFields.Add("ControlledTermStr", "$" + controlledTerm.Name);
                    groupFields.Add("Group", "$" + ds.Domain.Code + "CAT");
                    groupFields.Add("Subgroup", "$" + ds.Domain.Code + "SCAT");

                    //List<Observation> observations = mongoDataService.getGroupedNoSQLrecords(filterFields, groupFields);
                    List<SubjectObservation> subjObs =  _subjObsRepository.FindAll(x => x.DomainCode == ds.Domain.Code).ToList();
                    var people = _subjObsRepository.AggregateAsync<string,string>(x => x.DomainCode == x.DomainCode,
                                                        x => new { x.Name, x.Group, x.Subgroup }.ToString(),
                                                        g => new { _id = g.Key }.ToString()

                                                        );
                    people.ToString();
                   // subjObs.GroupBy(x => new { x.Name, x.Group, x.Subgroup });

                    //foreach (Observation obs in observations)
                    //{
                    //    obs.Class = ds.Domain.Class;
                    //    obs.DomainCode = ds.Domain.Code;
                    //    obs.TopicVariable = topic;
                    //    obs.Qualifiers = qualifiers;
                    //    obs.Timings = timings;
                    //    //obs.ControlledTerm = 
                    //    //if(obsClass.Equals("findings"))
                    //    //    obs.DefaultQualifier= getVarDef(ds.Domain.Code+"ORRES");
                    //    obs.Studies.Add(activity.Study);
                    //    _ObservationRepository.Insert(obs);
                    //}                                       
                }
                _dataContext.Save();
            }
           

        }

        public async Task<List<SubjectObservation>> test()
        {
            List<SubjectObservation> md;
            return md = await _subjObsRepository.FindAllAsync(x => true);
           
        }

        public async Task loadTest()
        {
            SubjectObservation subjOb = new SubjectObservation();
            subjOb.DomainCode = "VS";
            subjOb.Class = "Findings";
            //subjOb.Id = Guid.NewGuid();
            await _subjObsRepository.InsertAsync(subjOb);
        }
        //private VariableDefinition getVarDef(string name)
        //{
        //   return _variableRepository.GetSingle(d => d.Name.Equals(name));
        //}
    }
}
