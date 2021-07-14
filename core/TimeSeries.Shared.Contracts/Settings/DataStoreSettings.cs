namespace TimeSeries.Shared.Contracts.Settings
{
    public class DataStoreSettings
    {
        public SupportDBType Type { get; set; }

        public string ConnectionString { get; set; }
    }

    public enum SupportDBType
    {
        SqlLite,
        //SqlServer,
        //MariaDB,
        //MySQL
    }
}