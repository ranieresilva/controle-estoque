using System.Web.Mvc;

namespace ControleEstoque.Web.Controllers
{
    public class RelatorioController : Controller
    {
        [Authorize]
        public ActionResult Ressuprimento()
        {
            return View();
        }
    }
}