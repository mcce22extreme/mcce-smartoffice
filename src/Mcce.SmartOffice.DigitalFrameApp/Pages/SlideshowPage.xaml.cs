using Mcce.SmartOffice.DigitalFrameApp.ViewModels;

namespace Mcce.SmartOffice.DigitalFrameApp.Pages;

public partial class SlideshowPage : ContentPage
{
    private readonly SlideshowViewModel _viewModel;

    public SlideshowPage(SlideshowViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        BindingContext = _viewModel;

        _viewModel.OnUserImageIndexUpdate += OnUserImageIndexUpdated;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        await _viewModel.Activate();
    }

    private void OnUserImageIndexUpdated(object sender, int index)
    {
        UserImageCarousel.ScrollTo(index);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _viewModel.Deactivate();
    }
}
