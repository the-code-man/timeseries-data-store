using System.Threading.Tasks;

namespace TimeSeries.ServiceBus.Common
{
    public interface IDataProcessor<T>
    {
        Task Process(T data);
    }
}