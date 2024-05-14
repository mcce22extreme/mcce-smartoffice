namespace Mcce.SmartOffice.Api.Constants
{
    public static class MessageTopics
    {
        private const string TOPIC_PREFIX = "mcce-smartoffice/";

        public const string TOPIC_WEI_UPDATED = TOPIC_PREFIX + "wei";

        public static string TOPIC_DATAINGRESS { get; } = TOPIC_PREFIX + "dataingress";

        public static string TOPIC_BOOKING_CREATED { get; } = TOPIC_PREFIX + "booking/created";

        public static string TOPIC_BOOKING_REJECTED { get; } = TOPIC_PREFIX + "booking/rejected";

        public static string TOPIC_BOOKING_VALIDATED { get; } = TOPIC_PREFIX + "booking/validated";

        public static string TOPIC_BOOKING_CONFIRMED { get; } = TOPIC_PREFIX + "booking/confirmed";

        public static string TOPIC_BOOKING_ACTIVATED { get; } = TOPIC_PREFIX + "booking/activated";

        public static string TOPIC_WORKSPACE_ACTIVATE_USERIMAGES { get; } = TOPIC_PREFIX + "workspace/{0}/activate/userimages";

        public static string TOPIC_WORKSPACE_ACTIVATE_WORKSPACECONFIGURATION { get; } = TOPIC_PREFIX + "workspace/{0}/activate/workspaceconfiguration";
    }
}
