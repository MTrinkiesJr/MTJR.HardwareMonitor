using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MTJR.HardwareMonitor.Configuration;
using MTJR.HardwareMonitor.Data;
using MTJR.HardwareMonitor.Model;
using MTJR.HardwareMonitor.Services;
using Newtonsoft.Json;

namespace MTJR.HardwareMonitor
{
    /// <summary>
    /// Startup class for the service
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Construcutor to create fulfilled <see cref="Startup"/> initialized by <see cref="WebHostBuilder"/>
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Loaded configuration
        /// </summary>
        public IConfiguration Configuration { get; }
        

        /// <summary>
        /// Initializes necessary services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder =>
                {
                    builder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
                }));

            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });


            services.AddSingleton<MonitoringService>();
            services.AddHostedService(provider => provider.GetRequiredService<MonitoringService>());

            services.AddSingleton<ConfigurationService>();
            services.AddHostedService(provider => provider.GetRequiredService<ConfigurationService>());

            services.AddSingleton<IoBrokerApiService>();

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services.Configure<HardwareMonitorConfiguration>(Configuration.GetSection("HardwareMonitorConfiguration"));
            var config = services.BuildServiceProvider().GetService<IOptions<HardwareMonitorConfiguration>>()?.Value;
            
            services.AddHostedService<DataContextMigrationService>();
            services.AddDbContext<DataContext>(builder =>
            {
                if (config != null) builder.UseNpgsql(config.DatabaseConnectionString);
                builder.EnableSensitiveDataLogging();
                builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Server Hardware Monitoring API",
                    Description = "Manage hardware monitored server"
                });
                try
                {
                    options.IncludeXmlComments($"{AppDomain.CurrentDomain.BaseDirectory}\\MTJR.HardwareMonitor.xml");
                }
                catch
                {
                    // ignored
                }
            });

            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore)
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
                options.KeepAliveInterval = TimeSpan.FromSeconds(10);
                options.ClientTimeoutInterval = TimeSpan.FromMinutes(1);
            }).AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.Converters
                    .Add(new JsonStringEnumConverter());
            });

            var connectionList = new List<EventHubConnection>();
            services.AddSingleton(connectionList);
            services.AddSingleton<EventService>();

            services.AddMvcCore()
                .AddNewtonsoftJson(o => o.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter()))
                .AddRazorPages()
                .AddRazorRuntimeCompilation()
                .AddRazorViewEngine()
                .AddViews();
        }
        

        /// <summary>
        /// Configures injected services
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="env">Loaded environment</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("CorsPolicy");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Server Hardware Monitoring API");
                options.RoutePrefix = "swagger";
            });

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapHub<EventHub>("/events");
            });
        }
    }
}
