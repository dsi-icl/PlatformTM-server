using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlatformTM.Services.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PlatformTM.API.DevControllers
{
    [Route("dbinit")]
    public class DBinitController : Controller
    {
        private readonly DbInitializer _dbInitializer;

        public DBinitController(DbInitializer dbInitializer)
        {
            _dbInitializer = dbInitializer;
        }

        [HttpGet("dumptemplates")]
        [AllowAnonymous]
        public void GetTemplates()
        {
            _dbInitializer.DumpTemplatesJSON();
        }

        [HttpGet("dumpcvterms")]
        [AllowAnonymous]
        public void GetCVterms()
        {
            _dbInitializer.DumpCVterms();
        }

        [HttpGet("loadcvterms")]
        [AllowAnonymous]
        public void LoadCVterms()
        {
            _dbInitializer.LoadDictionaries();
        }

        [HttpGet("loadtemplates")]
        [AllowAnonymous]
        public void LoadTemplates()
        {
            _dbInitializer.LoadTemplates();
        }

    }
}
