using Mcce.SmartOffice.DigitalFrameApp.ViewModels;

namespace Mcce.SmartOffice.DigitalFrameApp.Pages;

public partial class PrepareSessionPage : ContentPage
{
    private readonly PrepareSessionViewModel _viewModel;

    public PrepareSessionPage(PrepareSessionViewModel viewModel)
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
