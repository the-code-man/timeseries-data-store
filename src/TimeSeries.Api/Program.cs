using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TimeSeries.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureLogging((_, builder) =>
            {
                builder.AddFile("logs/api-{Date}", isJson: false);
            })
            //.ConfigureAppConfiguration((hostingContext, configBuilder) =>
            //{
            //    var env = hostingContext.HostingEnvironment;
                
            //    configBuilder
            //    .AddJsonFile("appsettings.json", optional: true)
            //    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            //    .AddEnvironmentVariables();
            //})
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}
