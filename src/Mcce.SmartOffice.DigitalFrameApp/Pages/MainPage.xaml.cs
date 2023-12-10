using Mcce.SmartOffice.DigitalFrameApp.ViewModels;

namespace Mcce.SmartOffice.DigitalFrameApp
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            
            BindingContext = viewModel;
        }

        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            await _viewModel.Activate();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _viewModel.Deactivate();
        }
    }
}
