using System.ComponentModel.DataAnnotations;

namespace ControleEstoque.Web.Models
{
    public class NovaSenhaViewModel
    {
        [Required(ErrorMessage = "Informe a senha")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha:")]
        public string Senha { get; set; }

        [Compare("Senha", ErrorMessage = "Informe a confirmação da senha")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Senha:")]
        public string ConfirmacaoSenha { get; set; }

        public int Usuario { get; set; }
    }
}