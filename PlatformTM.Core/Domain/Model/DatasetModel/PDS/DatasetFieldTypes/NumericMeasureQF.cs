using System;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS.DatasetFieldTypes;

namespace PlatformTM.Core.Domain.Model.PDS
{
    public class NumericMeasureQF
    {
        public DatasetField ValueFields { get; set; } //--ORRES
        public DatasetField UnitOfMeasure { get; set; } //--ORRESU 
        public NumericMeasureQF()
        {
        }
    }
}
