using System;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TrafficControlService.Actors;
using TrafficControlService.Domain;
using TrafficControlService.Repositories;

namespace TrafficControlService
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
            services.AddSingleton<ISpeedingViolationCalculator>(
                       new DefaultSpeedingViolationCalculator("A12", 10, 100, 5));
            services.AddSingleton<IVehicleStateRepository, DaprVehicleStateRepository>();

            services.AddDaprClient(configure =>
            {

                configure.UseJsonSerializationOptions(
                    new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
            });
            services.AddControllers();
            services.AddActors(options => {
                options.Actors.RegisterActor<VehicleActor>();
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TrafficControlService", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                var daprHttpPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT");
                var daprGrpcPort = Environment.GetEnvironmentVariable("DAPR_GRPC_PORT");
                Console.WriteLine("dapr HTTP Port:{0} dapr gRPC Port:{1}", daprHttpPort, daprGrpcPort);
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TrafficControlService v1"));
            }

            app.UseRouting();

            app.UseAuthorization();
            app.UseCloudEvents();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapActorsHandlers();
            });
        }
    }
}
