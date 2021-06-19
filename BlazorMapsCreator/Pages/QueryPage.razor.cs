using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;

using RestSharp;

namespace BlazorMapsCreator.Pages
{
    public partial class QueryPage
    {
        [Inject] IConfiguration Configuration { get; set; }
        [Inject] Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }


        private string geography;
        private string subscriptionkey;

        private string datasetUdid;

        private Collections allCollections;
        private FeatureCollection featureCollection;

        private string errorMessage;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                geography = Configuration["AzureMaps:Geography"];
                subscriptionkey = Configuration["AzureMaps:SubscriptionKey"];

                datasetUdid = await LocalStorage.GetItemAsync<string>("dataset-udid");
                //tilesetUdid = await LocalStorage.GetItemAsync<string>("tileset-udid");

                StateHasChanged();
            }
        }

        private async Task QueryAllCollections()
        {
            if (string.IsNullOrEmpty(datasetUdid))
                datasetUdid = await LocalStorage.GetItemAsync<string>("dataset-udid");

            RestClient client = new($"https://{geography}.atlas.microsoft.com/wfs/datasets/{datasetUdid}/collections?subscription-key={subscriptionkey}&api-version=2.0")
            {
                Timeout = -1
            };
            RestRequest request = new(Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                allCollections = JsonSerializer.Deserialize<Collections>(response.Content);
            }
            else
            {
                ODataErrorResponse error = JsonSerializer.Deserialize<ODataErrorResponse>(response.Content);
                errorMessage = error.error.message;
            }
        }

        private async Task QueryCollectionAsync(string collection, string href)
        {
            featureCollection = null;
            //StateHasChanged();

            if (string.IsNullOrEmpty(datasetUdid))
                datasetUdid = await LocalStorage.GetItemAsync<string>("dataset-udid");

            RestClient client = new($"{href}&subscription-key={subscriptionkey}")
            {
                Timeout = -1
            };
            RestRequest request = new(Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                featureCollection = JsonSerializer.Deserialize<FeatureCollection>(response.Content);
                if (collection == "unit")
                    await LocalStorage.SetItemAsync("units", featureCollection.features.Select(f => f.id).ToArray());
            }
            else
            {
                ODataErrorResponse error = JsonSerializer.Deserialize<ODataErrorResponse>(response.Content);
                errorMessage = error.error.message;
            }

        }


    }
}