using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Projeto4_SegurancaInformacao.Utils
{
    public static class Util
    {
        public static string ConvertHashToString(byte[] bytes)
        {
            var builder = new StringBuilder();

            foreach (byte b in bytes) builder.Append(b.ToString("x2"));

            return builder.ToString();
        }

        public static Dictionary<string, string> GetDictionaryFromFile(string caminhoArquivo)
        {
            try
            {
                Dictionary<string, string> dicionario = new Dictionary<string, string>();
                using (StreamReader sr = new StreamReader(caminhoArquivo))
                {
                    while (sr.Peek() >= 0)
                    {
                        var linha = sr.ReadLine();
                        var splitLinha = linha.Split('|');
                        var usuario = splitLinha[1];
                        var senha = splitLinha[2];

                        dicionario.Add(usuario, senha);
                    }
                }

                return dicionario;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
