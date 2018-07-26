using Dapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace ControleEstoque.Web.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Informe o senha")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Informe o nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Informe o e-mail")]
        public string Email { get; set; }

        public virtual List<PerfilModel> Perfis { get; set; }

        public static UsuarioModel ValidarUsuario(string login, string senha)
        {
            UsuarioModel ret = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                var sql = "select * from usuario where login=@login and senha=@senha";
                var parametros = new { login, senha = CriptoHelper.HashMD5(senha) };
                ret = conexao.Query<UsuarioModel>(sql, parametros).SingleOrDefault();
            }

            return ret;
        }

        public static int RecuperarQuantidade()
        {
            var ret = 0;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                ret = conexao.ExecuteScalar<int>("select count(*) from usuario");
            }

            return ret;
        }

        public static List<UsuarioModel> RecuperarLista(int pagina = -1, int tamPagina = -1, string ordem = "")
        {
            var ret = new List<UsuarioModel>();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                string sql;
                if (pagina == -1 || tamPagina == -1)
                {
                    sql =
                        "select *" +
                        "from usuario" +
                        " order by " + (!string.IsNullOrEmpty(ordem) ? ordem : "nome");
                }
                else
                {
                    var pos = (pagina - 1) * tamPagina;
                    sql = string.Format(
                        "select *" +
                        " from usuario" +
                        " order by " + (!string.IsNullOrEmpty(ordem) ? ordem : "nome") +
                        " offset {0} rows fetch next {1} rows only",
                        pos > 0 ? pos - 1 : 0, tamPagina);
                }

                ret = conexao.Query<UsuarioModel>(sql).ToList();
            }

            return ret;
        }

        public static UsuarioModel RecuperarPeloId(int id)
        {
            UsuarioModel ret = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                var sql = "select * from usuario where (id = @id)";
                var parametros = new { id };
                ret = conexao.Query<UsuarioModel>(sql, parametros).SingleOrDefault();
            }

            return ret;
        }

        public static UsuarioModel RecuperarPeloLogin(string login)
        {
            UsuarioModel ret = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                var sql = "select * from usuario where (login = @login)";
                var parametros = new { login };
                ret = conexao.Query<UsuarioModel>(sql, parametros).SingleOrDefault();
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

                    var sql = "delete from usuario where (id = @id)";
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
                    var sql = "insert into usuario (nome, email, login, senha) values (@nome, @email, @login, @senha); select convert(int, scope_identity())";
                    var parametros = new { nome = this.Nome, email = this.Email, login = this.Login, senha = CriptoHelper.HashMD5(this.Senha) };
                    ret = conexao.ExecuteScalar<int>(sql, parametros);
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.Senha))
                    {
                        var sql = "update usuario set nome=@nome, email=@email, login=@login, senha=@senha where id = @id";
                        var parametros = new { id = this.Id, nome = this.Nome, email = this.Email, login = this.Login, senha = CriptoHelper.HashMD5(this.Senha) };
                        if (conexao.Execute(sql, parametros) > 0)
                        {
                            ret = this.Id;
                        }
                    }
                    else
                    {
                        var sql = "update usuario set nome=@nome, email=@email, login=@login where id = @id";
                        var parametros = new { id = this.Id, nome = this.Nome, email = this.Email, login = this.Login };
                        if (conexao.Execute(sql, parametros) > 0)
                        {
                            ret = this.Id;
                        }
                    }
                }
            }

            return ret;
        }

        public string RecuperarStringNomePerfis()
        {
            var ret = string.Empty;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                var sql =
                        "select p.nome " +
                        "from perfil_usuario pu, perfil p " +
                        "where (pu.id_usuario = @id_usuario) and (pu.id_perfil = p.id) and (p.ativo = 1)";
                var parametros = new { id_usuario = this.Id };
                var matriculas = conexao.Query<string>(sql, parametros).ToList();
                if (matriculas.Count > 0)
                {
                    ret = string.Join(";", matriculas);
                }
            }

            return ret;
        }

        public bool ValidarSenhaAtual(string senhaAtual)
        {
            var ret = false;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                var sql = "select count(*) from usuario where senha = @senha_atual and id = @id";
                var parametros = new { id = this.Id, senha_atual = CriptoHelper.HashMD5(senhaAtual) };
                ret = (conexao.ExecuteScalar<int>(sql, parametros) > 0);
            }

            return ret;
        }

        public bool AlterarSenha(string novaSenha)
        {
            var ret = false;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                var sql = "update usuario set senha = @senha where id = @id";
                var parametros = new { id = this.Id, senha = CriptoHelper.HashMD5(novaSenha) };
                ret = (conexao.Execute(sql, parametros) > 0);
            }

            return ret;
        }
    }
}