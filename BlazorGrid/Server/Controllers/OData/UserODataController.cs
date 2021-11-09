using BlazorGrid.Shared;
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
    public class UserODataController : ODataController
    {
        [EnableQuery]        
        public IQueryable<User> Get()
        {
            //normally goto a database, for now we'll generate data
            var users = new List<User>();
            
            for (int i = 0; i < 50; i++)
            {
                var newForeforcast = new User() { Id = Guid.NewGuid(), Name = $"name {i}", Email = $"email {i}", UserName = $"username {i}" };
                users.Add(newForeforcast); 
            }

            return users.AsQueryable();
        }
    }
}
