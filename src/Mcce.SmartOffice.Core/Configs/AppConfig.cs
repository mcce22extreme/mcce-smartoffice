﻿namespace Mcce.SmartOffice.Core.Configs
{
    public class AppConfig
    {
        public string BaseAddress { get; set; }

        public DbConfig DbConfig { get; set; }

        public MqttConfig MqttConfig { get; set; }
    }
}