using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Mcce.SmartOffice.Client.Enums;
using Mcce.SmartOffice.Client.Services;

namespace Mcce.SmartOffice.Client.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private bool _navigating = false;


        [ObservableProperty]
        private int _selectedIndex = -1;

        [ObservableProperty]
        private IViewModel _activeContent;

        public LoginViewModel Login { get; }
        public DashboardViewModel Dashboard { get; }
        public WorkspaceConfigurationListViewModel WorkspaceConfigurationList { get; }
        public WorkspaceListViewModel WorkspaceList { get; }
        public BookingListViewModel BookingList { get; }
        public UserImageListViewModel UserImageList { get; }
        public SeedDataViewModel SeedData { get; }
        public CreateBookingViewModel CreateBooking { get; }
        public WorkspaceDataListViewModel WorkspaceDataList { get; }

        public MainViewModel(
            INavigationService navigationService,
            LoginViewModel login,
            DashboardViewModel dashboard,
            WorkspaceConfigurationListViewModel workspaceConfigurationList,
            WorkspaceListViewModel workspaceList,
            BookingListViewModel bookingList,
            UserImageListViewModel userImageList,
            SeedDataViewModel seedData,
            CreateBookingViewModel createBooking,
            WorkspaceDataListViewModel workspaceDataList)
        {
            Login = login;
            Dashboard = dashboard;
            WorkspaceConfigurationList = workspaceConfigurationList;
            WorkspaceList = workspaceList;
            BookingList = bookingList;
            UserImageList = userImageList;
            SeedData = seedData;
            CreateBooking = createBooking;
            WorkspaceDataList = workspaceDataList;
            Login.LoginChanged += OnLoginChanged;

            navigationService.NavigationRequested += OnNavigationRequested;
        }

        private void OnLoginChanged(object sender, EventArgs e)
        {
            Dashboard.IsAdmin = Login.IsAdmin;
            BookingList.IsAdmin = Login.IsAdmin;

            SelectedIndex = 0;
        }

        public async Task ActivateContent(NavigationType type)
        {
            try
            {
                if(_navigating)
                {
                    return;
                }

                _navigating = true;

                ActiveContent = null;

                switch (type)
                {
                    case NavigationType.Dashboard:
                        SelectedIndex = 0;
                        ActiveContent = Dashboard;
                        break;
                    case NavigationType.CreateBooking:
                        SelectedIndex = 1;
                        ActiveContent = CreateBooking;
                        break;
                    case NavigationType.MyBookings:
                        SelectedIndex = 2;
                        BookingList.OnlyMyBookings = true;
                        ActiveContent = BookingList;
                        break;
                    case NavigationType.UserConfigs:
                        SelectedIndex = 3;
                        ActiveContent = WorkspaceConfigurationList;
                        break;
                    case NavigationType.UserImages:
                        SelectedIndex = 4;
                        ActiveContent = UserImageList;
                        break;
                    case NavigationType.Workspaces:
                        SelectedIndex = 6;
                        ActiveContent = WorkspaceList;
                        break;
                    case NavigationType.Bookings:
                        SelectedIndex = 7;
                        BookingList.OnlyMyBookings = false;
                        ActiveContent = BookingList;
                        break;
                    case NavigationType.WorkspaceData:
                        SelectedIndex = 8;
                        ActiveContent = WorkspaceDataList;
                        break;
                    case NavigationType.SeedData:
                        SelectedIndex = 9;
                        ActiveContent = SeedData;
                        break;
                }

                await ActiveContent?.Activate();
            }
            finally
            {
                _navigating = false;
            }
        }

        private async void OnNavigationRequested(object sender, NavigationRequestedArgs e)
        {
            await ActivateContent(e.Type);
        }

    }
}
