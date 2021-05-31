using Projeto4_SegurancaInformacao.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using Projeto4_SegurancaInformacao.Utils;

namespace Projeto4_SegurancaInformacao.Implementacoes
{
    class GeradorHashMD5 : IGeradorHash
    {
        private MD5 _Md5 = MD5.Create();

        public string GerarHash(string usuario, string senha, string salt)
        {
            try
            {
                using(_Md5 = MD5.Create())
                {
                    string baseString = $"{salt}{usuario}{senha}";
                    byte[] bytes = Encoding.ASCII.GetBytes(baseString);

                    return Util.ConvertHashToString(_Md5.ComputeHash(bytes));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
