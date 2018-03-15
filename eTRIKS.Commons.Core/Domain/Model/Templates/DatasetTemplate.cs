using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;

namespace PlatformTM.Core.Domain.Model.Templates
{
    public class DatasetTemplate : Identifiable<string>
    {
        public string Domain { get; set; } //TODO:consider changing it to a CVterm
        public string Class { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string Structure { get; set; }
        public bool IsRepeating { get; set; }

        public ICollection<DatasetTemplateField> Fields { get; private set; }

        public DatasetTemplate()
        {
            Fields = new List<DatasetTemplateField>();
        }
    }

}