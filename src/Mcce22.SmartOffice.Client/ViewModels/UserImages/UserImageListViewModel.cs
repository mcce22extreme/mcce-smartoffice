using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Mcce22.SmartOffice.Client.Managers;
using Mcce22.SmartOffice.Client.Models;
using Mcce22.SmartOffice.Client.Services;

namespace Mcce22.SmartOffice.Client.ViewModels
{
    public partial class UserImageListViewModel : ListViewModelBase<UserImageModel>
    {
        private readonly IUserImageManager _userImageManager;
        private readonly IUserManager _userManager;

        [ObservableProperty]
        private List<UserModel> _users;

        private UserModel _selectedUser;
        public UserModel SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (SetProperty(ref _selectedUser, value))
                {
                    UpdateUserImages();
                }
            }
        }

        public UserImageListViewModel(IUserImageManager userImageManager, IUserManager userManager, IDialogService dialogService)
            : base(dialogService)
        {
            _userImageManager = userImageManager;
            _userManager = userManager;
        }

        protected override async Task OnAdd()
        {
            await DialogService.ShowDialog(new UserImageDetailViewModel(_userImageManager, _userManager, DialogService));
        }

        protected override async Task OnDelete()
        {
            await _userImageManager.Delete(SelectedItem.Id);
        }

        protected override async Task<UserImageModel[]> OnReload()
        {
            var selectedUser = SelectedUser;

            var users = await _userManager.GetList();           

            Users = new List<UserModel>(users);

            if(selectedUser != null)
            {
                SelectedUser = Users.FirstOrDefault(x => x.Id == selectedUser.Id) ?? Users.FirstOrDefault();
            }
            else
            {
                SelectedUser = Users.FirstOrDefault();
            }

            var userImages = new List<UserImageModel>();

            if (SelectedUser != null)
            {
                userImages.AddRange(await _userImageManager.GetList(SelectedUser.Id));
            }

            return userImages.ToArray();
        }

        private async void UpdateUserImages()
        {
            if (SelectedUser != null)
            {
                var userImages = await _userImageManager.GetList(SelectedUser.Id);
                Items = new ObservableCollection<UserImageModel>(userImages);
            }
            else
            {
                Items = new ObservableCollection<UserImageModel>();
            }
        }
    }
}
