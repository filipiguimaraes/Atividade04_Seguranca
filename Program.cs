using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Projeto4_SegurancaInformacao.Implementacoes;
using Projeto4_SegurancaInformacao.Interfaces;
using System.Diagnostics;
using Projeto4_SegurancaInformacao.Utils;
using System.Linq;
using System.Collections.Generic;

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
        static string _Salt;
        static Dictionary<string, string> _ArquivoBase;
        static Dictionary<string, string> _ArquivoBaseSegura;
        private static Dictionary<string, string> _BaseSeguraExecucao;
        static string caminhoArquivoBaseEntrada;
        static string caminhoArquivoBaseSeguraSaida;
        static void Main(string[] args)
        {
            _Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            caminhoArquivoBaseEntrada = _Configuration.GetValue<string>("CaminhoArquivoBaseEntrada");
            caminhoArquivoBaseSeguraSaida = _Configuration.GetValue<string>("CaminhoArquivoBaseSeguraSaida");
            _Salt = _Configuration.GetValue<string>("Salt");

            _GeradorHashMd5 = new GeradorHashMD5();
            _GeradorSha256 = new GeradorHashSha256();
            _GeradorBaseSegura = new GeradorBaseSegura(caminhoArquivoBaseEntrada, caminhoArquivoBaseSeguraSaida, _GeradorSha256);

            _Random = new Random();
            _StopWatch = new Stopwatch();

            _BaseSeguraExecucao = new Dictionary<string, string>();
            TestarAutenticacao();
        }

        public static void GerarMetricas()
        {
            Console.WriteLine("Geracao Base segura - Inicio.");
            _StopWatch.Start();
            _GeradorBaseSegura.GerarBaseSegura(_Salt);
            _StopWatch.Stop();
            TimeSpan ts;

            ts = _StopWatch.Elapsed;
            Console.WriteLine($"Geracao Base segura - Tempo para execucao: {ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}:{ts.Milliseconds / 10:00}");
            Console.WriteLine("Geracao Base segura - Fim.");

            //string saltOrigem = geradorHashMd5.GerarHash("F31j04d4", "", "");

            Console.WriteLine("Geracao Dicionario de dados Normal - Inicio.");
            _StopWatch.Restart();
            _ArquivoBase = Util.GetDictionaryFromFile(caminhoArquivoBaseEntrada);
            _StopWatch.Stop();
            ts = _StopWatch.Elapsed;
            Console.WriteLine($"Geracao Dicionario de dados Normal- Tempo para execucao: {ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}:{ts.Milliseconds / 10:00}");
            Console.WriteLine("Geracao Dicionario de dados Normal - Fim.");

            Console.WriteLine("Geracao Dicionario de dados Segura - Inicio.");
            _StopWatch.Restart();
            _ArquivoBaseSegura = Util.GetDictionaryFromFile(caminhoArquivoBaseSeguraSaida);
            _StopWatch.Stop();
            ts = _StopWatch.Elapsed;
            Console.WriteLine($"Geracao Dicionario de dados Segura - Tempo para execucao: {ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}:{ts.Milliseconds / 10:00}");
            Console.WriteLine("Geracao Dicionario de dados Segura - Fim.");

            _AutenticadorBaseNormal = new AutenticadorBaseNormal(ref _ArquivoBase);
            _AutenticadorBaseSegura = new AutenticadorBaseSegura(ref _ArquivoBaseSegura, _GeradorSha256, _Salt);

            Console.WriteLine("Validacao Base de dados Normal - Inicio.");
            _StopWatch.Restart();
            for (int i = 0; i < 10; i++)
            {
                var usuarioSenha = _ArquivoBase.ElementAt(_Random.Next(0, _ArquivoBase.Count));
                _AutenticadorBaseNormal.AutenticarUsuario(usuarioSenha.Key, usuarioSenha.Value);
            }
            _StopWatch.Stop();
            ts = _StopWatch.Elapsed;
            Console.WriteLine($"Validacao Base de dados Normal - Tempo para execucao: {ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}:{ts.Milliseconds / 10:00}");
            Console.WriteLine("Validacao Base de dados Normal - Fim.");

            Console.WriteLine("Validacao Base de dados Segura - Inicio.");
            _StopWatch.Restart();
            for (int i = 0; i < 10; i++)
            {
                var usuarioHash = _ArquivoBaseSegura.ElementAt(_Random.Next(0, _ArquivoBaseSegura.Count));
                _AutenticadorBaseSegura.AutenticarUsuario(usuarioHash.Key, usuarioHash.Value);
            }
            _StopWatch.Stop();
            ts = _StopWatch.Elapsed;
            Console.WriteLine($"Validacao Base de dados Segura - Tempo para execucao: {ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}:{ts.Milliseconds / 10:00}");
            Console.WriteLine("Validacao Base de dados Segura - Fim.");

            Console.WriteLine("----------------------------------------------------------");
            TestarAutenticacao();
        }

        public static void Gravar()
        {
            Console.WriteLine("Digite o usuario: ");
            var usuario = Console.ReadLine();
            Console.WriteLine("Digite a senha: ");
            var senha = Console.ReadLine();

            var hash = _GeradorSha256.GerarHash(usuario, senha, _Salt);

            _BaseSeguraExecucao.Add(usuario, hash);

            Console.WriteLine("----------------------------------------------------------");
            TestarAutenticacao();
        }

        public static void Logar()
        {
            Console.WriteLine("Digite o usuario: ");
            var usuario = Console.ReadLine();
            Console.WriteLine("Digite a senha: ");
            var senha = Console.ReadLine();

            if(_AutenticadorBaseSegura == null){
                _AutenticadorBaseSegura = new AutenticadorBaseSegura(ref _ArquivoBaseSegura, _GeradorSha256, _Salt);
            }

            if (_AutenticadorBaseSegura.AutenticarUsuario(ref _BaseSeguraExecucao, usuario, senha))
                Console.WriteLine("Usuario validado sucesso.");
            else
                Console.WriteLine("Usuario invalido.");

            Console.WriteLine("----------------------------------------------------------");
            TestarAutenticacao();
        }


        public static void TestarAutenticacao()
        {
            Console.WriteLine("Escolha uma opção: ");
            Console.WriteLine("1 - Gravar");
            Console.WriteLine("2 - Logar");
            Console.WriteLine("3 - Gerar metricas");
            Console.WriteLine("4 - Sair");



            var opcao = Console.ReadLine().ToString();

            try
            {
                int opcaoInt = Convert.ToInt32(opcao);
                while (opcaoInt != 1 && opcaoInt != 2 && opcaoInt != 3)
                {
                    Console.WriteLine("Opcao invalida. ");
                    break;
                }

                if (opcao == "1")
                {
                    Gravar();
                }
                else if (opcao == "2")
                {
                    Logar();
                }
                else if (opcao == "3")
                {
                    GerarMetricas();
                }
                else if (opcao == "4")
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Formato invalido.");
                Console.WriteLine("----------------------------------------------------------");
                TestarAutenticacao();
            }
        }
    }
}
