using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Mcce.SmartOffice.AdminApp.Managers;
using Mcce.SmartOffice.AdminApp.Models;
using Mcce.SmartOffice.App.Services;
using Mcce.SmartOffice.App.ViewModels;
using SkiaSharp;

namespace Mcce.SmartOffice.AdminApp.ViewModels
{
    public partial class WorkspaceDataViewModel : ViewModelBase
    {
        private static readonly SKColor s_blue = new(25, 118, 210);
        private static readonly SKColor s_red = new(229, 57, 53);
        private static readonly SKColor s_yellow = new(198, 167, 0);
        private static readonly SKColor s_violet = new(238, 130, 238);

        private readonly ObservableCollection<ObservableValue> _weiValues = new ObservableCollection<ObservableValue>();
        private readonly ObservableCollection<ObservableValue> _temperatureValues = new ObservableCollection<ObservableValue>();
        private readonly ObservableCollection<ObservableValue> _humidityValues = new ObservableCollection<ObservableValue>();
        private readonly ObservableCollection<ObservableValue> _co2LevelValues = new ObservableCollection<ObservableValue>();
        private readonly IWorkspaceManager _workspaceManager;
        private readonly IWorkspaceDataManager _workspaceDataManager;
        private readonly IDispatcherTimer _updateTimer;

        [ObservableProperty]
        private ObservableCollection<WorkspaceModel> _workspaces;

        [ObservableProperty]
        private WorkspaceModel _selectedWorkspace;        

        [ObservableProperty]
        private List<WorkspaceDataModel> _workspaceDataEntries = new List<WorkspaceDataModel>();

        [ObservableProperty]
        private ISeries[] _weiSeries;

        [ObservableProperty]
        private ISeries[] _temperatureSeries;

        [ObservableProperty]
        private ISeries[] _humiditySeries;

        [ObservableProperty]
        private ISeries[] _co2LevelSeries;

        [ObservableProperty]
        private bool _weiVisible = true;

        [ObservableProperty]
        private bool _temperatureVisible = false;

        [ObservableProperty]
        private bool _humidityVisible = false;

        [ObservableProperty]
        private bool _co2Visible = false;

        [ObservableProperty]
        private DateTime _startDate = DateTime.Now;

        [ObservableProperty]
        private TimeSpan _startTime = DateTime.Now.TimeOfDay.Subtract(TimeSpan.FromMinutes(10));

        [ObservableProperty]
        private DateTime _endDate = DateTime.Now;

        [ObservableProperty]
        private TimeSpan _endTime = DateTime.Now.TimeOfDay;

        public ICartesianAxis[] WeiYAxes { get; set; } =
        {
            new Axis
            {
                Name = "WEI",
                NameTextSize = 14,
                NamePaint = new SolidColorPaint(s_violet),
                NamePadding = new LiveChartsCore.Drawing.Padding(0, 5),
                Padding =  new LiveChartsCore.Drawing.Padding(0, 0, 20, 0),
                TextSize = 12,
                LabelsPaint = new SolidColorPaint(s_violet),
                TicksPaint = new SolidColorPaint(s_violet),
                SubticksPaint = new SolidColorPaint(s_violet),
                DrawTicksPath = true,
                MinLimit = 0,
                MaxLimit= 100
            }
        };

        public ICartesianAxis[] TemperatureYAxes { get; set; } =
        {
            new Axis
            {
                Name = "Temperature",
                NameTextSize = 14,
                NamePaint = new SolidColorPaint(s_red),
                NamePadding = new LiveChartsCore.Drawing.Padding(0, 5),
                Padding =  new LiveChartsCore.Drawing.Padding(0, 0, 20, 0),
                TextSize = 12,
                LabelsPaint = new SolidColorPaint(s_red),
                TicksPaint = new SolidColorPaint(s_red),
                SubticksPaint = new SolidColorPaint(s_red),
                DrawTicksPath = true,
                MinLimit = -10.0,
                MaxLimit = 40.0
            }
        };

        public ICartesianAxis[] HumidityYAxes { get; set; } =
        {
            new Axis
            {
                Name = "Humidity",
                NameTextSize = 14,
                NamePaint = new SolidColorPaint(s_blue),
                NamePadding = new LiveChartsCore.Drawing.Padding(0, 5),
                Padding =  new LiveChartsCore.Drawing.Padding(0, 0, 20, 0),
                TextSize = 12,
                LabelsPaint = new SolidColorPaint(s_blue),
                TicksPaint = new SolidColorPaint(s_blue),
                SubticksPaint = new SolidColorPaint(s_blue),
                DrawTicksPath = true,
                MinLimit = 0,
                MaxLimit = 100
            }
        };

        public ICartesianAxis[] Co2LevelYAxes { get; set; } =
        {
            new Axis
            {
                Name = "CO2 Level",
                NameTextSize = 14,
                NamePaint = new SolidColorPaint(s_yellow),
                NamePadding = new LiveChartsCore.Drawing.Padding(0, 5),
                Padding =  new LiveChartsCore.Drawing.Padding(0, 0, 20, 0),
                TextSize = 12,
                LabelsPaint = new SolidColorPaint(s_yellow),
                TicksPaint = new SolidColorPaint(s_yellow),
                SubticksPaint = new SolidColorPaint(s_yellow),
                DrawTicksPath = true,
                MinLimit = 500,
                MaxLimit = 1000
            }
        };

        public SolidColorPaint LedgendBackgroundPaint { get; set; } = new SolidColorPaint(new SKColor(240, 240, 240));

        public WorkspaceDataViewModel(
            IWorkspaceManager workspaceManager,
            IWorkspaceDataManager workspaceDataManager,
            IDispatcherTimer dispatcherTimer,
            INavigationService navigationService,
            IDialogService dialogService)
            : base(navigationService, dialogService)
        {
            _workspaceManager = workspaceManager;
            _workspaceDataManager = workspaceDataManager;

            _weiSeries = new[]
            {
                new LineSeries<ObservableValue>
                {
                    LineSmoothness = 0,
                    Name = "WEI",
                    Values = _weiValues,
                    Stroke = new SolidColorPaint(s_violet, 2),
                    GeometrySize = 10,
                    GeometryStroke = new SolidColorPaint(s_violet, 2),
                    Fill = null,
                    ScalesYAt = 0
                },
            };

            _temperatureSeries = new[]
            {
                new LineSeries<ObservableValue>
                {
                    LineSmoothness = 0,
                    Name = "Temperature",
                    Values = _temperatureValues,
                    Stroke = new SolidColorPaint(s_red, 2),
                    GeometrySize = 10,
                    GeometryStroke = new SolidColorPaint(s_red, 2),
                    Fill = null,
                    ScalesYAt = 0
                },
            };

            _humiditySeries = new[]
            {
                new LineSeries<ObservableValue>
                {
                    LineSmoothness = 0,
                    Name = "Humidity",
                    Values = _humidityValues,
                    Stroke = new SolidColorPaint(s_blue, 2),
                    GeometrySize = 10,
                    GeometryStroke = new SolidColorPaint(s_blue, 2),
                    Fill = null,
                    ScalesYAt = 0
                },
            };

            _co2LevelSeries = new[]
            {
                new LineSeries<ObservableValue>
                {
                    LineSmoothness = 0,
                    Name = "CO2 Level",
                    Values = _co2LevelValues,
                    Stroke = new SolidColorPaint(s_yellow, 2),
                    GeometrySize = 10,
                    GeometryStroke = new SolidColorPaint(s_yellow, 2),
                    Fill = null,
                    ScalesYAt = 0
                },
            };

            _updateTimer = dispatcherTimer;
            _updateTimer.Interval = TimeSpan.FromSeconds(5);
            _updateTimer.Tick += OnTimerTick;
        }

        private async void OnTimerTick(object sender, EventArgs e)
        {
            _updateTimer.Stop();

            try
            {
                if (!IsBusy)
                {
                    await UpdateWorkspaceDataEntries(SelectedWorkspace);
                }
            }
            finally
            {
                _updateTimer.Start();
            }
        }

        protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            DeleteWorkspaceDataCommand.NotifyCanExecuteChanged();

            if(e.PropertyName == nameof(SelectedWorkspace))
            {
                await UpdateWorkspaceDataEntries(SelectedWorkspace);
            }
        }

        public override async Task Activate()
        {
            try
            {
                IsBusy = true;

                SelectedWorkspace = null;

                var workspaces = await _workspaceManager.GetWorkspaces();

                Workspaces = new ObservableCollection<WorkspaceModel>(workspaces);

                _updateTimer.Start();
            }
            finally
            {
                IsBusy = false;
            }

            SelectedWorkspace = Workspaces?.FirstOrDefault();
        }

        public override Task Deactivate()
        {
            _updateTimer.Stop();

            return Task.CompletedTask;
        }

        private async Task UpdateWorkspaceDataEntries(WorkspaceModel workspace)
        {
            try
            {
                if (IsBusy)
                {
                    return;
                }

                IsBusy = true;

                if (workspace == null)
                {
                    ResetChartData();
                }
                else
                {
                    var startDate = new DateTime(
                        StartDate.Year,
                        StartDate.Month,
                        StartDate.Day,
                        StartTime.Hours,
                        StartTime.Minutes,
                        StartTime.Seconds);

                    var endDate = new DateTime(
                        EndDate.Year,
                        EndDate.Month,
                        EndDate.Day,
                        EndTime.Hours,
                        EndTime.Minutes,
                        EndTime.Seconds);

                    var workspaceDataEntries = await _workspaceDataManager.GetWorkspaceData(workspace.WorkspaceNumber, startDate, endDate);

                    foreach (var entry in workspaceDataEntries)
                    {
                        if (!WorkspaceDataEntries.Any(x => x.Id == entry.Id))
                        {
                            _weiValues.Add(new ObservableValue(entry.Wei));
                            _temperatureValues.Add(new ObservableValue(entry.Temperature));
                            _humidityValues.Add(new ObservableValue(entry.Humidity));
                            _co2LevelValues.Add(new ObservableValue(entry.Co2Level));
                        }
                    }

                    WorkspaceDataEntries = new List<WorkspaceDataModel>(workspaceDataEntries);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ResetChartData()
        {
            WorkspaceDataEntries = new List<WorkspaceDataModel>();

            _weiValues.Clear();
            _temperatureValues.Clear();
            _humidityValues.Clear();
            _co2LevelValues.Clear();
        }

        [RelayCommand]
        private void ShowWei()
        {
            TemperatureVisible = HumidityVisible = Co2Visible = false;
            WeiVisible = true;
        }

        [RelayCommand]
        private void ShowTemperature()
        {
            WeiVisible = HumidityVisible = Co2Visible = false;
            TemperatureVisible = true;
        }

        [RelayCommand]
        private void ShowHumidity()
        {
            WeiVisible = TemperatureVisible = Co2Visible = false;
            HumidityVisible = true;
        }

        [RelayCommand]
        private void ShowCo2()
        {
            WeiVisible = TemperatureVisible = HumidityVisible = false;
            Co2Visible = true;
        }

        [RelayCommand(CanExecute = nameof(CanDeleteWorkspaceData))]
        private async Task DeleteWorkspaceData()
        {
            if (CanDeleteWorkspaceData())
            {
                if (await DialogService.ShowConfirmationDialog("Delete workspace data?", "Do you realy want to delete all data of the selected workspace?"))
                {
                    try
                    {
                        IsBusy = true;

                        await _workspaceDataManager.DeleteWorkspaceData(SelectedWorkspace.WorkspaceNumber);

                        ResetChartData();
                    }
                    finally
                    {
                        IsBusy = false;
                    }
                }
            }
        }

        private bool CanDeleteWorkspaceData()
        {
            return !IsBusy && SelectedWorkspace != null;
        }
    }
}
