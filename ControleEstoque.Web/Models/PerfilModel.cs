using Dapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace ControleEstoque.Web.Models
{
    public class PerfilModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Preencha o nome.")]
        public string Nome { get; set; }

        public bool Ativo { get; set; }

        public List<UsuarioModel> Usuarios { get; set; }

        public PerfilModel()
        {
            this.Usuarios = new List<UsuarioModel>();
        }

        public static int RecuperarQuantidade()
        {
            var ret = 0;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                ret = conexao.ExecuteScalar<int>("select count(*) from perfil");
            }

            return ret;
        }

        public static List<PerfilModel> RecuperarLista(int pagina, int tamPagina, string ordem = "")
        {
            var ret = new List<PerfilModel>();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                var pos = (pagina - 1) * tamPagina;

                var sql = string.Format(
                    "select *" +
                    " from perfil" +
                    " order by " + (!string.IsNullOrEmpty(ordem) ? ordem : "nome") +
                    " offset {0} rows fetch next {1} rows only",
                    pos > 0 ? pos - 1 : 0, tamPagina);

                ret = conexao.Query<PerfilModel>(sql).ToList();
            }

            return ret;
        }

        public void CarregarUsuarios()
        {
            this.Usuarios.Clear();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                var sql =
                    "select u.* " +
                    "from perfil_usuario pu, usuario u " +
                    "where (pu.id_perfil = @id_perfil) and (pu.id_usuario = u.id)";
                var parametros = new { id_perfil = this.Id };
                this.Usuarios = conexao.Query<UsuarioModel>(sql, parametros).ToList();
            }
        }

        public static List<PerfilModel> RecuperarListaAtivos()
        {
            var ret = new List<PerfilModel>();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                var sql = "select * from perfil where ativo=1 order by nome";
                ret = conexao.Query<PerfilModel>(sql).ToList();
            }

            return ret;
        }

        public static PerfilModel RecuperarPeloId(int id)
        {
            PerfilModel ret = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                var sql = "select * from perfil where (id = @id)";
                var parametros = new { id };
                ret = conexao.Query<PerfilModel>(sql, parametros).SingleOrDefault();
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

                    var sql = "delete from perfil where (id = @id)";
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

                using (var transacao = conexao.BeginTransaction())
                {
                    if (model == null)
                    {
                        var sql = "insert into perfil (nome, ativo) values (@nome, @ativo); select convert(int, scope_identity())";
                        var parametros = new { nome = this.Nome, ativo = (this.Ativo ? 1 : 0) };
                        ret = conexao.ExecuteScalar<int>(sql, parametros, transacao);
                        this.Id = ret;
                    }
                    else
                    {
                        var sql = "update perfil set nome=@nome, ativo=@ativo where id = @id";
                        var parametros = new { id = this.Id, nome = this.Nome, ativo = (this.Ativo ? 1 : 0) };
                        if (conexao.Execute(sql, parametros, transacao) > 0)
                        {
                            ret = this.Id;
                        }
                    }

                    if (this.Usuarios != null && this.Usuarios.Count > 0)
                    {
                        var sql = "delete from perfil_usuario where (id_perfil = @id_perfil)";
                        var parametros = new { id_perfil = this.Id };
                        conexao.Execute(sql, parametros, transacao);

                        if (this.Usuarios[0].Id != -1)
                        {
                            foreach (var usuario in this.Usuarios)
                            {
                                sql = "insert into perfil_usuario (id_perfil, id_usuario) values (@id_perfil, @id_usuario)";
                                var parametrosUsuario = new { id_perfil = this.Id, id_usuario = usuario.Id };
                                conexao.Execute(sql, parametrosUsuario, transacao);
                            }
                        }
                    }

                    transacao.Commit();
                }
            }

            return ret;
        }
    }
}