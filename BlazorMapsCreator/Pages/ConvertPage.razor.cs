using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;

using RestSharp;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMapsCreator.Pages
{
    public partial class ConvertPage
    {
        [Inject] Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
        [Inject] IWebHostEnvironment Environment { get; set; }

        private bool convertButtonDisabled = true;
        private bool statusButtonDisabled = true;
        private string geography;
        private string subscriptionkey;
        private string statusUrl;
        private string uploadUdid;
        private string convertUdid;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                geography = await LocalStorage.GetItemAsync<string>("geography");
                subscriptionkey = await LocalStorage.GetItemAsync<string>("subscriptionkey");
                uploadUdid = await LocalStorage.GetItemAsync<string>("upload-udid");
                convertButtonDisabled = false;
                StateHasChanged();
            }
        }

        private async Task ConvertPackage()
        {
            if (string.IsNullOrEmpty(uploadUdid))
                uploadUdid = await LocalStorage.GetItemAsync<string>("upload-udid");

            var client = new RestClient($"https://{geography}.atlas.microsoft.com/conversions?subscription-key={subscriptionkey}&api-version=2.0&udid={uploadUdid}&inputType=DWG&outputOntology=facility-2.0")
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

        private async Task ConvertPackageStatus()
        {
            if (string.IsNullOrEmpty(statusUrl))
                statusUrl = await LocalStorage.GetItemAsync<string>("statusUrl");
            var client = new RestClient($"{statusUrl}&subscription-key={subscriptionkey}")
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
                    convertUdid = resourceLocation.Segments[^1];
                    await LocalStorage.SetItemAsync("convert-udid", convertUdid);
                }
                else
                {
                    convertUdid = "checking again in 5 seconds...";
                    await Task.Delay(5000);
                    await ConvertPackageStatus();
                }
            }
        }
    }

}