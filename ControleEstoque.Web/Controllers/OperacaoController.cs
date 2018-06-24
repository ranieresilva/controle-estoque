using System.Web.Mvc;

namespace ControleEstoque.Web.Controllers
{
    public class OperacaoController : Controller
    {
        [Authorize]
        public ActionResult LancPerdaProduto()
        {
            return View();
        }
    }
}