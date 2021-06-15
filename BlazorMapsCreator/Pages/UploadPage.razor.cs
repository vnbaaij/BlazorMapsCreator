using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;

using RestSharp;

namespace BlazorMapsCreator.Pages
{
    public partial class UploadPage
    {

        [Inject] Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
        [Inject] IWebHostEnvironment Environment { get; set; }

        private bool uploadButtonDisabled = true;
        private bool metadataButtonDisabled = true;
        private bool deleteButtonDisabled = true;
        private string geography;
        private string subscriptionkey;
        private string statusUrl;
        private string udid;
        private MapDataMetadata metadata;
        private readonly long maxFileSize = 1024 * 1024 * 10;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                geography = await LocalStorage.GetItemAsync<string>("geography");
                subscriptionkey = await LocalStorage.GetItemAsync<string>("subscriptionkey");
                StateHasChanged();
            }
        }

        private async Task LoadFile(InputFileChangeEventArgs e)
        {
            try
            {
                string trustedFileNameForFileStorage = e.File.Name;
                string path = Path.Combine(Environment.ContentRootPath, Environment.EnvironmentName, "unsafe_uploads", trustedFileNameForFileStorage);

                await using FileStream fs = new(path, FileMode.Create);
                await e.File.OpenReadStream(maxFileSize).CopyToAsync(fs);
                fs.Close();

                await MapDataUpload(path);

                File.Delete(path);
            }
            catch (Exception)
            {

            }

        }

        private async Task MapDataUpload(string path)
        {
            byte[] mapBytes = File.ReadAllBytes(path);

            RestClient client = new($"https://{geography}.atlas.microsoft.com/mapData?api-version=2.0&dataFormat=dwgzippackage&subscription-key={subscriptionkey}")
            {
                Timeout = -1
            };
            RestRequest request = new(Method.POST);
            request.AddHeader("Content-Type", "application/octet-stream");
            request.AddParameter("", mapBytes, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                statusUrl = response.Headers.FirstOrDefault(p => p.Name == "Operation-Location").Value.ToString();
                await LocalStorage.SetItemAsync("statusUrl", statusUrl);
                uploadButtonDisabled = false;
            }
        }

        public async Task MapDataUploadStatus()
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
                    udid = resourceLocation.Segments[^1];
                    await LocalStorage.SetItemAsync("upload-udid", udid);
                    metadataButtonDisabled = false;
                    deleteButtonDisabled = false;
                }
                else
                {
                    udid = "checking again in 5 seconds...";
                    await Task.Delay(5000);
                    await MapDataUploadStatus();
                }
            }
        }

        public async Task MapDataMetadata()
        {
            if (string.IsNullOrEmpty(udid))
                udid = await LocalStorage.GetItemAsync<string>("upload-udid");
            RestClient client = new($"https://{geography}.atlas.microsoft.com/mapData/metadata/{udid}?api-version=2.0&subscription-key={subscriptionkey}")
            {
                Timeout = -1
            };
            RestRequest request = new(Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                metadata = JsonSerializer.Deserialize<MapDataMetadata>(response.Content);
            }
        }

        private void MapDataDelete()
        {
            RestClient client = new($"https://{geography}.atlas.microsoft.com/mapData/{udid}?api-version=2.0&subscription-key={subscriptionkey}")
            {
                Timeout = -1
            };
            RestRequest request = new(Method.DELETE);
            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful)
            {
                uploadButtonDisabled = true;
                metadataButtonDisabled = true;
                deleteButtonDisabled = true;

                udid = "";
                metadata = null;
            }
        }
    }
}