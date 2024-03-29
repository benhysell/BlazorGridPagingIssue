﻿using BlazorGrid.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace BlazorGrid.Server.Controllers.OData
{
    [ApiVersion("1.0")]
    public class WeatherForecastODataController : ODataController
    {
        [EnableQuery]        
        public IQueryable<WeatherForecast> Get()
        {
            //normally goto a database, for now we'll generate data
            var weatherForecasts = new List<WeatherForecast>();
            var startDate = DateTime.Now;

            for (int i = 0; i < 50; i++)
            {
                var newForeforcast = new WeatherForecast() { Date = startDate.AddHours(i), Summary = $"summary {i}", TemperatureC = i, BadWeather = i % 2 == 0, TemperatureK = (i + 273).ToString() };
                weatherForecasts.Add(newForeforcast);
            }

            return weatherForecasts.AsQueryable();
        }
    }
}
