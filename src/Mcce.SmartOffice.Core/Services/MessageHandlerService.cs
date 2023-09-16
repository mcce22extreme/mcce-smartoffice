using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mcce.SmartOffice.Core.Services
{
    public class MessageHandlerService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public MessageHandlerService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var scope = _serviceScopeFactory.CreateScope();

            var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
            var handlers = scope.ServiceProvider.GetRequiredService<IEnumerable<IMessageHandler>>();

            foreach (var handler in handlers)
            {
                foreach (var topic in handler.SupportedTopics)
                {
                    await messageService.Subscribe(topic, handler);
                }
            }
        }
    }
}
