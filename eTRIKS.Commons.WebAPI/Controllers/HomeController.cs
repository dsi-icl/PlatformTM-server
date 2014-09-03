using System.Web.Mvc;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = " eTRIKS API Home Page";

            return View();
        }
    }
}
