using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

using RestSharp;

namespace BlazorMapsCreator.Pages
{
    public partial class QueryPage
    {

        [Inject] Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }

        private bool createButtonDisabled = true;
        private bool statusButtonDisabled = true;
        private string geography;
        private string subscriptionkey;
        private string statusUrl;
        private string datasetUdid;
        private string tilesetUdid;

        private string allCollections;
        private string unitCollection;

        private string errorMessage;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                geography = await LocalStorage.GetItemAsync<string>("geography");
                subscriptionkey = await LocalStorage.GetItemAsync<string>("subscriptionkey");
                datasetUdid = await LocalStorage.GetItemAsync<string>("dataset-udid");
                tilesetUdid = await LocalStorage.GetItemAsync<string>("tileset-udid");

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
                allCollections = response.Content;
            }
            else
            {
                ODataErrorResponse error = JsonSerializer.Deserialize<ODataErrorResponse>(response.Content);
                errorMessage = error.error.message ;
            }
        }

        private async Task QueryUnitCollection()
        {
            if (string.IsNullOrEmpty(datasetUdid))
                datasetUdid = await LocalStorage.GetItemAsync<string>("dataset-udid");

            RestClient client = new($"https://{geography}.atlas.microsoft.com/wfs/datasets/{datasetUdid}/collections/unit/items?subscription-key={subscriptionkey}&api-version=2.0")
            {
                Timeout = -1
            };
            RestRequest request = new(Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                unitCollection = response.Content;
            }
            else
            {
                ODataErrorResponse error = JsonSerializer.Deserialize<ODataErrorResponse>(response.Content);
                errorMessage = error.error.message;
            }
        }
    }
}