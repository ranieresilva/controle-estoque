using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControleEstoqueCore.Domain.Entities
{
    public abstract class Entity
    {
        private List<string> _mensagemValidacao { get; set; }
        private List<string> mensagemValidacao
        {
            get { return _mensagemValidacao ?? (_mensagemValidacao = new List<string>()); }
        }
        protected void LimparMensagemValidacao()
        {
            mensagemValidacao.Clear();
        }
        protected void AdicionarCritica(string mensagem)
        {
            mensagemValidacao.Add(mensagem);
        }
        public string ObterMensagemValidacao()
        {
            return string.Join(". ", mensagemValidacao);
        }
        public abstract void Validate();
        public bool EhValido
        {
            get { return !mensagemValidacao.Any(); }
        }
    }
}
