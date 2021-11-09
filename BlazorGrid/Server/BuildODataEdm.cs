using BlazorGrid.Server.Controllers.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace BlazorGrid.Server
{
    public class BuildODataEdm
    {
        /// <summary>
        /// Build V1
        /// </summary>
        /// <returns></returns>
        public static IEdmModel BuildV1Model()
        {
            var builder = new ODataConventionModelBuilder();
            new UserODataModelConfiguration().Apply(builder, new ApiVersion(1, 0), string.Empty);
            new WeatherForecastODataModelConfiguration().Apply(builder, new ApiVersion(1, 0), string.Empty);
            
            return builder.GetEdmModel();
        }
    }
}
