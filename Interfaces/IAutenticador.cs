using System;
using System.Collections.Generic;
using System.Text;

namespace Projeto4_SegurancaInformacao.Interfaces
{
    interface IAutenticador
    {
        bool AutenticarUsuario(string usuario, string senha);
    }
}
