using Dapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace ControleEstoque.Web.Models
{
    public class PaisModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Preencha o nome.")]
        [MaxLength(30, ErrorMessage = "O nome pode ter no máximo 30 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Preencha o código internacional.")]
        [MaxLength(3, ErrorMessage = "O código internacional deve ter 3 caracteres.")]
        public string Codigo { get; set; }

        public bool Ativo { get; set; }

        public static int RecuperarQuantidade()
        {
            var ret = 0;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                ret = conexao.ExecuteScalar<int>("select count(*) from pais");
            }

            return ret;
        }

        public static List<PaisModel> RecuperarLista(int pagina = 0, int tamPagina = 0, string filtro = "", string ordem = "")
        {
            var ret = new List<PaisModel>();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                var filtroWhere = "";
                if (!string.IsNullOrEmpty(filtro))
                {
                    filtroWhere = string.Format(" where lower(nome) like '%{0}%'", filtro.ToLower());
                }

                var pos = (pagina - 1) * tamPagina;
                var paginacao = "";
                if (pagina > 0 && tamPagina > 0)
                {
                    paginacao = string.Format(" offset {0} rows fetch next {1} rows only",
                        pos > 0 ? pos - 1 : 0, tamPagina);
                }

                var sql =
                    "select *" +
                    " from pais" +
                    filtroWhere +
                    " order by " + (!string.IsNullOrEmpty(ordem) ? ordem : "nome") +
                    paginacao;

                ret = conexao.Query<PaisModel>(sql).ToList();
            }

            return ret;
        }

        public static PaisModel RecuperarPeloId(int id)
        {
            PaisModel ret = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                var sql = "select * from pais where (id = @id)";
                var parametros = new { id };
                ret = conexao.Query<PaisModel>(sql, parametros).SingleOrDefault();
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

                    var sql = "delete from pais where (id = @id)";
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
                    var sql = "insert into pais (nome, codigo, ativo) values (@nome, @codigo, @ativo); select convert(int, scope_identity())";
                    var parametros = new { nome = this.Nome, codigo = this.Codigo, ativo = (this.Ativo ? 1 : 0) };
                    ret = conexao.ExecuteScalar<int>(sql, parametros);
                }
                else
                {
                    var sql = "update pais set nome=@nome, codigo=@codigo, ativo=@ativo where id = @id";
                    var parametros = new { id = this.Id, nome = this.Nome, codigo = this.Codigo, ativo = (this.Ativo ? 1 : 0) };
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