using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce22.SmartOffice.Client.Managers;
using Mcce22.SmartOffice.Client.Models;
using Mcce22.SmartOffice.Client.Services;
using Newtonsoft.Json;

namespace Mcce22.SmartOffice.Client.ViewModels
{
    public partial class SeedDataViewModel : ViewModelBase
    {
        private static readonly Random Random = new Random();

        private readonly IUserManager _userManager;
        private readonly IWorkspaceManager _workspaceManager;
        private readonly IUserImageManager _userImageManager;
        private readonly IBookingManager _bookingManager;
        private readonly IWorkspaceDataManager _workspaceDataManager;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private int _progress;

        [ObservableProperty]
        private int _stepProgress;

        [ObservableProperty]
        private string _progressText;

        [ObservableProperty]
        private bool _activateUserSeed = true;

        [ObservableProperty]
        private bool _activateUserImageSeed = true;

        [ObservableProperty]
        private bool _activateWorkspaceSeed = true;

        [ObservableProperty]
        private bool _activateWorkspaceConfigSeed = true;

        [ObservableProperty]
        private bool _activateWorkspaceDataSeed = false;

        public SeedDataViewModel(
            IUserManager userManager,
            IWorkspaceManager workspaceManager,
            IUserImageManager userImageManager,
            IBookingManager bookingManager,
            IWorkspaceDataManager workspaceDataManager,
            IDialogService dialogService)
        {
            _userManager = userManager;
            _workspaceManager = workspaceManager;
            _userImageManager = userImageManager;
            _bookingManager = bookingManager;
            _workspaceDataManager = workspaceDataManager;
            _dialogService = dialogService;
        }

        protected override void UpdateCommandStates()
        {
            base.UpdateCommandStates();

            SeedDataCommand.NotifyCanExecuteChanged();
        }

        private bool CanSeed()
        {
            return !IsBusy;
        }

        [RelayCommand(CanExecute = nameof(CanSeed))]
        private async void SeedData()
        {
            try
            {
                var confirmDelete = new ConfirmDeleteViewModel("Caution!", $"This operation will erase all existing data. Are you sure you want to continue?", _dialogService);

                await _dialogService.ShowDialog(confirmDelete);

                if (confirmDelete.Confirmed)
                {
                    IsBusy = true;

                    Progress = 0;
                    if (ActivateUserImageSeed)
                    {
                        ProgressText = "Delete user images...";
                        await DeleteUserImages();
                    }

                    Progress = 10;
                    if (ActivateUserSeed)
                    {
                        ProgressText = "Delete users...";
                        await DeleteUsers();
                    }


                    Progress = 20;
                    if (ActivateWorkspaceSeed)
                    {
                        ProgressText = "Delete workspaces...";
                        await DeleteWorkspaces();
                    }

                    Progress = 30;
                    if (ActivateWorkspaceDataSeed)
                    {
                        ProgressText = "Delete workspace data...";
                        await DeleteWorkspaceData();
                    }

                    Progress = 40;
                    if (ActivateUserSeed)
                    {
                        ProgressText = "Seed users...";
                        await SeedUsers();
                    }

                    Progress = 50;
                    if (ActivateUserImageSeed)
                    {
                        ProgressText = "Seed user images...";
                        await SeedUserImages();
                    }

                    Progress = 60;
                    if (ActivateWorkspaceSeed)
                    {
                        ProgressText = "Seed workspaces...";
                        await SeedWorkspaces();
                    }

                    Progress = 70;
                    if (ActivateWorkspaceDataSeed)
                    {
                        ProgressText = "Seed workspace data...";
                        await SeedWorkspaceData();
                    }

                    Progress = 100;
                    ProgressText = "Done";
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SeedUsers()
        {
            var json = File.ReadAllText("seeddata\\users.json");
            var users = JsonConvert.DeserializeObject<UserModel[]>(json);

            StepProgress = 0;
            var count = 0;
            foreach (var user in users)
            {
                await _userManager.Save(user);

                count++;
                StepProgress = count * 100 / users.Length;
            }

            StepProgress = 100;
        }

        private async Task SeedWorkspaces()
        {
            var json = File.ReadAllText("seeddata\\workspaces.json");
            var workspaces = JsonConvert.DeserializeObject<WorkspaceModel[]>(json);

            StepProgress = 0;
            var count = 0;
            foreach (var workspace in workspaces)
            {
                await _workspaceManager.Save(workspace);

                count++;
                StepProgress = count * 100 / workspaces.Length;
            }
            StepProgress = 100;
        }

        private async Task SeedUserImages()
        {
            var users = await _userManager.GetList();
            var user = users.FirstOrDefault();

            var filePaths = Directory.GetFiles("sampleimages");

            StepProgress = 0;
            var count = 0;
            foreach (var filePath in filePaths)
            {
                await _userImageManager.Save(user.Id, filePath);

                count++;
                StepProgress = count * 100 / filePaths.Length;
            }
            StepProgress = 100;
        }

        private async Task SeedWorkspaceData()
        {
            var workspaces = await _workspaceManager.GetList();

            StepProgress = 0;

            var hours = 24;
            var count = 0;
            var maxCount = workspaces.Length * hours;

            var dateTimeNow = DateTime.Now;

            foreach (var workspace in workspaces)
            {
                for (int i = 1; i < hours; i++)
                {
                    var model = new WorkspaceDataModel
                    {
                        WorkspaceId = workspace.Id,
                        Timestamp = new DateTime(dateTimeNow.Year,dateTimeNow.Month,dateTimeNow.Day, i, 0, 0),
                        Temperature = Random.Next(16, 30),
                        Noise = Random.Next(20, 60),
                        Co2 = Random.Next(600, 1000),
                    };
                    await _workspaceDataManager.Save(model);

                    count++;
                    StepProgress = count * 100 / maxCount;
                }
            }

            StepProgress = 100;
        }

        private async Task DeleteUsers()
        {
            var users = await _userManager.GetList();

            StepProgress = 0;
            var count = 0;

            foreach (var user in users)
            {
                await _userManager.Delete(user.Id);

                count++;
                StepProgress = count * 100 / users.Length;
            }

            StepProgress = 100;
        }

        private async Task DeleteWorkspaces()
        {
            var workspaces = await _workspaceManager.GetList();

            StepProgress = 0;
            var count = 0;

            foreach (var workspace in workspaces)
            {
                await _workspaceManager.Delete(workspace.Id);

                count++;
                StepProgress = count * 100 / workspaces.Length;
            }

            StepProgress = 100;
        }

        private async Task DeleteUserImages()
        {
            var users = await _userManager.GetList();

            StepProgress = 0;
            var count = 0;

            foreach (var user in users)
            {
                await _userImageManager.Delete(user.Id);

                count++;
                StepProgress = count * 100 / users.Length;
            }

            StepProgress = 100;
        }

        private async Task DeleteWorkspaceData()
        {
            var workspaceData = await _workspaceDataManager.GetList();

            StepProgress = 0;
            var count = 0;

            foreach (var data in workspaceData)
            {
                await _workspaceDataManager.Delete(data.Id);

                count++;
                StepProgress = count * 100 / workspaceData.Length;
            }

            StepProgress = 100;
        }
    }
}
