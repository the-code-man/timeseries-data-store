using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace TimeSeries.Shared.Contracts.Extensions
{
    public static class DIExtensions
    {
        public static IServiceCollection AddAppConfiguration<T>(this IServiceCollection services, 
            IConfigurationSection key) where T : class, new()
        {
            var appSettings = new T();
            new ConfigureFromConfigurationOptions<T>(key).Configure(appSettings);
            return services.AddSingleton(appSettings);
        }
    }
}