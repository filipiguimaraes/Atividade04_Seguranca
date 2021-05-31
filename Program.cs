using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Projeto4_SegurancaInformacao.Implementacoes;
using Projeto4_SegurancaInformacao.Interfaces;
using System.Diagnostics;
using Projeto4_SegurancaInformacao.Utils;
using System.Linq;

namespace Projeto4_SegurancaInformacao
{

    /*
        
        Converter base para base segura - OK

        Funcao para validacao da senha - OK

        Calcular tempo de conversao da base - OK

        Calcular tempo médio para fazer autenticacao do usuário (Novo e antigo) - OK

        Detalhar algoritmo usado

        Disponibilizar link

     */
    class Program
    {
        static IConfiguration _Configuration;
        static IGeradorHash _GeradorHashMd5, _GeradorSha256;
        static IAutenticador _AutenticadorBaseNormal, _AutenticadorBaseSegura;
        static Random _Random;
        static GeradorBaseSegura _GeradorBaseSegura;
        static Stopwatch _StopWatch;
        static void Main(string[] args)
        {
            _Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            string caminhoArquivoBaseEntrada = _Configuration.GetValue<string>("CaminhoArquivoBaseEntrada");
            string caminhoArquivoBaseSeguraSaida = _Configuration.GetValue<string>("CaminhoArquivoBaseSeguraSaida");
            string salt = _Configuration.GetValue<string>("Salt");

            _GeradorHashMd5 = new GeradorHashMD5();
            _GeradorSha256 = new GeradorHashSha256();
            _GeradorBaseSegura = new GeradorBaseSegura(caminhoArquivoBaseEntrada, caminhoArquivoBaseSeguraSaida, _GeradorSha256);

            _Random = new Random();
            _StopWatch = new Stopwatch();
            TimeSpan ts;
            // Geracao de base segura

            Console.WriteLine("Geracao Base segura - Inicio.");
            _StopWatch.Start();
            _GeradorBaseSegura.GerarBaseSegura(salt);
            _StopWatch.Stop();

            ts = _StopWatch.Elapsed;
            Console.WriteLine($"Geracao Base segura - Tempo para execucao: {ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}:{ts.Milliseconds / 10:00}");
            Console.WriteLine("Geracao Base segura - Fim.");
            
            //string saltOrigem = geradorHashMd5.GerarHash("F31j04d4", "", "");

            Console.WriteLine("Geracao Dicionario de dados Normal - Inicio.");
            _StopWatch.Restart();
            var dicBaseNormal = Util.GetDictionaryFromFile(caminhoArquivoBaseEntrada);
            _StopWatch.Stop();
            ts = _StopWatch.Elapsed;
            Console.WriteLine($"Geracao Dicionario de dados Normal- Tempo para execucao: {ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}:{ts.Milliseconds / 10:00}");
            Console.WriteLine("Geracao Dicionario de dados Normal - Fim.");

            Console.WriteLine("Geracao Dicionario de dados Segura - Inicio.");
            _StopWatch.Restart();
            var dicBaseSegura = Util.GetDictionaryFromFile(caminhoArquivoBaseSeguraSaida);
            _StopWatch.Stop();
            ts = _StopWatch.Elapsed;
            Console.WriteLine($"Geracao Dicionario de dados Segura - Tempo para execucao: {ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}:{ts.Milliseconds / 10:00}");
            Console.WriteLine("Geracao Dicionario de dados Segura - Fim.");

            _AutenticadorBaseNormal = new AutenticadorBaseNormal(ref dicBaseNormal);
            _AutenticadorBaseSegura = new AutenticadorBaseSegura(ref dicBaseSegura, _GeradorSha256, salt);

            Console.WriteLine("Validacao Base de dados Normal - Inicio.");
            _StopWatch.Restart();
            for (int i = 0; i < 30000; i++)
            {
                var usuarioSenha = dicBaseNormal.ElementAt(_Random.Next(0, dicBaseNormal.Count));
                _AutenticadorBaseNormal.AutenticarUsuario(usuarioSenha.Key, usuarioSenha.Value);
            }
            _StopWatch.Stop();
            ts = _StopWatch.Elapsed;
            Console.WriteLine($"Validacao Base de dados Normal - Tempo para execucao: {ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}:{ts.Milliseconds / 10:00}");
            Console.WriteLine("Validacao Base de dados Normal - Fim.");

            Console.WriteLine("Validacao Base de dados Segura - Inicio.");
            _StopWatch.Restart();
            for (int i = 0; i < 30000; i++)
            {
                var usuarioHash = dicBaseNormal.ElementAt(_Random.Next(0, dicBaseNormal.Count));
                _AutenticadorBaseSegura.AutenticarUsuario(usuarioHash.Key, usuarioHash.Value);
            }
            _StopWatch.Stop();
            ts = _StopWatch.Elapsed;
            Console.WriteLine($"Validacao Base de dados Segura - Tempo para execucao: {ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}:{ts.Milliseconds / 10:00}");
            Console.WriteLine("Validacao Base de dados Segura - Fim.");

            Console.ReadKey();
        }
    }
}
