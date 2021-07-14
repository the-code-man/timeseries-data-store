namespace TimeSeries.Shared.Contracts.Settings
{
    public static class MessageBusQueue
    {
        public const string RAW_DATA = "raw_data_queue";
        public const string PROCESSED_DATA_AVG = "processed_data_avg_queue";
        public const string PROCESSED_DATA_MIN = "processed_data_min_queue";
        public const string PROCESSED_DATA_MAX = "processed_data_max_queue";
    }
}