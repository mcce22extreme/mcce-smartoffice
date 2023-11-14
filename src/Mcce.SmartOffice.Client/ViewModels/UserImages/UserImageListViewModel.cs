using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Mcce.SmartOffice.Client.Managers;
using Mcce.SmartOffice.Client.Models;
using Mcce.SmartOffice.Client.Services;

namespace Mcce.SmartOffice.Client.ViewModels
{
    public partial class UserImageListViewModel : ListViewModelBase<UserImageModel>
    {
        private readonly IUserImageManager _userImageManager;
                
        public UserImageListViewModel(IUserImageManager userImageManager, IDialogService dialogService)
            : base(dialogService)
        {
            _userImageManager = userImageManager;
        }

        protected override async Task OnAdd()
        {
            await DialogService.ShowDialog(new UserImageDetailViewModel(_userImageManager, DialogService));
        }

        protected override async Task OnDelete()
        {
            await _userImageManager.Delete(Path.GetFileName(SelectedItem.Url));
        }

        protected override async Task<UserImageModel[]> OnReload()
        {
            var userImages = new List<UserImageModel>();

            userImages.AddRange(await _userImageManager.GetList());

            return userImages.ToArray();
        }
    }
}
