using System.Threading;
using System.Threading.Tasks;
using TimeSeries.Shared.Contracts.Entities;

namespace TimeSeries.ServiceBus.Common
{
    public interface IProducer
    {
        Task Send(string queue, string sourceId, RawTimeSeries[] data, CancellationToken ct);
    }
}