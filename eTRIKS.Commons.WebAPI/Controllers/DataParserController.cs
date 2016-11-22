using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using eTRIKS.Commons.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    public class DataParserController : Controller
    {

        //private FileHandler _fileHandler;
        private TemplateService templateService;
        private FileService _fileService;

        public DataParserController(TemplateService tmplService, FileService fileService)
        {
            //_fileHandler = fileHandler;
            templateService = tmplService;
            _fileService = fileService;
        }

        [HttpGet]
        [Route("api/temp/loadDatamatrixtemplate")]
        public void loadmatrixTemp()
        {
            templateService.loadDataMatrixTemplate();
        }

        [HttpGet]
        [Route("api/temp/loadtemp")]
        public void LoadTemplateFromFile()
        {
            //string fileDir = ConfigurationManager.AppSettings["FileDirectory"];
            //string tempFilepath = "temp/dataTemplates/REtemplate.csv";
            //string tempVarFilepath = "temp/dataTemplates/REtemplateVars.csv";
            //templateService.loadDatasetTemplate(tempFilepath, tempVarFilepath);
        }

        [HttpGet]
        [Route("api/temp/matchids")]
        public void matchIds()
        {
           // string fileDir = ConfigurationManager.AppSettings["FileDirectory"];
            //string tempFilepath = "temp/BStemplate.csv";
            //string tempVarFilepath = "temp/BStemplateVars.csv";
           // _fileService.tempmethod();
        }

        [HttpGet]
        [Route("api/temp/loadTerms")]
        public void LoadTerms()
        {
            //string fileDir = ConfigurationManager.AppSettings["FileDirectory"];
            //string tempFilepath = "temp/assaytermsDict.csv";
            //string tempVarFilepath = "temp/assaytermsCVs.csv";
            //templateService.loadCVterms(tempFilepath, tempVarFilepath);
        }

        [HttpGet]
        [Route("api/temp/widetolong")]
        public void widetolong()
        {
            //string fileDir = ConfigurationManager.AppSettings["FileDirectory"];
            //string tempFilepath = "temp/BStemplate.csv";
            //string tempVarFilepath = "temp/BStemplateVars.csv";
           // _fileService.getLongFormat2();
        }


       
    }
}
