using Dapper;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ControleEstoque.Web.Models
{
    public class PerfilModel
    {
        #region Atributos

        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public virtual List<UsuarioModel> Usuarios { get; set; }

        #endregion

        #region Métodos

        public static int RecuperarQuantidade()
        {
            var ret = 0;

            using (var db = new ContextoBD())
            {
                ret = db.PerfisUsuarios.Count();
            }

            return ret;
        }

        public static List<PerfilModel> RecuperarLista(int pagina, int tamPagina, string filtro = "", string ordem = "")
        {
            var ret = new List<PerfilModel>();

            using (var db = new ContextoBD())
            {
                var filtroWhere = "";
                if (!string.IsNullOrEmpty(filtro))
                {
                    filtroWhere = string.Format(" where lower(nome) like '%{0}%'", filtro.ToLower());
                }

                var pos = (pagina - 1) * tamPagina;

                var sql = string.Format(
                    "select *" +
                    " from perfil" +
                    filtroWhere +
                    " order by " + (!string.IsNullOrEmpty(ordem) ? ordem : "nome") +
                    " offset {0} rows fetch next {1} rows only",
                    pos > 0 ? pos - 1 : 0, tamPagina);

                ret = db.Database.Connection.Query<PerfilModel>(sql).ToList();
            }

            return ret;
        }

        // TODO: carregar os usuários do perfil
        //public void CarregarUsuarios()
        //{
        //    this.Usuarios.Clear();

        //    using (var conexao = new SqlConnection())
        //    {
        //        conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
        //        conexao.Open();

        //        var sql =
        //            "select u.* " +
        //            "from perfil_usuario pu, usuario u " +
        //            "where (pu.id_perfil = @id_perfil) and (pu.id_usuario = u.id)";
        //        var parametros = new { id_perfil = this.Id };
        //        this.Usuarios = conexao.Query<UsuarioModel>(sql, parametros).ToList();
        //    }
        //}

        public static List<PerfilModel> RecuperarListaAtivos()
        {
            var ret = new List<PerfilModel>();

            using (var db = new ContextoBD())
            {
                ret = db.PerfisUsuarios
                    .Where(x => x.Ativo)
                    .OrderBy(x => x.Nome)
                    .ToList();
            }

            return ret;
        }

        public static PerfilModel RecuperarPeloId(int id)
        {
            PerfilModel ret = null;

            using (var db = new ContextoBD())
            {
                ret = db.PerfisUsuarios.Find(id);
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
                    var perfil = new PerfilModel { Id = id };
                    db.PerfisUsuarios.Attach(perfil);
                    db.Entry(perfil).State = EntityState.Deleted;
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
                    db.PerfisUsuarios.Add(this);
                }
                else
                {
                    db.PerfisUsuarios.Attach(this);
                    db.Entry(this).State = EntityState.Modified;
                }

                // TODO: salva os usuários do perfil
                //if (this.Usuarios != null && this.Usuarios.Count > 0)
                //{
                //    var sql = "delete from perfil_usuario where (id_perfil = @id_perfil)";
                //    var parametros = new { id_perfil = this.Id };
                //    conexao.Execute(sql, parametros, transacao);

                //    if (this.Usuarios[0].Id != -1)
                //    {
                //        foreach (var usuario in this.Usuarios)
                //        {
                //            sql = "insert into perfil_usuario (id_perfil, id_usuario) values (@id_perfil, @id_usuario)";
                //            var parametrosUsuario = new { id_perfil = this.Id, id_usuario = usuario.Id };
                //            conexao.Execute(sql, parametrosUsuario, transacao);
                //        }
                //    }
                //}

                db.SaveChanges();
                ret = this.Id;
            }

            return ret;
        }

        #endregion
    }
}