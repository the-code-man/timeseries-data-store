namespace TimeSeries.Shared.Contracts.Api
{
    public class ReadResponse<T> : Response
    {
        public T Data { get; set; }
    }
}
