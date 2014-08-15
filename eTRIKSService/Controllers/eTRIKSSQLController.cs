using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using eTRIKSService.Models;
using eTRIKSService.DataAccess;


namespace eTRIKSService.Controllers
{
    [RoutePrefix("api/etriksSQL")]
    public class eTRIKSSQLController : ApiController
    {
        [Route("")]
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [Route("{Activity}")]
        // GET api/values
        public Activity GetActivity(string activity)
        {
            Activity act = new Activity();
            act.name = "Test Activity";
            act.OID = "A100002";

            Activity_TAB act2 = new Activity_TAB();
            act2.OID = "A00001";
            act2.name = "Test Activity";
            try
            {
                eTRIKS_schemaEntitiesNew en = new eTRIKS_schemaEntitiesNew();
                en.Activity_TAB.Add(act2);
                en.SaveChanges();
            }
            catch (Exception ee)
            { }

            return act;
        }

        public void getActivityDataset()
        {
           
        }

        public void getDataSetVariables()
        { }
    }
}
