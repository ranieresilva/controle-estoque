using ControleEstoque.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControleEstoque.Web.Controllers
{
    [Authorize(Roles = "Gerente,Administrativo,Operador")]
    public class CadProdutoController : BaseController
    {
        private const int _quantMaxLinhasPorPagina = 5;

        public ActionResult Index()
        {
            ViewBag.ListaTamPag = new SelectList(new int[] { _quantMaxLinhasPorPagina, 10, 15, 20 }, _quantMaxLinhasPorPagina);
            ViewBag.QuantMaxLinhasPorPagina = _quantMaxLinhasPorPagina;
            ViewBag.PaginaAtual = 1;

            var lista = Mapper.Map<List<ProdutoViewModel>>(ProdutoModel.RecuperarLista(ViewBag.PaginaAtual, _quantMaxLinhasPorPagina));
            var quant = ProdutoModel.RecuperarQuantidade();

            var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
            ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;
            ViewBag.UnidadesMedida = Mapper.Map<List<UnidadeMedidaViewModel>>(UnidadeMedidaModel.RecuperarLista(1, 9999));
            ViewBag.Grupos = Mapper.Map<List<GrupoProdutoViewModel>>(GrupoProdutoModel.RecuperarLista(1, 9999));
            ViewBag.Marcas = Mapper.Map<List<MarcaProdutoViewModel>>(MarcaProdutoModel.RecuperarLista(1, 9999));
            ViewBag.Fornecedores = Mapper.Map<List<FornecedorViewModel>>(FornecedorModel.RecuperarLista());
            ViewBag.LocaisArmazenamento = Mapper.Map<List<LocalArmazenamentoViewModel>>(LocalArmazenamentoModel.RecuperarLista(1, 9999));

            return View(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ProdutoPagina(int pagina, int tamPag, string filtro, string ordem)
        {
            var lista = Mapper.Map<List<ProdutoViewModel>>(ProdutoModel.RecuperarLista(pagina, tamPag, filtro, ordem));

            return Json(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RecuperarProduto(int id)
        {
            var vm = Mapper.Map<ProdutoViewModel>(ProdutoModel.RecuperarPeloId(id));

            return Json(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RecuperarQuantidadeEstoqueProduto(int id)
        {
            var model = ProdutoModel.RecuperarPeloId(id);
            if (model != null)
            {
                return Json(new { OK = true, Result = model.QuantEstoque });
            }
            else
            {
                return Json(new { OK = false });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Gerente,Administrativo")]
        [ValidateAntiForgeryToken]
        public JsonResult ExcluirProduto(int id)
        {
            return Json(ProdutoModel.ExcluirPeloId(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SalvarProduto()
        {
            var resultado = "OK";
            var mensagens = new List<string>();
            var idSalvo = string.Empty;

            var nomeArquivoImagem = "";
            HttpPostedFileBase arquivo = null;
            if (Request.Files.Count > 0)
            {
                arquivo = Request.Files[0];
                nomeArquivoImagem = Guid.NewGuid().ToString() + ".jpg";
            }

            var vm = new ProdutoViewModel()
            {
                Id = Int32.Parse(Request.Form["Id"]),
                Codigo = Request.Form["Codigo"],
                Nome = Request.Form["Nome"],
                PrecoCusto = Request.Form["PrecoCusto"].ToDecimal(),
                PrecoVenda = Request.Form["PrecoVenda"].ToDecimal(),
                QuantEstoque = Request.Form["QuantEstoque"].ToInt32(),
                IdUnidadeMedida = Request.Form["IdUnidadeMedida"].ToInt32(),
                IdGrupo = Request.Form["IdGrupo"].ToInt32(),
                IdMarca = Request.Form["IdMarca"].ToInt32(),
                IdFornecedor = Request.Form["IdFornecedor"].ToInt32(),
                IdLocalArmazenamento = Request.Form["IdLocalArmazenamento"].ToInt32(),
                Ativo = (Request.Form["Ativo"] == "true"),
                Imagem = nomeArquivoImagem
            };

            var context = new ValidationContext(vm);
            var results = new List<ValidationResult>();
            var valido = Validator.TryValidateObject(vm, context, results);

            if (!valido)
            {
                resultado = "AVISO";
                mensagens = results.Select(x => x.ErrorMessage).ToList();
            }
            else
            {
                var model = new ProdutoModel()
                {
                    Id = vm.Id,
                    Codigo = vm.Codigo,
                    Nome = vm.Nome,
                    PrecoCusto = vm.PrecoCusto,
                    PrecoVenda = vm.PrecoVenda,
                    QuantEstoque = vm.QuantEstoque,
                    IdUnidadeMedida = vm.IdUnidadeMedida,
                    IdGrupo = vm.IdGrupo,
                    IdMarca = vm.IdMarca,
                    IdFornecedor = vm.IdFornecedor,
                    IdLocalArmazenamento = vm.IdLocalArmazenamento,
                    Ativo = vm.Ativo,
                    Imagem = vm.Imagem
                };

                try
                {
                    var nomeArquivoImagemAnterior = "";
                    if (model.Id > 0)
                    {
                        nomeArquivoImagemAnterior = ProdutoModel.RecuperarImagemPeloId(model.Id);
                    }

                    var id = model.Salvar();
                    if (id > 0)
                    {
                        idSalvo = id.ToString();
                        if (!string.IsNullOrEmpty(nomeArquivoImagem) && arquivo != null)
                        {
                            var diretorio = Server.MapPath("~/Content/Imagens");

                            var caminhoArquivo = Path.Combine(diretorio, nomeArquivoImagem);
                            arquivo.SaveAs(caminhoArquivo);

                            if (!string.IsNullOrEmpty(nomeArquivoImagemAnterior))
                            {
                                var caminhoArquivoAnterior = Path.Combine(diretorio, nomeArquivoImagemAnterior);
                                System.IO.File.Delete(caminhoArquivoAnterior);
                            }
                        }
                    }
                    else
                    {
                        resultado = "ERRO";
                    }
                }
                catch (Exception ex)
                {
                    resultado = "ERRO";
                }
            }

            return Json(new { Resultado = resultado, Mensagens = mensagens, IdSalvo = idSalvo });
        }
    }
}