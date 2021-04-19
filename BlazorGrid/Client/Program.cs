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
using Serilog.Core;

namespace BlazorGrid.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            SelfLog.Enable(m => Console.Error.WriteLine(m));
            var levelSwitch = new LoggingLevelSwitch();


            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                //.MinimumLevel.Debug()
                .Enrich.WithProperty("InstanceId", Guid.NewGuid().ToString("n"))
                .Enrich.FromLogContext()
                .WriteTo.BrowserConsole()
                //.WriteTo.BrowserHttp($"{builder.HostEnvironment.BaseAddress}ingest", controlLevelSwitch: levelSwitch)
                .CreateLogger();

            Log.Debug("Starting Grid Example");
            
            builder.RootComponents.Add<App>("app");
            builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddTelerikBlazor();
            builder.Services.AddBlazoredLocalStorage();

            

            await builder.Build().RunAsync();
        }
    }
}
