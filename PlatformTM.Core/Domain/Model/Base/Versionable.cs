namespace PlatformTM.Core.Domain.Model.Base
{
    public abstract class Versionable<TPrimaryKey> : Identifiable<TPrimaryKey>
    {
        public string Version { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }


    }
}
