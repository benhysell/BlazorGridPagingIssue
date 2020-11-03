﻿using BlazorGrid.Shared;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGrid.Server.Controllers.OData
{
    public class WeatherForecastODataModelConfiguration : IModelConfiguration
    {
        /// <summary>
        /// Applies model configurations using the provided builder for the specified API version.
        /// </summary>
        /// <param name="builder">The <see cref="ODataModelBuilder">builder</see> used to apply configurations.</param>
        /// <param name="apiVersion">The <see cref="ApiVersion">API version</see> associated with the <paramref name="builder"/>.</param>
        public void Apply(ODataModelBuilder builder, ApiVersion apiVersion)
        {
            //allows us to version the entity used as well

            //map entity to name of controller without 'controller'
            //var order = builder.EntitySet<BasePartNumberDto>("BasePartNumberOData");//                .EntityType.HasKey(o => o.Id);

            builder.EntitySet<WeatherForecast>("WeatherForecastOData").EntityType.HasKey(x=>x.Date);

            //if (apiVersion < ApiVersions.V2)
            //{
            //    order.Ignore(o => o.EffectiveDate);
            //}

            //if (apiVersion < ApiVersions.V3)
            //{
            //    order.Ignore(o => o.Description);
            //}

            //if (apiVersion >= ApiVersions.V1)
            //{
            //    order.Collection.Function("MostExpensive").ReturnsFromEntitySet<Order>("Orders");
            //}

            //if (apiVersion >= ApiVersions.V2)
            //{
            //    order.Action("Rate").Parameter<int>("rating");
            //}
        }
    }
}
