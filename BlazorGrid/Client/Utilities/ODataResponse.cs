using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGrid.Client.Utilities
{
    public class ODataResponse<T>
    {
        [JsonProperty("@odata.count")]
        public int Count { get; set; }

        [JsonProperty("value")]
        public List<T> Dtos { get; set; } = new List<T>();
    }
}
