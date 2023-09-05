using System.Windows;
using System.Windows.Controls;
using Mcce22.SmartOffice.Client.ViewModels;

namespace Mcce22.SmartOffice.Client.Views
{
    /// <summary>
    /// Interaction logic for ConfigView.xaml
    /// </summary>
    public partial class ConfigView : UserControl
    {
        public ConfigView()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await ((ConfigViewModel)DataContext).Load();
        }
    }
}
