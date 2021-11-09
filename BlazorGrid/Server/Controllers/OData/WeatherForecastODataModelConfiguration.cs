using BlazorGrid.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.ModelBuilder;
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
        public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string routePrefix)
        {            
            builder.EntitySet<WeatherForecast>("WeatherForecastOData").EntityType.HasKey(x=>x.TemperatureC);

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
