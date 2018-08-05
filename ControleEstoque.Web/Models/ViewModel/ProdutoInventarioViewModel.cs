namespace ControleEstoque.Web.Models
{
    public class ProdutoInventarioViewModel
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string NomeProduto { get; set; }
        public string NomeLocalArmazenamento { get; set; }
        public int QuantEstoque { get; set; }
        public string NomeUnidadeMedida { get; set; }
    }
}