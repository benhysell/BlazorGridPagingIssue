using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Builder;
using BlazorGrid.Shared;
using Microsoft.OData.Edm;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorGrid.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddNewtonsoftJson();



            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
            }
           );
            services.AddOData().EnableApiVersioning();
            services.AddODataApiExplorer(
          options =>
          {
                  // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                  // note: the specified format code will format the version as "'v'major[.minor][-status]"
                  options.GroupNameFormat = "'v'VVV";

                  // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                  // can also be used to control the format of the API version in route templates
                  options.SubstituteApiVersionInUrl = true;
          });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, VersionedODataModelBuilder modelBuilder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.EnableDependencyInjection();
                endpoints.Map("api/{**slug}", HandleApiFallback);                
                endpoints.MapFallbackToFile("index.html");
                endpoints.Count().Filter().OrderBy().Select().MaxTop(null).Expand();
                endpoints.ServiceProvider.GetRequiredService<ODataOptions>().UrlKeyDelimiter = Microsoft.OData.ODataUrlKeyDelimiter.Parentheses;                
                endpoints.MapVersionedODataRoute("odata", "odata/v{version:apiVersion}", modelBuilder); //this 'should' build up all of our odata end points, code left above to grab the rest.  Not sure why this returns more than 1
            });
        }

        /// <summary>
        /// return 404 when route not found for backend
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Task HandleApiFallback(HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return Task.FromResult(0);
        }
    }
}

