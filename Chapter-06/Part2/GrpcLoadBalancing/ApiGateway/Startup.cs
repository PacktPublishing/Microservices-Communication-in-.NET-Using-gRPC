using System.Collections.Generic;
using System.Linq;
using System.Net;
using Grpc.Net.Client.Balancer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ApiGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddOpenApiDocument();
            var addresses = Configuration.GetSection("ServerAddresses").Get<List<string>>();
            services.AddSingleton(Configuration);
            services.AddSingleton<IGrpcClientWrapper, GrpcClientWrapper>();
            services.AddSingleton<ResolverFactory>
                (new StaticResolverFactory(addr => addresses
                    .Select(a => new DnsEndPoint(a.Replace("//", string.Empty).Split(':')[1], int.Parse(a.Split(':')[2])))
                    .ToArray()));
            services.AddSingleton<ResolverFactory, DiskResolverFactory>();
            services.AddSingleton<LoadBalancerFactory, RandomizedBalancerFactory>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
