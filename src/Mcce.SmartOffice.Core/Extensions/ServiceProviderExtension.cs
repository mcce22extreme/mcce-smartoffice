using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Mcce.SmartOffice.Core.Extensions
{
    public static class ServiceProviderExtension
    {
        public static async Task InitializeDatabase<T>(this IServiceProvider serviceProvider) where T : DbContext
        {
            using var scope = serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<T>();

            if (dbContext.Database.IsSqlite())
            {
                var connectionString = dbContext.Database.GetConnectionString();

                var sb = new SqliteConnectionStringBuilder(connectionString);

                var directory = Path.GetDirectoryName(sb.DataSource);

                if (!Directory.Exists(directory))
                {
                    Log.Information($"Creating directory '{directory}' for database.");
                    Directory.CreateDirectory(directory);
                }

                await dbContext.Database.MigrateAsync();
            }
        }
    }
}
