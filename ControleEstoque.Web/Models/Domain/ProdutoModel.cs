using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ControleEstoque.Web.Models
{
    public class ProdutoModel
    {
        #region Atributos

        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public decimal PrecoCusto { get; set; }
        public decimal PrecoVenda { get; set; }
        public int QuantEstoque { get; set; }
        public int IdUnidadeMedida { get; set; }
        public virtual UnidadeMedidaModel UnidadeMedida { get; set; }
        public int IdGrupo { get; set; }
        public virtual GrupoProdutoModel Grupo { get; set; }
        public int IdMarca { get; set; }
        public virtual MarcaProdutoModel Marca { get; set; }
        public int IdFornecedor { get; set; }
        public virtual FornecedorModel Fornecedor { get; set; }
        public int IdLocalArmazenamento { get; set; }
        public virtual LocalArmazenamentoModel LocalArmazenamento { get; set; }
        public bool Ativo { get; set; }
        public string Imagem { get; set; }

        #endregion

        #region Métodos

        public static int RecuperarQuantidade()
        {
            var ret = 0;

            using (var db = new ContextoBD())
            {
                ret = db.Produtos.Count();
            }

            return ret;
        }

        public static List<ProdutoModel> RecuperarLista(int pagina = 0, int tamPagina = 0, string filtro = "", string ordem = "", bool somenteAtivos = false)
        {
            var ret = new List<ProdutoModel>();

            using (var db = new ContextoBD())
            {
                var filtroWhere = "";
                if (!string.IsNullOrEmpty(filtro))
                {
                    filtroWhere = string.Format("where (lower(nome) like '%{0}%') ", filtro.ToLower());
                }

                if (somenteAtivos)
                {
                    filtroWhere = (string.IsNullOrEmpty(filtroWhere) ? "where" : "and ") + "(ativo = 1) ";
                }

                var pos = (pagina - 1) * tamPagina;
                var paginacao = "";
                if (pagina > 0 && tamPagina > 0)
                {
                    paginacao = string.Format(" offset {0} rows fetch next {1} rows only",
                        pos > 0 ? pos - 1 : 0, tamPagina);
                }

                var sql =
                    "select id, codigo, nome, ativo, imagem, preco_custo as PrecoCusto, preco_venda as PrecoVenda, " +
                    "quant_estoque as QuantEstoque, id_unidade_medida as IdUnidadeMedida, id_grupo as IdGrupo, " +
                    "id_marca as IdMarca, id_fornecedor as IdFornecedor, id_local_armazenamento as IdLocalArmazenamento " +
                    "from produto " +
                    filtroWhere +
                    "order by " + (!string.IsNullOrEmpty(ordem) ? ordem : "nome") +
                    paginacao;

                ret = db.Database.Connection.Query<ProdutoModel>(sql).ToList();
            }

            return ret;
        }

        public static ProdutoModel RecuperarPeloId(int id)
        {
            ProdutoModel ret = null;

            using (var db = new ContextoBD())
            {
                ret = db.Produtos.Find(id);
            }

            return ret;
        }

        public static string RecuperarImagemPeloId(int id)
        {
            string ret = "";

            using (var db = new ContextoBD())
            {
                ret = db.Produtos
                    .Where(x => x.Id == id)
                    .Select(x => x.Imagem)
                    .SingleOrDefault();
            }

            return ret;
        }

        public static bool ExcluirPeloId(int id)
        {
            var ret = false;

            if (RecuperarPeloId(id) != null)
            {
                using (var db = new ContextoBD())
                {
                    var produto = new ProdutoModel { Id = id };
                    db.Produtos.Attach(produto);
                    db.Entry(produto).State = EntityState.Deleted;
                    db.SaveChanges();
                    ret = true;
                }
            }

            return ret;
        }

        public int Salvar()
        {
            var ret = 0;

            var model = RecuperarPeloId(this.Id);

            using (var db = new ContextoBD())
            {
                if (model == null)
                {
                    db.Produtos.Add(this);
                }
                else
                {
                    db.Produtos.Attach(this);
                    db.Entry(this).State = EntityState.Modified;
                }

                db.SaveChanges();
                ret = this.Id;
            }

            return ret;
        }

        public static string SalvarPedidoEntrada(DateTime data, Dictionary<int, int> produtos)
        {
            return SalvarPedido(data, produtos, "entrada_produto", true);
        }

        public static string SalvarPedidoSaida(DateTime data, Dictionary<int, int> produtos)
        {
            return SalvarPedido(data, produtos, "saida_produto", false);
        }

        public static string SalvarPedido(DateTime data, Dictionary<int, int> produtos, string nomeTabela, bool entrada)
        {
            var ret = "";

            try
            {
                // TODO: salvar pedido
                //using (var conexao = new SqlConnection())
                //{
                //    conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                //    conexao.Open();

                //    var numPedido = conexao.ExecuteScalar<int>($"select next value for sec_{nomeTabela}").ToString("D10");

                //    using (var transacao = conexao.BeginTransaction())
                //    {
                //        foreach (var produto in produtos)
                //        {
                //            var sql = $"insert into {nomeTabela} (numero, data, id_produto, quant) values (@numero, @data, @id_produto, @quant)";
                //            var parametrosInsert = new { numero = numPedido, data, id_produto = produto.Key, quant = produto.Value };
                //            conexao.Execute(sql, parametrosInsert, transacao);

                //            var sinal = (entrada ? "+" : "-");
                //            sql = $"update produto set quant_estoque = quant_estoque {sinal} @quant_estoque where (id = @id)";
                //            var parametrosUpdate = new { id = produto.Key, quant_estoque = produto.Value };
                //            conexao.Execute(sql, parametrosUpdate, transacao);
                //        }

                //        transacao.Commit();

                //        ret = numPedido;
                //    }
                //}
            }
            catch (Exception ex)
            {
            }

            return ret;
        }

        public static List<ProdutoInventarioViewModel> RecuperarListaParaInventario()
        {
            var ret = new List<ProdutoInventarioViewModel>();

            using (var db = new ContextoBD())
            {
                var sql =
                    "select " +
                    "p.id, p.codigo, p.nome as NomeProduto, p.quant_estoque as QuantEstoque, " +
                    "l.nome as NomeLocalArmazenamento, u.nome as NomeUnidadeMedida " +
                    "from produto p, local_armazenamento l, unidade_medida u " +
                    "where (p.ativo = 1) and " +
                    "(p.id_local_armazenamento = l.id) and " +
                    "(p.id_unidade_medida = u.id) " +
                    "order by l.nome, p.nome";
                ret = db.Database.Connection.Query<ProdutoInventarioViewModel>(sql).ToList();
            }

            return ret;
        }

        public static bool SalvarInventario(List<ItemInventarioViewModel> dados)
        {
            var ret = true;

            try
            {
                var data = DateTime.Now;

                using (var db = new ContextoBD())
                {
                    // TODO: salvar vários itens de inventário
                    //using (var transacao = conexao.BeginTransaction())
                    //{
                    //    foreach (var produtoInventario in dados)
                    //    {
                    //        var sql = "insert into inventario_estoque (data, id_produto, quant_estoque, quant_inventario, motivo) values (@data, @id_produto, @quant_estoque, @quant_inventario, @motivo)";
                    //        var parametrosInsert = new
                    //        {
                    //            data,
                    //            id_produto = produtoInventario.IdProduto,
                    //            quant_estoque = produtoInventario.QuantidadeEstoque,
                    //            quant_inventario = produtoInventario.QuantidadeInventario,
                    //            motivo = produtoInventario.Motivo ?? ""
                    //        };
                    //        conexao.Execute(sql, parametrosInsert, transacao);
                    //    }
                    //    transacao.Commit();
                    //}
                }
            }
            catch (Exception ex)
            {
                ret = false;
            }

            return ret;
        }

        #endregion
    }
}