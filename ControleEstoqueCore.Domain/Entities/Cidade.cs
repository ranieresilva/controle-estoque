using System;
using System.Collections.Generic;
using System.Text;

namespace ControleEstoqueCore.Domain.Entities
{
    public class Cidade : Entity
    {

        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public int IdEstado { get; set; }
        public virtual Estado Estado { get; set; }

        public static int RecuperarQuantidade()
        {
            var ret = 0;
        }


        public override void Validate()
        {
            if (string.IsNullOrEmpty(Nome))
                AdicionarCritica("O nome não foi informado");
        }
    }
}
