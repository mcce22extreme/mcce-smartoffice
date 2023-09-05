using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Mcce22.SmartOffice.Client.Enums;
using Mcce22.SmartOffice.Client.Services;

namespace Mcce22.SmartOffice.Client.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _selectedIndex = -1;
        
        public LoginViewModel Login { get; }
        public DashboardViewModel Dashboard { get; }
        public WorkspaceConfigurationListViewModel UserWorkspaceList { get; }
        public UserListViewModel UserList { get; }        
        public WorkspaceListViewModel WorkspaceList { get; }
        public BookingListViewModel BookingList { get; }
        public UserImageListViewModel SlideshowItemList { get; }
        public SeedDataViewModel SeedData { get; }
        public CreateBookingViewModel CreateBooking { get; }
        public WorkspaceDataListViewModel WorkspaceDataList { get; }
        public ConfigViewModel Config { get; }

        public MainViewModel(
            INavigationService navigationService,
            LoginViewModel login,
            DashboardViewModel dashboard,
            WorkspaceConfigurationListViewModel userWorkspaceList,
            UserListViewModel userList,            
            WorkspaceListViewModel workspaceList,
            BookingListViewModel bookingList,
            UserImageListViewModel slideshowItemList,
            SeedDataViewModel seedData,
            CreateBookingViewModel createBooking,
            WorkspaceDataListViewModel workspaceDataList,
            ConfigViewModel config)
        {
            Login = login;
            Dashboard = dashboard;
            UserWorkspaceList = userWorkspaceList;
            UserList = userList;            
            WorkspaceList = workspaceList;
            BookingList = bookingList;
            SlideshowItemList = slideshowItemList;
            SeedData = seedData;
            CreateBooking = createBooking;
            WorkspaceDataList = workspaceDataList;
            Config = config;
            Login.LoginChanged += OnLoginChanged;

            navigationService.NavigationRequested += OnNavigationRequested;
        }

        private void OnLoginChanged(object sender, EventArgs e)
        {
            Dashboard.IsAdmin = Login.IsAdmin;
            BookingList.IsAdmin = Login.IsAdmin;

            SelectedIndex = 0;
        }

        private void OnNavigationRequested(object sender, NavigationRequestedArgs e)
        {
            switch (e.Type)
            {
                case NavigationType.Dashboard:
                    SelectedIndex = 0;
                    break;
                case NavigationType.CreateBooking:
                    SelectedIndex = 1;
                    break;
                case NavigationType.UserConfigs:
                    SelectedIndex = 2;
                    break;
                case NavigationType.SlideshowItems:
                    SelectedIndex = 3;
                    break;
                case NavigationType.Workspaces:
                    SelectedIndex = 5;
                    break;
                case NavigationType.Bookings:
                    SelectedIndex = 6;
                    break;
                case NavigationType.Users:
                    SelectedIndex = 7;
                    break;
                case NavigationType.WorkspaceData:
                    SelectedIndex = 8;
                    break;
                case NavigationType.SeedData:
                    SelectedIndex = 9;
                    break;
                case NavigationType.Config:
                    SelectedIndex = 10;
                    break;
            }
        }

    }
}
