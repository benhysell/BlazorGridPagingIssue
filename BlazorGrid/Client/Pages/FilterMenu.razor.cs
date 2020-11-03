using Blazored.LocalStorage;
using BlazorGrid.Client.Utilities;
using BlazorGrid.Shared;
using Microsoft.AspNetCore.Components;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Telerik.Blazor.Components;
using Telerik.Blazor.Extensions;
using Telerik.DataSource;

namespace BlazorGrid.Client.Pages
{
    public partial class FilterMenu
    {
        /// <summary>
        /// Local Storage
        /// </summary>
        [Inject] public ILocalStorageService LocalStorage { get; set; }

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

        public string FilterArgs { get; set; }

        public TelerikGrid<BlazorGrid.Shared.WeatherForecast> GridReference { get; set; }

        /// <summary>
        /// Search token
        /// </summary>
        protected CancellationTokenSource _searchCancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Address Telerik Paging Issues
        /// </summary>
        /// <param name="nameOfElement"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task FirstReadGridAsync(string nameOfElement, GridReadEventArgs args)
        {
            if (_firstCallReadItems)
            {
                //fix for telerik paging issues                
                var args2 = new GridStateEventArgs<WeatherForecast>();
                await OnStateInitHandlerAsync(nameOfElement, args2);
                if (null != args2.GridState && args.Request.Page != args2.GridState.Page)
                {
                    args.Request.Page = args2.GridState.Page.Value;
                }
                _firstCallReadItems = false;
            }
            FilterArgs = args.Request.ToODataString();
        }


        /// <summary>
        /// Read Items Odata
        /// </summary>
        /// <param name="nameOfElement"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual async Task ReadItemsODataAsync(string nameOfElement, GridReadEventArgs args, string oDataInputString = null)
        {
            if (null == oDataInputString)
            {
                await FirstReadGridAsync(nameOfElement, args);
                oDataInputString = args.Request.ToODataString();

            }
            _searchCancellationTokenSource.Cancel(false);
            _searchCancellationTokenSource.Dispose();
            _searchCancellationTokenSource = new CancellationTokenSource();
            await Task.Delay(50, _searchCancellationTokenSource.Token).ContinueWith(async t =>
            {
                try
                {
                    var response = await HttpClient.GetAsync($"odata/v1/WeatherForecastOData?{oDataInputString}");
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = jsonString.TryParseJson<ODataResponse<BlazorGrid.Shared.WeatherForecast>>();

                    _weatherForecast = result.Result.Dtos;
                    _count = result.Result.Count;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Read Data Async");
                    throw;
                }
                finally
                {
                    StateHasChanged();
                }
            }, _searchCancellationTokenSource.Token
           );

        }



        /// <summary>
        /// On State Init Handler 
        /// </summary>
        /// <param name="nameOfElement"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual async Task OnStateInitHandlerAsync(string nameOfElement, GridStateEventArgs<BlazorGrid.Shared.WeatherForecast> args, string sortName = "")
        {
            try
            {
                var state = await LocalStorage.GetItemAsync<GridState<BlazorGrid.Shared.WeatherForecast>>(nameOfElement);
                if (null != state)
                {
                    args.GridState = state;
                }
                else if (string.Empty != sortName)
                {
                    if (0 == (GridReference.GetState()).SortDescriptors.Count())
                    {
                        state = new GridState<WeatherForecast>
                        {
                            SortDescriptors = new List<Telerik.DataSource.SortDescriptor>
                        { new SortDescriptor { Member = sortName, SortDirection = ListSortDirection.Ascending } }
                        };
                        args.GridState = state;
                    }
                }
            }
            catch (InvalidOperationException e)
            {
                Log.Error(e, "On State Init Exception");
                // the JS Interop for the local storage cannot be used during pre-rendering
                // so the code above will throw. Once the app initializes, it will work fine
            }
        }


        public async void OnStateChangedHandlerAsync(string nameOfElement, GridStateEventArgs<BlazorGrid.Shared.WeatherForecast> args)
        {
            Log.Information($"OnStateChangedHandlerAsync Save State Start");
            if (!_firstCallOnStateChanged)
            {
                await LocalStorage.SetItemAsync(nameOfElement, args.GridState);
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
