using Microsoft.AspNetCore.Mvc;

namespace PlatformTM.API.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = " eHS API Home Page";

            return View();
        }
    }
}
