using System.Web.Mvc;

namespace ControleEstoque.Web.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Sobre()
        {
            return View();
        }
    }
}