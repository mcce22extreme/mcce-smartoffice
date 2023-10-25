using CommunityToolkit.Mvvm.ComponentModel;
using Mcce22.SmartOffice.MobileApp.Managers;

namespace Mcce22.SmartOffice.MobileApp.ViewModels
{
    public partial class AccountViewModel : ObservableObject
    {
        private readonly IAccountManager _accountManager;

        [ObservableProperty]
        private string _firstName;

        [ObservableProperty]
        private string _lastName;

        [ObservableProperty]
        private string _userName;

        public AccountViewModel(IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        public async Task Load()
        {
            var userInfo = await _accountManager.GetUserInfo();

            FirstName = userInfo.FirstName;
            LastName = userInfo.LastName;
            UserName = userInfo.UserName;
        }
    }
}
