using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using eTRIKS.Commons.Service.Services;
using eTRIKS.Commons.Service.Services.HelperService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [Route("temp")]
    public class DataParserController : Controller
    {

        //private FileHandler _fileHandler;
        //private TemplateService templateService;
        private Formatter _formatterService;

        public DataParserController(Formatter formatterService)
        {
            //_fileHandler = fileHandler;
            //templateService = tmplService;
            _formatterService = formatterService;
        }

        //[HttpGet]
        //[Route("api/temp/loadDatamatrixtemplate")]
        //public void loadmatrixTemp()
        //{
        //    templateService.loadDataMatrixTemplate();
        //}

        //[HttpGet]
        //[Route("api/temp/loadtemp")]
        //public void LoadTemplateFromFile()
        //{
        //    //string fileDir = ConfigurationManager.AppSettings["FileDirectory"];
        //    //string tempFilepath = "temp/dataTemplates/REtemplate.csv";
        //    //string tempVarFilepath = "temp/dataTemplates/REtemplateVars.csv";
        //    //templateService.loadDatasetTemplate(tempFilepath, tempVarFilepath);
        //}

        //[HttpGet]
        //[Route("api/temp/matchids")]
        //public void matchIds()
        //{
        //   // string fileDir = ConfigurationManager.AppSettings["FileDirectory"];
        //    //string tempFilepath = "temp/BStemplate.csv";
        //    //string tempVarFilepath = "temp/BStemplateVars.csv";
        //   // _fileService.tempmethod();
        //}

        //[HttpGet]
        //[Route("api/temp/loadTerms")]
        //public void LoadTerms()
        //{
        //    //string fileDir = ConfigurationManager.AppSettings["FileDirectory"];
        //    //string tempFilepath = "temp/assaytermsDict.csv";
        //    //string tempVarFilepath = "temp/assaytermsCVs.csv";
        //    //templateService.loadCVterms(tempFilepath, tempVarFilepath);
        //}

        [HttpGet]
        [AllowAnonymous]
        [Route("widetolong")]
        public void widetolong()
        {
            //string fileDir = ConfigurationManager.AppSettings["FileDirectory"];
            //string tempFilepath = "temp/BStemplate.csv";
            //string tempVarFilepath = "temp/BStemplateVars.csv";
            // _fileService.getLongFormat2();
            _formatterService.TransformToHD("P-95\\Luminex data\\CRC305ABC_cytokines_results.csv");
        }



    }
}
