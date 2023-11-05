using System.Reflection;
using Mcce.SmartOffice.Core.Configs;
using Mcce.SmartOffice.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace Mcce.SmartOffice.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterAllTypes<T>(this IServiceCollection services, Assembly[] assemblies, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            var typesFromAssemblies = assemblies.SelectMany(a => a.DefinedTypes.Where(x => x.IsClass && !x.IsAbstract && x.GetInterfaces().Contains(typeof(T))));

            foreach (var type in typesFromAssemblies)
            {
                services.Add(new ServiceDescriptor(typeof(T), type, lifetime));
            }
            return services;
        }

        public static IServiceCollection AddDbContext<TContext>(this IServiceCollection services, DbConfig dbConfig, string databaseSchema) where TContext : DbContext
        {
            switch (dbConfig.DatabaseType)
            {
                case DbType.SqlServer:
                    services.AddDbContext<TContext>(opt => opt.UseSqlServer(dbConfig.ConnectionString, builder =>
                    {
                        builder.MigrationsHistoryTable("__EFMigrationsHistory", databaseSchema);
                        builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    }));
                    break;
                case DbType.InMemory:
                    services.AddDbContext<TContext>(opt => opt.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                    break;
            }

            return services;
        }
    }
}
