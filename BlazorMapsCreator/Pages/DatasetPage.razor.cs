using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;

using RestSharp;

namespace BlazorMapsCreator.Pages
{
    public partial class DatasetPage
    {
        [Inject] Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
        
        private bool createButtonDisabled = true;
        private bool statusButtonDisabled = true;
        private string geography;
        private string subscriptionkey;
        private string statusUrl;
        private string conversionUdid;
        private string datasetUdid;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                geography = await LocalStorage.GetItemAsync<string>("geography");
                subscriptionkey = await LocalStorage.GetItemAsync<string>("subscriptionkey");
                conversionUdid = await LocalStorage.GetItemAsync<string>("conversion-udid");
                
                createButtonDisabled = false;

                datasetUdid = await LocalStorage.GetItemAsync<string>("dataset-udid");
                if (!string.IsNullOrEmpty(datasetUdid))
                    statusButtonDisabled = false;
                StateHasChanged();
            }
        }

        private async Task CreateDataset()
        {
            if (string.IsNullOrEmpty(conversionUdid))
                conversionUdid = await LocalStorage.GetItemAsync<string>("conversion-udid");

            RestClient client = new($"https://{geography}.atlas.microsoft.com/datasets?subscription-key={subscriptionkey}&api-version=2.0&conversionId={conversionUdid}&type=facility")
            {
                Timeout = -1
            };
            RestRequest request = new(Method.POST);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                statusUrl = response.Headers.FirstOrDefault(p => p.Name == "Operation-Location").Value.ToString();
                await LocalStorage.SetItemAsync("statusUrl", statusUrl);

                statusButtonDisabled = false;
            }
        }

        private async Task CreateDatasetStatus()
        {
            if (string.IsNullOrEmpty(statusUrl))
                statusUrl = await LocalStorage.GetItemAsync<string>("statusUrl");
            RestClient client = new($"{statusUrl}&subscription-key={subscriptionkey}")
            {
                Timeout = -1
            };
            RestRequest request = new(Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                var para = response.Headers.FirstOrDefault(p => p.Name == "Resource-Location");
                if (para is not null)
                {
                    Uri resourceLocation = new(para.Value.ToString());
                    datasetUdid = resourceLocation.Segments[^1];
                    await LocalStorage.SetItemAsync("dataset-udid", datasetUdid);
                }
                else
                {
                    datasetUdid = "checking again in 5 seconds...";
                    await Task.Delay(5000);
                    await CreateDatasetStatus();
                }
            }
        }

    }
}