using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Demo.Users.WebApi.Extensions;
using Prometheus;

namespace OpenTelemetry.Demo.Users.WebApi
{
    public class Startup
    {
        public static IDisposable Collector;
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OpenTelemetry.Demo.Users.WebApi", Version = "v1" });
            });
            services.AddDbMigrations(Configuration);
            services.AddHttpClients(Configuration);
            services.AddOpenTelemetry(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OpenTelemetry.Demo.Users.WebApi v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            //Prometheus Http Metrics
            app.UseHttpMetrics();            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //Prometheus Base Metrics
                endpoints.MapMetrics();
                endpoints.MapControllers();
            });
        }
    }
}