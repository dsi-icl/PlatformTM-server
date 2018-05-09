using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Core.Domain.Model.DatasetModel.SDTM;
using PlatformTM.Core.Domain.Model.Users.Datasets;
using PlatformTM.Core.Domain.Model.Users.Queries;
using PlatformTM.Core.JoinEntities;
using PlatformTM.Services.DTOs;
using PlatformTM.Services.DTOs.Explorer;

namespace PlatformTM.Services.Services
{
    public class ProjectService
    {
        private readonly IRepository<Project, int> _projectRepository;
        private readonly IRepository<Activity, int> _activityRepository;
        private readonly IRepository<Assay, int> _assayRepository;
        private readonly IRepository<UserDataset, Guid> _userDatasetRepository;
        private readonly IRepository<SdtmRow, Guid> _sdtmRepository;
        private readonly IRepository<CombinedQuery, Guid> _combinedQueryRepository;
        private readonly IRepository<Dataset, int> _datasetRepository;


        private IServiceUoW uoW;

        public ProjectService(IServiceUoW uoW)
        {
            this.uoW = uoW;
            _projectRepository = uoW.GetRepository<Project, int>();
            _userDatasetRepository = uoW.GetRepository<UserDataset, Guid>();
            _activityRepository = uoW.GetRepository<Activity, int>();
            _assayRepository = uoW.GetRepository<Assay, int>();
            _combinedQueryRepository = uoW.GetRepository<CombinedQuery, Guid>();
            _sdtmRepository = uoW.GetRepository<SdtmRow, Guid>();
            _datasetRepository = uoW.GetRepository<Dataset, int>();
        }

        public ProjectDTO GetProjectById(int projectId)
        {
            var project =  _projectRepository.Get(projectId);
            return new ProjectDTO()
            {
                Name = project.Name,
                Title = project.Description,
                Accession = project.Accession
            };
        }
        
        public void DeleteProject(int projectId)
        {
            var projectToDelete = _projectRepository.Get(projectId);
            _projectRepository.Remove(projectToDelete);
            _sdtmRepository.DeleteMany(s => s.ProjectId == projectId);
            uoW.Save();
        }
         
        public ProjectDTO GetProjectFullDetails(int projectId)
        {
            var project = _projectRepository.FindSingle(p=>p.Id == projectId, 
                new List<string>()
                {
                   "Studies.Project",
                    "Studies.Arms.Arm",
                    "Studies.Subjects",
                    "Activities",
                    "Users.User"
                });
            var dto =  new ProjectDTO()
            {
                Name = project.Name,
                Title = project.Title,
                Desc = project.Description,
                Accession = project.Accession,
                Type = project.Type,
                Id = project.Id,
                StudyCount = project.Studies.Count,
                CohortCount = project.Studies.Sum(s => s.Arms.Count),
                SubjectCount = project.Studies.Sum(s => s.Subjects.Count),
                Users = project.Users?
                    .Select(u => u.User) //STUPID EF1.1
                    .Select(u => new StringBuilder(u.LastName + ", " + u.FirstName).ToString()).ToList(),
                //Users = project.Users?.Select(u=>new StringBuilder(u.LastName + ", " + u.FirstName).ToString()).ToList(),
                Studies = project.Studies.Select(
                    s=>new StudyDTO()
                    {
                        Accession = s.Accession,
                        Title = s.Description,
                        ProjectAcc = s.Project.Accession,
                        ProjectId = s.ProjectId,
                        Name = s.Name,
                        Id = s.Id,
                        ArmCount = s.Arms.Count,
                        ArmNames = s.Arms.Select(a=>a.Arm).SelectMany(a => new string[] { a.Name })
                        //ArmNames = s.Arms.SelectMany(a=>new string[] {a.Name} )
                    }).ToList()
            };
            //var users = project.Users?.Select(u => new StringBuilder(u.LastName + ", " + u.FirstName).ToString()).ToList();
            //var ownusers.Add(proj);
            return dto;
        }

		public ProjectDTO AddProject(ProjectDTO projectDto, string ownerId)
        {
            var name = projectDto.Name;
            string novowels = Regex.Replace(name, "(?<!^)[aouieyAOUIEY]", "");
            //var accession = "P-" + novowels.Substring(0,3).ToUpper();

            //var pExist = _projectRepository.FindSingle(p => p.Accession == accession);
            //if (pExist != null)
            //{
            //    if (novowels.Length > 3)
            //        accession = "P-" + (novowels.Substring(0, 2) + novowels[3]).ToUpper();
            //    else
            //        accession = accession + "01";
            //}
            var accession = RandomString(5);
            var project = new Project()
            {
                Name = projectDto.Name,
                Description = projectDto.Desc,
                Accession = accession,
                OwnerId = Guid.Parse(ownerId),
                Title = projectDto.Title,
                Type = projectDto.Type
            };


            //var owner =_userRepository.Get(Guid.Parse(ownerId));
            //project.Users = new List<User>() {owner};
            project.Users.Add(new ProjectUser() {ProjectId = projectDto.Id,UserId = Guid.Parse(ownerId) });
            project = _projectRepository.Insert(project);


            if (!uoW.Save().Equals("CREATED")) return null;
            projectDto.Id = project.Id;
            projectDto.Accession = project.Accession;
            return projectDto;
        }

        public string UpdateProject(ProjectDTO projectDto, int projectId)
        {
            var projectToUpdate = _projectRepository.Get(projectId);

            projectToUpdate.Name = projectDto.Name;
            projectToUpdate.Title = projectDto.Title;
            projectToUpdate.Description = projectDto.Desc;
            projectToUpdate.Type = projectDto.Type;
            _projectRepository.Update(projectToUpdate);
            return uoW.Save();
        }

        public IEnumerable<ProjectDTO> GetProjects(string userId)
        {
            var guidUserID = Guid.Parse(userId);
            var projects = _projectRepository.FindAll(
                p=>p.Users.Select(u=>u.User).Any(s=>s.Id == guidUserID) || p.OwnerId==guidUserID || p.IsPublic, 
                new List<string>()
                {
                   "Studies.Arms",
                   "Studies.Subjects"
                }).Select(p=> new ProjectDTO()
            {
                Accession = p.Accession,
                Id = p.Id,
                Name = p.Name,
                Title = p.Title,
                Desc = p.Description,
                Type = p.Type,
                StudyCount = p.Studies.Count,
                CohortCount = p.Studies.Sum(s=>s.Arms.Count),
                SubjectCount = p.Studies.Sum(s=>s.Subjects.Count)
            });
            return projects;
        }

        public IEnumerable<ActivityDTO> GetProjectActivities(int projectId)
        {
            IEnumerable<Activity> Activities;
            Activities = _activityRepository.FindAll(
                    d => d.ProjectId == projectId,
                    new List<string>(){
                        "Datasets.Template",
                        "Project"
                    }
                );
            return Activities.Select(p => new ActivityDTO
            {
                Name = p.Name,
                Id = p.Id,
                ProjectId = p.ProjectId,
                ProjectAcc = p.Project.Accession,
                isAssay = typeof(Assay) == p.GetType(),
                datasets = p.Datasets.Select(m => new DatasetDTO
                {
                    Name = m.Template.Domain,
                    Id = m.Id,
                    DomainId = m.TemplateId
                }).ToList()
            }).ToList();
        }

        public List<CombinedQueryDTO> GetProjectSavedQueries(int projectId, string userId)
        {
            var userQueries = _combinedQueryRepository.FindAll(d => d.UserId == Guid.Parse(userId) 
            && d.ProjectId == projectId
            && d.IsSavedByUser).ToList();
            var dtoQueries = userQueries.Select(QueryService.GetcQueryDTO).ToList();
            return dtoQueries;
        }

        public List<UserDatasetDTO> GetProjectSavedDatasets(int projectId, string UserId)
        {
            List<UserDataset> datasets = _userDatasetRepository.FindAll(
                d => d.OwnerId == UserId.ToString() && d.ProjectId == projectId ).ToList();
            return datasets.OrderBy(d => d.ProjectId).ThenBy(d => d.QueryId).Select(UserDatasetService.WriteDTO).ToList();
        }

        public List<UserDTO> GetProjectUsers(int projectId)
        {
            var project = _projectRepository.FindSingle(p => p.Id == projectId,
                new List<string>() {"Users.User"});
            var users = project.Users.Select(u => new UserDTO()
            {
                FirstName = u.User.FirstName,
                LastName = u.User.LastName,
                Organization = u.User.Organization,
                Email = u.User.Email,
                
            }).ToList();
            return users;
        }

        public List<DatasetVM> GetProjectClinicalDatasets(int projectId)
        {
            IEnumerable<Activity> Activities;
            Activities = _activityRepository.FindAll(
                d => (d.ProjectId == projectId && typeof(Activity)==d.GetType()),
                    new List<string>(){
                        "Datasets.Template",
                        "Datasets.DataFiles.Datafile"
                    }
                );
            var datasets = Activities.SelectMany(a => a.Datasets).Select(d=> new DatasetVM(){
                Id = d.Id,
                Name = d.Template.Domain,
                Files = d.DataFiles.Select(df => new FileVM(){
                    Id = df.DatafileId,
                    FileName = df.Datafile.FileName,
                    DataType = df.Datafile.DataType,
                    DateLastModified = df.Datafile.LastModified
                }).ToList()
            }).ToList();

            return datasets;
        }

        public List<AssayVM> GetProjectAssayDatasets(int projectId)
        {
            var assays = _assayRepository.FindAll(
                d => (d.ProjectId == projectId && typeof(Assay) == d.GetType()),
                    new List<string>(){
                        "Datasets.Template",
                        "TechnologyType",
                        "TechnologyPlatform",
                        "MeasurementType",
                        "Datasets.DataFiles.Datafile"
                    }
                );

            var assayVMs = assays.Select(a => new AssayVM()
            {
                Id = a.Id,
                Name = a.Name,
                Platform = a.TechnologyPlatform?.Name,
                Technology = a.TechnologyType?.Name,
                Type = a.MeasurementType?.Name,
                Datasets = a.Datasets.Select(d => new DatasetVM()
                {
                    Id = d.Id,
                    Name = d.Template.Domain,
                    Files = d.DataFiles.Select(df => new FileVM()
                    {
                        Id = df.DatafileId,
                        FileName = df.Datafile.FileName,
                        DataType = df.Datafile.DataType,
                        DateLastModified = df.Datafile.LastModified
                    }).ToList()
                }).ToList()
            }).ToList();

            return assayVMs;
        }


        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
