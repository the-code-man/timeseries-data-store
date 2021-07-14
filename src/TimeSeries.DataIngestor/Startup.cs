﻿using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TimeSeries.DataIngestor.Modules;
using TimeSeries.DataStore.Raw;
using TimeSeries.Shared.Contracts.Extensions;
using TimeSeries.Shared.Contracts.Settings;

namespace TimeSeries.DataIngestor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAppConfiguration<MessageBusSettings>(Configuration.GetSection(nameof(MessageBusSettings)));
            services.AddAppConfiguration<DataStoreSettings>(Configuration.GetSection(nameof(DataStoreSettings)));
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var busSettings = Configuration.GetSection(nameof(MessageBusSettings)).Get<MessageBusSettings>();

            builder.RegisterModule(new DataStoreModule());
            builder.RegisterModule(new ConsumerBusModule(busSettings));
            builder.RegisterModule(new ServiceModule());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        }
    }
}