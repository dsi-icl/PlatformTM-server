using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS.DatasetDescriptorTypes;
using PlatformTM.Core.Domain.Model.DatasetModel.SDTM;
using PlatformTM.Core.Domain.Model.Users;
using PlatformTM.Core.Domain.Model.Users.Datasets;
using PlatformTM.Core.Domain.Model.Users.Queries;
using PlatformTM.Core.JoinEntities;
using PlatformTM.Models.DTOs;
using PlatformTM.Models.DTOs.Explorer;

namespace PlatformTM.Models.Services
{
    public class ProjectService
    {
        private readonly IRepository<Project, int> _projectRepository;
        private readonly IRepository<Assessment, int> _activityRepository;
        private readonly IRepository<Assay, int> _assayRepository;
        private readonly IRepository<SdtmRow, Guid> _sdtmRepository;
        private readonly IRepository<CombinedQuery, Guid> _combinedQueryRepository;
        private readonly IRepository<Dataset, int> _datasetRepository;
        private readonly IRepository<User, Guid> _userRepository;

        private IServiceUoW uoW;

        public ProjectService(IServiceUoW uoW)
        {
            this.uoW = uoW;
            _projectRepository = uoW.GetRepository<Project, int>();
            _activityRepository = uoW.GetRepository<Assessment, int>();
            _assayRepository = uoW.GetRepository<Assay, int>();
            _combinedQueryRepository = uoW.GetRepository<CombinedQuery, Guid>();
            _sdtmRepository = uoW.GetRepository<SdtmRow, Guid>();
            _datasetRepository = uoW.GetRepository<Dataset, int>();
            _userRepository = uoW.GetRepository<User, Guid>();
        }

        public ProjectDTO GetProjectById(int projectId)
        {
            var project =  _projectRepository.Get(projectId);
            return new ProjectDTO()
            {
                Name = project.Name,
                Title = project.Title,
                Accession = project.Accession,
                Desc = project.Description
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
                   
                    "Studies.Cohorts",
                    "Studies.Subjects",
                    "Members"
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
                CohortCount = project.Studies.Sum(s => s.Cohorts.Count),
                SubjectCount = project.Studies.Sum(s => s.Subjects.Count),
                Users = project.Members?
                    .Select(u => u) //STUPID EF1.1
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
                        ArmCount = s.Cohorts.Count,
                        ArmNames = s.Cohorts.SelectMany(a => new string[] { a.Name })
                        //ArmNames = s.Arms.SelectMany(a=>new string[] {a.Name} )
                    }).ToList()
            };
            //var users = project.Users?.Select(u => new StringBuilder(u.LastName + ", " + u.FirstName).ToString()).ToList();
            //var ownusers.Add(proj);
            return dto;
        }

        public IEnumerable<ObservationDatasetDescriptor> GetDescriptors(int projectId)
        {
            var descriptors = new List<ObservationDatasetDescriptor>();
            descriptors.Add(new ObservationDatasetDescriptor("MH Dataset Descriptor"));
            return descriptors;
        }

        public ProjectDTO AddProject(ProjectDTO projectDto, string ownerId)
        {
            var name = projectDto.Name;
            string novowels = Regex.Replace(name, "(?<!^)[aouieyAOUIEY]", "");

            string accession;
            if(novowels.Length >=3)

             accession = "P-" + novowels.Substring(0,3).ToUpper();

            //var pExist = _projectRepository.FindSingle(p => p.Accession == accession);
            //if (pExist != null)
            //{
            //    if (novowels.Length > 3)
            //        accession = "P-" + (novowels.Substring(0, 2) + novowels[3]).ToUpper();
            //    else
            //        accession = accession + "01";
            //}
            else accession = RandomString(5);
            var project = new Project()
            {
                Name = projectDto.Name,
                Description = projectDto.Desc,
                Accession = accession,
                OwnerId = Guid.Parse(ownerId),
                Title = projectDto.Title,
                Type = projectDto.Type
            };


            var owner =_userRepository.Get(Guid.Parse(ownerId));
            project.Members = new List<User>() {owner};
            //project.Members.Add(new User() {  ProjectId = projectDto.Id,UserId = Guid.Parse(ownerId) });
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
                p=>p.Members.Select(u=>u).Any(s=>s.Id == guidUserID) || p.OwnerId==guidUserID || p.IsPublic, 
                new List<string>()
                {
                   "Studies.Cohorts",
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
                CohortCount = p.Studies.Sum(s=>s.Cohorts.Count),
                SubjectCount = p.Studies.Sum(s=>s.Subjects.Count)
            });
            return projects;
        }

        public IEnumerable<ActivityDTO> GetActivities(int projectId, Type type)
        {
             
            IEnumerable<Assessment> Activities = null;

            if(type == null)
            {
                Activities = _activityRepository.FindAll(
                    d => d.Study.Project.Id == projectId,
                    new List<string>(){
                        
                        "Study.Project"
                    }
                );
            }
            else if (type == typeof(Assessment))

             Activities = _activityRepository.FindAll(
                    d => d.Study.Project.Id == projectId && d is Assessment
                );
            else if (type == typeof(Assay))
                Activities = _activityRepository.FindAll(
                    d => d.Study.Project.Id == projectId && d is Assay
                );
            else if (type == typeof(SubjectRecording))
                Activities = _activityRepository.FindAll(
                    d => d.Study.Project.Id == projectId && d is SubjectRecording
                );

            if (Activities == null)
                return null;
            return Activities.Select(p => new ActivityDTO
            {
                Name = p.Name,
                Id = p.Id,
                ProjectId = p.Id,
                //ProjectAcc = p.Study.Project.Accession,
                isAssay = p is Assay,
                //datasets = p.Datasets.Select(m => new DatasetDTO
                //{
                //    Name = m.Domain,
                //    Id = m.Id,
                //    DomainId = m.Domain
                //}).ToList()
            }).ToList();
        }

        public List<UserDTO> GetProjectUsers(int projectId)
        {
            var project = _projectRepository.FindSingle(p => p.Id == projectId,
                new List<string>() {"Members"});
            var users = project.Members.Select(u => new UserDTO()
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                Organization = u.Organization,
                Email = u.Email,
                
            }).ToList();
            return users;
        }

        public List<DatasetVM> GetProjectClinicalDatasets(int projectId)
        {
            //IEnumerable<Assessment> Activities;
            //Activities = _activityRepository.FindAll(
            //    d => (d.ProjectId == projectId && (d is Assessment || d is SubjectRecording)),
            //        new List<string>(){
            //            "Datasets.Template",
            //            "Datasets.DataFiles.Datafile"
            //        }
            //    ).ToList();
            //var datasets = Activities.SelectMany(a => a.Datasets).Select(d=> new DatasetVM(){
            //    Id = d.Id,
            //    Name = d.Template.Domain,
            //    Files = d.DataFiles.Select(df => new FileVM(){
            //        Id = df.DatafileId,
            //        FileName = df.Datafile.FileName,
            //        DataType = df.Datafile.DataType,
            //        DateLastModified = df.Datafile.LastModified
            //    }).ToList()
            //}).ToList();

            return null;
        }

        public List<AssayVM> GetProjectAssayDatasets(int projectId)
        {
     //       var assays = _assayRepository.FindAll(
     //           d => (d.Study.Project.Id == projectId),
     //               new List<string>(){
                        
     //                   "Project",
     //                   "TechnologyType",
     //                   "TechnologyPlatform",
     //                   "MeasurementType",
     //                   "Datasets.DataFiles"
     //               }
     //           );

     //       var assayVMs = assays.Select(a => new AssayVM()
     //       {
     //           Id = a.Id,
     //           Name = a.Name,
     //           Platform = a.TechnologyPlatform?.Name,
     //           Technology = a.TechnologyType?.Name,
     //           Type = a.MeasurementType?.Name,
     //           Datasets = a.Datasets.Select(d => new DatasetVM()
     //           {
     //               Id = d.Id,
					//Name = d.Descriptor.Title == "BS"?"Samples Annotation":d.Domain,
     //               Files = d.DataFiles.Select(df => new FileVM()
     //               {
     //                   Id = df.Id,
     //                   FileName = df.FileName,
     //                   DataType = df.Type,
     //                   DateLastModified = df.Modified
     //               }).ToList()
     //           }).ToList()
     //       }).ToList();

            return null;
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
