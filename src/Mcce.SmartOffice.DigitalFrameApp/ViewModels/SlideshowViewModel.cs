﻿using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.App.Services;
using Mcce.SmartOffice.App.ViewModels;
using Mcce.SmartOffice.DigitalFrameApp.Managers;
using Mcce.SmartOffice.DigitalFrameApp.Pages;

namespace Mcce.SmartOffice.DigitalFrameApp.ViewModels
{
    public partial class SlideshowViewModel : ViewModelBase
    {
        public event EventHandler<int> OnUserImageIndexUpdate;

        private readonly ISessionManager _sessionManager;
        private readonly IDispatcherTimer _dispatcherTimer;

        private string _currentImage;

        [ObservableProperty]
        private ObservableCollection<string> _userImages = new ObservableCollection<string>();

        public SlideshowViewModel(
            ISessionManager sessionManager,
            IDispatcherTimer dispatcherTimer,
            INavigationService navigationService,
            IDialogService dialogService)
            : base(navigationService, dialogService)
        {
            _sessionManager = sessionManager;
            _dispatcherTimer = dispatcherTimer;

            _dispatcherTimer.Interval = TimeSpan.FromSeconds(10);
            _dispatcherTimer.Tick += OnTimerTicker;
        }

        public override Task Activate()
        {
            UserImages.Clear();

            var userImages = _sessionManager.GetCurrentUserImages();

            if(userImages.Length > 0)
            {
                foreach (var image in userImages)
                {
                    UserImages.Add(image);
                }
            }

            _dispatcherTimer.Start();

            return base.Activate();
        }

        public override Task Deactivate()
        {
            _dispatcherTimer.Stop();

            UserImages.Clear();

            return base.Deactivate();
        }

        private void OnTimerTicker(object sender, EventArgs e)
        {
            if (UserImages.Count > 0)
            {
                var index = UserImages.Contains(_currentImage) ? UserImages.IndexOf(_currentImage) : 0;

                if (index == UserImages.Count - 1)
                {
                    index = 0;
                }
                else
                {
                    index += 1;
                }

                _currentImage = UserImages[index];

                OnUserImageIndexUpdate?.Invoke(this, index);
            }
        }

        [RelayCommand]
        private async Task EndSession()
        {
            _dispatcherTimer.Stop();

            UserImages.Clear();

            await NavigationService.GoToAsync(nameof(EndSessionPage));
        }
    }
}
