using BlazorGrid.Client.Utilities;
using Microsoft.AspNetCore.Components;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Telerik.Blazor.Components;
using Telerik.Blazor.Extensions;

namespace BlazorGrid.Client.Pages
{
    public partial class WeatherForecast
    {
        /// <summary>
        /// Telerik Storage
        /// </summary>
        [Inject] public TelerikLocalStorage TelerikLocalStorage { get; set; }

        /// <summary>
        /// http client
        /// </summary>
        [Inject] public HttpClient HttpClient { get; set; }

        /// <summary>
        /// Weather Forecasts
        /// </summary>
        private List<BlazorGrid.Shared.WeatherForecast> _weatherForecast;

        /// <summary>
        /// Count
        /// </summary>
        private int _count;

        /// <summary>
        /// Logging
        /// </summary>
        private static readonly ILogger Log = Serilog.Log.ForContext<WeatherForecast>();

        /// <summary>
        /// First call on state change
        /// </summary>
        private bool _firstCallOnStateChanged = true;

        /// <summary>
        /// First call on read items
        /// </summary>
        private bool _firstCallReadItems = true;

        
        /// <summary>
        /// Read grid items...note if this is the first call check status of saved grid state for proper paging
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected async Task ReadItems(GridReadEventArgs args)
        {
            Log.Information("ReadItems Start");
            if (_firstCallReadItems)
            {
                Log.Information("ReadItems First Call");
                var args2 = new GridStateEventArgs<BlazorGrid.Shared.WeatherForecast>();
                
                await OnStateInitHandlerAsync(nameof(WeatherForecast), args2);
                if (null != args2.GridState && args.Request.Page != args2.GridState.Page)
                {                    
                    args.Request.Page = args2.GridState.Page.Value;
                }
                
                Log.Information($"ReadItems request page: {args.Request.Page} saved state page: {args2.GridState?.Page.Value}");
                _firstCallReadItems = false;
            }            
            
            await RequestDataAsync(args.Request.ToODataString());
            Log.Information("ReadItems Stop");
        }

        /// <summary>
        /// Make request to the backend
        /// </summary>
        /// <param name="oDataQuery"></param>
        /// <returns></returns>
        protected async Task RequestDataAsync(string oDataQuery)
        {
            var response = await HttpClient.GetAsync($"odata/v1/WeatherForecastOData?{oDataQuery}");
            var jsonString = await response.Content.ReadAsStringAsync();
            var result = jsonString.TryParseJson<ODataResponse<BlazorGrid.Shared.WeatherForecast>>();
            
            _weatherForecast = result.Result.Dtos;
            _count = result.Result.Count;
            StateHasChanged();
        }


        public async Task OnStateInitHandlerAsync(string nameOfElement, GridStateEventArgs<BlazorGrid.Shared.WeatherForecast> args)
        {
            try
            {
                Log.Information($"OnStateInitHandlerAsync Start");
                var state = await TelerikLocalStorage.GetItemAsync<GridState<BlazorGrid.Shared.WeatherForecast>>(nameOfElement);
                if (null != state)
                {
                    args.GridState = state;
                    Log.Information($"OnStateInitHandlerAsync Grid State Set");
                }
                else
                {
                    Log.Information($"OnStateInitHandlerAsync NULL STATE");
                }

            }
            catch (InvalidOperationException e)
            {
                // the JS Interop for the local storage cannot be used during pre-rendering
                // so the code above will throw. Once the app initializes, it will work fine
                Log.Error($"OnStateInitHandlerAsync error", e);
            }
            Log.Information($"OnStateInitHandlerAsync Stop");
        }

        public async void OnStateChangedHandlerAsync(string nameOfElement, GridStateEventArgs<BlazorGrid.Shared.WeatherForecast> args)
        {
            Log.Information($"OnStateChangedHandlerAsync Save State Start");
            if (!_firstCallOnStateChanged)
            {                
                await TelerikLocalStorage.SetItem(nameOfElement, args.GridState);
            }
            else
            {
                Log.Information($"OnStateChangedHandlerAsync First Call");
                _firstCallOnStateChanged = false;
            }
            Log.Information($"OnStateChangedHandlerAsync Save State Stop");
        }
    }
}
