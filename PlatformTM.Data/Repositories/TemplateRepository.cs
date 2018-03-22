using System.Collections.Generic;
using System.Linq;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model.Templates;

namespace PlatformTM.Data.Repositories
{
    public class TemplateRepository
    {
        private readonly IRepository<DatasetTemplate,string > _repository;
        private readonly List<string> _includeFields = new List<string>() {"Fields"};
        private readonly List<string> _includeAll = new List<string>() { "Fields.ControlledVocabulary.Xref.DB" };

        public TemplateRepository(IRepository<DatasetTemplate, string> repository)
        {
            _repository = repository;
        }

        public List<DatasetTemplate> GetClinicalTemplatesWFields()
        {
            var domains = _repository.FindAll(
                   d => d.Id.Contains("D-SDTM"),
                   new List<string>(){
                        "Fields"
                   })
               .OrderBy(d => d.Class)
               .ToList();
            return domains;
        }

        public DatasetTemplate GetTemplateFullInfo(string templateId)
        {
            var template = _repository.FindSingle(
                d => d.Id.Equals(templateId), _includeAll);
            return template;
        }

        public List<DatasetTemplate> GetAssayFeatureTemplates()
        {
            var templates = _repository.FindAll(
                d => d.Class == "Assay Features", _includeFields);
            return templates.ToList();
        }

        public List<DatasetTemplate> GetAssaySampleTemplates()
        {
            var templates = _repository.FindAll(
                d => d.Class == "Assay Samples", _includeFields);
            return templates.ToList();
        }

        public List<DatasetTemplate> GetAssayDataTemplates()
        {
            var templates = _repository.FindAll(
                d => d.Class == "Assay Observations", _includeFields);
            return templates.ToList();
        }
    }
}
