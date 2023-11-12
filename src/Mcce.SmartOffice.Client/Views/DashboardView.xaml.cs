using System.Windows;
using System.Windows.Controls;

namespace Mcce.SmartOffice.Client.Views
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //for (int i = 0; i < 10; i++)
            //{
            //    WrapPanel.Children.Add(new CardView());
            //}
        }
    }
}
