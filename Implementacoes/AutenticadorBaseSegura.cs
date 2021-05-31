using Projeto4_SegurancaInformacao.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projeto4_SegurancaInformacao.Implementacoes
{
    class AutenticadorBaseSegura : IAutenticador
    {
        private Dictionary<string, string> _ArquivoBase;
        private IGeradorHash _GeradorSha256;
        private string _Salt;

        public AutenticadorBaseSegura(ref Dictionary<string, string> arquivoBase, IGeradorHash geradorSha256, string salt = "")
        {
            _GeradorSha256 = geradorSha256;
            _Salt = salt;
            _ArquivoBase = arquivoBase;
        }

        public bool AutenticarUsuario(string usuario, string senha)
        {

            var hash = _GeradorSha256.GerarHash(usuario, senha, _Salt);
            var usuarioSenha = _ArquivoBase.FirstOrDefault(x => x.Key == usuario);

            if (usuarioSenha.Value.Equals(hash)) return true;
            else return false;
        }
    }
}
