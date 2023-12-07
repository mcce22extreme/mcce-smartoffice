using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Mcce.SmartOffice.Client.Managers;
using Mcce.SmartOffice.Client.Models;
using Mcce.SmartOffice.Client.Services;
using SkiaSharp;

namespace Mcce.SmartOffice.Client.ViewModels
{
    public partial class WorkspaceDataListViewModel : ViewModelBase
    {
        private static readonly SKColor s_blue = new(25, 118, 210);
        private static readonly SKColor s_red = new(229, 57, 53);
        private static readonly SKColor s_yellow = new(198, 167, 0);
        private static readonly SKColor s_violet = new(238, 130, 238);

        private readonly IWorkspaceManager _workspaceManager;
        private readonly IDialogService _dialogService;
        private readonly IAuthService _authService;
        private readonly IWorkspaceDataEntryManager _workspaceDataManager;
        private readonly Timer _updateTimer;
        private readonly ObservableCollection<ObservableValue> _weiValues;
        private readonly ObservableCollection<ObservableValue> _temperatureValues;
        private readonly ObservableCollection<ObservableValue> _humidityValues;
        private readonly ObservableCollection<ObservableValue> _co2LevelValues;

        [ObservableProperty]
        private List<WorkspaceModel> _workspaces;

        private WorkspaceModel _selectedWorkspace;
        public WorkspaceModel SelectedWorkspace
        {
            get { return _selectedWorkspace; }
            set
            {
                if (SetProperty(ref _selectedWorkspace, value))
                {
                    _weiValues.Clear();
                    _temperatureValues.Clear();
                    _humidityValues.Clear();
                    _co2LevelValues.Clear();

                    UpdateWorkspaceDataEntries(_selectedWorkspace);
                }
            }
        }

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
        private bool _showWeiDiagram = true;

        [ObservableProperty]
        private bool _showTemperatureDiagram = true;

        [ObservableProperty]
        private bool _showHumidityDiagram = true;

        [ObservableProperty]
        private bool _showCo2LevelDiagram = true;

        [ObservableProperty]
        private DateTime _startDate = DateTime.Now.Subtract(TimeSpan.FromDays(1));

        [ObservableProperty]
        private DateTime? _endDate;

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
                DrawTicksPath = true
            }
        };

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
                DrawTicksPath = true
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
                DrawTicksPath = true
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
                DrawTicksPath = true
            }
        };

        public SolidColorPaint LegendTextPaint { get; set; } =
            new SolidColorPaint
            {
                Color = SKColors.White,
            };

        public SolidColorPaint LedgendBackgroundPaint { get; set; } = new SolidColorPaint(new SKColor(240, 240, 240));

        public WorkspaceDataListViewModel(
                IWorkspaceDataEntryManager workspaceDataManager,
                IWorkspaceManager workspaceManager,
                IDialogService dialogService,
                IAuthService authService)
        {
            _workspaceDataManager = workspaceDataManager;
            _workspaceManager = workspaceManager;
            _dialogService = dialogService;
            _authService = authService;
            _weiValues = new ObservableCollection<ObservableValue>();
            _temperatureValues = new ObservableCollection<ObservableValue>();
            _humidityValues = new ObservableCollection<ObservableValue>();
            _co2LevelValues = new ObservableCollection<ObservableValue>();

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

            _updateTimer = new Timer();
            _updateTimer.Interval = 5000;
            _updateTimer.Elapsed += OnTimerElapsed;
            _updateTimer.Start();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            DeleteCommand.NotifyCanExecuteChanged();
            ReloadCommand.NotifyCanExecuteChanged();
        }

        public async Task Load()
        {
            try
            {
                IsBusy = true;

                var workspaces = await _workspaceManager.GetList();

                Workspaces = new List<WorkspaceModel>(workspaces);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void UpdateWorkspaceDataEntries(WorkspaceModel workspace)
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
                    var workspaceDataEntries = await _workspaceDataManager.GetList(workspace.WorkspaceNumber, StartDate, EndDate);

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

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            //if (!IsBusy && _authService.LoggedIn)

            if (!IsBusy)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => UpdateWorkspaceDataEntries(SelectedWorkspace)));
            }
        }

        [RelayCommand(CanExecute = nameof(CanReload))]
        private void Reload()
        {
            if (CanReload())
            {
                ResetChartData();
                UpdateWorkspaceDataEntries(SelectedWorkspace);
            }
        }

        private bool CanReload()
        {
            return !IsBusy && SelectedWorkspace != null;
        }

        [RelayCommand(CanExecute = nameof(CanDelete))]
        private async Task Delete()
        {
            if (!IsBusy)
            {
                try
                {
                    IsBusy = true;

                    var confirmDelete = new ConfirmDeleteViewModel("Delete all data entries of workspace?", "Do you really want to delete all data entries of the selected workspace?", _dialogService);

                    await _dialogService.ShowDialog(confirmDelete);

                    if (confirmDelete.Confirmed)
                    {
                        await _workspaceDataManager.Delete(SelectedWorkspace.WorkspaceNumber);

                        _weiValues.Clear();
                        _temperatureValues.Clear();
                        _humidityValues.Clear();
                        _co2LevelValues.Clear();

                        UpdateWorkspaceDataEntries(SelectedWorkspace);
                    }
                }
                finally
                {
                    IsBusy = false;
                }

                UpdateWorkspaceDataEntries(SelectedWorkspace);
            }
        }

        private bool CanDelete()
        {
            return !IsBusy && SelectedWorkspace != null && WorkspaceDataEntries.Any();
        }
    }
}
