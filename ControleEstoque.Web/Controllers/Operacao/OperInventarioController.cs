using ControleEstoque.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControleEstoque.Web.Controllers
{
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