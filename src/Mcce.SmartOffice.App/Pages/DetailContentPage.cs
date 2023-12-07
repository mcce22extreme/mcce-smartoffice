using Mcce.SmartOffice.App.ViewModels;
using Mcce.SmartOffice.Client.Common;

namespace Mcce.SmartOffice.App.Pages
{
    public abstract class DetailContentPage : ContentPage
    {
        private readonly IDetailViewModel _viewModel;

        public DetailContentPage(IDetailViewModel viewModel)
        {
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            await _viewModel.Activate();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Shell.Current.Navigating += OnNavigating;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            Shell.Current.Navigating -= OnNavigating;
        }

        private async void OnNavigating(object sender, ShellNavigatingEventArgs e)
        {
            var deferral = e.GetDeferral();

            if (await _viewModel.CanGoBack())
            {
                deferral.Complete();

                PlatformHelpers.HideKeyboard();
            }
            else
            {
                e.Cancel();
            }
        }
    }
}
