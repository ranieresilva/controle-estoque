using System.ComponentModel.DataAnnotations;

namespace ControleEstoque.Web.Models
{
    public class AlteracaoSenhaUsuarioViewModel
    {
        [Required(ErrorMessage = "Preencha a senha atual.")]
        [Display(Name = "Senha Atual")]
        public string SenhaAtual { get; set; }

        [Required(ErrorMessage = "Preencha a nova senha.")]
        [MinLength(3, ErrorMessage = "A nova senha deve ter no mínimo 3 caracteres")]
        [Display(Name = "Nova Senha")]
        public string NovaSenha { get; set; }

        [Required(ErrorMessage = "Preencha a confirmação da nova senha.")]
        [Compare("NovaSenha", ErrorMessage = "A senha e a confirmação devem ser iguais.")]
        [Display(Name = "Confirmação da Nova Senha")]
        public string ConfirmacaoNovaSenha { get; set; }
    }
}