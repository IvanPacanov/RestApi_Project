using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestApi_Dicom.Data;

namespace RestApi_Dicom
{
    public class test
    {

        public string z1 { get; set; }
        public string z2 { get; set; }
        public string typ { get; set; }
        public string poziom { get; set; }
        public string z3 { get; set; }
    }
    public enum EQueryLevel
    {
        ePatient = 0,
        eStudy = 1,
        eSeries = 2,
        eImage = 3
    }
    public enum ERootType
    {
        ePatientRootType = 0,
        eStudyRootType = 1
    }
    public class Program
    {
        private static string ip;
        public static void Main(string[] args)
        {
            Console.WriteLine("Enter the IP with post <ip>:<port>");
            ip = Console.ReadLine();
            //"https://192.168.1.104:5000"
            ip = "https://" + ip;
            Console.Clear();
            Console.WriteLine("Starting the server");
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
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
                     Console.WriteLine("Checking IP...");
                     webBuilder.UseUrls("https://localhost:5000", "https://odin:5000", ip);
                     Console.WriteLine("Ip Correct");
                     webBuilder.UseIISIntegration();
                     webBuilder.UseStartup<Startup>();

                     Console.Clear();
                     Console.WriteLine("Server started");
                 }).UseDefaultServiceProvider(options =>
                 options.ValidateScopes = false);
    }
}
