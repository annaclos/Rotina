using Microsoft.Extensions.DependencyInjection;
using NFK001.Business.NFK001;
using NFK001.Contracts.NFK001;
using NFK001.Infra;

namespace NFK001.Dependecies
{
    public static class Ioc
    {
        /// <summary>
        /// Set up the dependency injection container
        /// </summary>
        /// <returns>ServiceProvider</returns>
        public static ServiceProvider RegisterDependenciesInjection()
        {
            return new ServiceCollection()
                .AddSingleton<DapperContext>()
                .AddTransient<INFK001DAL, NFK001DAL>()
                .AddTransient<INFK001BLL, NFK001BLL>()
                .BuildServiceProvider();
        }
    }
}
