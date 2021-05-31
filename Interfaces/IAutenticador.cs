using System;
using System.Collections.Generic;
using System.Text;

namespace Projeto4_SegurancaInformacao.Interfaces
{
    interface IAutenticador
    {
        bool AutenticarUsuario(string usuario, string senha);

        public bool AutenticarUsuario(ref Dictionary<string, string> arquivoBase, string usuario, string senha);
    }
}
