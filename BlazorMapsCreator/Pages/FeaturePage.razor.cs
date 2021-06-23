using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;

using RestSharp;

namespace BlazorMapsCreator.Pages
{
    public partial class FeaturePage
    {
        [Inject] IConfiguration Configuration { get; set; }
        [Inject] Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }

        private string geography;
        private string subscriptionkey;
        private string statesetUdid;
        private string datasetUdid;

        private string body;
        private List<string> messages = new();
        private string[] units;
        private bool[] unitOccupied;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                geography = Configuration["AzureMaps:Geography"];
                subscriptionkey = Configuration["AzureMaps:SubscriptionKey"];
                datasetUdid = await LocalStorage.GetItemAsync<string>("dataset-udid");
                statesetUdid = await LocalStorage.GetItemAsync<string>("stateset-udid");

                units = await LocalStorage.GetItemAsync<string[]>("units");
                if (units != null)
                    unitOccupied = new bool[units.Length];
            }

            StateHasChanged();
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

            body = "{\"styles\":[{\"keyname\":\"occupied\",\"type\":\"boolean\",\"rules\":[{\"true\":\"#FF0000\",\"false\":\"#00FF00\"}]},{\"keyname\": \"meetingType\",\"type\": \"string\",\"rules\": [{\"private\": \"#FF0000\",\"confidential\": \"#FF00AA\",\"allHands\": \"#00FF00\",\"brownBag\": \"#964B00\"}]}]}";
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

            messages.Clear();

            if (string.IsNullOrEmpty(statesetUdid))
                statesetUdid = await LocalStorage.GetItemAsync<string>("stateset-udid");
            RestClient client = new($"https://{geography}.atlas.microsoft.com/featurestatesets/{statesetUdid}/featureStates/UNIT26?api-version=2.0&subscription-key={subscriptionkey}")
            {
                Timeout = -1
            };
            RestRequest request = new(Method.PUT);
            request.AddHeader("Content-Type", "application/json");

            body = "{\"states\": [{\"keyName\": \"occupied\",\"value\": true,\"eventTimestamp\": \"" + DateTime.Now.ToString("o") + "\"}]}";
            request.AddParameter("", body, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                messages.Add("Feature state altered successfuly");
            }
        }

        private async Task UpdateFeaturestates()
        {

            if (string.IsNullOrEmpty(statesetUdid))
                statesetUdid = await LocalStorage.GetItemAsync<string>("stateset-udid");

            messages.Clear();
            int i = 0;
            foreach (string unit in units)
            {
                RestClient client = new($"https://{geography}.atlas.microsoft.com/featurestatesets/{statesetUdid}/featureStates/{unit}?api-version=2.0&subscription-key={subscriptionkey}")
                {
                    Timeout = -1
                };
                RestRequest request = new(Method.PUT);
                request.AddHeader("Content-Type", "application/json");

                body = "{\"states\": [{\"keyName\": \"occupied\",\"value\":" + unitOccupied[i].ToString().ToLower() + ",\"eventTimestamp\": \"" + DateTime.Now.ToString("o") + "\"}]}";
                request.AddParameter("", body, ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);
                if (response.IsSuccessful)
                {
                    messages.Add($"Feature state for unit {unit} altered successfuly");
                }
                i++;
            }
        }
    }
}