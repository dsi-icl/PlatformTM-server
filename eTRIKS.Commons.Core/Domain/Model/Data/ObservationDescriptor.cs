using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using eTRIKS.Commons.Core.Domain.Model.Templates;

namespace eTRIKS.Commons.Core.Domain.Model.Data
{
    public class ObservationDescriptor : Identifiable<int>
    {
        //TODO: rename ObjectofObservation to Feature
        public ObjectOfObservation O3 { get; set; } //VSTESTCD //
        public int ObjectOfObservationId { get; set; }
        public DomainTemplate Domain { get; set; }
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




}
