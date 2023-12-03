namespace Mcce.SmartOffice.App.Services
{
    public interface INavigationService
    {
        Task GoToAsync(string route);

        Task GoBackAsync();
    }

    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task GoToAsync(string route)
        {
            var shell = _serviceProvider.GetRequiredService<Shell>();

            await shell.GoToAsync(route);            
        }

        public async Task GoBackAsync()
        {
            await GoToAsync("..");
        }
    }
}
