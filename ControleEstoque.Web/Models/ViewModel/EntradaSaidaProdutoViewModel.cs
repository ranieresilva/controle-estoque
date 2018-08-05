using System;
using System.Collections.Generic;

namespace ControleEstoque.Web.Models
{
    public class EntradaSaidaProdutoViewModel
    {
        public DateTime Data { get; set; }
        public Dictionary<int, int> Produtos { get; set; }
    }
}