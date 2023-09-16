using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Mcce.SmartOffice.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterAllTypes<T>(this IServiceCollection services, Assembly[] assemblies, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            var typesFromAssemblies = assemblies.SelectMany(a => a.DefinedTypes.Where(x => x.IsClass && !x.IsAbstract && x.GetInterfaces().Contains(typeof(T))));

            foreach (var type in typesFromAssemblies)
            {
                services.Add(new ServiceDescriptor(typeof(T), type, lifetime));
            }
        }
    }
}
