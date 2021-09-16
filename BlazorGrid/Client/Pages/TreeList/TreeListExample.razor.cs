using Blazored.LocalStorage;
using BlazorGrid.Client.Utilities;
using BlazorGrid.Shared;
using Microsoft.AspNetCore.Components;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Telerik.Blazor.Components;
using Telerik.Blazor.Extensions;

namespace BlazorGrid.Client.Pages.TreeList
{
    public partial class TreeListExample
    {
        [Inject] public ILocalStorageService LocalStorage { get; set; }

        public List<TreeListItemDto> TreeListItems { get; set; }


        protected override async Task OnInitializedAsync()
        {
            //Build a simple flat list
            var topLevelParent = new TreeListItemDto() { Id = Guid.Empty, ParentId = null, Name = "Top", Description = "top level" };
            TreeListItems = new List<TreeListItemDto>()
            {
                new TreeListItemDto() { Id = Guid.Empty, ParentId = null, Name = "Top", Description = "top level" },
                new TreeListItemDto() { Id = new Guid("62eef57c-bb1a-4b88-9dc0-a9b3f5fa42c3"), ParentId = Guid.Empty, Name = "Child A", Description = "A" },
                new TreeListItemDto() { Id = new Guid("ccaa293a-31ae-46ae-9980-c0dce1bd0a58"), ParentId = Guid.Empty, Name = "Child B", Description = "B" },
                new TreeListItemDto() { Id = new Guid("dedc0013-f4c0-4252-b4f9-063f0c0a05c9"), ParentId = Guid.Empty, Name = "Child C", Description = "C" },
                new TreeListItemDto() { Id = new Guid("19c414e1-59de-4fa8-8c79-fb3eb734a6b7"), ParentId = Guid.Empty, Name = "Child D", Description = "D" },

                new TreeListItemDto() { Id = new Guid("741dead1-2f11-43cb-b872-abd1bc7932d1"), ParentId = new Guid("62eef57c-bb1a-4b88-9dc0-a9b3f5fa42c3"), Name = "Child AA", Description = "AA" },
                new TreeListItemDto() { Id = new Guid("c524cd57-ab88-410f-b57c-c0dd23833df1"), ParentId = new Guid("62eef57c-bb1a-4b88-9dc0-a9b3f5fa42c3"), Name = "Child AB", Description = "AB" },
                new TreeListItemDto() { Id = new Guid("296ca930-22f8-4f1d-9a9b-91c9536d2535"), ParentId = new Guid("62eef57c-bb1a-4b88-9dc0-a9b3f5fa42c3"), Name = "Child AC", Description = "AC" },

                new TreeListItemDto() { Id = new Guid("56a37218-4c5f-4fea-8ad7-6ee65e6f6ab0"), ParentId = new Guid("ccaa293a-31ae-46ae-9980-c0dce1bd0a58"), Name = "Child BA", Description = "BA" }
            };
        }

        async Task OnStateInitHandler(TreeListStateEventArgs<TreeListItemDto> args)
        {
            var localStorage = await LocalStorage.GetItemAsync<TreeListState<TreeListItemDto>>("TreeListExample");
            if (null == localStorage)
            {
                Log.Warning("Empty state");
                localStorage = new TreeListState<TreeListItemDto>()
                {
                    //collapse all items in the TreeList upon initialization of the state
                    ExpandedItems = new List<TreeListItemDto>()
                };
            }
            //Log.Warning($"Tree State {localStorage.ExpandedItems.Count()}");
            args.TreeListState = localStorage;
        }

        async Task OnStateChangedHandler(TreeListStateEventArgs<TreeListItemDto> args)
        {
            var state = args.TreeListState;            
            await LocalStorage.SetItemAsync("TreeListExample", state);
        }
    }
}
