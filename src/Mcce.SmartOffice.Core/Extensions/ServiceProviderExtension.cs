using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Mcce.SmartOffice.Core.Extensions
{
    public static class ServiceProviderExtension
    {
        public static async Task InitializeDatabase<T>(this IServiceProvider serviceProvider) where T : DbContext
        {
            using var scope = serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<T>();

            if (dbContext.Database.IsCosmos())
            {
                await dbContext.Database.EnsureCreatedAsync();
            }
        }
    }
}
