using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Debugging;
using Serilog;
using BlazorGrid.Client.Utilities;
using Blazored.LocalStorage;

namespace BlazorGrid.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            SelfLog.Enable(m => Console.Error.WriteLine(m));            
            Log.Logger = new LoggerConfiguration()                                
                .Enrich.WithProperty("InstanceId", Guid.NewGuid().ToString("n"))
                .Enrich.FromLogContext()
                .WriteTo.BrowserConsole()                
                .CreateLogger();

            Log.Debug("Starting Grid Example");


            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTelerikBlazor();
            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}
