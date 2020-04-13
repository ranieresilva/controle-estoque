using System;
using System.Collections.Generic;
using System.Text;

namespace ControleEstoque.Domain.Entidades
{
    public class Pais : Entidade
    {


        public int Id { get; set; }
        public string Nome { get; set; }
        public string Codigo { get; set; }
        public bool Ativo { get; set; }

        public override void Validate()
        {
            throw new NotImplementedException();
        }
    }
}
