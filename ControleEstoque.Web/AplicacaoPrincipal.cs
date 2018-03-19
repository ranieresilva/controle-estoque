using ControleEstoque.Web.Models;
using System.Security.Principal;

namespace ControleEstoque.Web
{
    public class AplicacaoPrincipal : GenericPrincipal
    {
        public UsuarioModel Dados { get; set; }

        public AplicacaoPrincipal(IIdentity identity, string[] roles, int id) : base(identity, roles)
        {
            Dados = UsuarioModel.RecuperarPeloId(id);
        }
    }
}