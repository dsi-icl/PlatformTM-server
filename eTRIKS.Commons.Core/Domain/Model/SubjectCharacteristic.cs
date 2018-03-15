namespace PlatformTM.Core.Domain.Model
{
    public class SubjectCharacteristic : Characteristic
    {
        public HumanSubject Subject { get; set; }
        public string SubjectId { get; set; }
    }
}
