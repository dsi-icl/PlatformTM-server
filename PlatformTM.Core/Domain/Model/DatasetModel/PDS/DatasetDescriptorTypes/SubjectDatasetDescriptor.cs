using System;
using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS.DatasetFieldTypes;

namespace PlatformTM.Core.Domain.Model.DatasetModel.PDS.DatasetDescriptorTypes
{
    public class SubjectDatasetDescriptor : DatasetDescriptor
    {
        public IdentifierField StudyIdentifierField { get; set; }
        public IdentifierField SubjectIdentifierField { get; set; }

        public List<DatasetField> CharasteristicFeature { get; set; }



        public DatasetField ClassifierField { get; set; }

        public List<DatasetField> ObservedPropertyFields { get; set; }


        //the descriptor was populated from the dataset but in this case the descriptor is loaded

    }
}
