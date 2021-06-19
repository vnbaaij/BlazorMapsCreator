using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;

using RestSharp;

namespace BlazorMapsCreator.Pages
{
    public partial class ConvertPage
    {
        [Inject] IConfiguration Configuration { get; set; }
        [Inject] Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }

        private bool convertButtonDisabled = true;

        private string geography;
        private string subscriptionkey;
        private string statusUrl;
        private string uploadUdid;
        private string conversionUdid;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                geography = Configuration["AzureMaps:Geography"];
                subscriptionkey = Configuration["AzureMaps:SubscriptionKey"];

                uploadUdid = await LocalStorage.GetItemAsync<string>("upload-udid");
                convertButtonDisabled = false;
                StateHasChanged();
            }
        }

        private async Task ConvertPackage()
        {
            if (string.IsNullOrEmpty(uploadUdid))
                uploadUdid = await LocalStorage.GetItemAsync<string>("upload-udid");

            RestClient client = new($"https://{geography}.atlas.microsoft.com/conversions?subscription-key={subscriptionkey}&api-version=2.0&udid={uploadUdid}&inputType=DWG&outputOntology=facility-2.0")
            {
                Timeout = -1
            };
            RestRequest request = new(Method.POST);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                statusUrl = response.Headers.FirstOrDefault(p => p.Name == "Operation-Location").Value.ToString();
                await LocalStorage.SetItemAsync("statusUrl", statusUrl);


            }
        }

        private async Task ConvertPackageStatus()
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
                    conversionUdid = resourceLocation.Segments[^1];
                    await LocalStorage.SetItemAsync("conversion-udid", conversionUdid);
                }
                else
                {
                    conversionUdid = "checking again in 15 seconds...";
                    await Task.Delay(15000);
                    await ConvertPackageStatus();
                }
            }
        }
    }

}