using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ControleEstoque.Web.Models
{
    public class PerfilViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Preencha o nome.")]
        public string Nome { get; set; }

        public bool Ativo { get; set; }

        public virtual List<UsuarioViewModel> Usuarios { get; set; }

        public PerfilViewModel()
        {
            this.Usuarios = new List<UsuarioViewModel>();
        }
    }
}