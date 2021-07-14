namespace TimeSeries.Shared.Contracts.Internal
{
    public class ReadResponse<T> : Response
    {
        public ReadResponse()
        {
            IsSuccess = false;
            ErrorMessage = string.Empty;
        }

        public T Data { get; set; }
    }
}
