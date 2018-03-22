using System;
using System.Collections.Generic;
using System.Linq;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model.DatasetModel.SDTM;
using PlatformTM.Core.JoinEntities;
using Observation = PlatformTM.Core.Domain.Model.Observation;

namespace PlatformTM.Services.Services
{
    public class ObservationService
    {
        private readonly IServiceUoW _dataContext;
        //private readonly IRepository<Activity, int> _activityRepository;
        private readonly IRepository<Observation, int> _ObservationRepository;
        //private readonly IRepository<VariableDefinition, int> _variableRepository;
        //private readonly IRepository<Dataset, int> _datasetRepository;
        //private readonly IRepository<ObjectOfObservation, int> _O3Repository;
        //private readonly IRepository<PropertyDescriptor, int> _ObsDescriptorRepository;
        //private readonly IRepository<ObservationDescriptor, int> _ObsDescriptionRepository;
        private readonly IRepository<SdtmRow, Guid> _sdtmRepository;

        //MongoDB Repositories
        //private IRepository<HumanSubject, string> subjectRepository;

        //private IRepository<Study, int> _studyRepository;


        public ObservationService(IServiceUoW uoW)
        {
            _dataContext = uoW;
            //_activityRepository = uoW.GetRepository<Activity, int>();
            _ObservationRepository = uoW.GetRepository<Observation, int>();
            //_datasetRepository = uoW.GetRepository<Dataset, int>();

            //_variableRepository = uoW.GetRepository<VariableDefinition, int>();
            //subjectRepository = uoW.GetRepository<HumanSubject, string>();

            //_studyRepository = uoW.GetRepository<Study, int>();
            //_O3Repository = uoW.GetRepository<ObjectOfObservation, int>();
            //_ObsDescriptionRepository = uoW.GetRepository<ObservationDescriptor, int>();
            //_ObsDescriptorRepository = uoW.GetRepository<PropertyDescriptor, int>();
            _sdtmRepository = uoW.GetRepository<SdtmRow, Guid>();

        }


        public void DeleteDatasetObservations(int DatasetId, int DatafileId)
        {
            _ObservationRepository.DeleteMany(o => o.DatasetId == DatasetId && o.DatafileId == DatafileId );
        }
        /*
        public void LoadObservations(Dataset dataset, SdtmRowDescriptor sdtmEntityDescriptor, List<SdtmRow> sdtmRowList)
        {

            //Retrieve all Observtion Descriptors for this domain and this project if previously a dataset of the same domain has been loaded?
            var obsDescriptionList = _ObsDescriptionRepository.FindAll(
                s => s.ProjectId.Equals(dataset.Activity.ProjectId) && s.DomainId.Equals(dataset.DomainId),
                new List<Expression<Func<ObservationDescriptor, object>>>() { od => od.O3 }).ToList();

            var O3List = new List<ObjectOfObservation>();
            if (obsDescriptionList.Count != 0)
                O3List.AddRange(obsDescriptionList.Select(observationDescription => observationDescription.O3));

            #region Create O3 and ObservationDescription
            foreach (var sdtmRow in sdtmRowList)
            {
                //O3 name is not enough to uniquely identify the observation in place
                //e.g. white blood cells with two categories Heamatology and Chemistry
                //TODO: if an observation was previoulsy loaded via a dataset that had no group for it
                //e.g. white blood cells that was meant to be heamtology but not explicitly mentioned
                //then the same white blood cells is found but with a group, it will be loaded as a different O3
                //In any case, the CVterm of whitebloodcells for heamatology should be different from whitebloodcell chemsitry
                //ACTUALLY TOPIC variable SHOULD not be white blood cells but rather the TEST itself which in this case would be
                //Heamtatology test
                string O3 = sdtmRow.Topic; //THIS IS --TESTCD AND --TERM FOR NOW
                ObjectOfObservation O3obj;
                ObservationDescriptor obsDescriptor;



                if (sdtmRow.Group != null && sdtmRow.Subgroup != null)
                    O3obj = O3List.Find(o3 => o3.Name.Equals(O3)
                                              && o3.ProjectId.Equals(dataset.Activity.ProjectId)
                                              && o3.Domain.Code.Equals(sdtmRow.DomainCode)
                                              && o3.Group.Equals(sdtmRow.Group)
                                              && o3.Subgroup.Equals(sdtmRow.Subgroup));
                else if (sdtmRow.Group != null)
                {
                    O3obj = O3List.Find(o3 => o3.Name.Equals(O3)
                                              && o3.ProjectId.Equals(dataset.Activity.ProjectId)
                                              && o3.Domain.Code.Equals(sdtmRow.DomainCode)
                                              && o3.Group.Equals(sdtmRow.Group));
                }
                else
                    O3obj = O3List.Find(o3 => o3.Name.Equals(O3)
                                              && o3.ProjectId.Equals(dataset.Activity.ProjectId)
                                              && o3.Domain.Code.Equals(sdtmRow.DomainCode));
                if (O3obj != null) continue;

                //Create new O3
                O3obj = new ObjectOfObservation();
                O3obj.Name = sdtmRow.Topic; //EOS
                O3obj.Synonym = sdtmRow.TopicSynonym ?? null; //Eosinophils
                O3obj.ProjectId = dataset.Activity.ProjectId; //id for P-BVS
                O3obj.DomainId = dataset.DomainId; //id for D-SDTM-LB
                O3obj.CVTermStr = sdtmRow.TopicControlledTerm ?? null; //TEMP //Value of DECOD or LOINC
                //O3obj.CVtermId = lookupCVterm(sdtmRow.TopicVariableControlledTerm);
                O3obj.Group = sdtmRow.Group; //Hematology
                O3obj.Subgroup = sdtmRow.Subgroup; //value of SCAT
                _O3Repository.Insert(O3obj);
                O3List.Add(O3obj);

                //Create ObservationDescriptor
                obsDescriptor = new ObservationDescriptor();
                obsDescriptor.O3 = O3obj;
                obsDescriptor.DomainId = dataset.DomainId;
                obsDescriptor.ProjectId = dataset.Activity.ProjectId;

                _ObsDescriptionRepository.Insert(obsDescriptor);

            }
            _dataContext.Save();
            #endregion

            obsDescriptionList = _ObsDescriptionRepository.FindAll(
                s => s.ProjectId.Equals(dataset.Activity.ProjectId) && s.DomainId.Equals(dataset.DomainId),
                new List<Expression<Func<ObservationDescriptor, object>>>() { od => od.O3 }).ToList();

            //NOW WITH IDS TO PASS ON TO MONGO 
            O3List = new List<ObjectOfObservation>();
            O3List.AddRange(obsDescriptionList.Select(observationDescription => observationDescription.O3));

            //ALL descriptors mapped from the SDTM entity descriptor
            var descriptorsList = LoadObsDescriptors(sdtmEntityDescriptor, dataset.Activity.ProjectId);

            foreach (var sdtmRow in sdtmRowList)
            {
                //WARNING
                //depending ON THE STRING EQUALITY HERE TO IDENTIFY O3 ACROSS DATASETS AND ACROSS STUDIES OF THE SAME PROJECT
                //SHOULD REALLY USE THE CONTROLLED TERM INSTEAD (I.E. DECODE / LOINC)

                //1- GET O3
                ObjectOfObservation O3obj;
                string O3 = sdtmRow.Topic; //THIS IS --TESTCD AND --TERM FOR NOW
                if (sdtmRow.Group != null && sdtmRow.Subgroup != null)
                    O3obj = O3List.Find(o3 => o3.Name.Equals(O3)
                                              && o3.ProjectId.Equals(dataset.Activity.ProjectId)
                                              && o3.Domain.Code.Equals(sdtmRow.DomainCode)
                                              && o3.Group.Equals(sdtmRow.Group)
                                              && o3.Subgroup.Equals(sdtmRow.Subgroup));
                else if (sdtmRow.Group != null)
                {
                    O3obj = O3List.Find(o3 => o3.Name.Equals(O3)
                                              && o3.ProjectId.Equals(dataset.Activity.ProjectId)
                                              && o3.Domain.Code.Equals(sdtmRow.DomainCode)
                                              && o3.Group.Equals(sdtmRow.Group));
                }
                else
                    O3obj = O3List.Find(o3 => o3.Name.Equals(O3)
                                              && o3.ProjectId.Equals(dataset.Activity.ProjectId)
                                              && o3.Domain.Code.Equals(sdtmRow.DomainCode));


                //2- Get its observationDefinition
                var obsDescription = obsDescriptionList.Find(od => od.ObjectOfObservationId.Equals(O3obj.Id));

                //3- Create new observation
                Study study = _studyRepository.FindSingle(s => s.Name.Equals(sdtmRow.StudyId));
                var observation = new Core.Domain.Model.ObservationModel.Observation();
                observation.O3Id = O3obj.Id;
                observation.ObjectOfObservation = O3obj.Name;
                observation.Study = sdtmRow.StudyId;
                observation.StudyDBId = study != null ? study.Id : 0;
                observation.SO2Id = sdtmRow.USubjId;
                observation.SO2Type = "SUBJECT";


                #region 3.1 Add Default property value

                //THE DEFAULT DESCRIPTOR IS NOT A NATIVE DEFAULT DESCRIPTOR PRE-SPECIFIED , 
                //IT'S just a regular descriptor which could take on any qualifier value
                //e.g. OCCUR, ORRES, STRESC, STRESN
                // and only at the time of creating the obsevation first time the observationDefinition.DefaultQualifier will be set to one of the qualifiers
                //based on ... Cannot be based on information pre-assigned to the SDTMentity descriptor since that applies to the dataset as a whole and not on a row 
                //basis (will not solve the situation where some observations will have their defualt set to STRESN and others set to STRESC)
                // so at the time of observation creation first time based on the following rules set its observationDefinition.DefaultQualifier
                // if domain is findings and if there's a value for STRESN then this is the default, 
                //else if there's a value for STRESC then this is default else ORRES is the default
                //this equality is going to be achieved 


                VariableDefinition defQVariable = null; //the variable that will be read from the sdtm row as the default
                PropertyDescriptor defDescriptor; //= obsDescription.DefaultPropertyDescriptor; //the matching descriptor and set it to observationDescription

                if (dataset.Domain.Class.ToLower().Equals("findings"))
                {
                    var defVar = sdtmEntityDescriptor.ResultVariables.Find(rv => rv.Name.Equals("STRESN"));
                    if (sdtmRow.ResultQualifiers["STRESN"] != null)
                    {
                        defQVariable = sdtmEntityDescriptor.ResultVariables.Find(rv => rv.Name.Equals("STRESN"));
                        defDescriptor = descriptorsList.Find(d => d.Name.Equals("STRES"));
                    }
                    else if (sdtmRow.ResultQualifiers["STRESC"] != null)
                    {
                        defQVariable = sdtmEntityDescriptor.ResultVariables.Find(rv => rv.Name.Equals("STRESC"));
                        defDescriptor = descriptorsList.Find(d => d.Name.Equals("STRES"));
                    }
                    else //if (sdtmRow.ResultQualifiers["ORRES"] != null)
                    {
                        defQVariable = sdtmEntityDescriptor.ResultVariables.Find(rv => rv.Name.Equals("ORRES"));
                        defDescriptor = descriptorsList.Find(d => d.Name.Equals("ORRES"));
                    }
                }
                else if (sdtmEntityDescriptor.DomainCode.Equals("AE"))
                {
                    defDescriptor = descriptorsList.Find(d => d.Name.Equals("OCCUR"));
                    //there is no defQvariable in this case as we will make it up it doesn not occur in the original dataset
                }
                else
                {
                    defQVariable = sdtmEntityDescriptor.DefaultQualifier;
                    defDescriptor = descriptorsList.Find(d => d.Name.Equals(defQVariable.Name));
                }

                obsDescription.DefaultPropertyDescriptor = defDescriptor;

                //TODO: THIS COULD EITHER BE CVTERMS SO BOTH VARIABLE DEFINITION AND OBSERVATION DESCRIPTOR WOULD SHARE THE SAME CVTERM
                //todo: CHANGE ALL VARIABLE NAMES TO ENUMS


                if (sdtmEntityDescriptor.DomainCode.Equals("AE"))
                {
                    CategoricalValue propValue = new CategoricalValue();
                    propValue.Property = defDescriptor;
                    propValue.Value = "Y";
                }
                else if (defDescriptor.ValueType == ObsValueType.Numerical)
                {
                    NumericalValue propertyValue = new NumericalValue();
                    propertyValue.Value = float.Parse(sdtmRow.ResultQualifiers[defQVariable.Name]);
                    //propertyValue.Unit = 
                    propertyValue.Property = defDescriptor;
                    observation.DefaultObservedProperty = propertyValue;
                }
                else
                {
                    CategoricalValue propValue = new CategoricalValue();
                    propValue.Property = defDescriptor;
                    propValue.Value = sdtmRow.ResultQualifiers[defQVariable.Name];
                }
                #endregion

                ///////////////////////////////////////////////////////////////////////////////////////////

                #region 3.2 Add other qualifiers

                foreach (var qualifier in sdtmRow.Qualifiers)
                {
                    var key = qualifier.Key.Substring(2);
                    //GET THE MATCHING VARIABLE FROM the sdtm desriptor using names qualifier.key == 
                    //then use its name 
                    if (qualifier.Value != null)
                    {
                        //ADD descriptor to obsDescription if not previously added
                        //HOW IS THE LINK BETWEEN THE VALUE IN THE SDTM ENTITY AND THE DESCRIPTOR IS ESTABLISHED
                        //CURRENTLY IT IS USING THE SAME NAME FOR THE DESCRIPTOR AS THE NAME OF THE VARIABLE WITHOUT THE PREFIX

                        PropertyDescriptor descriptor = obsDescription.ObservedPropertyDescriptors.Find(d => d.Name.Equals(key));
                        if (descriptor == null)
                        {
                            descriptor = descriptorsList.Find(d => d.Name.Equals(key));
                            obsDescription.ObservedPropertyDescriptors.Add(descriptor);
                        }
                        //ADD new observation instance

                        if(descriptor.ValueType.Equals(ObsValueType.Categorical))
                        //TODO: for now not checking on property value type and creating categorical value for all other than result for findings
                        observation.ObservedProperties.Add(new CategoricalValue()
                        {
                            Value = qualifier.Value,
                            Property = descriptor,

                            //CVTerm = 
                        });



                    }
                }

                #endregion

                ///////////////////////////////////////////////////////////////////////////////////////////

                #region 4 Add Temporal Qualifiers

                #endregion

                #region 5 Add TimeSeries Descriptors

                #endregion
            }
        }
        
        internal List<PropertyDescriptor> LoadObsDescriptors(SdtmRowDescriptor sdtmEntityDescriptor, int projectId)
        {

            var descriptorList = new List<PropertyDescriptor>();

           // SetDescriptorTypes(sdtmEntityDescriptor);

            //IF DOMAIN.CLASS which can be identified from sdtmEntityDescriptor
            //THEN cREATE descriptor for MATCHIN CLASS
            //BUT then how will this be linked with the variable name in the data file?

            #region HACK
            //TODO: chage this to enum 

            //A HACK TO ADD OCCURENCE descriptor for adverse events which would not be in the standard
            if (sdtmEntityDescriptor.DomainCode.Equals("AE"))
            {
                var descriptor = _ObsDescriptorRepository.FindSingle(d => d.Name.Equals("OCCUR"));
                //o => o.Type == DescriptorType.
                //&& o.Domain.Equals(sdtmEntityDescriptor.DomainCode));

                if (descriptor == null)
                {
                    descriptor = PropertyDescriptorFactory.CreateAEdefDescriptor();
                    _ObsDescriptorRepository.Insert(descriptor);
                }
                descriptorList.Add(descriptor);
            }

            //A HACK BECAUSE I DONT WANT TO HAVE TWO STANDARD RESULT DESCRIPTORS AS IT IS THE CASE WITH SDTM VARAIBLES
            if (sdtmEntityDescriptor.Class.Equals("Findings"))
            {
                var descriptorST = _ObsDescriptorRepository.FindSingle(d => d.Name.Equals("STRES"));
                if (descriptorST == null)
                {
                    descriptorST = PropertyDescriptorFactory.CreateStandardResDescriptor();
                    _ObsDescriptorRepository.Insert(descriptorST);
                }
                descriptorList.Add(descriptorST);
            }
            #endregion

            foreach (var variable in sdtmEntityDescriptor.QualifierVariables.Union(sdtmEntityDescriptor.ResultVariables))
            {
                //todo: SHOULD BE ABLE TO DIFFERENTIATE HERE THE DIFFERENT SUBCLASSES OF SDTM RECORD QUALIFIERS
                //TODO: INTO FEATURE_QUALIFIERS, DEFAULT_OBSERVED_PROPERTY, OBSERVED_PROPERTIES, SUBJECT_QUALIFIERS AND SAMPLE_QUALIFIERS
                //TODO: should find equality via CVterm not name text
                //TODO: CVterm should be assigned to each variable definition which shuold then be an EQUIVALENT CVterm used for the descriptor
                var descriptor = _ObsDescriptorRepository.FindSingle(o => o.Name.Equals(variable.Name.Substring(2)));
                if (descriptor == null)
                {
                    //FOR NOW the Descriptor is CREATED FROM the variable information
                    descriptor = new PropertyDescriptor();
                    descriptor.Name = variable.Name.Substring(2);
                    //SOME KIND OF A METHOD HERE THAT RETURNS THE DESCRIPTOR TYPE THIS VARIABLE MATCHES
                    //descriptor.Type = variable.DescriptorType; //THIS IS SUPPOSED TO HAVE NOW THE NEW SUBCLASSES //DescriptorType.ObservedPropertyDescriptor;
                    descriptor.Description = variable.Label;
                    descriptor.ProjectId = projectId;

                    //_ObsDescriptorRepository.Insert(descriptor);
                }
                descriptorList.Add(descriptor);
            }

            //_dataContext.Save();

            //TODO: consider doing the same for other qualifiers such as timings and setting variable qualifiers to qualifiers

            return descriptorList;
        }
        */

        internal bool LoadObservations(List<SdtmRow> sdtmData, SdtmRowDescriptor sdtmRowDescriptor, bool reload)
        {
            var dsDomainCode = sdtmRowDescriptor.DomainCode;
            var dsClass = sdtmRowDescriptor.Class;

            var datasetId = sdtmData.First().DatasetId;
            var dataFileId = sdtmData.First().DatafileId;
            var projectId = sdtmData.First().ProjectId;

            if (reload)
            {
                _ObservationRepository.DeleteMany(o => o.DatasetId == datasetId && o.DatafileId == dataFileId);
                _dataContext.Save().Equals("CREATED");
            }


            //Retrieve ObjectOfObservations previously loaded for this project
            // CURRENTLY THIS IS NOT REALLY THE OBJECTOFOBSERVATION BUT THE OBSERVATION DESCRIPTOR WHICH IS
            //DEFINED PER DATASET/DATAFILE AND IS DELETABLE ACROSS LOADS AS LONG AS THE O3 CV IS UNIQUE ACROSS DATASETS
            //AND NOT JUST INFLUENCED BY ONE DATASET OR THE FIRST DATASET LOADED AS IT IS NOW!!
            var projectO3s = _ObservationRepository.FindAll(o => o.ProjectId == projectId && o.DomainCode == dsDomainCode).ToList();
           
            List<string> O3keys = projectO3s.Select(currObservation => currObservation.Class + currObservation.DomainCode + currObservation.Group + currObservation.Name).ToList();

            var observations = sdtmData.GroupBy(o => new {domain= o.DomainCode, o3 = o.Topic, group = o.Group, subgroup = o.Subgroup, o3CVterm = o.TopicControlledTerm??o.TopicSynonym});
            foreach (var observation in observations)
            {
                var O3key = dsClass + observation.Key.domain + observation.Key.group + observation.Key.o3;
                if(O3keys.Contains(O3key))
                    continue;
                Observation obsDescriptor = new Observation();
                obsDescriptor.Name = observation.Key.o3;
                obsDescriptor.Group = observation.Key.group;
                obsDescriptor.Subgroup = observation.Key.subgroup;
                obsDescriptor.Class = sdtmRowDescriptor.Class;
                obsDescriptor.DomainCode = sdtmRowDescriptor.DomainCode;
                obsDescriptor.DomainName = sdtmRowDescriptor.Domain;
                obsDescriptor.TopicVariable = sdtmRowDescriptor.TopicVariable;

                if (sdtmRowDescriptor.ObsIsAFinding)
                    obsDescriptor.Qualifiers.AddRange(sdtmRowDescriptor.ResultVariables.Select(
                        qualifier => new ObservationQualifier()
                        {
                            Observation = obsDescriptor, Qualifier = qualifier
                        }));
                    //sdtmRowDescriptor.ResultVariables.Union(sdtmRowDescriptor.QualifierVariables).ToList())};
                else
                    obsDescriptor.Qualifiers.AddRange(sdtmRowDescriptor.QualifierVariables.Select(
                         qualifier => new ObservationQualifier()
                         {
                             Observation = obsDescriptor,
                             Qualifier = qualifier
                         }));

                obsDescriptor.Timings.AddRange(sdtmRowDescriptor.GetAllTimingVariables().FindAll(v=>v!=null).Select(
                        qualifier => new ObservationTiming()
                        {
                            Observation = obsDescriptor,
                            Qualifier = qualifier
                        }));

                obsDescriptor.ControlledTermStr = observation.Key.o3CVterm;
   
                obsDescriptor.DatasetId = datasetId;
                obsDescriptor.DatafileId = dataFileId;
                obsDescriptor.ProjectId = projectId;

                obsDescriptor.DefaultQualifier = sdtmRowDescriptor.GetDefaultQualifier(observation.FirstOrDefault());

               

                //    //LOAD CVTERMS CORRESPONDING TO MEDdRA codes
                //    ob = CreateObsCVterms(observation.FirstOrDefault(), sdtmRowDescriptor, out ob);
                //    ob.ControlledTermId = observation.FirstOrDefault().QualifierQualifiers?[AECVtermIdVar];
                //}
                _ObservationRepository.Insert(obsDescriptor);
            }
            var success = _dataContext.Save().Equals("CREATED");

            //addO3IdstoSDTMrows()
            if (success)
            {
                //TODO: PROBLEM!!
                //IN CASE OF A SECOND FILE LOADED TO A PREVIOUSLY CREATED DATASET, DATAFILE WILL BE DIFFERENT
                //OBSERVATIONS WERE SAVED WITH THE FISRT DATASETID AND THE FIRST DATAFILE
                //WHAT HAPPENS WHERE WE WANT TO UNLOAD A FILE THAT BROUGHT DIFFERENT O3s to the DATASET DIFFERENT
                //FROM THE FIRST DATAFILE?
                //I STILL NEED TO BE ABLE TO FIND O3s that were loaded from a certain DATAFILE
                var savedObs = _ObservationRepository.FindAll(o => o.DatasetId == datasetId && o.DatafileId == dataFileId).ToList();
                foreach (var observation in observations)
                {
                    foreach (var sdtmRow in observation)
                    {
                        var O3 =
                            savedObs.FirstOrDefault(
                                o => o.Name == sdtmRow.Topic && o.Group == sdtmRow.Group && o.Subgroup == sdtmRow.Subgroup);
                        if (O3 != null) sdtmRow.DBTopicId = O3.Id;
                        _sdtmRepository.Update(sdtmRow);
                    }
                }
            }

            return success;
        }

        private Observation CreateObsCVterms(SdtmRow sdtmRow, SdtmRowDescriptor sdtmDescriptor, out Observation ob)
        {
            //const string AESOCvar = "AESOC";
            //const string AEHG
            
            throw new NotImplementedException();
        }
        /*
private void SetDescriptorTypes(SdtmEntityDescriptor sdtmEntityDescriptor)
{
List<string> FindingsFeatureDescriptors = new List<string>()
{
"STAT","REASND","NAM","METHOD","RUNID","LEAD","LLOQ","ULOQ"
};

List<string> FindingsObservedPropertyDescriptors = new List<string>()
{
"ORRES","STRESC","STRESN","RESCAT","XFN","TOXGR","SEV"
};

List<string> FindingsSampleDescriptors = new List<string>()
{
"SPEC","SPCCND","SPCUFL"
};
List<string> FindingsSubjectDescriptors = new List<string>()
{
"POS","BODSYS","LOC","CSTATE","FAST"
};

if (sdtmEntityDescriptor.Class.Equals("Findings"))
{
//iterate over all varaibels and set their descriptor type which would match the same
//descriptor type on the observation descriptor
foreach (var variable in sdtmEntityDescriptor.QualifierVariables.Union(sdtmEntityDescriptor.ResultVariables))
{
  if (FindingsFeatureDescriptors.Contains(variable.Name))
      variable.DescriptorType = DescriptorType.FeatureDescriptor;
  if (FindingsSampleDescriptors.Contains(variable.Name))
      variable.DescriptorType = DescriptorType.SampleDescriptor;
  if (FindingsSubjectDescriptors.Contains(variable.Name))
      variable.DescriptorType = DescriptorType.SubjectDescriptor;
  if (FindingsObservedPropertyDescriptors.Contains(variable.Name))
      variable.DescriptorType = DescriptorType.ObservedPropertyDescriptor;
}
}



}
*/
    }
}
