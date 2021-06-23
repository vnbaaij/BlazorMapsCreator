using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

using BlazorFluentUI;

using BlazorMapsCreator.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;

using RestSharp;

namespace BlazorMapsCreator.Pages.Lists
{
    public partial class ListTilesetsPage
    {
        [Inject] IConfiguration Configuration { get; set; }

        private string geography;
        private string subscriptionkey;
        private MarkupString details;

        private List<string> messages = new();
        private TilesetListResponse tilesetResponse;

        public List<IDetailsRowColumn<TilesetDetailInfo>> Columns = new();
        Selection<TilesetDetailInfo> Selection = new();

        private void GetData()
        {

            RestClient client = new($"https://{geography}.atlas.microsoft.com/tilesets?subscription-key={subscriptionkey}&api-version=2.0")
            {
                Timeout = -1
            };
            RestRequest request = new(Method.GET);

            IRestResponse response = client.Execute(request);


            if (response.IsSuccessful)
            {
                tilesetResponse = JsonSerializer.Deserialize<TilesetListResponse>(response.Content);
            }

        }
        private void OnClick(TilesetDetailInfo item)
        {
            Console.WriteLine("Clicked!");
        }


        protected override void OnInitialized()
        {
            geography = Configuration["AzureMaps:Geography"];
            subscriptionkey = Configuration["AzureMaps:SubscriptionKey"];

            Selection.GetKey = (item => item.tilesetId);
            Columns.Add(new DetailsRowColumn<TilesetDetailInfo>("Tileset Id", x => x.tilesetId) { MaxWidth = 150, IsResizable = true, Index = 0 });
            Columns.Add(new DetailsRowColumn<TilesetDetailInfo>("Dataset Id", x => x.datasetId) { Index = 1, MaxWidth = 150, IsResizable = true });
            //Columns.Add(new DetailsRowColumn<TilesetDetailInfo>("Min zoom", x => x.minZoom!) { Index = 2, MaxWidth = 150, IsResizable = true });
            //Columns.Add(new DetailsRowColumn<TilesetDetailInfo>("Max zoom", x => x.maxZoom!) { Index = 3, MaxWidth = 100, IsResizable = true });
            //Columns.Add(new DetailsRowColumn<TilesetDetailInfo>("Ontology", x => x.ontology) { Index = 4, MaxWidth = 100, IsResizable = true });
            //Columns.Add(new DetailsRowColumn<TilesetDetailInfo>("Description", x => x.description) { Index = 7 });

            GetData();

            base.OnInitialized();
        }


        private void Delete()
        {
            messages.Clear();

            foreach (TilesetDetailInfo item in Selection.GetSelection())
            {
                RestClient client = new($"https://{geography}.atlas.microsoft.com/tilesets/{item.tilesetId}?subscription-key={subscriptionkey}&api-version=2.0")
                {
                    Timeout = -1
                };
                RestRequest request = new(Method.DELETE);

                IRestResponse response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    messages.Add($"Data with '{item.tilesetId}' has been deleted");
                }
                tilesetResponse.tilesets.Remove(item);
            }
            Selection.ClearSelection();
            StateHasChanged();
        }

        private void Get()
        {
            messages.Clear();

            TilesetDetailInfo item = Selection.GetSelection()[0];

            RestClient client = new($"https://{geography}.atlas.microsoft.com/tilesets/{item.tilesetId}?subscription-key={subscriptionkey}&api-version=2.0")
            {
                Timeout = -1
            };

            RestRequest request = new(Method.GET);

            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful)
            {
                TilesetDetailInfo tilesetGetResponse = JsonSerializer.Deserialize<TilesetDetailInfo>(response.Content);
                if (tilesetGetResponse != null)
                {
                    StringBuilder sb = new StringBuilder(tilesetGetResponse.description);
                    sb.AppendLine($"Dataset: {tilesetGetResponse.datasetId}");

                    sb.AppendLine("Bounding box:");
                    sb.AppendLine($"  Min lon/lat: {tilesetGetResponse.bbox[0]}, {tilesetGetResponse.bbox[1]}");
                    sb.AppendLine($"  Max lon/lat: {tilesetGetResponse.bbox[2]}, {tilesetGetResponse.bbox[3]}");
                    sb.AppendLine($"Min zoom: {tilesetGetResponse.minZoom}");
                    sb.AppendLine($"Max zoom: {tilesetGetResponse.maxZoom}");
                    sb.AppendLine($"Ontology: {tilesetGetResponse.ontology}");
                    sb.AppendLine($"Description: {tilesetGetResponse.description ?? "-"}");

                    details = (MarkupString)sb.ToString().Replace(" ", "&nbsp;").Replace("\r\n", "<br />");
                }

            }
            StateHasChanged();
        }
    }
}