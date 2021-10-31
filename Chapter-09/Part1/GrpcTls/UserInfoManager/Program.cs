﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;

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
                        options.ListenLocalhost(5002, o => o.Protocols =
                            HttpProtocols.Http1AndHttp2);
                        options.ListenLocalhost(5000, o => o.Protocols =
                            HttpProtocols.Http2);            
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
