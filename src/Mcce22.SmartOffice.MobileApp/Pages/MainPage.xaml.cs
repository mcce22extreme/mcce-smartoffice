using Mcce22.SmartOffice.MobileApp.ViewModels;

namespace Mcce22.SmartOffice.MobileApp.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }
    }
}
