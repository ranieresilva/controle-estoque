using ControleEstoque.Web.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ControleEstoque.Web.Controllers
{
    [Authorize(Roles = "Gerente,Administrativo,Operador")]
    public class OperInventarioController : Controller
    {
        public ActionResult Index()
        {
            var model = ProdutoModel.RecuperarListaParaInventario();
            return View(model);
        }

        [HttpPost]
        public JsonResult Salvar(List<ItemInventarioViewModel> dados)
        {
            var ok = ProdutoModel.SalvarInventario(dados);
            return Json(new { OK = ok });
        }
    }
}