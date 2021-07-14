using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DataIngestor.Client
{
    public class DataSimulator
    {
        private readonly string _apiBaseAddress;
        private readonly CancellationTokenSource _tokenSource;
        private readonly IHttpClientFactory _httpClientFactory;

        private Task _dataIngestSimulateTask;

        public DataSimulator(string apiBaseAddress)
        {
            _apiBaseAddress = apiBaseAddress;
            _tokenSource = new CancellationTokenSource();
            _httpClientFactory = InitializeHttpClientFactory();
        }

        public void Start(string dataSource, int interval, int batchSize)
        {
            _dataIngestSimulateTask = new Task(async () =>
            {
                string ingestApiUrl = $"{_apiBaseAddress}/api/timeseries/v2/WriteData/{dataSource}";
                var msOffset = interval / batchSize;
                var timeSeriesEventBatches = new List<TimeSeriesEvent>(batchSize);

                var httpClient = _httpClientFactory.CreateClient("UnSecure");

                while (!_tokenSource.Token.WaitHandle.WaitOne(interval))
                {
                    Trace.TraceInformation("Sending new batch");

                    var currentDateTime = DateTime.Now;
                    var rand = new Random();
                    var batch = Enumerable
                                .Range(0, batchSize)
                                .Select(c => new TimeSeriesEvent(currentDateTime.AddMilliseconds(c * msOffset),
                                                             Enumerable.Range(1, rand.Next(1, 5))
                                                                       .Select(c => Math.Round(rand.NextDouble() * c, 2))
                                                                       .ToArray())).ToArray();

                    var response = await httpClient
                                         .PostAsync(ingestApiUrl, new StringContent(JsonSerializer.Serialize(batch), Encoding.UTF8, "application/json"),
                                         _tokenSource.Token);

                    if (response.IsSuccessStatusCode)
                    {
                        Trace.TraceInformation("Data successfully ingested");
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        Trace.TraceError($"Api returned an error for data ingestion. Status -> {response.StatusCode}, Message -> {errorMessage}");
                    }
                }

                _tokenSource.Token.ThrowIfCancellationRequested();
            },
            _tokenSource.Token, TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach);
            _dataIngestSimulateTask.ContinueWith(exception => Trace.TraceError(exception.Exception.InnerException.Message), TaskContinuationOptions.OnlyOnFaulted);
            _dataIngestSimulateTask.Start();

            Trace.TraceInformation($"Data simulation started for {dataSource} source, with a batch size of {batchSize} and generating data every {interval} ms.");
        }

        public void Stop()
        {
            _tokenSource.Cancel();
            _dataIngestSimulateTask.Wait();
        }

        private static IHttpClientFactory InitializeHttpClientFactory()
        {
            var handler = new HttpClientHandler
            {
                // Never ever do this in production. 
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            var services = new ServiceCollection();
            services.AddHttpClient("UnSecure").ConfigurePrimaryHttpMessageHandler(() => handler);

            return services.BuildServiceProvider().GetService<IHttpClientFactory>();
        }
    }
}