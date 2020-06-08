using System.Web.Mvc;

namespace ControleEstoque.Web.Controllers
{
    public class RelatorioController : BaseController
    {
        [Authorize]
        public ActionResult Ressuprimento()
        {
            return View();
        }
    }
}