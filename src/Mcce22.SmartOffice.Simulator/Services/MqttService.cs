using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mcce22.SmartOffice.Simulator.Messages;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using Oocx.ReadX509CertificateFromPem;

namespace Mcce22.SmartOffice.Simulator.Services
{
    public interface IMqttService
    {
        event EventHandler<MessageReceivedArgs> MessageReceived;

        Task Connect();

        Task PublishMessage(DataIngressMessage message);
    }

    public class MqttService : IMqttService
    {
        private static readonly MqttFactory Factory = new MqttFactory();

        private readonly IMqttClient _mqttClient;
        private readonly AppSettings _appSettings;

        public event EventHandler<MessageReceivedArgs> MessageReceived;

        public MqttService(AppSettings appSettings)
        {
            _appSettings = appSettings;

            _mqttClient = Factory.CreateMqttClient();
        }

        public async Task Connect()
        {
            if (!_mqttClient.IsConnected)
            {
                try
                {                   
                    var options = new MqttClientOptionsBuilder()
                        .WithTcpServer(_appSettings.MqttHostName, _appSettings.MqttPort)
                        .WithCredentials(_appSettings.MqttUserName, _appSettings.MqttPassword)
                        .Build();

                    _mqttClient.ApplicationMessageReceivedAsync += OnMqttMessageReceived;

                    await _mqttClient.ConnectAsync(options, CancellationToken.None);

                    await _mqttClient.SubscribeAsync(Topics.DEVICE_ACTIVATED);
                }
                catch (Exception)
                {
                    await Task.Delay(5000);
                    await Connect();
                }
            }
        }

        private Task OnMqttMessageReceived(MqttApplicationMessageReceivedEventArgs arg)
        {
            var payload = arg.ApplicationMessage.ConvertPayloadToString();

            var message = JsonConvert.DeserializeObject<DeviceActivatedMessage>(payload);

            MessageReceived?.Invoke(this, new MessageReceivedArgs(message));

            return Task.CompletedTask;
        }

        public async Task PublishMessage(DataIngressMessage message)
        {
            if (_mqttClient.IsConnected)
            {
                var msg = new MqttApplicationMessageBuilder()
                    .WithTopic(Topics.DATA_INGRESS)
                    .WithPayload(JsonConvert.SerializeObject(message))
                    .Build();

                await _mqttClient.PublishAsync(msg, CancellationToken.None);
            }
        }
    }

    public class MqttMessage
    {
        public string Topic { get; set; }
    }

    public class MessageReceivedArgs : EventArgs
    {
        public DeviceActivatedMessage Message { get; }

        public MessageReceivedArgs(DeviceActivatedMessage message)
        {
            Message = message;
        }
    }
}
