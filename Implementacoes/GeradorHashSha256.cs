using Projeto4_SegurancaInformacao.Interfaces;
using Projeto4_SegurancaInformacao.Utils;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Projeto4_SegurancaInformacao.Implementacoes
{
    class GeradorHashSha256 : IGeradorHash
    {
        private SHA256 _Sha256;
        public string GerarHash(string usuario, string senha, string salt)
        {
            try
            {
                using (_Sha256 = SHA256.Create())
                {
                    string baseString = $"{usuario}{senha}{salt}";
                    byte[] bytes = Encoding.UTF8.GetBytes(baseString);

                    return Util.ConvertHashToString(_Sha256.ComputeHash(bytes));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
