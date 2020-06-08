using System.Web.Mvc;

namespace ControleEstoque.Web.Controllers
{
    public class GraficoController : BaseController
    {
        [Authorize]
        public ActionResult PerdaMes()
        {
            return View();
        }

        [Authorize]
        public ActionResult EntradaSaidaMesa()
        {
            return View();
        }
    }
}