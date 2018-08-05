using Dapper;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace ControleEstoque.Web.Models
{
    public class MarcaProdutoModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }

        public static int RecuperarQuantidade()
        {
            var ret = 0;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                ret = conexao.ExecuteScalar<int>("select count(*) from marca_produto");
            }

            return ret;
        }

        public static List<MarcaProdutoModel> RecuperarLista(int pagina, int tamPagina, string ordem = "")
        {
            var ret = new List<MarcaProdutoModel>();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                var pos = (pagina - 1) * tamPagina;

                var sql = string.Format(
                    "select *" +
                    " from marca_produto" +
                    " order by " + (!string.IsNullOrEmpty(ordem) ? ordem : "nome") +
                    " offset {0} rows fetch next {1} rows only",
                    pos > 0 ? pos - 1 : 0, tamPagina);

                ret = conexao.Query<MarcaProdutoModel>(sql).ToList();
            }

            return ret;
        }

        public static MarcaProdutoModel RecuperarPeloId(int id)
        {
            MarcaProdutoModel ret = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                var sql = "select * from marca_produto where (id = @id)";
                var parametros = new { id };
                ret = conexao.Query<MarcaProdutoModel>(sql, parametros).SingleOrDefault();
            }

            return ret;
        }

        public static bool ExcluirPeloId(int id)
        {
            var ret = false;

            if (RecuperarPeloId(id) != null)
            {
                using (var conexao = new SqlConnection())
                {
                    conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                    conexao.Open();

                    var sql = "delete from marca_produto where (id = @id)";
                    var parametros = new { id };
                    ret = (conexao.Execute(sql, parametros) > 0);
                }
            }

            return ret;
        }

        public int Salvar()
        {
            var ret = 0;

            var model = RecuperarPeloId(this.Id);

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                if (model == null)
                {
                    var sql = "insert into marca_produto (nome, ativo) values (@nome, @ativo); select convert(int, scope_identity())";
                    var parametros = new { nome = this.Nome, ativo = (this.Ativo ? 1 : 0) };
                    ret = conexao.ExecuteScalar<int>(sql, parametros);
                }
                else
                {
                    var sql = "update marca_produto set nome=@nome, ativo=@ativo where id = @id";
                    var parametros = new { id = this.Id, nome = this.Nome, ativo = (this.Ativo ? 1 : 0) };
                    if (conexao.Execute(sql, parametros) > 0)
                    {
                        ret = this.Id;
                    }
                }
            }

            return ret;
        }
    }
}