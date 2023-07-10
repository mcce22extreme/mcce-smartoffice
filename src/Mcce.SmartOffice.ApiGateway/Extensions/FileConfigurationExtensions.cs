using System.Text.RegularExpressions;
using Mcce.SmartOffice.Core.Extensions;
using MMLib.SwaggerForOcelot.Configuration;
using Newtonsoft.Json;
using Ocelot.Configuration.File;
using Serilog;

namespace Mcce.SmartOffice.ApiGateway.Extensions
{
    public static class FileConfigurationExtensions
    {
        public static IServiceCollection ConfigureRoutePlaceholders(this IServiceCollection services)
        {
            services.PostConfigure<FileConfiguration>(cfg =>
            {
                foreach (var route in cfg.Routes)
                {
                    foreach (var hostAndPort in route.DownstreamHostAndPorts)
                    {
                        var host = hostAndPort.Host;
                        if (host.StartsWith("{") && host.EndsWith("}"))
                        {
                            var placeholder = host.TrimStart('{').TrimEnd('}');

                            var env = Environment.GetEnvironmentVariable($"SMARTOFFICE_{placeholder.ToUpper()}");

                            if (env.HasValue())
                            {
                                var builder = new UriBuilder(env);

                                route.DownstreamScheme = builder.Scheme;
                                hostAndPort.Host = builder.Host;
                                hostAndPort.Port = builder.Port;
                            }
                        }
                    }
                }

                Log.Debug("Ocelot route configuration: \n" + JsonConvert.SerializeObject(cfg, Formatting.Indented));
            });

            services.PostConfigure<List<SwaggerEndPointOptions>>(options =>
            {
                foreach (var opt in options)
                {
                    foreach (var config in opt.Config)
                    {
                        var match = Regex.Match(config.Url, "{([A-z-_]+)}");
                        if (match.Success)
                        {
                            var placeholder = match.Groups[1].Value;
                            var env = Environment.GetEnvironmentVariable($"SMARTOFFICE_{placeholder.ToUpper()}");

                            if (env.HasValue())
                            {
                                config.Url = config.Url.Replace($"{{{placeholder}}}", env);
                            }
                        }
                    }
                }

                Log.Debug("Swagger for endpoint configuration: \n" + JsonConvert.SerializeObject(options, Formatting.Indented));
            });

            return services;
        }
    }
}
