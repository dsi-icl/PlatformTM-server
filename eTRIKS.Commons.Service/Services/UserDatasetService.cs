using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel.SDTM;
using eTRIKS.Commons.Core.Domain.Model.DesignElements;
using eTRIKS.Commons.Core.Domain.Model.Users.Datasets;
using eTRIKS.Commons.Service.DTOs;

namespace eTRIKS.Commons.Service.Services
{
    public class UserDatasetService
    {
        private readonly IRepository<UserDataset, Guid> _userDatasetRepository;
        private IRepository<Project, int> _projectRepository;
        private readonly IServiceUoW _unitOfWork;

        public UserDatasetService(IServiceUoW uoW)
        {
            _unitOfWork = uoW;
            _userDatasetRepository = uoW.GetRepository<UserDataset, Guid>();
            _projectRepository = uoW.GetRepository<Project, int>();
        }

        public void DeleteDataset(string datasetId) 
        {
            _userDatasetRepository.DeleteMany(d => d.Id == Guid.Parse(datasetId));
        }

        public List<UserDatasetDTO> GetUserProjectDatasets(int projectId, string UserId)
        {
            //var project = _projectRepository.FindSingle(p => p.Id == projectId);
            List<UserDataset> datasets = _userDatasetRepository.FindAll(
                d => d.OwnerId == UserId.ToString() && d.ProjectId == projectId).ToList();
            return datasets.OrderBy(d=>d.ProjectId).ThenBy(d=>d.QueryId).Select(WriteDTO).ToList();
        }

        public List<UserDatasetDTO> GetUserDatasets(string userId)
        {
            List<UserDataset> datasets = _userDatasetRepository.FindAll(
                d => d.OwnerId == userId && d.IsSaved).ToList();
            return datasets.OrderBy(d => d.ProjectId).ThenBy(d => d.QueryId).Select(WriteDTO).ToList();
        }

        public UserDatasetDTO GetUserDataset(string datasetId, string userId)
        {
            var userDataset = _userDatasetRepository.FindSingle(d => d.Id == Guid.Parse(datasetId));
            var dto = WriteDTO(userDataset);
            return dto;
        }

        public UserDatasetDTO AddUserDataset(UserDatasetDTO dto, string userId)
        {
            var NewDS = ReadDTO(dto);
            NewDS.OwnerId = userId;

            //var project = _projectRepository.FindSingle(p => p.Accession == dto.ProjectAcc);
            NewDS.ProjectId = dto.ProjectId;
            NewDS.Id = Guid.NewGuid();

            var addedUserDataset = _userDatasetRepository.Insert(NewDS);
            if (!_unitOfWork.Save().Equals("CREATED")) return null;
            dto.Id = addedUserDataset.Id.ToString();
            return dto;
        }

        public void UpdateUserDataset(UserDataset dataset, string userId)
        {
            //check that the owner of this dataset is the caller
            //var dataset = ReadDTO(dto);
            var datasetToUpdate = _userDatasetRepository.FindSingle(d => d.Id == dataset.Id);
            //datasetToUpdate = ReadDTO(dto, datasetToUpdate);
            datasetToUpdate.IsSaved = true;
            datasetToUpdate.LastModified = DateTime.Today.ToString("f");
            datasetToUpdate.Description = dataset.Description;
            datasetToUpdate.Name = dataset.Name;
            _userDatasetRepository.Update(datasetToUpdate);
        }

        private UserDataset ReadDTO(UserDatasetDTO dto, UserDataset copyInto=null)
        {
            UserDataset ds;
            ds = copyInto ?? new UserDataset();

            ds.Name = dto.Name;
            ds.Description = dto.Description;
            ds.Tags = dto.Tags;
            //ds.OwnerId = dto.OwnerId;
            //ds.ProjectId = dto.ProjectId;
            ds.Type = dto.Type;
            ds.LastModified = DateTime.Now.ToString("d");// ToShortDateString();
        
            ds.Filters= new List<DataFilter>();
            //ds.Fields = new List<DataField>();
            foreach (var filterDto in dto.Filters)
            {

                if (filterDto.IsNumeric)
                {
                    var filter = new DataFilterRange();
                    filter.DataField = getDataField(filterDto.Field);
                    filter.Lowerbound = filterDto.From;
                    filter.Upperbound = filterDto.To;
                    ds.Filters.Add(filter);
                }
                else
                {
                    var filter = new DataFilterExact();
                    filter.DataField = getDataField(filterDto.Field);
                    filter.Values = filterDto.FilterValues;
                    ds.Filters.Add(filter);
                }

            }

            foreach (var fieldDto in dto.Fields)
            {
               // ds.Fields.Add(getDataField(fieldDto));
            }
            return ds;
        }

        public static UserDatasetDTO WriteDTO(UserDataset dataset)
        {
            return new UserDatasetDTO()
            {
                Id = dataset.Id.ToString(),
                Description = dataset.Description,
                LastModified = dataset.LastModified,
                Name = dataset.Name,
                OwnerId = dataset.OwnerId,
                ProjectId = dataset.ProjectId,
                Type = dataset.Type,
                SubjectCount = dataset.SubjectCount,
                SampleCount = dataset.SampleCount,
                FileStatus = dataset.FileStatus,
                Filters = dataset.Filters.Select(f=> new DataFilterDTO()
                {
                    
                }).ToList(),
                //Fields = dataset.Fields.Select(f => new DataFieldDTO()
                //{
                //    Entity = f.Entity,
                //    EntityId = f.EntityId,
                //    Property = f.Property,
                //    PropertyId = f.PropertyId,
                //    DataType = f.DataType,
                //    IsFiltered = f.IsFiltered,
                //    FieldName = f.FieldName
                //}).ToList()
            };
        }

        private DataField getDataField(DataFieldDTO dto)
        {
            var field = new DataField()
            {
                Entity = dto.Entity,
                EntityId = dto.EntityId,
                Property = dto.Property,
                PropertyId = dto.PropertyId,
                DataType = dto.DataType,
                IsFiltered = dto.IsFiltered,
                FieldName = dto.FieldName
            };
            return field;
        }
    }
}
