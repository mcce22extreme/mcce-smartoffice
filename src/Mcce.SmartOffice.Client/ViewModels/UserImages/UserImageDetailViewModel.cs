using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.Client.Managers;
using Mcce.SmartOffice.Client.Services;
using Microsoft.Win32;

namespace Mcce.SmartOffice.Client.ViewModels
{
    public partial class UserImageDetailViewModel : DialogViewModelBase
    {
        private readonly IUserImageManager _userImageManager;

        [ObservableProperty]
        private string _filePath;

        [ObservableProperty]
        private bool _hasFile;

        public UserImageDetailViewModel(IUserImageManager userImageManager, IDialogService dialogService)
            : base(dialogService)
        {
            Title = "Add user image";

            _userImageManager = userImageManager;
        }

        [RelayCommand]
        private void SelectFile()
        {
            var dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                FilePath = dialog.FileName;
                HasFile = !string.IsNullOrEmpty(FilePath);
            }
        }

        protected override async Task OnSave()
        {
            await _userImageManager.Save(FilePath);
        }
    }
}
