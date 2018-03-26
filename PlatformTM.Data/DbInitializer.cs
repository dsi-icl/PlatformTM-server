using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model.ControlledTerminology;
using PlatformTM.Core.Domain.Model.Templates;
using PlatformTM.Data.Configuration;

namespace PlatformTM.Data
{
    public class DbInitializer
    {
        private IServiceUoW _db;
        private  IRepository<DatasetTemplate, string> _desriptorRepository;
        private  IRepository<Dictionary, string> _dictRepository;
        private readonly List<string> _includeFields = new List<string>() { "Fields" };
        private readonly List<string> _includeDictionary = new List<string>() { "Fields.ControlledVocabulary" };
        private readonly List<string> _includeTermsXrefDB = new List<string>() { "Terms.Xref.DB", "Xref.DB" };
        private DataAccessSettings _ConfigSettings { get; set; }

        const string _termsJSONfilename = "dictionaryDump.json";
        const string _templatesJSONfilename = "templatesDump.json";

        private string _JSONDir;

        public DbInitializer(IServiceUoW uoW, IOptions<DataAccessSettings> settings)
        {
            _db = uoW;
            _ConfigSettings = settings.Value;
            _JSONDir = _ConfigSettings.DBinitDirectory;
            Directory.CreateDirectory(_JSONDir);
        }

        public void SeedDB()
        {
            LoadDictionaries();
            LoadTemplates();
        }

        public void LoadDictionaries()
        {

            var reader = File.OpenText(Path.Combine(_JSONDir, _termsJSONfilename));

            List<Dictionary> dicts = JsonConvert.DeserializeObject<List<Dictionary>>(reader.ReadToEnd());

            _dictRepository = _db.GetRepository<Dictionary, string>();
            _dictRepository.InsertMany(dicts);

            _db.Save();
            reader.Dispose();
        }

        public void LoadTemplates()
        {

            var reader = File.OpenText(Path.Combine(_JSONDir, _templatesJSONfilename));

            List<DatasetTemplate> templates = JsonConvert.DeserializeObject<List<DatasetTemplate>>(reader.ReadToEnd());

            _desriptorRepository = _db.GetRepository<DatasetTemplate, string>();
            _desriptorRepository.InsertMany(templates);

            _db.Save();
            reader.Dispose();
        }

        public void DumpTemplatesJSON()
        {
            var domains = _desriptorRepository.FindAll(
                   null,
                _includeFields)
               .OrderBy(d => d.Class)
               .ToList();

            JsonSerializer jsonSerializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };

            using (StreamWriter sw = new StreamWriter(File.Create(Path.Combine(_JSONDir, _templatesJSONfilename))))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartArray();
                foreach (var temp in domains)
                {
                    jsonSerializer.Serialize(writer, temp);
                }
                writer.WriteEndArray();
            }
        }

        public void DumpCVterms()
        {
            var cvterms = _dictRepository.FindAll(null, _includeTermsXrefDB).OrderBy(d => d.Id).ToList();
            JsonSerializer serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };
            using (StreamWriter sw = new StreamWriter(File.Create(Path.Combine(_JSONDir, _termsJSONfilename))))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartArray();
                foreach (var term in cvterms)
                {
                    serializer.Serialize(writer, term);
                }
                writer.WriteEndArray();
            }
        }

    }
}
