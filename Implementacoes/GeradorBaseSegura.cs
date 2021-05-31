using Microsoft.VisualBasic.CompilerServices;
using Projeto4_SegurancaInformacao.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Projeto4_SegurancaInformacao.Implementacoes
{
    class GeradorBaseSegura
    {
        private string _CaminhoArquivoBaseEntrada;
        private string _CaminhoArquivoBaseSaida;
        private IGeradorHash _GeradorHash;
        private string _Salt = string.Empty;

        public GeradorBaseSegura(string caminhoArquivoBaseEntrada, string caminhoArquivoBaseSaida, IGeradorHash geradorHash)
        {
            _CaminhoArquivoBaseEntrada = caminhoArquivoBaseEntrada;
            _CaminhoArquivoBaseSaida = caminhoArquivoBaseSaida;
            _GeradorHash = geradorHash;
        }

        public void GerarBaseSegura(string salt)
        {
            _Salt = salt;
            GerarBaseSegura();
        }

        public void GerarBaseSegura()
        {
            try
            {
                using (StreamReader sr = new StreamReader(_CaminhoArquivoBaseEntrada))
                {
                    using (StreamWriter sw = new StreamWriter(_CaminhoArquivoBaseSaida))
                    {
                        while (sr.Peek() >= 0)
                        {
                            string linha = sr.ReadLine();
                            var splitLinha = linha.Split('|');

                            var usuario = splitLinha[1];
                            var senha = splitLinha[2];

                            var hash = _GeradorHash.GerarHash(usuario, senha, _Salt);

                            sw.WriteLine($"|{usuario}|{hash}|");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
