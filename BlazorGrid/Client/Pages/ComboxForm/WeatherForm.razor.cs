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

namespace BlazorGrid.Client.Pages.ComboxForm
{
    public partial class WeatherForm
    {
        [Parameter] public int? TempForCombobox { get; set; } = null;

        /// <summary>
        /// The Form Element
        /// </summary>
        public FormData FormElement { get; set; } = new FormData();


        /// <summary>
        /// http client
        /// </summary>
        [Inject] public HttpClient HttpClient { get; set; }
        
        /// <summary>
        /// Logging
        /// </summary>
        private static readonly ILogger Log = Serilog.Log.ForContext<WeatherForecast>();

        private CancellationTokenSource _searchCancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// WeatherForecasts
        /// </summary>
        private List<BlazorGrid.Shared.WeatherForecast> _weatherForecasts { get; set; }

        /// <summary>
        /// Override for on read, so we don't goto the backend
        /// </summary>
        private bool SkipOnRead = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (null == TempForCombobox)
            {
                Log.Warning("NULL TempForCombox");
                //PerformPost = true;
                await OnReadAsync(null);
            }
            else
            {
                Log.Warning($"TempForCombox {TempForCombobox.Value}");
                FormElement.Temp = TempForCombobox.Value;
            }
        }

        //protected override async Task OnParametersSetAsync()
        //{
        //    base.OnParametersSet();
        //    if (null == TempForCombobox)
        //    {
        //        Log.Warning("NULL TempForCombox");
        //        //PerformPost = true;
        //        await OnReadAsync(null);
        //    }
        //    else
        //    {
        //        Log.Warning($"TempForCombox {TempForCombobox.Value}");
        //        FormElement.Temp = TempForCombobox.Value;
        //    }
        //}


        /// <summary>
        /// Placeholder to simulate going to the backend
        /// </summary>
        /// <returns></returns>
        protected async Task HandleValidSubmitAsync()
        {
            
            //simulate sending to the backend
        }

        /// <summary>
        /// Read Graph Node Logic
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task OnReadAsync(ComboBoxReadEventArgs args)
        {
            if (!SkipOnRead)
            {
                _searchCancellationTokenSource.Cancel(false);
                _searchCancellationTokenSource.Dispose();
                _searchCancellationTokenSource = new CancellationTokenSource();
                var oDataQuery = "$top=10&$skip=0&";
                var delay = 0;
                if (args?.Request.Filters.Count > 0) // there is user filter input, skips providing data on initialization
                {
                    oDataQuery = $"{oDataQuery}&{args.Request.ToODataString()}";
                    delay = 500;
                }
                await Task.Delay(delay, _searchCancellationTokenSource.Token);

                var response = await HttpClient.GetAsync($"odata/v1/WeatherForecastOData?{oDataQuery}");
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = jsonString.TryParseJson<ODataResponse<BlazorGrid.Shared.WeatherForecast>>();
                _weatherForecasts = result.Result.Dtos;
               
            }
            else
            {
                SkipOnRead = false;
            }
        }
    }
}
