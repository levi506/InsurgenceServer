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
            var host = new WebHostBuilder()
#if DEBUG
                .UseUrls("http://localhost:5003")
#else
                .UseUrls("http://5.135.154.100:5003", "http://localhost:5003")
#endif
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();
            host.Run();
        }
    }
}
