using System.Text;
using System.Text.Json;

namespace NFK001
{
    public static class Util
    {
        /// <summary>
        /// Finalizar rotina batch
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        public static extern int ExitProcess(EnCodeProcess Codigo);

        /// <summary>
        /// Configurações do sistema.
        /// </summary>
        public static AppSettings AppSettings { get; private set; }

        /// <summary>
        /// Início da classe.
        /// </summary>
        static Util()
        {
            // Carregando configurações.
            LoadAppSettings();
        }

        /// <summary>
        /// Carrega as configurações do sistema.
        /// </summary>
        private static void LoadAppSettings()
        {
            if (AppSettings is null)
            {
                using StreamReader reader = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"));
                AppSettings = JsonSerializer.Deserialize<AppSettings>(reader.ReadToEnd());
            }
        }

        /// <summary>
        /// Grava o log do sistema.
        /// </summary>
        /// <param name="description">Descrição do log.</param>
        /// <param name="tipo">Tipo do log.</param>
        public static void Log(string description = null, EnTipoLog tipo = EnTipoLog.Padrao)
        {
            string outLog = string.Empty;

            switch (tipo)
            {
                case EnTipoLog.Padrao:
                    outLog = $"{DateTime.Now:HH:mm:ss} > ";
                    break;
                case EnTipoLog.Divisor:
                    outLog = $"{string.Join(string.Empty, Enumerable.Range(0, 80).Select(_ => "_"))}{Environment.NewLine}";
                    break;
                case EnTipoLog.Inicio:
                    outLog = $"{Environment.NewLine}> Início do processo de geração de relatórios de pedidos. - {DateTime.Now:dd/MM/yyyy HH:mm:ss}{Environment.NewLine}";
                    outLog = $"{outLog}{string.Join(string.Empty, Enumerable.Range(0, 80).Select(_ => "_"))}{Environment.NewLine}";
                    break;
                case EnTipoLog.Fim:
                    outLog = $"{Environment.NewLine}{string.Join(string.Empty, Enumerable.Range(0, 80).Select(_ => "_"))}{Environment.NewLine}";
                    outLog = $"{outLog}{Environment.NewLine}> Fim do processo de geração de relatórios de pedidos. - {DateTime.Now:dd/MM/yyyy HH:mm:ss}{Environment.NewLine}";
                    outLog = $"{outLog}{string.Join(string.Empty, Enumerable.Range(0, 80).Select(_ => "_"))}{Environment.NewLine}";
                    break;
                case EnTipoLog.Erro:
                    outLog = $"{string.Join(string.Empty, Enumerable.Range(0, 80).Select(_ => "="))}{Environment.NewLine}";
                    break;
                default:
                    break;
            }

            if (description != null && tipo == EnTipoLog.Erro)
                outLog = $"{outLog}\n{description}\n{outLog}";
            else if (description != null)
                outLog = $"{outLog}{description}";

            if (!string.IsNullOrEmpty(outLog))
            {
                string filePath = AppSettings.Log.Path;
                string fileName = string.Format(AppSettings.Log.FilePrefix, DateTime.Today);

                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                using StreamWriter writer = new(Path.Combine(filePath, fileName), true);
                writer.WriteLine(outLog);
                writer.Close();
                // Logar no console
                Console.WriteLine(outLog);
            }
        }

        public static void LimpaArquivoLog()
        {
            try
            {
                string filePath = AppSettings.Log.Path;
                string fileName = AppSettings.Log.FilePrefix;
                string logPath = Path.Combine(filePath, fileName);
                DateTime lastAccess = File.GetLastWriteTime(logPath);
                DateTime today = DateTime.Today;
                if (lastAccess.Date < today && today.DayOfWeek == DayOfWeek.Sunday)
                {
                    if (File.Exists(logPath))
                    {
                        // 'Limpa o arquivo para gravação
                        StreamWriter arq = new(logPath, false);
                        arq.WriteLine(string.Empty);
                        arq.Close();
                    }
                }
            }
            catch
            {
                throw new Exception("ERRO: Limpar arquivo log");
            }
        }

        /// <summary>
        /// Recuperar Exception e todas InnerException sub-sequentes
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>String exception</returns>
        public static string GetError(Exception ex, bool parcialError = false)
        {
            StringBuilder builder = new();
            while (ex is not null)
            {
                builder.AppendLine(!parcialError ? ex.Message : GetLinesOfString(ex.Message));
                ex = ex.InnerException;
            }
            return builder.ToString();
        }

        /// <summary>
        /// Recuperar uma pequena parte de uma string com várias linhas
        /// </summary>
        /// <param name="input"String completa</param>
        /// <returns>String quebrada</returns>
        private static string GetLinesOfString(string input, int lines = 2)
        {
            List<string> parts = input.Split(new[] { '\r', '\n' }).Where(i => !string.IsNullOrWhiteSpace(i)).ToList();
            if (parts.Count > lines)
                return string.Join(Environment.NewLine, parts.GetRange(0, lines));
            return string.Join(Environment.NewLine, parts);
        }

        /// <summary>
        /// Simular um linq foreach em um enumerable
        /// </summary>
        /// <typeparam name="T">Generic param</typeparam>
        /// <param name="source">Generic enumerable</param>
        /// <param name="action">Action to execute</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source)
                action(item);
        }

        /// <summary>
        /// Realizar comparação de string não validando maíusculas e minúsculas
        /// </summary>
        /// <param name="value">String</param>
        /// <param name="compare">String comparada</param>
        /// <returns>Equals or Not</returns>
        public static bool EqualsIgnoreCase(this string value, string compare)
        {
            return value.Equals(compare, StringComparison.OrdinalIgnoreCase);
        }
    }
}
