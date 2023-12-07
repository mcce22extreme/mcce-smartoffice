using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Mcce.SmartOffice.Simulator.ViewModels;

namespace Mcce.SmartOffice.Simulator
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();

            StateChanged += MainWindowStateChangeRaised;

            DataContext = viewModel;

            viewModel.PropertyChanged += OnPropertyChanged;
        }

        // Can execute
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        // Minimize
        private void CommandBinding_Executed_Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        // Maximize
        private void CommandBinding_Executed_Maximize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        // Restore
        private void CommandBinding_Executed_Restore(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        // Close
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        // State change
        private void MainWindowStateChangeRaised(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                ChromeGrid.Height += 8;
                RestoreButton.Visibility = Visibility.Visible;
                MaximizeButton.Visibility = Visibility.Collapsed;
                MessageGrid.Margin = new Thickness(10, 0, 0, 0);
            }
            else
            {
                ChromeGrid.Height = 30;
                RestoreButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Visible;
                MessageGrid.Margin = new Thickness(0);
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.DeskCanvasTop))
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    var viewModel = sender as MainViewModel;

                    var deskAnimation = new DoubleAnimation(viewModel.DeskCanvasTop, TimeSpan.FromSeconds(1));
                    Desk.BeginAnimation(Canvas.TopProperty, deskAnimation);

                    var legCanvasAnimation = new DoubleAnimation(viewModel.LegCanvasTop, TimeSpan.FromSeconds(1));
                    LeftLeg.BeginAnimation(Canvas.TopProperty, legCanvasAnimation);
                    RightLeg.BeginAnimation(Canvas.TopProperty, legCanvasAnimation);

                    var legHeightAnimation = new DoubleAnimation(viewModel.LegHeight, TimeSpan.FromSeconds(1));
                    LeftLeg.BeginAnimation(HeightProperty, legHeightAnimation);
                    RightLeg.BeginAnimation(HeightProperty, legHeightAnimation);

                    var imageFrameAnimation = new DoubleAnimation(viewModel.ImageFrameCanvasTop, TimeSpan.FromSeconds(1));
                    ImageFrame.BeginAnimation(Canvas.TopProperty, imageFrameAnimation);

                    //var imageAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1));
                    //ImageFrame.BeginAnimation(OpacityProperty, imageAnimation);

                    var wifiAnimation = new DoubleAnimation(viewModel.WifiCanvasTop, TimeSpan.FromSeconds(1));
                    WifiSign.BeginAnimation(Canvas.TopProperty, wifiAnimation);
                }));
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox)sender).ScrollToEnd();
        }
    }
}
