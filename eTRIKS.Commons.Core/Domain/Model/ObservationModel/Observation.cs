using System;
using System.Collections.Generic;
using eTRIKS.Commons.Core.Domain.Model.Base;

namespace eTRIKS.Commons.Core.Domain.Model.ObservationModel
{
    public class Observation : Identifiable<Guid>
    {
        /// <summary>
        /// Mapped to the value of FeatureIdentifier set in the feature annotation dataset (Topic Variable in SDTM / FeatureIdentifier in other templates)
        /// </summary>
        public string FeatureName { get; set; } //Headache or PTB2 //FEATURE 
        public int FeatureId { get; set; }

        /// <summary>
        /// mapped to the value of Sample Submitted Identifier (BSREFID in case of SDTM-BS) or (USUBJID in case of SDTM-DM)
        /// </summary>
        public string SubjectOfObservationName { get; set; }
        /// <summary>
        /// Mapped to the value of the MySQL Study/Sample table primary key (SubjectId or SampleId)
        /// </summary>
        public int SubjectOfObservationId { get; set; }


        public string StudyName { get; set; } //CRC305A CRC305B
        public int StudyId { get; set; } //MySQL Study Table primary key

        /// <summary>
        /// The value for a particular qualifier property (Property Descriptor) 
        /// </summary>
        /// <example> Value for Log Ratio , Value for Severity, Value for Test result, Value for Occurence ... </example>
        public ObservedPropertyValue ObservedValue { get; set; }
        
        //TODO: to reconsider these two propeties later
        public ObservedPropertyValue TemporalValue { get; set; }
        public ObservedPropertyValue TimeSeriesValue { get; set; }

        //TODO: need to decide whether I shuold keep these here or move to feature obj and subj class... 
        //public List<ObservedPropertyValue> FeatureProperties { get; set; } //Properties about the feature/O3 featured in this observation NOT the observation itself
        //public List<ObservedPropertyValue> SO2Properties { get; set; } //Properties of the sample or the subject being observed

        public int DatasetId { get; set; }
        public int DatafileId { get; set; }
        /// <summary>
        /// Also maps to AssayId in case of Assay Observations
        /// </summary>
        public int ActivityId { get; set; }
        public int? ProjectId { get; set; }

    }

    //The way an observation was queried before is by including the O3id and the QO2 id in the observation request and 
    //subjObs.qualifiers.SingleOrDefault(q => q.Key.Equals(obsreq.QO2)).Value;
    //this will be

    //foreach requestedObservation
    //observations = observationRepository.findAll(o => o3.id = req.O3id)
    //this iwill get all observations for this requested O3
    //then
    //foreach (var qo2 in obs.results.Where(r => r.O2id.Equals(reqobj.qo2id))) 
    //TODO: if default qualifier used dont include square brackets
    //TODO: assumption to be validated that visits/timepoints are study and dont differ between observations or studies of the same project
    // row[obsreq.Id] = qo2.result.value
    // row[obsreq.Id+" unit"] qo2.result.unit.name//subjObs.qualifiers.SingleOrDefault(q => q.Key.Equals(obsreq.QO2)).Value;



    

    
}
