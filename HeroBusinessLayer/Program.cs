using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
//using Serilog;

namespace HeroBusinessLayerRoleBased
{
    public class Program
    {
        public static void Main(string[] args)
        {

            // Log.Logger = new LoggerConfiguration()
            //.Enrich.FromLogContext()
            //.WriteTo.File(@"c:\temp\log2.txt")
            //.CreateLogger();


            try
            {
                //Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                //Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                //Log.CloseAndFlush();
            }
        }



        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                      //.UseSerilog()
                      .ConfigureLogging(logging =>
                      {
                          logging.ClearProviders();
                          logging.AddConsole();
                      })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
