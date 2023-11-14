using System.Windows;
using System.Windows.Controls;
using Mcce.SmartOffice.Client.ViewModels;

namespace Mcce.SmartOffice.Client.Views
{
    /// <summary>
    /// Interaction logic for UserImageListView.xaml
    /// </summary>
    public partial class UserImageListView : UserControl
    {
        public UserImageListView()
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
