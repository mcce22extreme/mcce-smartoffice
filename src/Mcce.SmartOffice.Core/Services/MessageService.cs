using Mcce.SmartOffice.Core.Configs;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.CircuitBreaker;
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
        private static readonly string SenderIdentifier = Guid.NewGuid().ToString();

        private static readonly MqttFactory Factory = new MqttFactory();

        private static readonly AsyncPolicy CircuitBreakerPolicy = Policy
            .Handle<MqttCommunicationException>()
            .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));

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
                jObject["Sender"] = SenderIdentifier;

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
            if (await Connect())
            {
                await _mqttClient.UnsubscribeAsync(topic);

                _handlers.Remove(topic);
            }
        }

        private async Task OnMqttMessageReceived(MqttApplicationMessageReceivedEventArgs args)
        {
            var payload = args.ApplicationMessage.ConvertPayloadToString();

            if (_handlers.ContainsKey(args.ApplicationMessage.Topic))
            {
                foreach (var handler in _handlers[args.ApplicationMessage.Topic])
                {
                    await handler.Handle(args.ApplicationMessage.Topic, payload);
                }
            }
        }

        private async Task<bool> Connect()
        {
            if (!_mqttClient.IsConnected)
            {
                try
                {
                    await CircuitBreakerPolicy.ExecuteAsync(async () =>
                    {
                        var options = new MqttClientOptionsBuilder()
                            .WithCredentials(_mqttConfig.UserName, _mqttConfig.Password)
                            .WithTcpServer(_mqttConfig.HostName, _mqttConfig.Port)
                            .Build();

                        _mqttClient.ApplicationMessageReceivedAsync += OnMqttMessageReceived;

                        await _mqttClient.ConnectAsync(options, CancellationToken.None);

                        Log.Information($"Connection to message broker '{_mqttConfig.HostName}:{_mqttConfig.Port}' was successful!");
                    });
                }
                catch (BrokenCircuitException ex)
                {
                    Log.Error(ex, ex.Message);

                    // Circuit is broken => return
                    return false;
                }
                catch (MqttCommunicationException ex)
                {
                    Log.Error(ex, ex.Message);

                    await Task.Delay(5000);

                    // Try to reconnect unitl circuit is broken
                    return await Connect();
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
