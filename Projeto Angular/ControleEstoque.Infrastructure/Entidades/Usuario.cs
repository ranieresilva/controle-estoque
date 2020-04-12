using System;
using System.Collections.Generic;
using System.Text;

namespace ControleEstoque.Domain.Entidades
{
    public class Usuario : Entidade
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        public string Nome { get; set; }
        public string SobreNome { get; set; }
        public string Email { get; set; }



        public override void Validate()
        {
            throw new NotImplementedException();
        }
    }
}
