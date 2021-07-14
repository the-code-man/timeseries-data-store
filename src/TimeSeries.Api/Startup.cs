using Autofac;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Linq;
using TimeSeries.Api.Hubs;
using TimeSeries.Api.Modules;
using TimeSeries.DataStore.Raw;
using TimeSeries.Shared.Contracts.Extensions;
using TimeSeries.Shared.Contracts.Settings;

namespace TimeSeries.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(c => c.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.AddApiVersioning(v =>
            {
                v.DefaultApiVersion = new ApiVersion(1, 0);
                v.AssumeDefaultVersionWhenUnspecified = true;
                v.ReportApiVersions = true;
            });

            services.AddSignalR();

            services.AddAppConfiguration<ServiceSettings>(Configuration.GetSection(nameof(ServiceSettings)));
            services.AddAppConfiguration<MessageBusSettings>(Configuration.GetSection(nameof(MessageBusSettings)));
            services.AddAppConfiguration<DataStoreSettings>(Configuration.GetSection(nameof(DataStoreSettings)));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Data aggregator API v1", Version = "1.0" });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "Data aggregator API v2", Version = "2.0" });
                c.ResolveConflictingActions(a => a.First());
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var busSettings = Configuration.GetSection(nameof(MessageBusSettings)).Get<MessageBusSettings>();

            builder.RegisterModule(new DataStoreModule());
            builder.RegisterModule(new BusModule(busSettings));
            builder.RegisterModule(new ApiModule());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime, IBusControl bus)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Data aggregator API v1"));
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v2/swagger.json", "Data aggregator API v2"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/notificationHub");
            });

            appLifetime.ApplicationStarted.Register(bus.Start);
            appLifetime.ApplicationStopped.Register(bus.Stop);
        }
    }
}
