using System;
using PlatformTM.Core.Domain.Model.DatasetModel;

namespace PlatformTM.Core.Domain.Model.ObservationModel
{
    public class NumericMeasureQF
    {
        public VariableDefinition ValueFields { get; set; } //--ORRES
        public VariableDefinition UnitOfMeasure { get; set; } //--ORRESU 
        public NumericMeasureQF()
        {
        }
    }
}
