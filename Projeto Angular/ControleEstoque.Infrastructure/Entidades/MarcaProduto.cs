using System;
using System.Collections.Generic;
using System.Text;

namespace ControleEstoque.Domain.Entidades
{
    public class MarcaProduto : Entidade
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }


        public override void Validate()
        {
            throw new NotImplementedException();
        }
    }
}
