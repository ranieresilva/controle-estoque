using Dapper;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ControleEstoque.Web.Models
{
    public class EstadoModel
    {
        #region Atributos

        public int Id { get; set; }
        public string Nome { get; set; }
        public string UF { get; set; }
        public bool Ativo { get; set; }
        public int IdPais { get; set; }
        public virtual PaisModel Pais { get; set; }

        #endregion

        #region Métodos

        public static int RecuperarQuantidade()
        {
            var ret = 0;

            using (var db = new ContextoBD())
            {
                ret = db.Estados.Count();
            }

            return ret;
        }

        public static List<EstadoModel> RecuperarLista(int pagina = 0, int tamPagina = 0, string filtro = "", string ordem = "", int idPais = 0)
        {
            var ret = new List<EstadoModel>();

            using (var db = new ContextoBD())
            {
                var filtroWhere = "";
                if (!string.IsNullOrEmpty(filtro))
                {
                    filtroWhere = string.Format(" where lower(nome) like '%{0}%'", filtro.ToLower());
                }

                if (idPais > 0)
                {
                    filtroWhere +=
                        (string.IsNullOrEmpty(filtroWhere) ? " where" : " and") +
                        string.Format(" id_pais = {0}", idPais);
                }

                var pos = (pagina - 1) * tamPagina;
                var paginacao = "";
                if (pagina > 0 && tamPagina > 0)
                {
                    paginacao = string.Format(" offset {0} rows fetch next {1} rows only",
                        pos > 0 ? pos - 1 : 0, tamPagina);
                }

                var sql =
                    "select id, nome, uf, id_pais as IdPais, ativo" +
                    " from estado" +
                    filtroWhere +
                    " order by " + (!string.IsNullOrEmpty(ordem) ? ordem : "nome") +
                    paginacao;

                ret = db.Database.Connection.Query<EstadoModel>(sql).ToList();
            }

            return ret;
        }

        public static EstadoModel RecuperarPeloId(int id)
        {
            EstadoModel ret = null;

            using (var db = new ContextoBD())
            {
                ret = db.Estados.Find(id);
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
                    var estado = new EstadoModel { Id = id };
                    db.Estados.Attach(estado);
                    db.Entry(estado).State = EntityState.Deleted;
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
                    db.Estados.Add(this);
                }
                else
                {
                    db.Estados.Attach(this);
                    db.Entry(this).State = EntityState.Modified;
                }

                db.SaveChanges();
                ret = this.Id;
            }

            return ret;
        }

        #endregion
    }
}