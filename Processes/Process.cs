using Microsoft.Extensions.DependencyInjection;
using NFK001.Contracts.NFK001;
using NFK001.Dependecies;

namespace NFK001.Processes
{
    public class Process
    {
        public static EnCodeProcess Init(string data)
        {
            // Resolve and use the services
            using IServiceScope scope = Ioc.RegisterDependenciesInjection().CreateScope();

            // Resolve the service
            INFK001BLL _INFK001BLL = scope.ServiceProvider.GetService<INFK001BLL>();

            Util.Log("Início do processo");
            EnCodeProcess retr = _INFK001BLL.Update(data).Result;
            Util.Log("Fim do processo");

            return retr;
        }
    }
}
