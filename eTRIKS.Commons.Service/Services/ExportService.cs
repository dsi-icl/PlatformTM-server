using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.DTOs;

namespace eTRIKS.Commons.Service.Services
{
    public class ExportService
    {
        private IRepository<Subject, string> _subjectRepository;
        private IRepository<SubjectObservation, Guid> _subObservationRepository;
        private IServiceUoW _dataContext;

        public ExportService(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _subObservationRepository = uoW.GetRepository<SubjectObservation, Guid>();
            _subjectRepository = uoW.GetRepository<Subject, string>();
        }
        private async Task<List<Subject>> ExportDataset(string projectId, ExportRequestDTO.Criteria criteria)
        {
            //TODO: NEED TO ADD PROJECTID to MONGORECORDS
            var filterFields = new List<object>();
            var filterField = new Dictionary<string, object>();
            filterField.Add("DOMAIN", "DM");
            filterFields.Add(filterField);
            
            foreach (var filter in criteria.ExactFilters)
            {
                filterField = new Dictionary<string, object>();
                //IMPLICIT AND between filters of different fields
                string fieldName = filter.Field;
                List<string> values = filter.Values;


                //CONSTRUCT the equivalent of a BSON $OR document for multiple filters on the same field
                var valueList = new List<Dictionary<string, string>>();
                foreach (var value in values)
                {
                    var filterValue = new Dictionary<string, string> { { fieldName, value } }; //{"STUDYID":"CRC305C"}
                    valueList.Add(filterValue);// [{"STUDYID":"CRC305C"},{"STUDYID":"CRC305A"}]
                }

                filterField.Add("$or", valueList);
                filterFields.Add(filterField);
            }

            foreach (var filter in criteria.RangeFilters)
            {
                filterField = new Dictionary<string, object>();
                //IMPLICIT AND between filters of different fields
                var fieldName = filter.Field;
                var range = filter.Range;


                //CONSTRUCT the equivalent of a BSON {field : {$gt:v,$lt:v2}}
                var valueRange = new Dictionary<string, string>();
                valueRange.Add("$gt",range.Lowerbound.ToString());
                valueRange.Add("$lt", range.Upperbound.ToString());
                filterField.Add(fieldName,valueRange);
                filterFields.Add(filterField);
            }


            //exportDTO.SubjectCriteria.Filters
            //_dataContext.AddClassMap(fieldName, "Name");
            List<Subject> subjectData = await _subjectRepository.FindAllAsync(filterFields: filterFields);

            return subjectData;
        }

        public async Task<List<Subject>>  ExportDatasets(string projectId, ExportRequestDTO dto)
        {
            List<Subject> subjectData = await ExportDataset(projectId, dto.SubjectCriteria);
           // var clinicalData = await ExportDataset(projectId, dto.ClinicalCriteria);
            //var sampleData = await ExportDataset(projectId, dto.SampleCriteria);
            return subjectData;


        }


        //public ExportRequestDTO getSampleRequest()
        //{
        //    var exportrequest = new ExportRequestDTO();
        //    var subjCriteria = new ExportRequestDTO.Criteria();
        //    exportrequest.SubjectCriteria = subjCriteria;

        //    subjCriteria.Filters.Add(new ExportRequestDTO.FilterExact(){ Field = "STUDYID", Values = new List<string>(){"CRC305C","CRC305A"}});
        //    subjCriteria.Filters.Add(new ExportRequestDTO.FilterExact() { Field = "ARMCD", Values = new List<string>() { "PLACEBO" } });
        //    subjCriteria.Filters.Add(new ExportRequestDTO.FilterRange() { Field = "AGE", Range = new ExportRequestDTO.Range(){ Upperbound = 24,Lowerbound = 12} });

        //    return exportrequest;
        //}
    }
}
