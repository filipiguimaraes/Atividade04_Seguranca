using System;
using System.Collections.Generic;
using System.Text;

namespace Projeto4_SegurancaInformacao.Interfaces
{
    interface IGeradorHash
    {
        string GerarHash(string usuario, string senha, string salt);
    }
}
