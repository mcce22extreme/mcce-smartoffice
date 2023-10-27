﻿namespace Mcce.SmartOffice.Core.Configs
{
    public class AppConfig
    {
        public string BaseAddress { get; set; }

        public string AppConfigUrl { get; set; }

        public string FrontendUrl { get; set; }

        public AuthConfig AuthConfig { get; set; }

        public DbConfig DbConfig { get; set; }

        public MqttConfig MqttConfig { get; set; }

        public CorsConfig CorsConfig { get; set; }
    }
}
