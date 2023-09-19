using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce.SmartOffice.Core.Constants;
using Mcce.SmartOffice.Core.Services;
using Mcce22.SmartOffice.Simulator.Managers;
using Mcce22.SmartOffice.Simulator.Messages;
using Newtonsoft.Json;

namespace Mcce22.SmartOffice.Simulator.ViewModels
{
    public partial class MainViewModel : ObservableObject, IMessageHandler
    {
        private const string TOPIC_DATA_INGRESS = "mcce22-smart-office/dataingress";
        private const double DEFAULT_DESK_CANVAS_TOP = 0;
        private const double MAX_DESK_HEIGHT = 120;
        private const double MIN_DESK_HEIGHT = 70;
        private const double DEFAULT_LEG_CANVAS_TOP = 420;
        private const double DEFAULT_LEG_HEIGHT = 80;
        private const double DEFAULT_IMAGEFRAME_CANVAS_TOP = 336;
        private const double DEFAULT_WIFI_CANVAS_TOP = 315;

        private readonly Timer _delayTimer;
        private readonly Timer _imageTimer;
        private readonly IWorkspaceManager _workspaceManager;
        private readonly IMessageService _messageService;

        private List<string> _imageUrls = new List<string>();
        private int _currentImageIndex;
        private bool _updateSendInProgress;

        [ObservableProperty]
        private string _workspaceNumber;

        [ObservableProperty]
        private double _deskCanvasTop = DEFAULT_DESK_CANVAS_TOP;

        [ObservableProperty]
        private double _legCanvasTop = DEFAULT_LEG_CANVAS_TOP;

        [ObservableProperty]
        private double _legHeight = DEFAULT_LEG_HEIGHT;

        [ObservableProperty]
        private double _deskHeight = MIN_DESK_HEIGHT;

        [ObservableProperty]
        private double _imageFrameCanvasTop = DEFAULT_IMAGEFRAME_CANVAS_TOP;

        [ObservableProperty]
        private double _wifiCanvasTop = DEFAULT_WIFI_CANVAS_TOP;

        [ObservableProperty]
        private double _temperature = 20;

        [ObservableProperty]
        private double _humidity = 20;

        [ObservableProperty]
        private double _co2Level = 700;

        [ObservableProperty]
        private string _messageLog;

        [ObservableProperty]
        private string[] _workspaces;

        private string _selectedWorkspace;
        public string SelectedWorkspace
        {
            get { return _selectedWorkspace; }
            set
            {
                var oldTopics = SupportedTopics.ToArray();

                if (SetProperty(ref _selectedWorkspace, value))
                {
                    UpdateSubscriptions(SupportedTopics, oldTopics);
                }
            }
        }

        private async void UpdateSubscriptions(string[] newTopics, string[] oldTopics)
        {
            foreach (var topics in oldTopics)
            {
                await _messageService.Unsubscribe(topics);
            }

            foreach (var topics in newTopics)
            {
                await _messageService.Subscribe(topics, this);
            }
        }

        [ObservableProperty]
        private ImageSource _imageSource;

        public string TopicWorkspaceActivatedUserImages
        {
            get { return string.Format(MessageTopics.TOPIC_WORKSPACE_ACTIVATE_USERIMAGES, SelectedWorkspace); }
        }

        public string TopicWorkspaceActivatedWorkspaceConfiguration
        {
            get { return string.Format(MessageTopics.TOPIC_WORKSPACE_ACTIVATE_WORKSPACECONFIGURATION, SelectedWorkspace); }
        }

        public string[] SupportedTopics => new[]
        {
            TopicWorkspaceActivatedUserImages,
            TopicWorkspaceActivatedWorkspaceConfiguration
        };

        public MainViewModel(IWorkspaceManager workspaceManager, IMessageService messageService)
        {
            _workspaceManager = workspaceManager;
            _messageService = messageService;

            _delayTimer = new Timer();
            _delayTimer.Interval = 5000;
            _delayTimer.Elapsed += OnDelayTimerElapsed;

            _imageTimer = new Timer();
            _imageTimer.Interval = 5000;
            _imageTimer.Elapsed += OnImageTimerElapsed;

            UpdateImageSource();

            LoadWorkspaces();
        }

        private async void LoadWorkspaces()
        {
            var workspaces = await _workspaceManager.GetWorkspaces();

            Workspaces = workspaces.Select(x => x.WorkspaceNumber).ToArray();

            SelectedWorkspace = Workspaces.FirstOrDefault();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Temperature) ||
                e.PropertyName == nameof(Humidity) ||
                e.PropertyName == nameof(Co2Level))
            {
                if (!_updateSendInProgress)
                {
                    _updateSendInProgress = true;
                    _delayTimer.Stop();
                    _delayTimer.Start();
                }
            }

            base.OnPropertyChanged(e);
        }

        private void OnDelayTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(async () =>
            {
                if (SelectedWorkspace != null)
                {
                    var msg = new DataIngressMessage
                    {
                        WorkspaceNumber = SelectedWorkspace,
                        Temperature = Temperature,
                        Humidity = Humidity,
                        Co2Level = Co2Level,
                    };

                    await _messageService.Publish(TOPIC_DATA_INGRESS, new
                    {
                        WorkspaceNumber = SelectedWorkspace,
                        Temperature,
                        Humidity,
                        Co2Level,
                    });

                    MessageLog += $"[OUT] {JsonConvert.SerializeObject(msg)}" + Environment.NewLine;
                }

                _delayTimer.Stop();
                _updateSendInProgress = false;
            });
        }

        private void SetImageSource(string resourcePath)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(resourcePath, UriKind.RelativeOrAbsolute);
                bitmap.EndInit();

                ImageSource = bitmap;
            });
        }

        private void OnImageTimerElapsed(object sender, ElapsedEventArgs e)
        {
            UpdateImageSource();
        }

        private void UpdateImageSource()
        {
            if (_imageUrls.Count > 0)
            {
                if (_currentImageIndex == _imageUrls.Count)
                {
                    _currentImageIndex = 0;
                }

                SetImageSource(_imageUrls[_currentImageIndex]);

                _currentImageIndex++;
            }
            else
            {
                SetImageSource("/Mcce22.SmartOffice.Simulator;component/images/smartoffice.png");
            }
        }

        public Task Handle(string topic, string payload)
        {
            var info = JsonConvert.DeserializeObject<WorkspaceActivateMessage>(payload);

            if (topic == TopicWorkspaceActivatedUserImages)
            {
                _imageUrls.Clear();
                _imageUrls.AddRange(info.UserImages);

                UpdateImageSource();

                _imageTimer.Start();
            }

            if (topic == TopicWorkspaceActivatedWorkspaceConfiguration)
            {
                if (info.DeskHeight > MAX_DESK_HEIGHT)
                {
                    DeskHeight = MAX_DESK_HEIGHT;
                }
                else if (info.DeskHeight < MIN_DESK_HEIGHT)
                {
                    DeskHeight = MIN_DESK_HEIGHT;
                }
                else
                {
                    DeskHeight = info.DeskHeight;
                }

                UpdateDeskParams(DeskHeight);
            }

            return Task.CompletedTask;
        }

        private class WorkspaceActivateMessage
        {
            public int DeskHeight { get; set; }

            public string[] UserImages { get; set; }
        }

        private void UpdateDeskParams(double deskHeight)
        {
            LegCanvasTop = DEFAULT_LEG_CANVAS_TOP - (deskHeight - MIN_DESK_HEIGHT) * 2;
            LegHeight = DEFAULT_LEG_HEIGHT + (deskHeight - MIN_DESK_HEIGHT) * 2;
            ImageFrameCanvasTop = DEFAULT_IMAGEFRAME_CANVAS_TOP - (deskHeight - MIN_DESK_HEIGHT) * 2;
            WifiCanvasTop = DEFAULT_WIFI_CANVAS_TOP - (deskHeight - MIN_DESK_HEIGHT) * 2;
            DeskCanvasTop = DEFAULT_DESK_CANVAS_TOP - (deskHeight - MIN_DESK_HEIGHT) * 2;
        }

        [RelayCommand]
        private void ClearMessageLog()
        {
            MessageLog = string.Empty;
        }
    }
}
