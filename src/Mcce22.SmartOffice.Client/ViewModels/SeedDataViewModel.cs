using System;
using System.IO;
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

        private readonly IWorkspaceManager _workspaceManager;
        private readonly IUserImageManager _userImageManager;
        private readonly IBookingManager _bookingManager;
        private readonly IWorkspaceDataEntryManager _workspaceDataManager;
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
            IWorkspaceManager workspaceManager,
            IUserImageManager userImageManager,
            IBookingManager bookingManager,
            IWorkspaceDataEntryManager workspaceDataManager,
            IDialogService dialogService)
        {
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
        private async Task SeedData()
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

                    Progress = 20;
                    if (ActivateWorkspaceDataSeed)
                    {
                        ProgressText = "Delete workspace data...";
                        await DeleteWorkspaceData();
                    }

                    Progress = 30;
                    if (ActivateWorkspaceSeed)
                    {
                        ProgressText = "Delete workspaces...";
                        await DeleteWorkspaces();
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
            catch (Exception ex)
            {
                await _dialogService.ShowDialog(new ErrorViewModel(ex, _dialogService));
            }
            finally
            {
                IsBusy = false;
            }
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
            var filePaths = Directory.GetFiles("sampleimages");

            StepProgress = 0;
            var count = 0;
            foreach (var filePath in filePaths)
            {
                await _userImageManager.Save(filePath);

                count++;
                StepProgress = count * 100 / filePaths.Length;
            }
            StepProgress = 100;
        }

        private async Task SeedWorkspaceData()
        {
            var workspaces = await _workspaceManager.GetList();

            StepProgress = 0;

            // Create simulated workspace data entries between the given start- and enddate in a 15 minute intervall
            var startDate = DateTime.Now.Subtract(TimeSpan.FromDays(1));
            var endDate = DateTime.Now;
            var timeDifference = endDate - startDate;
            var maxCount = (int)(timeDifference.TotalMinutes/15) * workspaces.Length;
            var count = 0;

            foreach (var workspace in workspaces)
            {
                var date = startDate;

                while (date < endDate)
                {
                    date = date.AddMinutes(15);

                    var model = new WorkspaceDataModel
                    {
                        WorkspaceNumber = workspace.WorkspaceNumber,
                        Timestamp = new DateTime(date.Year,date.Month,date.Day,date.Hour, date.Minute, 0),
                        Temperature = Random.Next(16, 30),
                        Humidity = Random.Next(10, 70),
                        Co2Level = Random.Next(600, 1000),
                    };

                    await _workspaceDataManager.Save(model);

                    count++;
                    ProgressText = $"Seed workspace data ({count}/{maxCount})...";
                    StepProgress = count * 100 / maxCount;
                }                
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
                await _workspaceManager.Delete(workspace.WorkspaceNumber);

                count++;
                StepProgress = count * 100 / workspaces.Length;
            }

            StepProgress = 100;
        }

        private async Task DeleteUserImages()
        {
            StepProgress = 0;
            var count = 0;

            var userImages = await _userImageManager.GetList();

            foreach (var userImage in userImages)
            {
                await _userImageManager.Delete(Path.GetFileName(userImage.Url));
                count++;
                StepProgress = count * 100 / userImages.Length;
            }

            StepProgress = 100;
        }

        private async Task DeleteWorkspaceData()
        {
            StepProgress = 0;
            var count = 0;

            var workspaces = await _workspaceManager.GetList();

            foreach (var workspace in workspaces)
            {
                await _workspaceDataManager.Delete(workspace.WorkspaceNumber);
                count++;
                StepProgress = count * 100 / workspaces.Length;
            }

            StepProgress = 100;
        }
    }
}
