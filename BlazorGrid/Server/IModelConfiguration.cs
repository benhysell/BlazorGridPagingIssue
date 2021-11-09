using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.ModelBuilder;

namespace BlazorGrid.Server
{
    public interface IModelConfiguration
    {
        /// <summary>
        /// Applies model configurations using the provided builder for the specified API version.
        /// </summary>
        /// <param name="builder">The <see cref="ODataModelBuilder">builder</see> used to apply configurations.</param>
        /// <param name="apiVersion">The <see cref="ApiVersion">API version</see> associated with the <paramref name="builder"/>.</param>
        /// <param name="routePrefix">The route prefix associated with the configuration, if any.</param>
        void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix);
    }
}
