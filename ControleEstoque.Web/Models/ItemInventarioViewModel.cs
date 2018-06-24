namespace ControleEstoque.Web.Models
{
    public class ItemInventarioViewModel
    {
        public int IdProduto { get; set; }
        public int QuantidadeEstoque { get; set; }
        public int QuantidadeInventario { get; set; }
        public string Motivo { get; set; }
    }
}