using ControleEstoque.Web.Models;
using System.Web.Mvc;

namespace ControleEstoque.Web.Controllers
{
    public class OperEntradaProdutoController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Produtos = ProdutoModel.RecuperarLista(somenteAtivos: true);

            return View();
        }
    }
}