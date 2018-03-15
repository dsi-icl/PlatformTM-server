using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Core.Domain.Model.DatasetModel.SDTM;
using PlatformTM.Core.JoinEntities;

namespace PlatformTM.Services.Services.Loading.SDTM
{
    public class ObservationLoader
    {
        private readonly IServiceUoW _dataContext;
        private readonly IRepository<Observation, int> _observationRepository;
        private readonly IRepository<SdtmRow, Guid> _sdtmRepository;
        public ObservationLoader(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _observationRepository = uoW.GetRepository<Observation, int>();
            _sdtmRepository = uoW.GetRepository<SdtmRow, Guid>();
        }

        public async Task<bool> LoadObservations(Dataset dataset, int fileId, bool reload)
        {
            var sdtmRowDescriptor = SdtmRowDescriptor.GetSdtmRowDescriptor(dataset);
            List<SdtmRow> sdtmData = await _sdtmRepository.FindAllAsync(
                    dm => dm.DatasetId.Equals(dataset.Id) && dm.DatafileId.Equals(fileId));

            var dsDomainCode = sdtmRowDescriptor.DomainCode;
            var dsClass = sdtmRowDescriptor.Class;

            var datasetId = sdtmData.First().DatasetId;
            var dataFileId = sdtmData.First().DatafileId;
            var projectId = sdtmData.First().ProjectId;

            if (reload)
            {
                _observationRepository.DeleteMany(o => o.DatasetId == datasetId && o.DatafileId == dataFileId);
                _dataContext.Save();
            }


            //Retrieve ObjectOfObservations previously loaded for this project
            // CURRENTLY THIS IS NOT REALLY THE OBJECTOFOBSERVATION BUT THE OBSERVATION DESCRIPTOR WHICH IS
            //DEFINED PER DATASET/DATAFILE AND IS DELETABLE ACROSS LOADS AS LONG AS THE O3 CV IS UNIQUE ACROSS DATASETS
            //AND NOT JUST INFLUENCED BY ONE DATASET OR THE FIRST DATASET LOADED AS IT IS NOW!!
            var projectO3s = _observationRepository.FindAll(o => o.ProjectId == projectId && o.DomainCode == dsDomainCode).ToList();

            //List<string> O3keys =
            //    projectO3s.Select(
            //        currObservation =>
            //            currObservation.Class + currObservation.DomainCode + currObservation.Group +
            //            currObservation.Name).ToList();
            var O3map = projectO3s.ToDictionary(o3 => o3.Class + o3.DomainCode + o3.Group + o3.Name+o3.ControlledTermStr);

            var observations =
                sdtmData.GroupBy(
                    o =>
                        new
                        {
                            domain = o.DomainCode,
                            o3 = o.Topic,
                            group = o.Group,
                            subgroup = o.Subgroup,
                            o3CVterm = o.TopicControlledTerm ?? o.TopicSynonym
                        });
            var obsPrevLoaded = new Dictionary<string,Observation>();
            foreach (var observation in observations)
            {
                var O3key = dsClass + observation.Key.domain + observation.Key.group + observation.Key.o3 + observation.Key.o3CVterm;
                Observation o3;
                if (O3map.TryGetValue(O3key, out o3))
                {
                    obsPrevLoaded.Add(O3key,o3);
                    continue;
                }

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
                            Observation = obsDescriptor,
                            Qualifier = qualifier
                        }));
                //sdtmRowDescriptor.ResultVariables.Union(sdtmRowDescriptor.QualifierVariables).ToList())};
                
                obsDescriptor.Qualifiers.AddRange(sdtmRowDescriptor.QualifierVariables.Select(
                         qualifier => new ObservationQualifier()
                         {
                             Observation = obsDescriptor,
                             Qualifier = qualifier
                         }));

                obsDescriptor.Timings.AddRange(sdtmRowDescriptor.TimeVariables.Select(
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

                var newObs = _observationRepository.Insert(obsDescriptor);
                obsPrevLoaded.Add(O3key,newObs);
            }
            var success = _dataContext.Save().Equals("CREATED");

            if (success)
            {
                //TODO: PROBLEM!!
                //IN CASE OF A SECOND FILE LOADED TO A PREVIOUSLY CREATED DATASET, DATAFILE WILL BE DIFFERENT
                //OBSERVATIONS WERE SAVED WITH THE FISRT DATASETID AND THE FIRST DATAFILE
                //WHAT HAPPENS WHERE WE WANT TO UNLOAD A FILE THAT BROUGHT DIFFERENT O3s to the DATASET DIFFERENT
                //FROM THE FIRST DATAFILE?
                //I STILL NEED TO BE ABLE TO FIND O3s that were loaded from a certain DATAFILE
                var savedObs = _observationRepository.FindAll(o => o.DatasetId == datasetId && o.DatafileId == dataFileId).ToList();
                foreach (var observation in observations)
                {
                    foreach (var sdtmRow in observation)
                    {
                        Observation O3;
                        var o3key = dsClass + observation.Key.domain + observation.Key.group + observation.Key.o3 + observation.Key.o3CVterm;

                        if (obsPrevLoaded.TryGetValue(o3key, out O3)) sdtmRow.DBTopicId = O3.Id;
                        _sdtmRepository.Update(sdtmRow);
                    }
                }
            }

            return success;
        }

        public void UnloadObservations(int datasetId, int fileId)
        {
            _observationRepository.DeleteMany(o => o.DatafileId == fileId && o.DatasetId == datasetId);
            _sdtmRepository.DeleteMany(s => s.DatafileId == fileId && s.DatasetId == datasetId);

            _dataContext.Save();
        }
    }
}
