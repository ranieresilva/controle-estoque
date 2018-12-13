using ControleEstoque.Web.Models;
using Rotativa;
using System.Web.Mvc;

namespace ControleEstoque.Web.Controllers
{
    [Authorize(Roles = "Gerente,Administrativo,Operador")]
    public class RelatRessuprimentoController : Controller
    {
        public ActionResult Filtro()
        {
            if (Request.HttpMethod == "GET")
            {
                return View("~/Views/Relatorio/FiltroRelatRessuprimentoView.cshtml");
            }
            else
            {
                var minimo = 0;
                int.TryParse(Request.Form.Get("minimo"), out minimo);
                if (minimo <= 0)
                {
                    ViewBag.Mensagem = "Informe a quantidade mínima de cada produto.";
                    return View("~/Views/Relatorio/FiltroRelatRessuprimentoView.cshtml");
                }

                return RedirectToAction("Index", new { minimo });
            }
        }

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