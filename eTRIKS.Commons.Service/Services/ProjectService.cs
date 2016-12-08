
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.Users;
using eTRIKS.Commons.Core.JoinEntities;
using eTRIKS.Commons.Service.DTOs;


namespace eTRIKS.Commons.Service.Services
{
    public class ProjectService
    {
        private readonly IRepository<Project, int> _projectRepository;
        private readonly IRepository<Activity, int> _activityRepository;
        private readonly IRepository<Assay, int> _assayRepository;
        private readonly IRepository<User, Guid> _userRepository;

        private IServiceUoW uoW;

        public ProjectService(IServiceUoW uoW)
        {
            this.uoW = uoW;
            _projectRepository = uoW.GetRepository<Project, int>();
            _activityRepository = uoW.GetRepository<Activity, int>();
            _assayRepository = uoW.GetRepository<Assay, int>();
            _userRepository = uoW.GetRepository<User, Guid>();
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

        public ProjectDTO GetProjectFullDetails(int projectId)
        {
            var project = _projectRepository.FindSingle(p=>p.Id == projectId, 
                new List<string>()
                {
                   "Studies.Project",
                    "Studies.Arms.Arm",
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
            var accession = "P-" + novowels.Substring(0,3).ToUpper();

            var pExist = _projectRepository.FindSingle(p => p.Accession == accession);
            if (pExist != null)
            {
                if (novowels.Length > 3)
                    accession = "P-" + (novowels.Substring(0, 2) + novowels[3]).ToUpper();
                else
                    accession = accession + "01";
            }
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
                        "Datasets.Domain",
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
                    Name = m.Domain.Name,
                    Id = m.Id,
                    DomainId = m.DomainId
                }).ToList()
            }).ToList();
        }
        
    }
}
