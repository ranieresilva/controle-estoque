using System.Web.Mvc;

namespace ControleEstoque.Web.Controllers
{
    public class OperacaoController : Controller
    {
        [Authorize]
        public ActionResult SaidaEstoque()
        {
            return View();
        }

        [Authorize]
        public ActionResult LancPerdaProduto()
        {
            return View();
        }

        [Authorize]
        public ActionResult Inventario()
        {
            return View();
        }
    }
}