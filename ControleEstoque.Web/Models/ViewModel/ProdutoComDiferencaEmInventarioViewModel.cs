namespace ControleEstoque.Web.Models
{
    public class ProdutoComDiferencaEmInventarioViewModel
    {
        public int Id { get; set; }
        public string NomeProduto { get; set; }
        public string CodigoProduto { get; set; }
        public int QuantidadeEstoque { get; set; }
        public int QuantidadeInventario { get; set; }
        public string Motivo { get; set; }
    }
}