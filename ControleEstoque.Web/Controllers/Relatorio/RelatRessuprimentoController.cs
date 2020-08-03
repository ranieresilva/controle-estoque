using ControleEstoque.Web.Models;
using Rotativa;
using System.Web.Mvc;

namespace ControleEstoque.Web.Controllers
{
    [Authorize(Roles = "Gerente,Administrativo,Operador")]
    public class RelatRessuprimentoController : BaseController
    {
        [HttpGet]
        public ActionResult Filtro()
        {
            return View("~/Views/Relatorio/FiltroRelatRessuprimentoView.cshtml");
        }

        [HttpPost]
        public ActionResult ValidarFiltro(int? minimo)
        {
            var ok = true;
            var mensagem = "";
            if ((minimo ?? 0) <= 0)
            {
                ok = false;
                mensagem = "Informe a quantidade mínima de cada produto.";
            }

            return Json(new { ok, mensagem });
        }

        [HttpGet]
        public ActionResult Index(int minimo)
        {
            if (minimo == 0)
            {
                return RedirectToAction("Filtro");
            }

            var estoque = ProdutoModel.RecuperarRelatRessuprimento(minimo);

            return new ViewAsPdf("~/Views/Relatorio/RelatRessuprimentoView.cshtml", estoque);
        }
    }
}