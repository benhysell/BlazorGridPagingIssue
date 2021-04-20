using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGrid.Client.Pages.TreeList
{
    public class TreeListItemDto
    {
        public Guid Id { get; set; }

        public Guid? ParentId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
