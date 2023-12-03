using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.MobileApp.Managers;
using Mcce.SmartOffice.MobileApp.Models;

namespace Mcce.SmartOffice.MobileApp.ViewModels
{
    public partial class UserImagesViewModel : ObservableObject
    {
        public string Title
        {
            get
            {
                var title = "My Images";

                if(UserImages?.Count > 0)
                {
                    title += $"  ({UserImages?.Count})";
                }

                return title;
            }
        }

        private readonly IUserImageManager _userImageManager;
        [ObservableProperty]
        private List<UserImageModel> _userImages;

        [ObservableProperty]
        private UserImageModel _selectedUserImage;

        [ObservableProperty]
        private bool _isBusy;

        public UserImagesViewModel(IUserImageManager userImageManager)
        {
            _userImageManager = userImageManager;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            LoadUserImagesCommand.NotifyCanExecuteChanged();
            DeleteUserImageCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanLoadUserImages))]
        public async Task LoadUserImages()
        {
            if (CanLoadUserImages())
            {
                IsBusy = true;

                try
                {
                    SelectedUserImage = null;
                    var userImages = await _userImageManager.GetUserImages();

                    UserImages = new List<UserImageModel>(userImages);
                    OnPropertyChanged(nameof(Title));
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        public bool CanLoadUserImages()
        {
            return !IsBusy;
        }

        [RelayCommand(CanExecute = nameof(CanAddUserImage))]
        public async Task AddUserImage()
        {
            if (CanAddUserImage())
            {
                var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Add user image"
                });

                if (result != null)
                {
                    try
                    {
                        using var stream = await result.OpenReadAsync();

                        IsBusy = true;

                        await _userImageManager.AddUserImage(stream, result.FileName, result.ContentType);
                    }
                    finally
                    {
                        IsBusy = false;
                    }
                    await LoadUserImages();
                }
            }
        }

        public bool CanAddUserImage()
        {
            return !IsBusy && MediaPicker.Default.IsCaptureSupported;
        }

        [RelayCommand(CanExecute = nameof(CanTakePhoto))]
        public async Task TakePhoto()
        {
            if (CanTakePhoto())
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    var photo = await MediaPicker.Default.CapturePhotoAsync();

                    if (photo != null)
                    {
                        try
                        {
                            using var stream = await photo.OpenReadAsync();

                            IsBusy = true;

                            await _userImageManager.AddUserImage(stream, photo.FileName, photo.ContentType);
                        }
                        finally
                        {
                            IsBusy = false;
                        }

                        await LoadUserImages();
                    }
                }
            }
        }

        public bool CanTakePhoto()
        {
            return !IsBusy && MediaPicker.Default.IsCaptureSupported;
        }

        [RelayCommand(CanExecute = nameof(CanDeleteUserImage))]
        public async Task DeleteUserImage()
        {
            if (CanDeleteUserImage())
            {
                var result = await Application.Current.MainPage.DisplayAlert("Delete Image?", "Do you really want to delete the current image?", "Yes", "No");

                if (result)
                {
                    try
                    {
                        IsBusy = true;

                        await _userImageManager.DeleteUserImage(SelectedUserImage.ImageKey);

                        UserImages.Remove(SelectedUserImage);
                        SelectedUserImage = null;
                        OnPropertyChanged(nameof(Title));
                    }
                    finally
                    {
                        IsBusy = false;
                    }

                    await LoadUserImages();
                }
            }
        }

        public bool CanDeleteUserImage()
        {
            return !IsBusy && SelectedUserImage != null;
        }
    }
}
