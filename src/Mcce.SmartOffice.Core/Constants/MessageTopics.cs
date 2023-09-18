﻿namespace Mcce.SmartOffice.Core.Constants
{
    public static class MessageTopics
    {
        private const string TOPIC_PREFIX = "mcce-smartoffice/";

        public static string TOPIC_DATAINGRESS { get; } = TOPIC_PREFIX + "dataingress";

        public static string TOPIC_BOOKING_ACTIVATED { get; } = TOPIC_PREFIX + "booking/activated";

        public static string TOPIC_WORKSPACE_ACTIVATE_USERIMAGES { get; } = TOPIC_PREFIX + "workspace/{0}/activate/userimages";

        public static string TOPIC_WORKSPACE_ACTIVATE_WORKSPACECONFIGURATION { get; } = TOPIC_PREFIX + "workspace/{0}/activated/workspaceconfiguration";
    }
}