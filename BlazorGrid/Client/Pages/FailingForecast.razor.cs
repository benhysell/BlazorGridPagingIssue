using BlazorGrid.Client.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;
using Telerik.Blazor.Components;
using Telerik.Blazor.Extensions;

namespace BlazorGrid.Client.Pages
{
    public partial class FailingForecast
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
        /// weather forecast
        /// </summary>
        private List<BlazorGrid.Shared.WeatherForecast> _weatherForecast;

        /// <summary>
        /// total number of forecasts
        /// </summary>
        private int _count;

        /// <summary>
        /// Logging
        /// </summary>
        private static readonly ILogger Log = Serilog.Log.ForContext<FailingForecast>();

        /// <summary>
        /// bool for first call of state change
        /// </summary>
        private bool _firstCallOnStateChanged = true;
        
        /// <summary>
        /// Read grid items...note if this is the first call check status of saved grid state for proper paging
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected async Task ReadItems(GridReadEventArgs args)
        {
            Log.Information("ReadItems Start");            
            
            //query local storage for state of grid
            var args2 = await TelerikLocalStorage.GetItemAsync<GridState<BlazorGrid.Shared.WeatherForecast>>(nameof(FailingForecast));           
            Log.Information($"ReadItems request page: {args.Request.Page} saved state page: {args2?.Page.Value}");            
           
            //make request to backend
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
                    Log.Information($"OnStateInitHandlerAsync Grid State Set page: {state.Page}");
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
