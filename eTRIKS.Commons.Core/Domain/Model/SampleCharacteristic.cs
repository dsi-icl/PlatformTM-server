namespace eTRIKS.Commons.Core.Domain.Model
{
    public class SampleCharacteristic : Characterisitc
    {
        public Biosample Sample { get; set; }
        public int SampleId { get; set; }
    }
}
