using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

using BlazorFluentUI;

using BlazorMapsCreator.Models;

using RestSharp;

namespace BlazorMapsCreator.Pages
{
    public partial class ListAliasesPage : PageBase<AliasListItem>
    {
        private AliasListResponse aliasListResponse;

        private void GetData()
        {

            RestClient client = new($"https://{Geography}.atlas.microsoft.com/aliasses?subscription-key={SubscriptionKey}&api-version=2.0")
            {
                Timeout = -1
            };
            RestRequest request = new(Method.GET);

            IRestResponse response = client.Execute(request);


            if (response.IsSuccessful)
            {
                aliasListResponse = JsonSerializer.Deserialize<AliasListResponse>(response.Content);
                itemList = new List<AliasListItem>(itemList.OrderBy(x => x.createdTimestamp));
            }

        }
        private void OnClick(AliasListItem item)
        {
            Console.WriteLine("Clicked!");
        }


        protected override void OnInitialized()
        {
            base.OnInitialized();

            Selection.GetKey = (item => item.aliasId);
            Columns.Add(new DetailsRowColumn<AliasListItem>("Udid", x => x.aliasId) { MaxWidth = 150, IsResizable = true, Index = 0 });
            Columns.Add(new DetailsRowColumn<AliasListItem>("Created", x => x.createdTimestamp!) { Index = 1, MaxWidth = 150, IsResizable = true, OnColumnClick = OrderCreated });
            Columns.Add(new DetailsRowColumn<AliasListItem>("Updated", x => x.lastUpdatedTimestamp!) { Index = 2, MaxWidth = 150, IsResizable = true, OnColumnClick = OrderUpdated });
            //Columns.Add(new DetailsRowColumn<AliasMetadata>("Dataitem", x => x.creatorDataItemId ) { Index = 3, MaxWidth = 100, IsResizable = true });

            GetData();
        }

        private void OrderCreated(IDetailsRowColumn<AliasListItem> column)
        {
            // since we're creating a new list, we need to make a copy of what was previously selected
            var selected = Selection.GetSelection();

            //create new sorted list
            itemList = new List<AliasListItem>(column.IsSorted ? itemList.OrderBy(x => x.createdTimestamp) : itemList.OrderByDescending(x => x.createdTimestamp));

            column.IsSorted = !column.IsSorted;
            StateHasChanged();
        }

        private void OrderUpdated(IDetailsRowColumn<AliasListItem> column)
        {
            // since we're creating a new list, we need to make a copy of what was previously selected
            var selected = Selection.GetSelection();

            //create new sorted list
            itemList = new List<AliasListItem>(column.IsSorted ? itemList.OrderBy(x => x.lastUpdatedTimestamp) : itemList.OrderByDescending(x => x.lastUpdatedTimestamp));

            column.IsSorted = !column.IsSorted;
            StateHasChanged();
        }



        private void Delete()
        {
            messages.Clear();

            foreach (AliasListItem item in Selection.GetSelection())
            {
                RestClient client = new($"https://{Geography}.atlas.microsoft.com/aliases/{item.aliasId}?subscription-key={SubscriptionKey}&api-version=2.0")
                {
                    Timeout = -1
                };
                RestRequest request = new(Method.DELETE);

                IRestResponse response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    messages.Add(new MessageItem($"Item with id '{item.aliasId}' has been deleted"));
                }
                itemList.Remove(item);
            }
            Selection.ClearSelection();
            StateHasChanged();
        }
    }
}