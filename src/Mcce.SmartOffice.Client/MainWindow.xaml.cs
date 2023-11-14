using System;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using Mcce.SmartOffice.Client.Enums;
using Mcce.SmartOffice.Client.Services;
using Mcce.SmartOffice.Client.ViewModels;

namespace Mcce.SmartOffice.Client
{
    /// <summary>
    /// Interaction logic for MainWindow2.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel mainViewModel, IDialogService dialogService)
        {
            InitializeComponent();

            StateChanged += MainWindowStateChangeRaised;

            dialogService.DialogContainer = ChildWindowContainer;

            DataContext = mainViewModel;
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
                HamburgerMenuControl.HamburgerWidth += 5;
                RestoreButton.Visibility = Visibility.Visible;
                MaximizeButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                ChromeGrid.Height = 30;
                HamburgerMenuControl.HamburgerWidth = 48;
                RestoreButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Visible;
            }
        }

        private async void OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs args)
        {
            var item = args.InvokedItem as HamburgerMenuItem;

            await ((MainViewModel)DataContext).ActivateContent((NavigationType)item.Tag);
        }

        private void OnOptionsItemInvoked(object sender, ItemClickEventArgs args)
        {
            HamburgerMenuControl.SelectedItem = null;
            HamburgerMenuControl.SelectedOptionsItem = null;

            ((MainViewModel)DataContext).Login.Logout();
        }
    }
}
