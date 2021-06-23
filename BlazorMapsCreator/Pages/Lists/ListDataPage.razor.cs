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
    public partial class ListDataPage
    {
        [Inject] IConfiguration Configuration { get; set; }

        private string geography;
        private string subscriptionkey;

        private List<string> messages = new();
        private MapDataListResponse mapDataResponse;

        public List<IDetailsRowColumn<MapDataDetailInfo>> Columns = new();
        Selection<MapDataDetailInfo> Selection = new();

        private void GetData()
        {

            RestClient client = new($"https://{geography}.atlas.microsoft.com/mapdata?subscription-key={subscriptionkey}&api-version=2.0")
            {
                Timeout = -1
            };
            RestRequest request = new(Method.GET);

            IRestResponse response = client.Execute(request);


            if (response.IsSuccessful)
            {
                mapDataResponse = JsonSerializer.Deserialize<MapDataListResponse>(response.Content);
                mapDataResponse.mapDataList = new List<MapDataDetailInfo>(mapDataResponse.mapDataList.OrderBy(x => x.created));
            }

        }
        private void OnClick(MapDataDetailInfo item)
        {
            Console.WriteLine("Clicked!");
        }


        protected override void OnInitialized()
        {
            geography = Configuration["AzureMaps:Geography"];
            subscriptionkey = Configuration["AzureMaps:SubscriptionKey"];

            Selection.GetKey = (item => item.udid);
            Columns.Add(new DetailsRowColumn<MapDataDetailInfo>("Udid", x => x.udid) { MaxWidth = 150, IsResizable = true, Index = 0 });
            Columns.Add(new DetailsRowColumn<MapDataDetailInfo>("Created", x => x.created!) { Index = 1, MaxWidth = 150, IsResizable = true, OnColumnClick = OrderCreated });
            Columns.Add(new DetailsRowColumn<MapDataDetailInfo>("Updated", x => x.updated!) { Index = 2, MaxWidth = 150, IsResizable = true, OnColumnClick = OrderUpdated });
            Columns.Add(new DetailsRowColumn<MapDataDetailInfo>("Status", x => x.uploadStatus!) { Index = 3, MaxWidth = 100, IsResizable = true });
            Columns.Add(new DetailsRowColumn<MapDataDetailInfo>("Size (kB)", x => x.sizeInBytes!) { Index = 4, MaxWidth = 100, IsResizable = true, OnColumnClick = OrderSize });
            Columns.Add(new DetailsRowColumn<MapDataDetailInfo>("Format", x => x.dataFormat!) { Index = 5, MaxWidth = 100, IsResizable = true });
            Columns.Add(new DetailsRowColumn<MapDataDetailInfo>("Location", x => x.location!) { Index = 6, MaxWidth = 450, IsResizable = true });
            Columns.Add(new DetailsRowColumn<MapDataDetailInfo>("Description", x => x.description) { Index = 7 });

            GetData();

            base.OnInitialized();
        }

        private void OrderCreated(IDetailsRowColumn<MapDataDetailInfo> column)
        {
            // since we're creating a new list, we need to make a copy of what was previously selected
            var selected = Selection.GetSelection();

            //create new sorted list
            mapDataResponse.mapDataList = new List<MapDataDetailInfo>(column.IsSorted ? mapDataResponse.mapDataList.OrderBy(x => x.created) : mapDataResponse.mapDataList.OrderByDescending(x => x.created));

            column.IsSorted = !column.IsSorted;
            StateHasChanged();
        }

        private void OrderUpdated(IDetailsRowColumn<MapDataDetailInfo> column)
        {
            // since we're creating a new list, we need to make a copy of what was previously selected
            var selected = Selection.GetSelection();

            //create new sorted list
            mapDataResponse.mapDataList = new List<MapDataDetailInfo>(column.IsSorted ? mapDataResponse.mapDataList.OrderBy(x => x.updated) : mapDataResponse.mapDataList.OrderByDescending(x => x.updated));

            column.IsSorted = !column.IsSorted;
            StateHasChanged();
        }

        private void OrderSize(IDetailsRowColumn<MapDataDetailInfo> column)
        {
            // since we're creating a new list, we need to make a copy of what was previously selected
            var selected = Selection.GetSelection();

            //create new sorted list
            mapDataResponse.mapDataList = new List<MapDataDetailInfo>(column.IsSorted ? mapDataResponse.mapDataList.OrderBy(x => x.sizeInBytes) : mapDataResponse.mapDataList.OrderByDescending(x => x.sizeInBytes));

            column.IsSorted = !column.IsSorted;
            StateHasChanged();
        }

        private void Delete()
        {
            messages.Clear();

            foreach (MapDataDetailInfo item in Selection.GetSelection())
            {
                RestClient client = new($"https://{geography}.atlas.microsoft.com/mapdata/{item.udid}?subscription-key={subscriptionkey}&api-version=2.0")
                {
                    Timeout = -1
                };
                RestRequest request = new(Method.DELETE);

                IRestResponse response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    messages.Add($"Data with '{item.udid}' has been deleted");
                }
                mapDataResponse.mapDataList.Remove(item);
            }
            Selection.ClearSelection();
            StateHasChanged();
        }
    }
}