using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace radium_UTXO_server
{
    public class Program
    {
        public static ManualResetEvent shutdown_Event = new ManualResetEvent(false);
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().RunAsync();
            UtxoServer.LoadSaved();
            UtxoServer.SyncThread.Start();

            shutdown_Event.Reset();
            shutdown_Event.WaitOne();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
