/************************************************************/
/********   Created By: Dilshan Silva 12-12-2014   **********/
/********  Services to handle functions on a Study **********/
/************************************************************/

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.DTOs;

namespace eTRIKS.Commons.Service.Services
{
    public class StudyService
    {
        private IServiceUoW _studyServiceUnit;
        private readonly IRepository<Study, int> _studyRepository;
        private readonly IRepository<Project, int> _projectRepository;
        public StudyService(IServiceUoW uoW)
        {
            _studyServiceUnit = uoW;
            _studyRepository = _studyServiceUnit.GetRepository<Study, int>();
            _projectRepository = uoW.GetRepository<Project, int>();

        }

        //public void addDatasetVariables(List<VariableDefinition> variableDefinitions)
        //{
        //    for (int i = 0; i < variableDefinitions.Count; i++)
        //    {
        //        _variableDefinitionRepository.Insert(variableDefinitions[i]);
        //    }
        //    _studyServiceUnit.Save();
        //}

        public string Updatestudy(StudyDTO studyDto, int studyId)
        {
            var studyToUpdate = _studyRepository.Get(studyId);

            studyToUpdate.Name = studyDto.Name;
            studyToUpdate.Description = studyDto.Title;
            _studyRepository.Update(studyToUpdate);
            return _studyServiceUnit.Save();
        }

        public StudyDTO Addstudy(StudyDTO studyDto)
        {
            if (studyDto.ProjectAcc != null)
            {
                var project = _projectRepository.FindSingle(p => p.Accession == studyDto.ProjectAcc,
                new List<Expression<Func<Project, object>>>()
                {
                    p => p.Studies
                });
                
                if (project != null) {
                    int num = 1;
                    num = project.Studies.Count;
                    var abbr = studyDto.ProjectAcc.Substring(2, 3);
                    studyDto.Accession = "S-" + abbr + "-" + (num+1).ToString("00");
                    studyDto.ProjectId = project.Id;
                }
            }
            
            
            var study = new Study() { Name = studyDto.Name, Description = studyDto.Title , Accession = studyDto.Accession, ProjectId = studyDto.ProjectId};

            study = _studyRepository.Insert(study);
            if (!_studyServiceUnit.Save().Equals("CREATED")) return null;
            studyDto.Id = study.Id;
            studyDto.Accession = study.Accession;
            return studyDto;
        }

        public StudyDTO GetstudyId(int studyId)
        {
            var study = _studyRepository.FindSingle(s => s.Id == studyId,new List<Expression<Func<Study, object>>>()
            {
                s=>s.Project
            });
            return new StudyDTO()
            {
                Name = study.Name,
                Title = study.Description,
                Accession = study.Accession,
                Id = study.Id,
                ProjectId = study.ProjectId,
                ProjectAcc = study.Project.Accession
            };
        }
    }
}
