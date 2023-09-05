using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce22.SmartOffice.Client.Enums;
using Mcce22.SmartOffice.Client.Managers;
using Mcce22.SmartOffice.Client.Models;
using Mcce22.SmartOffice.Client.Services;

namespace Mcce22.SmartOffice.Client.ViewModels
{
    public partial class ConfigViewModel : ViewModelBase
    {
        private readonly IConfigManager _configManager;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private MqttConfigModel _mqttConfig;

        public ConfigViewModel(IConfigManager configManager, INavigationService navigationService)
        {
            _configManager = configManager;
            _navigationService = navigationService;
        }

        protected override void UpdateCommandStates()
        {
            SaveCommand.NotifyCanExecuteChanged();
            CancelCommand.NotifyCanExecuteChanged();
        }

        public async Task Load()
        {
            try
            {
                IsBusy = true;

                MqttConfig = await _configManager.GetMqttConfig();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public bool CanSave()
        {
            return !IsBusy;
        }

        [RelayCommand(CanExecute = nameof(CanSave))]
        public async void Save()
        {
            try
            {
                await _configManager.SaveMqttConfig(MqttConfig);

                _navigationService.Navigate(NavigationType.Dashboard);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public bool CanCancel()
        {
            return !IsBusy;
        }

        [RelayCommand(CanExecute = nameof(CanCancel))]
        public void Cancel()
        {
            _navigationService.Navigate(NavigationType.Dashboard);
        }
    }
}
