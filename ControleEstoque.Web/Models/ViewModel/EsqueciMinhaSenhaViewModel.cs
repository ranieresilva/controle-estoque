using System.ComponentModel.DataAnnotations;

namespace ControleEstoque.Web.Models
{
    public class EsqueciMinhaSenhaViewModel
    {
        [Required(ErrorMessage = "Informe o login")]
        public string Login { get; set; }
    }
}