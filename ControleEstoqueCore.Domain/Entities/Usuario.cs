using System;
using System.Collections.Generic;
using System.Text;

namespace ControleEstoqueCore.Domain.Entities
{
    public class Usuario : Entity
    {

        public int Id { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public virtual List<Perfil> Perfils { get; set; }


        public override void Validate()
        {
            throw new NotImplementedException();
        }
    }
}
