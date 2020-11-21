using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RestApi_Dicom
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        //        public static IHostBuilder CreateHostBuilder(string[] args) =>
        //            Host.CreateDefaultBuilder(args)
        //                .ConfigureWebHostDefaults(webBuilder =>
        //                {
        //                    webBuilder.UseStartup<Startup>();
        //                }).UseDefaultServiceProvider(options =>
        //                options.ValidateScopes = false);
        //    }
        //}



        public static IHostBuilder CreateHostBuilder(string[] args) =>
             Host.CreateDefaultBuilder(args)
                 .ConfigureWebHostDefaults(webBuilder =>
                 {
                     webBuilder.UseKestrel();
                     webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                     webBuilder.UseUrls("https://localhost:5000", "https://odin:5000", "https://192.168.1.100:5000");
                     webBuilder.UseIISIntegration();
                     webBuilder.UseStartup<Startup>();
                 }).UseDefaultServiceProvider(options =>
                 options.ValidateScopes = false);
    }
}
