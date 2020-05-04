using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Telerik.DataSource;

namespace BlazorGrid.Client.Utilities
{
    public class TelerikLocalStorage
    {
        protected IJSRuntime _jSRuntimeInstance { get; set; }

        public TelerikLocalStorage(IJSRuntime jsRuntime)
        {
            _jSRuntimeInstance = jsRuntime;
        }

        public ValueTask SetItem(string key, object data)
        {
            return _jSRuntimeInstance.InvokeVoidAsync("localStorage.setItem", new object[] {
            key,
            JsonConvert.SerializeObject(data)
        });
        }

        public async Task<T> GetItemAsync<T>(string key)
        {
            var data = await _jSRuntimeInstance.InvokeAsync<string>("localStorage.getItem", key);
            if (!string.IsNullOrEmpty(data))
            {
                return JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings()
                {
                    Converters = new JsonConverter[]
                    {
                    new FilterDescriptorJsonConverter()
                    },
                    NullValueHandling = NullValueHandling.Ignore
                });
            }
            else
            {
                Log.Error($"Empty Data, {key}, {data}");
            }

            return default;
        }

        public ValueTask RemoveItem(string key)
        {
            return _jSRuntimeInstance.InvokeVoidAsync("localStorage.removeItem", key);
        }
    }

    // to store the serialized grid state, we need to have a custom serialized
    // based on Newtonsoft.Json serialization. In the future this may not be required
    public class FilterDescriptorJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(FilterDescriptorBase);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var filterDescriptor = JObject.Load(reader);

            return filterDescriptor.ToObject<FilterDescriptor>(serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTelerikLocalStorage(this IServiceCollection services)
        {
            return services
                .AddScoped<TelerikLocalStorage>()
                ;
        }
    }
}
