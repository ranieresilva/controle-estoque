using System.ComponentModel.DataAnnotations;

namespace ControleEstoque.Web.Models
{
    public class UnidadeMedidaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Preencha o nome.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Preencha a sigla.")]
        public string Sigla { get; set; }

        public bool Ativo { get; set; }
    }
}