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

        public static IServiceCollection AddDbContext<TContext>(this IServiceCollection services, DbConfig dbConfig) where TContext : DbContext
        {
            switch (dbConfig.DatabaseType)
            {
                case DbType.CosmosDb:
                    services.AddDbContext<TContext>(opt => opt.UseCosmos(dbConfig.ConnectionString, dbConfig.DatabaseName));
                    break;
                case DbType.SQLite:
                    services.AddDbContext<TContext>(opt => opt.UseSqlite(dbConfig.ConnectionString));
                    break;
                case DbType.SqlServer:
                    services.AddDbContext<TContext>(opt => opt.UseSqlServer(dbConfig.ConnectionString, builder =>
                    {
                        builder.MigrationsHistoryTable("__EFMigrationsHistory", dbConfig.DatabaseSchema);
                        builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    }));
                    break;
            }

            return services;
        }
    }
}
