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

        public JsonResult Salvar([ModelBinder(typeof(EntradaProdutoViewModelModelBinder))]EntradaProdutoViewModel dados)
        {
            var numPedido = ProdutoModel.SalvarPedidoEntrada(dados.Data, dados.Produtos);
            var ok = (numPedido != "");

            return Json(new { OK = ok, Numero = numPedido });
        }
    }
}