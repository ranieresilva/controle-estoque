using ControleEstoque.Web.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ControleEstoque.Web.Controllers
{
    [Authorize(Roles = "Gerente,Administrativo")]
    public class OperLancamentoPerdaProdutoController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Inventarios = ProdutoModel.RecuperarListaInventarioComDiferenca();

            return View();
        }

        [HttpGet]
        public JsonResult RecuperarListaProdutoComDiferencaEmInventario(string inventario)
        {
            var ret = ProdutoModel.RecuperarListaProdutoComDiferencaEmInventario(inventario);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Salvar(List<LancamentoPerdaViewModel> dados)
        {
            var ret = ProdutoModel.SalvarLancamentoPerda(dados);
            return Json(ret);
        }
    }
}