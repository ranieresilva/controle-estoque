using System.ComponentModel.DataAnnotations;

namespace ControleEstoque.Web.Models
{
    public class ProdutoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Preencha o código.")]
        [MaxLength(10, ErrorMessage = "O código pode ter no máximo 10 caracteres.")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "Preencha o nome.")]
        [MaxLength(50, ErrorMessage = "O nome pode ter no máximo 50 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Preencha o preço de custo.")]
        public decimal PrecoCusto { get; set; }

        [Required(ErrorMessage = "Preencha o preço de venda.")]
        public decimal PrecoVenda { get; set; }

        [Required(ErrorMessage = "Preencha a quantidade em estoque.")]
        public int QuantEstoque { get; set; }

        [Required(ErrorMessage = "Selecione a unidade de medida.")]
        public int IdUnidadeMedida { get; set; }

        [Required(ErrorMessage = "Selecione o grupo.")]
        public int IdGrupo { get; set; }

        [Required(ErrorMessage = "Selecione a marca.")]
        public int IdMarca { get; set; }

        [Required(ErrorMessage = "Selecione o fornecedor.")]
        public int IdFornecedor { get; set; }

        [Required(ErrorMessage = "Selecione o local de armazenamento.")]
        public int IdLocalArmazenamento { get; set; }

        public bool Ativo { get; set; }

        public string Imagem { get; set; }
    }
}