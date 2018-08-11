using Dapper;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ControleEstoque.Web.Models
{
    public class UsuarioModel
    {
        #region Atributos

        public int Id { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public virtual List<PerfilModel> Perfis { get; set; }

        #endregion

        #region Métodos

        public static UsuarioModel ValidarUsuario(string login, string senha)
        {
            UsuarioModel ret = null;
            senha = CriptoHelper.HashMD5(senha);

            using (var db = new ContextoBD())
            {
                ret = db.Usuarios
                    .Where(x => x.Login == login && x.Senha == senha)
                    .SingleOrDefault();
            }

            return ret;
        }

        public static int RecuperarQuantidade()
        {
            var ret = 0;

            using (var db = new ContextoBD())
            {
                ret = db.Usuarios.Count();
            }

            return ret;
        }

        public static List<UsuarioModel> RecuperarLista(int pagina = -1, int tamPagina = -1, string filtro = "", string ordem = "")
        {
            var ret = new List<UsuarioModel>();

            using (var db = new ContextoBD())
            {
                var filtroWhere = "";
                if (!string.IsNullOrEmpty(filtro))
                {
                    filtroWhere = string.Format(" where lower(nome) like '%{0}%'", filtro.ToLower());
                }

                string sql;
                if (pagina == -1 || tamPagina == -1)
                {
                    sql =
                        "select *" +
                        "from usuario" +
                        filtroWhere +
                        " order by " + (!string.IsNullOrEmpty(ordem) ? ordem : "nome");
                }
                else
                {
                    var pos = (pagina - 1) * tamPagina;
                    sql = string.Format(
                        "select *" +
                        " from usuario" +
                        filtroWhere +
                        " order by " + (!string.IsNullOrEmpty(ordem) ? ordem : "nome") +
                        " offset {0} rows fetch next {1} rows only",
                        pos > 0 ? pos - 1 : 0, tamPagina);
                }

                ret = db.Database.Connection.Query<UsuarioModel>(sql).ToList();
            }

            return ret;
        }

        public static UsuarioModel RecuperarPeloId(int id)
        {
            UsuarioModel ret = null;

            using (var db = new ContextoBD())
            {
                ret = db.Usuarios.Find(id);
            }

            return ret;
        }

        public static UsuarioModel RecuperarPeloLogin(string login)
        {
            UsuarioModel ret = null;

            using (var db = new ContextoBD())
            {
                ret = db.Usuarios
                    .Where(x => x.Login == login)
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
                    var usuario = new UsuarioModel { Id = id };
                    db.Usuarios.Attach(usuario);
                    db.Entry(usuario).State = EntityState.Deleted;
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
                    db.Usuarios.Add(this);
                }
                else
                {
                    db.Usuarios.Attach(this);
                    db.Entry(this).State = EntityState.Modified;

                    // TODO: alterar ou não a senha
                }

                db.SaveChanges();
                ret = this.Id;
            }

            return ret;
        }

        public string RecuperarStringNomePerfis()
        {
            var ret = string.Empty;

            // TODO: lista de nomes de perfis
            //using (var conexao = new SqlConnection())
            //{
            //    conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
            //    conexao.Open();

            //    var sql =
            //            "select p.nome " +
            //            "from perfil_usuario pu, perfil p " +
            //            "where (pu.id_usuario = @id_usuario) and (pu.id_perfil = p.id) and (p.ativo = 1)";
            //    var parametros = new { id_usuario = this.Id };
            //    var matriculas = conexao.Query<string>(sql, parametros).ToList();
            //    if (matriculas.Count > 0)
            //    {
            //        ret = string.Join(";", matriculas);
            //    }
            //}

            return ret;
        }

        public bool ValidarSenhaAtual(string senhaAtual)
        {
            var ret = false;

            using (var db = new ContextoBD())
            {
                ret = db.Usuarios
                    .Where(x => x.Senha == senhaAtual && x.Id == this.Id)
                    .Any();
            }

            return ret;
        }

        public bool AlterarSenha(string novaSenha)
        {
            var ret = false;

            using (var db = new ContextoBD())
            {
                this.Senha = CriptoHelper.HashMD5(novaSenha);
                db.Usuarios.Attach(this);
                db.Entry(this).Property(x => x.Senha).IsModified = true;
                db.SaveChanges();
            }

            return ret;
        }

        #endregion
    }
}