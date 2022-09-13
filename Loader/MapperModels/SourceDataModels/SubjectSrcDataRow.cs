using System;
namespace Loader.MapperModels.SourceDataModels
{
    public class SubjectSrcDataRow
    {
        public string SubjectId { get; set; }
        public Dictionary<string, string> DataRecord { get; set; }

        public SubjectSrcDataRow()
        {
            DataRecord = new();
        }
    }
}

