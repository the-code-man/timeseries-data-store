using CommandLine;
using DataIngestor.Client;
using System;

namespace TimeSeries.DataIngestor.Client
{
    class Program
    {
        public class Options
        {
            [Option('a', "apiBaseAddress", Required = true, HelpText = "Base address of the data aggregator api")]
            public string ApiBaseAddress { get; set; }

            [Option('d', "dataSource", Required = true, HelpText = "Source against which data has to be simulated")]
            public string DataSource { get; set; }

            [Option('b', "batchSize", Required = false, Default = 1, HelpText = "Size of timeseries batch to send in one request")]
            public int BatchSize { get; set; }

            [Option('i', "interval", Required = false, Default = 1000, HelpText = "Millisecond interval between subsequent requests.")]
            public int Interval { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    var dataSimulator = new DataSimulator(o.ApiBaseAddress);

                    try
                    {
                        dataSimulator.Start(o.DataSource, o.Interval, o.BatchSize);
                        Console.WriteLine($"Data simulation started for {o.DataSource} source, with a batch size of {o.BatchSize} and generating data every {o.Interval} ms.");
                        Console.WriteLine("Press any key to stop");
                        Console.ReadKey();
                    }
                    finally
                    {
                        dataSimulator.Stop();
                    }
                });
        }
    }
}
