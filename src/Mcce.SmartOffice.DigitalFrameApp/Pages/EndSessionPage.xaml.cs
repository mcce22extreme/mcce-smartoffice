using Mcce.SmartOffice.DigitalFrameApp.ViewModels;

namespace Mcce.SmartOffice.DigitalFrameApp.Pages;

public partial class EndSessionPage : ContentPage
{
    private readonly EndSessionViewModel _viewModel;

    public EndSessionPage(EndSessionViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        BindingContext = _viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        await _viewModel.Activate();
    }
}
