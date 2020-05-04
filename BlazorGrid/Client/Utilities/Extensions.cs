using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGrid.Client.Utilities
{
    public static class Extensions
    {

        public static (bool Success, TDto Result) TryParseJson<TDto>(this string @this)
        {
            var success = true;
            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            var result = JsonConvert.DeserializeObject<TDto>(@this, settings);
            return (success, result);
        }

    }
}
