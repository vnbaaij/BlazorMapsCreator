using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

using RestSharp;

namespace BlazorMapsCreator.Pages
{
    public partial class FeaturePage
    {
        [Inject] Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }

        private string geography;
        private string subscriptionkey;
        private string statesetUdid;
        private string datasetUdid;

        private string body;
        private string message;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                geography = await LocalStorage.GetItemAsync<string>("geography");
                subscriptionkey = await LocalStorage.GetItemAsync<string>("subscriptionkey");
                datasetUdid = await LocalStorage.GetItemAsync<string>("dataset-udid");
                statesetUdid = await LocalStorage.GetItemAsync<string>("stateset-udid");

                StateHasChanged();
            }
        }

        private async Task CreateStateset()
        {
            if (string.IsNullOrEmpty(datasetUdid))
                datasetUdid = await LocalStorage.GetItemAsync<string>("dataset-udid");

            RestClient client = new($"https://{geography}.atlas.microsoft.com/featurestatesets?subscription-key={subscriptionkey}&api-version=2.0&datasetId={datasetUdid}")
            {
                Timeout = -1
            };
            RestRequest request = new(Method.POST);
            request.AddHeader("Content-Type", "application/json");

            body = "{\"styles\":[{\"keyname\":\"occupied\",\"type\":\"boolean\",\"rules\":[{\"true\":\"#FF0000\",\"false\":\"#00FF00\"}]}]}";
            request.AddParameter("", body, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                statesetUdid = response.Content.Split(":")[1][1..^2];
                await LocalStorage.SetItemAsync("stateset-udid", statesetUdid);

            }
        }

        private async Task UpdateFeaturestate()
        {
            //
            if (string.IsNullOrEmpty(statesetUdid))
                statesetUdid = await LocalStorage.GetItemAsync<string>("stateset-udid");
            RestClient client = new($"https://{geography}.atlas.microsoft.com/featurestatesets/{statesetUdid}/featureStates/UNIT26?api-version=2.0&subscription-key={subscriptionkey}")
            {
                Timeout = -1
            };
            RestRequest request = new(Method.PUT);
            request.AddHeader("Content-Type", "application/json");

            body = "{\"states\": [{\"keyName\": \"occupied\",\"value\": true,\"eventTimestamp\": \"" + DateTime.Now.ToString("o") +"\"}]}";
            request.AddParameter("", body, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                message = "Feature state altered successfuly";
            }
        }
    }
}