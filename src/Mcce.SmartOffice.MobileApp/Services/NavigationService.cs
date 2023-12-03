namespace Mcce.SmartOffice.MobileApp.Services
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
            var shell = App.Current.MainPage as AppShell ?? _serviceProvider.GetRequiredService<AppShell>();

            App.Current.MainPage = shell;

            await shell.GoToAsync(route);            
        }

        public async Task GoBackAsync()
        {
            await GoToAsync("..");
        }
    }
}
