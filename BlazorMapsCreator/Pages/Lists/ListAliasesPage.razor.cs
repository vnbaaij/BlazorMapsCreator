using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

using BlazorFluentUI;

using BlazorMapsCreator.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;

using RestSharp;

namespace BlazorMapsCreator.Pages.Lists
{
    public partial class ListAliasesPage
    {
        [Inject] IConfiguration Configuration { get; set; }

        private string geography;
        private string subscriptionkey;

        private List<string> messages = new();
        private AliasListResponse aliasListResponse;

        public List<IDetailsRowColumn<AliasListItem>> Columns = new();
        Selection<AliasListItem> Selection = new();

        private void GetData()
        {

            RestClient client = new($"https://{geography}.atlas.microsoft.com/aliasses?subscription-key={subscriptionkey}&api-version=2.0")
            {
                Timeout = -1
            };
            RestRequest request = new(Method.GET);

            IRestResponse response = client.Execute(request);


            if (response.IsSuccessful)
            {
                aliasListResponse = JsonSerializer.Deserialize<AliasListResponse>(response.Content);
                aliasListResponse.aliases = new List<AliasListItem>(aliasListResponse.aliases.OrderBy(x => x.createdTimestamp));
            }

        }
        private void OnClick(AliasListItem item)
        {
            Console.WriteLine("Clicked!");
        }


        protected override void OnInitialized()
        {
            geography = Configuration["AzureMaps:Geography"];
            subscriptionkey = Configuration["AzureMaps:SubscriptionKey"];

            Selection.GetKey = (item => item.aliasId);
            Columns.Add(new DetailsRowColumn<AliasListItem>("Udid", x => x.aliasId) { MaxWidth = 150, IsResizable = true, Index = 0 });
            Columns.Add(new DetailsRowColumn<AliasListItem>("Created", x => x.createdTimestamp!) { Index = 1, MaxWidth = 150, IsResizable = true, OnColumnClick = OrderCreated });
            Columns.Add(new DetailsRowColumn<AliasListItem>("Updated", x => x.lastUpdatedTimestamp!) { Index = 2, MaxWidth = 150, IsResizable = true, OnColumnClick = OrderUpdated });
            //Columns.Add(new DetailsRowColumn<AliasMetadata>("Dataitem", x => x.creatorDataItemId ) { Index = 3, MaxWidth = 100, IsResizable = true });


            GetData();

            base.OnInitialized();
        }

        private void OrderCreated(IDetailsRowColumn<AliasListItem> column)
        {
            // since we're creating a new list, we need to make a copy of what was previously selected
            var selected = Selection.GetSelection();

            //create new sorted list
            aliasListResponse.aliases = new List<AliasListItem>(column.IsSorted ? aliasListResponse.aliases.OrderBy(x => x.createdTimestamp) : aliasListResponse.aliases.OrderByDescending(x => x.createdTimestamp));

            column.IsSorted = !column.IsSorted;
            StateHasChanged();
        }

        private void OrderUpdated(IDetailsRowColumn<AliasListItem> column)
        {
            // since we're creating a new list, we need to make a copy of what was previously selected
            var selected = Selection.GetSelection();

            //create new sorted list
            aliasListResponse.aliases = new List<AliasListItem>(column.IsSorted ? aliasListResponse.aliases.OrderBy(x => x.lastUpdatedTimestamp) : aliasListResponse.aliases.OrderByDescending(x => x.lastUpdatedTimestamp));

            column.IsSorted = !column.IsSorted;
            StateHasChanged();
        }



        private void Delete()
        {
            messages.Clear();

            foreach (AliasListItem item in Selection.GetSelection())
            {
                RestClient client = new($"https://{geography}.atlas.microsoft.com/aliases/{item.aliasId}?subscription-key={subscriptionkey}&api-version=2.0")
                {
                    Timeout = -1
                };
                RestRequest request = new(Method.DELETE);

                IRestResponse response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    messages.Add($"Data with '{item.aliasId}' has been deleted");
                }
                aliasListResponse.aliases.Remove(item);
            }
            Selection.ClearSelection();
            StateHasChanged();
        }
    }
}