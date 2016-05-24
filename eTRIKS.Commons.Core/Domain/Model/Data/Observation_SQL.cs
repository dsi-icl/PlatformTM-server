using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using eTRIKS.Commons.Core.Domain.Model.Timing;
using eTRIKS.Commons.Core.Domain.Model.DesignElements;

namespace eTRIKS.Commons.Core.Domain.Model.Data
{
    //IF Observation Data is going to be stored in MySQL then will leave references to other entites such as O3 and Subject
    //As long as the Descriptor is saved in MySQL with references to project, stud
    public class ObservationSql
    {
        public ObjectOfObservation O3 { get; set; } //Headache or PTB2 //FEATURE
        //public SubjectOfObservation Subject { get; set; } //Subject1 or Sample 1
        public string SubjectOfObservationId { get; set; }
        public Study Study { get; set; } //CRC305A CRC305B
        public int StudyId { get; set; }

        public List<ObservedPropertyValue> ObservedProperties { get; set;}
        public ObservedPropertyValue DefaultObservedProperty { get; set; }

        //HOW TO REPRESENT VALUES
        //1- A List of a Value Class that reference the descriptor and has a Value for it

        //public MeasureOfObservationProperty ObservedPropertyValue { get; set; }
        //public VariableDefinition TimeDescriptor { get; set; } //should be a key, value
        public Visit Visit { get; set; }
       
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



    //public abstract class ObservedPropertyValue
    //{
    //    public ObservationDescriptor Property { get; set; }
    //}

    //public class CategoricalValue : ObservedPropertyValue
    //{
    //    public string Value { get; set; }
    //    public CVterm CVTerm { get; set; }
    //}

    //public class OrdinalValue : ObservedPropertyValue
    //{
    //    public string Value { get; set; }
    //    public int order { get; set; }
    //}

    //public class NumericValue : ObservedPropertyValue
    //{
    //    public float Value { get; set; }
    //    public string Unit { get; set; }
    //    public CVterm UnirCVterm { get; set; }
    //}
    //public class PropertyQualifier : MeasureOfObservationProperty
    //{
    //    public string VerbatimValue { get; set; }
    //    public CVterm CVterm { get; set; }
    //}

    //public class PropertyQuantifier : MeasureOfObservationProperty
    //{
    //    public double NumericValue { get; set; }
    //    public string UnitofMeasurement { get; set; }
    //    //public double ReferenceRangeUpper { get; set; }
    //    //public double ReferenceRangeLower { get; set; }
    //    //public string RefrenceRangeIndicator { get; set; }
    //}
}
