using System;
using System.Collections.Generic;

namespace ControleEstoque.Web.Models
{
    public class EntradaProdutoViewModel
    {
        public DateTime Data { get; set; }
        public Dictionary<int, int> Produtos { get; set; }
    }
}