using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel;
using eTRIKS.Commons.Core.Domain.Model.ObservationModel;
using eTRIKS.Commons.Service.DTOs;
using Observation = eTRIKS.Commons.Core.Domain.Model.ObservationModel.Observation;

namespace eTRIKS.Commons.Service.Services.Loading.AssayData
{
    public class HDloader
    {
        private readonly IRepository<Dataset, int> _datasetRepository;
        private readonly IRepository<DataFile, int> _dataFileRepository;
        private IRepository<Observation, Guid> _observationRepository;
        private IServiceUoW _dataContext;

        public HDloader(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _datasetRepository = uoW.GetRepository<Dataset, int>();
            _dataFileRepository = uoW.GetRepository<DataFile, int>();
            _observationRepository = uoW.GetRepository<Observation, Guid>();
        }

        internal  bool LoadHDdata(int datasetId, int fileId, DataTable dataTable)
        {
            var dataset = _datasetRepository.FindSingle(d => d.Id == datasetId);
            var dataFile = _dataFileRepository.Get(fileId);

            //if (!dataFile.State.ToLower().Equals("new"))
            //{
            //    /**
            //     * Replacing previously loaded file
            //     * Remove file from collection before reloading it
            //     */
            //    _observationRepository.DeleteMany(s => s.DatafileId == fileId && s.DatasetId == datasetId);
            //    Debug.WriteLine("RECORD(s) SUCCESSFULLY DELETED FOR DATASET:" + datasetId + " ,DATAFILE:" + fileId);
            //}

            var projectId = dataFile.ProjectId;
            var observations = new List<Observation>();
            var totalLoaded = 0.0;
            var prop = new PropertyDescriptor()
            {
                Name = "CONC",
                ProjectId = projectId,
                Description = "Concentration",
                ObsClass = "ASSAYMEASURES",
                Id = 99
            };

            try
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    var obs = new Observation();

                    obs.FeatureName = row["FEAT"]?.ToString();
                    obs.SubjectOfObservationId = row["SPECIMEN"]?.ToString();
                    obs.SubjectOfObservationName = row["SPECIMEN"]?.ToString();
                    double val;
                    if(double.TryParse(row["OBSVAL"]?.ToString(), out val))
                        obs.ObservedValue = new NumericalValue()
                        {
                            Property = prop,
                            Unit = row["OBSVALU"]?.ToString(),
                            Value = val
                        };
                    else
                    {
                        obs.ObservedValue = new MissingValue() {Property = prop};
                    }
                    obs.Id = Guid.NewGuid();
                    obs.DatasetId = datasetId;
                    obs.ActivityId = dataset.ActivityId;
                    obs.DatafileId = fileId;
                    obs.ProjectId = projectId;


                    observations.Add(obs);

                    if (observations.Count % 500 == 0)
                    {
                        _observationRepository.InsertMany(observations);
                        totalLoaded += observations.Count;
                        dataFile.State = Math.Round(totalLoaded/dataTable.Rows.Count*100).ToString("##");

                        _dataFileRepository.Update(dataFile);
                        _dataContext.Save();
                        observations.Clear();
                    }
                }
                _observationRepository.InsertMany(observations);
                Debug.WriteLine(dataTable.Rows.Count + " RECORD(s) SUCCESSFULLY ADDED FOR DATASET:" + datasetId + " ,DATAFILE:" + fileId);
            }
            catch (Exception)
            {
                dataFile.State = "FAILED";
                dataFile.IsLoadedToDB = false;
                _dataFileRepository.Update(dataFile);
                _dataContext.Save();
                return false;
            }


            dataFile.State = "LOADED";
            dataFile.IsLoadedToDB = true;
            _dataFileRepository.Update(dataFile);
            _dataContext.Save();
            return true;
        }
    }
}
