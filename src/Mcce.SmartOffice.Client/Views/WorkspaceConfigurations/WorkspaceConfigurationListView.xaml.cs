using System.Windows;
using System.Windows.Controls;
using Mcce.SmartOffice.Client.ViewModels;

namespace Mcce.SmartOffice.Client.Views
{
    public partial class WorkspaceConfigurationListView : UserControl
    {
        public WorkspaceConfigurationListView()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ((IListViewModel)DataContext).Reload();
        }
    }
}
