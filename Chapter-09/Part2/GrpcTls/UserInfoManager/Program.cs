using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography.X509Certificates;

namespace UserInfoManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.ConfigureHttpsDefaults(o =>
                        {
                            o.ServerCertificate =
                                new X509Certificate2("UserInfoManager.pfx", "password");
                        });
                        options.ListenLocalhost(5002, o => o.Protocols =
                            HttpProtocols.Http1);
                        options.ListenLocalhost(5000, o => o.Protocols =
                            HttpProtocols.Http2);
                        options.ListenAnyIP(5001, o => o.UseHttps());
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
