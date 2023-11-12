using System;
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Mcce.SmartOffice.Client.Models;
using Mcce.SmartOffice.Client.ViewModels;

namespace Mcce.SmartOffice.Client.Views
{
    /// <summary>
    /// Interaction logic for CreateBookingView.xaml
    /// </summary>
    public partial class CreateBookingView : UserControl
    {
        private static Brush _defaultBrush = Brushes.Transparent;
        private static Brush _selectedBrush = Brushes.Orange;
        private static Brush _availableBrush = Brushes.LightGreen;
        private static Brush _notAvailableBrush = Brushes.DarkRed;

        private Rectangle _selectedRect;

        public CreateBookingView()
        {
            InitializeComponent();

            Loaded += OnLoaded;
            DataContextChanged += OnDataContextChanged;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await ((CreateBookingViewModel)DataContext).Load();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var ctx = e.OldValue as CreateBookingViewModel;

            if (ctx != null)
            {
                ctx.WorkspaceAvailabilityUpdated -= OnWorkspaceAvailabilityUpdated;
                ctx.PropertyChanged -= OnDataContextPropertyChanged;
            }

            ctx = e.NewValue as CreateBookingViewModel;

            if (ctx != null)
            {
                ctx.WorkspaceAvailabilityUpdated += OnWorkspaceAvailabilityUpdated;
                ctx.PropertyChanged += OnDataContextPropertyChanged;
            }
        }

        private void OnDataContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var ctx = sender as CreateBookingViewModel;

            if (e.PropertyName == nameof(CreateBookingViewModel.SelectedWorkspace))
            {
                if (_selectedRect != null)
                {
                    var workspace = _selectedRect.Tag as WorkspaceModel;

                    if (workspace.IsAvailable.HasValue)
                    {
                        _selectedRect.Fill = workspace.IsAvailable == true ? _availableBrush : _notAvailableBrush;
                        
                    }
                    else
                    {
                        _selectedRect.Fill = _defaultBrush;
                    }

                    _selectedRect = null;
                }

                var rect = WorkspaceCanvas.Children.OfType<Rectangle>().FirstOrDefault(x => x.Tag == ctx.SelectedWorkspace);

                if (rect != null)
                {
                    _selectedRect = rect;
                    _selectedRect.Fill = _selectedBrush;
                }
            }
            else if (e.PropertyName == nameof(CreateBookingViewModel.Workspaces))
            {
                WorkspaceCanvas.ResetZoom();

                var rects = WorkspaceCanvas.Children.OfType<Rectangle>().ToList();

                foreach (var rect in rects)
                {
                    WorkspaceCanvas.Children.Remove(rect);
                }

                foreach (var workspace in ctx.Workspaces)
                {
                    var rect = new Rectangle
                    {
                        Fill = Brushes.Transparent,
                        Opacity= 0.5,
                        Width = workspace.Width,
                        Height = workspace.Height,
                        Tag = workspace
                    };

                    WorkspaceCanvas.Children.Add(rect);

                    Canvas.SetTop(rect, workspace.Top);
                    Canvas.SetLeft(rect, workspace.Left);
                }
            }
        }

        private void OnWorkspaceAvailabilityUpdated(object sender, EventArgs e)
        {
            var ctx = sender as CreateBookingViewModel;

            foreach (var rect in WorkspaceCanvas.Children.OfType<Rectangle>())
            {
                var workspace = rect.Tag as WorkspaceModel;
                rect.Fill = workspace.IsAvailable == true ? _availableBrush : _notAvailableBrush;
            }
        }

        private void OnCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var dataContext = DataContext as CreateBookingViewModel;

                if (_selectedRect != null)
                {
                    var workspace = _selectedRect.Tag as WorkspaceModel;

                    if (workspace.IsAvailable.HasValue)
                    {
                        _selectedRect.Fill = workspace.IsAvailable == true ? _availableBrush : _notAvailableBrush;
                    }
                    else
                    {
                        _selectedRect.Fill = _defaultBrush;
                    }
                }

                if (_selectedRect == e.Source)
                {
                    _selectedRect = null;
                    dataContext.SelectedWorkspace = null;

                }
                else
                {
                    _selectedRect = e.Source as Rectangle;
                    if (_selectedRect != null)
                    {
                        _selectedRect.Fill = _selectedBrush;
                        dataContext.SelectedWorkspace = _selectedRect.Tag as WorkspaceModel;
                    }else
                    {
                        dataContext.SelectedWorkspace = null;
                    }
                }
            }
        }
    }
}
