/************************************************************/
/********   Created By: Dilshan Silva 12-12-2014   **********/
/********  Services to handle functions on a Study **********/
/************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;

namespace eTRIKS.Commons.Service.Services
{
    public class StudyService
    {
        private IRepository<VariableDefinition, int> _variableDefinitionRepository;
        private IServiceUoW _studyServiceUnit;

        public StudyService(IServiceUoW uoW)
        {
            _studyServiceUnit = uoW;
            _variableDefinitionRepository = uoW.GetRepository<VariableDefinition, int>();
        }

        public void addDatasetVariables(List<VariableDefinition> variableDefinitions)
        {
            for (int i = 0; i < variableDefinitions.Count; i++)
            {
                _variableDefinitionRepository.Insert(variableDefinitions[i]);
            }
            _studyServiceUnit.Save();
        }
    }
}
