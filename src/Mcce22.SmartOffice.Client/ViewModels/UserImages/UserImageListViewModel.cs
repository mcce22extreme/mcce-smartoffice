﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Mcce22.SmartOffice.Client.Managers;
using Mcce22.SmartOffice.Client.Models;
using Mcce22.SmartOffice.Client.Services;

namespace Mcce22.SmartOffice.Client.ViewModels
{
    public class UserImageListViewModel : ListViewModelBase<UserImageModel>
    {
        private readonly IUserImageManager _userImageManager;
        private readonly IUserManager _userManager;

        private List<UserModel> _users;
        public List<UserModel> Users
        {
            get { return _users; }
            set { SetProperty(ref _users, value); }
        }

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
            var users = await _userManager.GetList();

            Users = new List<UserModel>(users);
            SelectedUser = Users.FirstOrDefault();

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
