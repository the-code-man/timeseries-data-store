using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using TimeSeries.Shared.Contracts.Entities;

namespace TimeSeries.Api.Hubs
{
    public class NotificationHub : Hub
    {
        public ChannelReader<RawTimeSeries[]> StreamProcessedRawData(RawTimeSeries[] rawData, CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<RawTimeSeries[]>();
            _ = WriteToStreamAsync(channel.Writer, rawData, cancellationToken);
            return channel.Reader;
        }

        private static async Task WriteToStreamAsync(ChannelWriter<RawTimeSeries[]> writer, RawTimeSeries[] rawData, CancellationToken cancellationToken)
        {
            try
            {
                await writer.WriteAsync(rawData, cancellationToken);
            }
            catch (Exception ex)
            {
                writer.TryComplete(ex);
            }

            writer.TryComplete();
        }
    }
}