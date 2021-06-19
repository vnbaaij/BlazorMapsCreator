using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;

using RestSharp;

namespace BlazorMapsCreator.Pages
{
    public partial class TilesetPage
    {
        [Inject] IConfiguration Configuration { get; set; }
        [Inject] Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }

        private bool createButtonDisabled = true;
        private bool statusButtonDisabled = true;
        private string geography;
        private string subscriptionkey;
        private string statusUrl;
        private string datasetUdid;
        private string tilesetUdid;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                geography = Configuration["AzureMaps:Geography"];
                subscriptionkey = Configuration["AzureMaps:SubscriptionKey"];

                datasetUdid = await LocalStorage.GetItemAsync<string>("dataset-udid");
                createButtonDisabled = false;

                //tilesetUdid = await LocalStorage.GetItemAsync<string>("tileset-udid");
                //if (!string.IsNullOrEmpty(tilesetUdid))
                statusButtonDisabled = false;
                StateHasChanged();
            }
        }

        private async Task CreateTileset()
        {
            if (string.IsNullOrEmpty(datasetUdid))
                datasetUdid = await LocalStorage.GetItemAsync<string>("dataset-udid");

            RestClient client = new($"https://{geography}.atlas.microsoft.com/tilesets?subscription-key={subscriptionkey}&api-version=2.0&datasetId={datasetUdid}&type=facility")
            {
                Timeout = -1
            };
            var request = new RestRequest(Method.POST);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                statusUrl = response.Headers.FirstOrDefault(p => p.Name == "Operation-Location").Value.ToString();
                await LocalStorage.SetItemAsync("statusUrl", statusUrl);

                statusButtonDisabled = false;
            }
        }

        private async Task CreateTilesetStatus()
        {
            if (string.IsNullOrEmpty(statusUrl))
                statusUrl = await LocalStorage.GetItemAsync<string>("statusUrl");

            RestClient client = new($"{statusUrl}&subscription-key={subscriptionkey}")
            {
                Timeout = -1
            };
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                var para = response.Headers.FirstOrDefault(p => p.Name == "Resource-Location");
                if (para is not null)
                {
                    Uri resourceLocation = new(para.Value.ToString());
                    tilesetUdid = resourceLocation.Segments[^1];
                    await LocalStorage.SetItemAsync("tileset-udid", tilesetUdid);
                }
                else
                {
                    tilesetUdid = "checking again in 15 seconds...";
                    await Task.Delay(15000);
                    await CreateTilesetStatus();
                }
            }
        }

        private async Task GetMapTile()
        {
            if (string.IsNullOrEmpty(tilesetUdid))
                tilesetUdid = await LocalStorage.GetItemAsync<string>("tileset-udid");

            RestClient client = new($"https://{geography}.atlas.microsoft.com/map/tile?subscription-key={subscriptionkey}&api-version=2.0&tilesetId={tilesetUdid}&zoom=6&x=10&y=22")
            {
                Timeout = -1
            };
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful)
            {
                var para = response.Headers.FirstOrDefault(p => p.Name == "Resource-Location");
                if (para is not null)
                {
                    Uri resourceLocation = new(para.Value.ToString());
                    tilesetUdid = resourceLocation.Segments[^1];
                    await LocalStorage.SetItemAsync("tileset-udid", tilesetUdid);
                }
                else
                {
                    tilesetUdid = "checking again in 15 seconds...";
                    await Task.Delay(15000);

                }
            }
        }
    }
}