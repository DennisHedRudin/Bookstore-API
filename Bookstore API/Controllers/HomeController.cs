using System.Web.Mvc;

namespace Bookstore_API.Controllers
{
    public class HomeController : Controller
    {
        // Redirect root to the Web API help page so browsing to '/' doesn't return 404
        public ActionResult Index()
        {
            return Redirect("~/help");
        }
    }
}
