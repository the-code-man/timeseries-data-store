namespace TimeSeries.Shared.Contracts.Settings
{
    public static class MessageBusQueue
    {
        public const string RAW_DATA_PROCESS = "raw_data_process_queue";
        public const string AVG_DATA_PROCESS = "avg_data_process_queue";
        public const string MIN_DATA_PROCESS = "min_data_process_queue";
        public const string MAX_DATA_PROCESS = "max_data_process_queue";
        public const string PROCESSED_DATA = "processed_data_queue";
    }
}