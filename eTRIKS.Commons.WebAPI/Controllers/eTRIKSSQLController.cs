using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Data.Entity;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    //[RoutePrefix("api/etriksSQL")]

    public class eTRIKSSQLController : ApiController
    {
       // [Route("")]
        //// GET api/values
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

       // ISATemplate_schemaEntities isat = new ISATemplate_schemaEntities();

      //  [Route("")]

        /*
        public IQueryable<DomainDataset> getDomainDatasets()
        {


            var domainDatasets = from dds in isat.Domain_Dataset_Template_TAB
                                 select new DomainDataset()
                        {
                            OID = dds.OID,
                            description = dds.description,
                            domainDatasetName = dds.domainName,
                            structure= dds.structure,
                            repeating = dds.repeating             
                        };

            return domainDatasets;

           // var book = isat.Domain_Dataset_Template_TAB.Include(b => b.).Select() 

        }
         */
        //[Route("{Domain}")]
        /*
        public DomainDataset getDomainDatasetVariables(string domain)
        {
            domain = "sdtmig-3-1-3:Dataset.CE";
           
            var domainDatasetVariables = from dds in isat.Domain_Dataset_Template_TAB.Include(dsddd => dsddd.Domain_Variable_Template_TAB)
                                         where dds.OID == domain
                                         select dds;
            List<Domain_Dataset_Template_TAB> domainDatasetVariableList = domainDatasetVariables.ToList();

            //var domainDatasetvariables = from dds in isat.Domain_Dataset_Template_TAB.Include(dsv => dsv.Domain_Variable_Template_TAB)
            //                             where dds.OID == domain
            //                             select new eTRIKSService.DTO.DomainDataset()
            //                             {
            //                                 OID = dds.OID,
            //                                 description = dds.description,
            //                                 structure = dds.structure,
            //                                 repeating = dds.repeating,
            //                                 variableList = dds.Domain_Variable_Template_TAB
            //                             };

            DomainDataset domainDataset = new DomainDataset();

            domainDataset.OID = domainDatasetVariableList[0].OID;
            domainDataset.description = domainDatasetVariableList[0].description;
            domainDataset.structure = domainDatasetVariableList[0].structure;
            domainDataset.repeating = domainDatasetVariableList[0].repeating;

            foreach (var item in domainDatasetVariableList[0].Domain_Variable_Template_TAB)
            {
                DomainVariable dv = new DomainVariable();

                dv.OID = item.OID;
                dv.name = item.name;
                dv.description = item.description;
               // dv.datasetId = item.datasetId;
                dv.dataType = item.dataType;
                dv.role = item.role;
                dv.variableType = item.variableType;

                domainDataset.variableList.Add(dv);

            }


            return domainDataset;

        }

         */
        /*
        [Route("{Activity}")]
        // GET api/values
        public Activity GetActivity(string activity)
        {
            Activity act = new Activity();
            act.name = "Test Activity";
            act.OID = "A100002";

            ////Temp Test Code
            //// Insert a record
            //Activity_TAB act2 = new Activity_TAB();
            //act2.OID = "A00001";
            //act2.name = "Test Activity";
            //act2.studyId = "S1";
            //try
            //{
            //    eTRIKSDataModel_Entities en = new eTRIKSDataModel_Entities();
            //    en.Activity_TAB.Add(act2);
            //    en.SaveChanges();
            //}
            //catch (Exception ee)
            //{ }
            //// Insert record END

            //// Update record
            //eTRIKSDataModel_Entities ent = new eTRIKSDataModel_Entities();
            //var actQuery = from activity_ in ent.Activity_TAB
            //               where activity_.OID == activity
            //               select activity_;
            //Activity_TAB actUpdate = actQuery.Single();
            //actUpdate.name = "Activity Updated";
            //ent.SaveChanges();
            ////Update record END

            ////Delete record
            //eTRIKSDataModel_Entities entDel = new eTRIKSDataModel_Entities();
            //Activity_TAB actDel = new Activity_TAB() { OID = activity };
            //entDel.Activity_TAB.Attach(actDel);
            //entDel.Activity_TAB.Remove(actDel);
            //entDel.SaveChanges();
            ////Delete record END

            ////Search record
            eTRIKSDataModel_Entities ent = new eTRIKSDataModel_Entities();
            var actQuery = from activity_ in ent.Activity_TAB
                           where activity_.OID == activity
                           select activity_;
            List<Activity_TAB> empList = actQuery.ToList();

            //Test Commit

            return act;
        }

        */

        



        public void getActivityDataset()
        {
           
        }

        public void getDataSetVariables()
        { }
    }
}
