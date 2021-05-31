using Projeto4_SegurancaInformacao.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projeto4_SegurancaInformacao.Implementacoes
{
    class AutenticadorBaseNormal : IAutenticador
    {

        Dictionary<string, string> _ArquivoBase;

        public AutenticadorBaseNormal(ref Dictionary<string, string> arquivoBase)
        {
            _ArquivoBase = arquivoBase;
        }

        public bool AutenticarUsuario(string usuario, string senha)
        {
            var usuarioSenha = _ArquivoBase.FirstOrDefault(x => x.Key == usuario);

            if (usuarioSenha.Value.Equals(senha)) return true;
            else return false;
        }
    }
}
