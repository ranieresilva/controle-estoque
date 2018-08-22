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
                ret = db.PerfisUsuarios
                    .Include(x => x.Usuarios)
                    .Where(x => x.Id == id)
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

            using (var db = new ContextoBD())
            {
                var model = db.PerfisUsuarios
                    .Include(x => x.Usuarios)
                    .Where(x => x.Id == this.Id)
                    .SingleOrDefault();

                if (model == null)
                {
                    if (this.Usuarios != null && this.Usuarios.Count > 0)
                    {
                        foreach (var usuario in this.Usuarios)
                        {
                            db.Usuarios.Attach(usuario);
                            db.Entry(usuario).State = EntityState.Unchanged;
                        }
                    }

                    db.PerfisUsuarios.Add(this);
                }
                else
                {
                    model.Nome = this.Nome;
                    model.Ativo = this.Ativo;

                    if (this.Usuarios != null)
                    {
                        foreach (var usuario in model.Usuarios.FindAll(x => !this.Usuarios.Exists(u => u.Id == x.Id)))
                        {
                            model.Usuarios.Remove(usuario);
                        }

                        foreach (var usuario in this.Usuarios.FindAll(x => x.Id > 0 && !model.Usuarios.Exists(u => u.Id == x.Id)))
                        {
                            db.Usuarios.Attach(usuario);
                            db.Entry(usuario).State = EntityState.Unchanged;
                            model.Usuarios.Add(usuario);
                        }
                    }
                }

                db.SaveChanges();
                ret = this.Id;
            }

            return ret;
        }

        #endregion
    }
}