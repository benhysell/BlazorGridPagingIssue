using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BlazorGrid.Shared
{
    public class User : IdentityUser<Guid>
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
    }
}
