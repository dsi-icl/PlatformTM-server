using System;
using System.Collections.Generic;

namespace PlatformTM.Core.Domain.Model.DatasetModel.SDTM
{
    public class SdtmFindingDataset
    {
        public ObservationModel.CodedQualifiedField Topic;
        //public NumericMeadureQF OriginalResult;
        //public NumericMeasureQF StandardResult;
        //public RangeFieldsMapper
        public VariableDefinition SampleAttributes;
        public List<VariableDefinition> SubjectAttributes;
        public List<VariableDefinition> FeatureAttributes;
        //how can I solve the problem of some qualifiers being single values and some are multiple values such as anatomical location
        //how to create a type that can be single sometimes or multiple
        //if a type is created then the relation to the another object becomes an association relationship 
        //if no specific type created for a property then it is considered an attribute and an attribute could only be of a single value.

        public SdtmFindingDataset()
        {
        }
    }
}
