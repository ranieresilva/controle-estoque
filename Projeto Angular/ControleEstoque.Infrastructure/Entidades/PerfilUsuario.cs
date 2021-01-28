using System;
using System.Collections.Generic;
using System.Text;

namespace ControleEstoque.Domain.Entidades
{
    public class PerfilUsuario : Entidade
    {
        public int Id { get; set; }
        public int PerfilId { get; set; }

        public override void Validate()
        {
            throw new NotImplementedException();
        }
    }
}
