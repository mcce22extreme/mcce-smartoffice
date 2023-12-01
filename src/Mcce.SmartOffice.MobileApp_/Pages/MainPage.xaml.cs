using MauiIcons.Core;
using Mcce.SmartOffice.MobileApp.ViewModels;

namespace Mcce.SmartOffice.MobileApp.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();

            _ = new MauiIcon();

            BindingContext = viewModel;
        }

        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            await ((MainViewModel)BindingContext).LoadAccountInfo();
        }
    }
}
