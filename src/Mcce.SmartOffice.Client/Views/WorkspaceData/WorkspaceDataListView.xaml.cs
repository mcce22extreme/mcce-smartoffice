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
    /// <summary>
    /// Interaction logic for WorkspaceDataListView.xaml
    /// </summary>
    public partial class WorkspaceDataListView : UserControl
    {
        private static Brush _defaultBrush = Brushes.Transparent;
        private static Brush _selectedBrush = Brushes.Orange;

        private Rectangle _selectedRect;

        public WorkspaceDataListView()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await ((WorkspaceDataListViewModel)DataContext).Load();
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
            var dataContext = DataContext as WorkspaceDataListViewModel;

            if (e.PropertyName == nameof(WorkspaceDataListViewModel.SelectedWorkspace))
            {
                var rect = WorkspaceCanvas.Children.OfType<Rectangle>().FirstOrDefault(x => x.Tag == dataContext.SelectedWorkspace);

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
            else if (e.PropertyName == nameof(WorkspaceDataListViewModel.Workspaces))
            {
                WorkspaceCanvas.ResetZoom();

                var rects = WorkspaceCanvas.Children.OfType<Rectangle>().ToList();

                foreach (var rect in rects)
                {
                    WorkspaceCanvas.Children.Remove(rect);
                }

                foreach (var workspace in dataContext.Workspaces)
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
                var dataContext = DataContext as WorkspaceDataListViewModel;

                if (_selectedRect != null)
                {
                    _selectedRect.Fill = _defaultBrush;
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
                    }
                }
            }
        }
    }
}
