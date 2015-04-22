using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
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
        private IRepository<Activity, int> _activityRepository;
        public ObservationService(IServiceUoW uoW)
        {
            _activityRepository = uoW.GetRepository<Activity, int>();
        }
        public void loadObservations(string studyId)
        {

            MongoDbDataRepository mongoDataService = new MongoDbDataRepository();
            List<Activity> activity_list = _activityRepository.Get(
               d => d.StudyId.Equals(studyId),
                 new List<Expression<Func<Activity, object>>>(){
                        d => d.Datasets.Select(t => t.Domain), d=>d.Datasets.Select(t=>t.Variables),
                         d => d.Datasets.Select(t => t.Domain), d=>d.Datasets.Select(
                                                                    t=>t.Variables.Select(k=>k.VariableDefinition))
                }).ToList();

            foreach (Activity activity in activity_list)
            {
                foreach (Dataset ds in activity.Datasets)
                {
                    List<VariableDefinition> qualifiers;
                    List<VariableDefinition> timings;
                    VariableDefinition topic;
                    VariableDefinition controlledTerm = null;


                    if(ds.Domain.Class.ToLower().Equals("relationship"))
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

                    if(ds.Domain.Class.ToLower().Equals("findings")){

                        controlledTerm = ds.Variables
                                            .Select(l => l.VariableDefinition)
                                            .Where(v => v.Name == ds.Domain.Code+"TESTCD")
                                            .FirstOrDefault();
                        topic = ds.Variables
                                            .Select(l => l.VariableDefinition)
                                            .Where(v => v.Name == ds.Domain.Code + "TEST")
                                            .FirstOrDefault();
                    }



                    Dictionary<string, string> filterFields = new Dictionary<string, string>();
                    filterFields.Add("DOMAIN", ds.Domain.Code);

                    Dictionary<string, string> groupFields = new Dictionary<string, string>();
                    groupFields.Add("Name", "$"+topic.Name);
                    groupFields.Add("ControlledTermStr", "$" + controlledTerm.Name);
                    groupFields.Add("Group", "$" + ds.Domain.Code + "CAT");
                    groupFields.Add("Subgroup", "$" + ds.Domain.Code + "SCAT");

                    List<Observation> observations = mongoDataService.getGroupedNoSQLrecords(filterFields, groupFields);

                    foreach (Observation obs in observations)
                    {
                        obs.Class = ds.Domain.Class;
                        obs.DomainCode = ds.Domain.Code;
                        obs.TopicVariable = topic;
                        obs.qualifiers = qualifiers;
                        obs.timings = timings;
                    }
                    Console.Out.WriteLine(observations);

                                                                    
                }
            }


        }
    }
}
