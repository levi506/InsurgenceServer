using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace AdminSiteNew
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var host = new WebHostBuilder()
                    .UseUrls("http://5.135.154.100:5003", "http://localhost:5003")
                    .UseKestrel()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseIISIntegration()
                    .UseStartup<Startup>()
                    .Build();

                host.Run();
            }
            else
            {
                var host = new WebHostBuilder()
                .UseUrls("http://localhost:5004")
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

                host.Run();
            }
                

        }
    }
}
