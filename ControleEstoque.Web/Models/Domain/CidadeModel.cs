using Dapper;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ControleEstoque.Web.Models
{
    public class CidadeModel
    {
        #region Atributos

        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public int IdEstado { get; set; }
        public virtual EstadoModel Estado { get; set; }

        #endregion

        #region Métodos

        public static int RecuperarQuantidade()
        {
            var ret = 0;

            using (var db = new ContextoBD())
            {
                ret = db.Cidades.Count();
            }

            return ret;
        }

        public static List<CidadeViewModel> RecuperarLista(int pagina = 0, int tamPagina = 0, string filtro = "", string ordem = "", int idEstado = 0)
        {
            var ret = new List<CidadeViewModel>();

            using (var db = new ContextoBD())
            {
                var pos = (pagina - 1) * tamPagina;

                var filtroWhere = "";
                if (!string.IsNullOrEmpty(filtro))
                {
                    filtroWhere = string.Format(" (lower(c.nome) like '%{0}%') and", filtro.ToLower());
                }

                if (idEstado > 0)
                {
                    filtroWhere += string.Format(" (id_estado = {0}) and", idEstado);
                }

                var paginacao = "";
                if (pagina > 0 && tamPagina > 0)
                {
                    paginacao = string.Format(" offset {0} rows fetch next {1} rows only",
                        pos > 0 ? pos - 1 : 0, tamPagina);
                }

                var sql =
                    "select c.id, c.nome, c.ativo, c.id_estado as IdEstado, e.id_pais as IdPais" +
                    " from cidade c, estado e" +
                    " where" +
                    filtroWhere +
                    " (c.id_estado = e.id)" +
                    " order by " + (!string.IsNullOrEmpty(ordem) ? ordem : "c.nome") +
                    paginacao;

                ret = db.Database.Connection.Query<CidadeViewModel>(sql).ToList();
            }

            return ret;
        }

        public static CidadeViewModel RecuperarPeloId(int id)
        {
            CidadeViewModel ret = null;

            using (var db = new ContextoBD())
            {
                ret = db.Cidades
                    .Include(x => x.Estado)
                    .Where(x => x.Id == id)
                    .Select(x => new CidadeViewModel
                    {
                        Id = x.Id,
                        Nome = x.Nome,
                        Ativo = x.Ativo,
                        IdEstado = x.IdEstado,
                        IdPais = x.Estado.IdPais
                    })
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
                    var cidade = new CidadeModel { Id = id };
                    db.Cidades.Attach(cidade);
                    db.Entry(cidade).State = EntityState.Deleted;
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
                    db.Cidades.Add(this);
                }
                else
                {
                    db.Cidades.Attach(this);
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