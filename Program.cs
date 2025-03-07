using Process = NFK001.Processes.Process;

namespace NFK001
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Util.Log(null, EnTipoLog.Inicio);

                EnCodeProcess codeProcess = Process.Init(args[0]);

                Util.Log(null, EnTipoLog.Fim);
                Util.ExitProcess(codeProcess);
            }
            catch (Exception ex)
            {
                Util.Log($"Erro crítico para execução: {Util.GetError(ex)}", EnTipoLog.Erro);
                Util.Log(null, EnTipoLog.Fim);
                Util.ExitProcess(EnCodeProcess.Erro);
            }
        }
    }
}