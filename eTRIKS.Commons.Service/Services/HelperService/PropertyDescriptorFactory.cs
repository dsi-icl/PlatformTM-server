using eTRIKS.Commons.Core.Domain.Model.ObservationModel;

namespace eTRIKS.Commons.Service.Services.HelperService
{
    public class PropertyDescriptorFactory
    {
        public static PropertyDescriptor CreateAEdefDescriptor()
        {
            return new PropertyDescriptor()
            {
                Name = "OCCUR",
                Type = DescriptorType.DefObservedPropDescriptor,
                Description = "Occurrence",
                ObsClass = "EVENTS"//,
                //CVterm = new CVterm() { }
            };
        }

        public static PropertyDescriptor CreateOriResDescriptor()
        {
            return new PropertyDescriptor()
            {
                Name = "ORRES",
                Type = DescriptorType.ObservedPropertyDescriptor,
                Description = "Original Result",
                ObsClass = "FINDINGS"//,
                //CVterm = new CVterm() { }
            };
        }

        public static PropertyDescriptor CreateStandardResDescriptor()
        {
            return new PropertyDescriptor()
            {
                Name = "STRES",
                Type = DescriptorType.ObservedPropertyDescriptor,
                Description = "Standard Result"//,
                //CVterm = new CVterm() { }
            };
        }
    }
}
