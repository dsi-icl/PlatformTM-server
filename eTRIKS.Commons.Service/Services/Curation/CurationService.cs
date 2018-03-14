using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.RegularExpressions;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using eTRIKS.Commons.Core.Domain.Model.Curation;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel;
using eTRIKS.Commons.Core.Domain.Model.Templates;
using eTRIKS.Commons.Service.DTOs.Curation;

namespace eTRIKS.Commons.Service.Services.Curation
{
    public class CurationService
    {
        private readonly IServiceUoW _dataServiceUnit;
        private readonly IRepository<DataFile, int> _fileRepository;
        private readonly IRepository<SingleColumn, Guid> _singleColumnsRepository;
        private readonly IRepository<SingleRow, Guid> _singleRawsRepository;
        private readonly FileService _fileService;

        public CurationService(IServiceUoW uoW, FileService fileService)
        {
            _dataServiceUnit = uoW;
            _fileRepository = uoW.GetRepository<DataFile, int>();
            _singleColumnsRepository = uoW.GetRepository<SingleColumn, Guid>();
            _singleRawsRepository = uoW.GetRepository<SingleRow, Guid>();
            _fileService = fileService;
        }

        public void CsvToSingleRows(int fileId)
        {
           _singleRawsRepository.DeleteMany(d => d.FileId == fileId);

             
            // File to DATATABLE 
            var file = _fileRepository.Get(fileId);
            string filePath = file.Path + "\\" + file.FileName;
            DataTable dataTable = _fileService.ReadOriginalFile(filePath);

            

            foreach (DataRow row in dataTable.Rows)
            {

                SingleRow singleRow = new SingleRow();
                singleRow.FileId = fileId;
                singleRow.Id = Guid.NewGuid();

                // Dictionary<string,string> dic = new Dictionary<string, string>();
                singleRow.RowValues = new Dictionary<string, string>();
                foreach (var col in dataTable.Columns)
                {
                    singleRow.RowValues.Add(col.ColumnName, row[col].ToString());
                }
                _singleRawsRepository.Insert(singleRow);
                //singleRow.listofRows.Add(dic);
            }
           
            
        }
        
        public void CsvToSingleColumns(int fileId)
        {
            _singleColumnsRepository.DeleteMany(d => d.colHeader.DataFileId == fileId);

            // File to DATATABLE 
            var file = _fileRepository.Get(fileId);
            string filePath = file.Path + "\\" + file.FileName;
            DataTable dataTable = _fileService.ReadOriginalFile(filePath);
            foreach (DataColumn column in dataTable.Columns)
            {
                SingleColumn singlecolumn = new SingleColumn();

                // LIST OF VALUES FOR EACH COLUMN 
                singlecolumn.colValues = new List<object>();
                foreach (var row in dataTable.Rows)
                {
                    singlecolumn.colValues.Add(row[column]);
                }
                singlecolumn.colHeader = new HeaderElements();

                // COLUMN HEADER INFORMATION 
                singlecolumn.colHeader.ColumnName = column.ColumnName;
                singlecolumn.colHeader.DataFileId = fileId;

                Regex Num = new Regex("^(\\d|-)?(\\d|,)*\\.?\\d*$");
                Regex Time = new Regex("^(([0-1]?[0-9])|([2][0-3])):([0-5]?[0-9])(:([0-5]?[0-9]))?$");
                Regex Date = new Regex("^((0?[13578]|10|12)(-|\\/)(([1-9])|(0[1-9])|([12])([0-9]?)|(3[01]?))(-|\\/)((19)([2-9])(\\d{1})|(20)([01])(\\d{1})|([8901])(\\d{1}))|(0?[2469]|11)(-|\\/)(([1-9])|(0[1-9])|([12])([0-9]?)|(3[0]?))(-|\\/)((19)([2-9])(\\d{1})|(20)([01])(\\d{1})|([8901])(\\d{1})))$");

                Match TimeMatch = Time.Match(singlecolumn.colValues.First().ToString());
                Match DateMatch = Date.Match(singlecolumn.colValues.First().ToString());
                Match NumMatch = Num.Match(singlecolumn.colValues.First().ToString());

                if (DateMatch.Success)
                {
                    // singlecolumn.colHeader.ColumnType = SingleColumn.TypeValue.Date;
                    singlecolumn.colHeader.Type = "Date";
                }
                else if (TimeMatch.Success)
                {
                    // singlecolumn.colHeader.ColumnType = SingleColumn.TypeValue.Time;
                    singlecolumn.colHeader.Type = "Time";
                }
                else if (NumMatch.Success)
                {
                    // singlecolumn.colHeader.ColumnType = SingleColumn.TypeValue.Num;
                    singlecolumn.colHeader.Type = "Num";
                }
                else
                {
                    // singlecolumn.colHeader.ColumnType = SingleColumn.TypeValue.String;
                    singlecolumn.colHeader.Type = "String";
                }

                _singleColumnsRepository.Insert(singlecolumn);
            }
        }

        public List<List<CurationDTO>> GetSuggestions(int fileId)
        {
            var headersToCheck = _singleRawsRepository.FindAll(s => s.FileId == fileId).First().RowValues.Keys.ToList();
            List<List<CurationDTO>> listOfListDto = null;

            foreach (var header in headersToCheck)
            {

                List<CVterm> suggestions = new List<CVterm>();   // in reality this will  be in another class
                                                               // if OLS has any suggestions we return a list of DTOs. each contains info about each sugestion


                List < CurationDTO > listOfDto = null;
                
                // if there uis any suggestion 
                if (suggestions.ToString().Any())
                {
                    listOfDto = new List<CurationDTO>();
                    foreach (var suggestion in suggestions)
                    {
                        var curationDTO = new CurationDTO();
                        curationDTO.SourceHeader = header;
                        curationDTO.DataFileId = fileId;
                        curationDTO.IsMapped = true;
                        curationDTO.MappedVariable = suggestion.Definition;
                        curationDTO.MappedVariableSynonym = suggestion.Synonyms;
                        //  IsIdentifier = suggestion.IsIdentifier,
                        //  curationDTO.IsTopic = true,
                        //  IsQualifier = suggestion.IsQualifier,
                        //  IsUnitRequired = suggestion.IsUnitRequired,
                        //  IsUnit = suggestion.IsUnit
                        listOfDto.Add(curationDTO);
                    }
                }
                else
                {
                    listOfDto = new List<CurationDTO>();
                    var curationDTO = new CurationDTO()
                    {
                        IsMapped = false
                    };
                    listOfDto.Add(curationDTO);
                }
                listOfListDto.Add(listOfDto);
            }
            return listOfListDto;
        }


        //////public void SearchMDR(string phrase)
        //////{
        //////    using (var client = new HttpClient())
        //////    {
        //////        client.BaseAddress = new Uri("http://localhost:2342");
        //////        client.DefaultRequestHeaders.Accept.Clear();
        //////        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //////        var response = client.GetAsync("api/ElasticSearch/search").Result;
        //////        if (response.IsSuccessStatusCode)
        //////        {
        //////            string responseString = response.Content.ReadAsStringAsync().Result;
        //////           // var modelObject = response.Content.ReadAsAsync<Student>().Result;
        //////        }
        //////    }
        //////}
    }
}
