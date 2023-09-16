using Mcce.SmartOffice.Core.Configs;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Mcce.SmartOffice.Core.Services
{
    public interface IMessageService
    {
        Task Publish<T>(string topic, T payload);

        Task Subscribe(string topic, IMessageHandler handler);

        Task Unsubscribe(string topic);
    }

    public class MessageService : IMessageService
    {
        private static readonly string _sender = Guid.NewGuid().ToString();

        private static readonly MqttFactory Factory = new MqttFactory();

        private readonly IMqttClient _mqttClient;
        private readonly IDictionary<string, List<IMessageHandler>> _handlers = new Dictionary<string, List<IMessageHandler>>();
        private readonly MqttConfig _mqttConfig;

        public MessageService(MqttConfig mqttConfig)
        {
            _mqttClient = Factory.CreateMqttClient();
            _mqttConfig = mqttConfig;
        }

        public async Task Publish<T>(string topic, T payload)
        {
            if (await Connect())
            {
                var jObject = JObject.FromObject(payload);
                jObject["Sender"] = _sender;

                var msg = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(JsonConvert.SerializeObject(jObject))
                    .Build();

                await _mqttClient.PublishAsync(msg, CancellationToken.None);
            }
        }

        public async Task Subscribe(string topic, IMessageHandler handler)
        {
            if (await Connect())
            {
                await _mqttClient.SubscribeAsync(topic);

                if (_handlers.ContainsKey(topic))
                {
                    _handlers[topic].Add(handler);
                }
                else
                {
                    _handlers[topic] = new List<IMessageHandler> { handler };
                }
            }
        }

        public async Task Unsubscribe(string topic)
        {
            await _mqttClient.UnsubscribeAsync(topic);

            _handlers.Remove(topic);
        }

        private async Task OnMqttMessageReceived(MqttApplicationMessageReceivedEventArgs args)
        {
            var payload = args.ApplicationMessage.ConvertPayloadToString();

            if (_handlers.ContainsKey(args.ApplicationMessage.Topic))
            {
                foreach (var handler in _handlers[args.ApplicationMessage.Topic])
                {
                    await handler.Handle(args.ClientId, payload);
                }
            }
        }

        private async Task<bool> Connect()
        {
            if (!_mqttClient.IsConnected)
            {
                try
                {
                    var options = new MqttClientOptionsBuilder()
                        .WithCredentials(_mqttConfig.UserName, _mqttConfig.Password)
                        .WithTcpServer(_mqttConfig.HostName, _mqttConfig.Port)
                        .Build();

                    _mqttClient.ApplicationMessageReceivedAsync += OnMqttMessageReceived;

                    await _mqttClient.ConnectAsync(options, CancellationToken.None);

                    Log.Information($"Connection to message broker '{_mqttConfig.HostName}:{_mqttConfig.Port}' was successful!");

                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, ex);

                    await Task.Delay(5000);
                    await Connect();
                }
            }

            return _mqttClient.IsConnected;
        }
    }

    public interface IMessageHandler
    {
        string[] SupportedTopics { get; }

        Task Handle(string topic, string payload);
    }
}
