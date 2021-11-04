using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.Domain.Model.Templates;

namespace PlatformTM.Core.Domain.Model.ObservationModel
{
    //EQUIVALENT TO SDTMdescriptor
    public class ObservationDescriptor : Identifiable<int>
    {
        //TODO: rename ObjectofObservation to Feature
        public ObjectOfObservation O3 { get; set; } //VSTESTCD //
        public int ObjectOfObservationId { get; set; }
        public DatasetTemplate Domain { get; set; }
        public string DomainId { get; set; }
        public PropertyDescriptor DefaultPropertyDescriptor { get; set; }//AspectOfObservationDescriptor//MeasureOfObservationDescriptor
        public int DefaultPropertyDescriptorId { get; set; }
        public List<PropertyDescriptor> ObservedPropertyDescriptors { get; set; }
        public List<PropertyDescriptor> FeatureDescriptors { get; set; }
        public List<PropertyDescriptor> SubjectDescriptors { get; set; }
        public List<PropertyDescriptor> SampleDescriptors { get; set; }
        public List<PropertyDescriptor> TemporalDescriptors { get; set; }
        public List<PropertyDescriptor> TimeseriesDescriptors { get; set; }
        public bool HasTimeSeries { get; set; }
        public Project Project { get; set; }
        public int ProjectId { get; set; }

        //TODO:ADD REFERENCE TO SUBJECTS AND STUDIES?

        //public List<ObservationDescriptor> ObservationGroupings { get; set; }



    }

    //Sometimes qualifer variables are not simple single valued properties but a compound structure of multiple related qualifiers
    //e.g. Dosage
    //DOSE, DOSU, DOSE FORM, DOSING FREQUENCY, INTENDED DOSE REGIMEN
    //Numeric Result
    //RESULT, UNIT
    //Anatomical Location of Intervention
    //LOC, LAT, DIR, PORTOT
    //Controlled Vocabulary
    //NAME, CODE
    //Numeric Measure with reference range
    //RESULT, UNIT, UPLIMIT, LOWERLIMIT
    //Simple Categorical
    //VALUE_STR
    //Simple Numeric
    //Ordinal
    //Severity

    //These are reusable descriptors across projects 
    //public class PropertyDescriptor : Identifiable<int>
    //{
    //    //public VariableReference DatasetVariable { get; set; }
    //    public DescriptorType Type { get; set; }
    //    public ObsValueType ValueType { get; set; }
    //    public string Name { get; set; }
    //    public CVterm CVterm { get; set; }
    //    public string Description { get; set; }
    //    public string ObsClass { get; set; }
    //    public List<ObservationDescriptor> RelDescriptors { get; set; } //UNIT
    //    public Project Project { get; set; }
    //    public int ProjectId { get; set; }
    //}




    //TODO: TemporalQualifiers such as
    //DURATION, ELAPSED_TIME, INTERVAL

    //TODO: TimeSeriesDescriptors
    //DATE_TIME, STUDY_DAY, TIME_POINT


    public enum DescriptorType
    {
        FeatureDescriptor, //Properties ABOUT the feature (e.g. findings:Reason not done, status , )
        ObservedPropertyDescriptor,
        DefObservedPropDescriptor,
        SubjectDescriptor,
        SampleDescriptor,
        TemporalQualifier,
        TimeSeriesDescriptor
    }

    public enum ObsValueType
    {
        Categorical,
        Numerical,
        Ordinal,
        Dosage,
        AnatomicalLocation
    }

    public  class Observable{}
    public class Property : Observable { }
    public class Feature :Observable{
        public List<Observable> observableProperties { get; set; }
        public string Name
        {
            get; set;
        }
    }
    public class ObservationFeature : Observable
    {
        //This is can be temp and it can headache
        //if its temperature then i get the result from the observation object that corresponds
        //to temp... obs.foi=temp or obs.foi=headache the only difference is temp is a feature with
        //no other properties IT IS the PROPERTY of a feature that in OUR context is not really the
        //main feature of Interest ... 
        //yeb2a el foi is of a type that can be either a property or a feature==> hence observable
        //tab lama el obs.foi is of temp yeb2a it's instantiated ... it is an Observable Feature Of Interest
        //but it is not a Feature that has properties ...malhash Feature...heya el feature
    }

    //class ObservableEntity : ObservableFeature { }

    public class ObservationPhenomenon {
        public Feature Feature { get; set; }
        public Property Property { get; set; }
    }


    
    //class Feature : Observable { }
    class Observatio
    {
        public ObservationPhenomenon Phenomenon;
        //Property ObservedProperty { get; set; }
        //Observablfee observedEntity { get; set; }
        Observable FeatureOfInterest { get; set; }

        public void test()
        {
            Observatio obHO = new Observatio();
            Feature headache = new Feature(); //OR FIND
            Property occurence = new Property(); //OR FIND
            headache.observableProperties.Add(occurence);

            obHO.FeatureOfInterest = headache;

            obHO.Phenomenon.Feature = headache;
            obHO.Phenomenon.Property = occurence;

            Observatio obTemp = new Observatio();
            Feature person = new Feature(); //OR find
            Property temp = new Property(); //OR FIND
            //malhash properties;

            obTemp.Phenomenon.Feature= person;
            //obTemp.observedProperty = temp;
            obTemp.FeatureOfInterest = temp;


            //This is to simulate a list of object-of-observations (FeaturesOfInterest) retrieved from DB
            var features = new List<Observable>();
            features.Add(headache);
            features.Add(temp);

            obHO.FeatureOfInterest = features[0];
            obTemp.FeatureOfInterest = features[1];
        }

        
    }

}
