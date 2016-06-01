using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using eTRIKS.Commons.Core.Domain.Model.Timing;

namespace eTRIKS.Commons.Core.Domain.Model.Data
{
    //IF Observation Data is going to be stored in MySQL then will leave references to other entites such as O3 and Subject
    //As long as the Descriptor is saved in MySQL with references to project, stud
    public class Observation : Identifiable<Guid>
    {
        public string ObjectOfObservation { get; set; } //Headache or PTB2 //FEATURE
        public int O3Id { get; set; }
        //public string SubjectOfObservation { get; set; } //Subject1 or Sample 1
        public string SO2Id { get; set; }
        public string SO2Type { get; set; }
        public string Study { get; set; } //CRC305A CRC305B
        public int StudyDBId { get; set; }

        public List<ObservedPropertyValue> ObservedProperties { get; set; }
        public ObservedPropertyValue DefaultObservedProperty { get; set; }
        public List<ObservedPropertyValue> FeatureProperties { get; set; }
        public List<ObservedPropertyValue> TemporalProperties { get; set; }
        public List<ObservedPropertyValue> TimeSeriesDescriptors { get; set; }


        //public MeasureOfObservationProperty ObservedPropertyValue { get; set; }
        //public VariableDefinition TimeDescriptor { get; set; } //should be a key, value
        //public Visit Visit { get; set; }

        //ALREADY this observation has an observationdescription


    }

    //The way an observation was queried before is by including the O3id and the QO2 id in the observation request and 
    //subjObs.qualifiers.SingleOrDefault(q => q.Key.Equals(obsreq.QO2)).Value;
    //this will be

    //foreach requestedObservation
    //observations = observationRepository.findAll(o => o3.id = req.O3id)
    //this iwill get all observations for this requested O3
    //then
    //foreach (var qo2 in obs.results.Where(r => r.O2id.Equals(reqobj.qo2id))) 
    //TODO: if default qualifier used dont include square brackets
    //TODO: assumption to be validated that visits/timepoints are study and dont differ between observations or studies of the same project
    // row[obsreq.Id] = qo2.result.value
    // row[obsreq.Id+" unit"] qo2.result.unit.name//subjObs.qualifiers.SingleOrDefault(q => q.Key.Equals(obsreq.QO2)).Value;



    

    
}
