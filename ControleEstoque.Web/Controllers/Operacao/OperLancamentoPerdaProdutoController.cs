using System.Web.Mvc;

namespace ControleEstoque.Web.Controllers
{
    [Authorize(Roles = "Gerente,Administrativo")]
    public class OperLancamentoPerdaProdutoController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}