using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.Domain.Model.Templates;
using PlatformTM.Core.JoinEntities;

namespace PlatformTM.Core.Domain.Model.DatasetModel
{
    public class Dataset : Identifiable<int>
    {
        //public string DataFile { get; set; }
        public int ActivityId { get; set; }
        public string TemplateId { get; set; }
        public Activity Activity { get; private set; }
        public DatasetTemplate Template { get; set; }
        public ICollection<VariableReference> Variables { get;  set; }


        //public ICollection<Study> Studies { get; set; }
        public ICollection<StudyDataset> Studies { get; set; }
        /*Consider putting this back when support for many-tp-many relationship is available*/
        //public ICollection<DataFile> DataFiles { get; set; }
        public ICollection<DatasetDatafile> DataFiles { get; set; }
        
        //public ICollection<DataFile> StandardDataFiles { get; set; }
        public string State { get; set; }

        public Dataset()
        {
            Variables = new List<VariableReference>();
            DataFiles = new HashSet<DatasetDatafile>();
        }

        public void AddDataFile(DataFile file)
        {
            //if(DataFiles.Contains())
        }
    }
}