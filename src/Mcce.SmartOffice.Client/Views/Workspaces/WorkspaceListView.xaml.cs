using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Mcce.SmartOffice.Client.Models;
using Mcce.SmartOffice.Client.ViewModels;

namespace Mcce.SmartOffice.Client.Views
{
    public partial class WorkspaceListView : UserControl
    {
        private static Brush _defaultBrush = Brushes.Transparent;
        private static Brush _selectedBrush = Brushes.Orange;

        private Rectangle _selectedRect;

        public WorkspaceListView()
        {
            InitializeComponent();

            Loaded += OnLoaded;
            DataContextChanged += OnDataContextChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ((IListViewModel)DataContext).Reload();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var dataContext = e.OldValue as INotifyPropertyChanged;
            if (dataContext != null)
            {
                dataContext.PropertyChanged -= OnDataContextPropertyChanged;
            }

            dataContext = e.NewValue as INotifyPropertyChanged;
            if (dataContext != null)
            {
                dataContext.PropertyChanged += OnDataContextPropertyChanged;
            }
        }

        private void OnDataContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var dataContext = DataContext as WorkspaceListViewModel;

            if (e.PropertyName == nameof(WorkspaceListViewModel.SelectedItem))
            {
                var rect = WorkspaceCanvas.Children.OfType<Rectangle>().FirstOrDefault(x => x.Tag == dataContext.SelectedItem);

                if (_selectedRect != null)
                {
                    _selectedRect.Fill = _defaultBrush;
                }

                if (rect != null)
                {
                    _selectedRect = rect;
                    _selectedRect.Fill = _selectedBrush;
                }
            }
            else if (e.PropertyName == nameof(WorkspaceListViewModel.Items))
            {
                WorkspaceCanvas.ResetZoom();

                var rects = WorkspaceCanvas.Children.OfType<Rectangle>().ToList();

                foreach (var rect in rects)
                {
                    WorkspaceCanvas.Children.Remove(rect);
                }

                foreach (var workspace in dataContext.Items)
                {
                    var rect = new Rectangle
                    {
                        Fill = _defaultBrush,
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

        private void OnCanvasMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var dataContext = DataContext as WorkspaceListViewModel;

                if (_selectedRect != null)
                {
                    _selectedRect.Fill = _defaultBrush;
                }

                if (_selectedRect == e.Source)
                {
                    _selectedRect = null;
                    dataContext.SelectedItem = null;

                }
                else
                {
                    _selectedRect = e.Source as Rectangle;
                    if (_selectedRect != null)
                    {
                        _selectedRect.Fill = _selectedBrush;
                        dataContext.SelectedItem = _selectedRect.Tag as WorkspaceModel;
                    }
                }
            }
        }
    }
}
