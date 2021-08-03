using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

using BlazorFluentUI;

using BlazorMapsCreator.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;

using RestSharp;

namespace BlazorMapsCreator.Pages
{
    public partial class DataPage : PageBase<MapDataDetailInfo>
    {
        [Inject] IWebHostEnvironment Environment { get; set; }
        [Inject] Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }

        private MapDataListResponse mapDataResponse;

        protected bool GetDisabled = true;
        protected string DownloadLink = "";
        private string statusUrl;
        private string status;

        private readonly long maxFileSize = 1024 * 1024 * 10;

        private async Task LoadFile(InputFileChangeEventArgs e)
        {
            try
            {
                string trustedFileNameForFileStorage = e.File.Name;
                string path = Path.Combine(Environment.ContentRootPath, Environment.EnvironmentName, "unsafe_uploads", trustedFileNameForFileStorage);

                await using FileStream fs = new(path, FileMode.Create);
                await e.File.OpenReadStream(maxFileSize).CopyToAsync(fs);
                fs.Close();

                status = "Uploading file...";
                StateHasChanged();
                await UploadData(path);

                File.Delete(path);

                ListData();
                StateHasChanged();
            }
            catch (Exception)
            {

            }

        }

        private async Task UploadData(string path)
        {
            byte[] mapBytes = File.ReadAllBytes(path);

            string uploadDataFormat = path.EndsWith("zip") ? "dwgzippackage" : "geojson";

            RestClient client = new($"{baseUrl}/mapData?api-version=2.0&dataFormat={uploadDataFormat}&subscription-key={SubscriptionKey}");
            RestRequest request = new(Method.POST);
            request.AddHeader("Content-Type", "application/octet-stream");
            request.AddParameter("", mapBytes, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                statusUrl = response.Headers.FirstOrDefault(p => p.Name == "Operation-Location").Value.ToString();
                await LocalStorage.SetItemAsync("statusUrl", statusUrl);
                // uploadButtonDisabled = false;
                await UploadDataStatus();
            }
        }

        public async Task UploadDataStatus()
        {
            if (string.IsNullOrEmpty(statusUrl))
                statusUrl = await LocalStorage.GetItemAsync<string>("statusUrl");
            RestClient client = new($"{statusUrl}&subscription-key={SubscriptionKey}");
            RestRequest request = new(Method.GET);
            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful)
            {
                var para = response.Headers.FirstOrDefault(p => p.Name == "Resource-Location");
                if (para is not null)
                {
                    Uri resourceLocation = new(para.Value.ToString());
                    string udid = resourceLocation.Segments[^1];
                    status = $"{udid} uploaded!";
                    await LocalStorage.SetItemAsync("upload-udid", udid);
                    messages.Add(new MessageItem($"Data with id '{udid}' has been uploaded", MessageBarType.Success));
                }
                else
                {
                    status = "checking again in 15 seconds...";
                    await Task.Delay(15000);
                    await UploadDataStatus();
                }
            }
        }

        private void ListData()
        {

            RestClient client = new($"{baseUrl}/mapdata?subscription-key={SubscriptionKey}&api-version=2.0");
            RestRequest request = new(Method.GET);

            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful)
            {
                mapDataResponse = JsonSerializer.Deserialize<MapDataListResponse>(response.Content);
                itemList = new List<MapDataDetailInfo>(mapDataResponse.mapDataList);
            }
        }

        private void OnClick(MapDataDetailInfo item)
        {
            UpdateGetandDownload();
        }

        private void UpdateGetandDownload()
        {
            GetDisabled = Selection.Count != 1;
            if (GetDisabled) DownloadLink = "";
            StateHasChanged();
        }

        private void Selection_OnSelectionChanged(object? sender, EventArgs e)
        {
            UpdateGetandDownload();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Selection.GetKey = (item => item.udid);
            Columns.Add(new DetailsRowColumn<MapDataDetailInfo>("Udid", x => x.udid) { MaxWidth = 150, IsResizable = true, Index = 0 });
            Columns.Add(new DetailsRowColumn<MapDataDetailInfo>("Created", x => x.created!) { Index = 1, MaxWidth = 150, IsResizable = true, OnColumnClick = OrderCreated });
            Columns.Add(new DetailsRowColumn<MapDataDetailInfo>("Updated", x => x.updated!) { Index = 2, MaxWidth = 150, IsResizable = true, OnColumnClick = OrderUpdated });
            Columns.Add(new DetailsRowColumn<MapDataDetailInfo>("Status", x => x.uploadStatus!) { Index = 3, MaxWidth = 100, IsResizable = true });
            Columns.Add(new DetailsRowColumn<MapDataDetailInfo>("Size (kB)", x => x.sizeInBytes!) { Index = 4, MaxWidth = 100, IsResizable = true, OnColumnClick = OrderSize });
            Columns.Add(new DetailsRowColumn<MapDataDetailInfo>("Format", x => x.dataFormat!) { Index = 5, MaxWidth = 100, IsResizable = true });
            Columns.Add(new DetailsRowColumn<MapDataDetailInfo>("Location", x => x.location!) { Index = 6, MaxWidth = 450, IsResizable = true });
            Columns.Add(new DetailsRowColumn<MapDataDetailInfo>("Description", x => x.description) { Index = 7 });

            ListData();

            Selection.OnSelectionChanged += Selection_OnSelectionChanged;

            messages.Clear();
        }

        private void OrderCreated(IDetailsRowColumn<MapDataDetailInfo> column)
        {
            // since we're creating a new list, we need to make a copy of what was previously selected
            var selected = Selection.GetSelection();

            //create new sorted list
            itemList = new List<MapDataDetailInfo>(column.IsSorted ? itemList.OrderBy(x => x.created) : itemList.OrderByDescending(x => x.created));

            column.IsSorted = !column.IsSorted;
            StateHasChanged();
        }

        private void OrderUpdated(IDetailsRowColumn<MapDataDetailInfo> column)
        {
            // since we're creating a new list, we need to make a copy of what was previously selected
            var selected = Selection.GetSelection();

            //create new sorted list
            itemList = new List<MapDataDetailInfo>(column.IsSorted ? itemList.OrderBy(x => x.updated) : itemList.OrderByDescending(x => x.updated));

            column.IsSorted = !column.IsSorted;
            StateHasChanged();
        }

        private void OrderSize(IDetailsRowColumn<MapDataDetailInfo> column)
        {
            // since we're creating a new list, we need to make a copy of what was previously selected
            var selected = Selection.GetSelection();

            //create new sorted list
            itemList = new List<MapDataDetailInfo>(column.IsSorted ? itemList.OrderBy(x => x.sizeInBytes) : itemList.OrderByDescending(x => x.sizeInBytes));

            column.IsSorted = !column.IsSorted;
            StateHasChanged();
        }

        private void DeleteData()
        {
            messages.Clear();

            foreach (MapDataDetailInfo item in Selection.GetSelection())
            {
                RestClient client = new($"{baseUrl}/mapdata/{item.udid}?subscription-key={SubscriptionKey}&api-version=2.0");
                RestRequest request = new(Method.DELETE);

                IRestResponse response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    messages.Add(new MessageItem($"Data with id '{item.udid}' has been deleted"));
                }

                //Also delete any dowloaded version of the data
                string ext = "";
                switch (item.dataFormat.ToLowerInvariant())
                {
                    case "dwgzippackage":
                        ext = "zip";
                        break;
                    case "zip":
                        ext = "zip";
                        break;
                    case "geojson":
                        ext = "json";
                        break;
                    default:
                        break;
                }
                try
                {
                    File.Delete(Path.Combine(Environment.WebRootPath, "Data", $"{item.udid}.{ext}"));
                }
                catch (Exception)
                {

                }

                itemList.Remove(item);
            }
            Selection.ClearSelection();
            StateHasChanged();
        }

        private void GetDataDetailInfo()
        {
            MapDataDetailInfo item = Selection.GetSelection()[0];

            if (item.uploadStatus.ToLowerInvariant() != "completed")
            {
                messages.Add(new($"Data with '{item.udid}' has not finished uploading", MessageBarType.Warning));
                return;
            }

            // Store udid of selected item in local storage so that a conversion can be started later.
            LocalStorage.SetItemAsync("upload-udid", item.udid);

            RestClient client = new($"{baseUrl}/mapData/{item.udid}?subscription-key={SubscriptionKey}&api-version=2.0");

            RestRequest request = new(Method.GET);

            string filetype;
            string accept;
            switch (item.dataFormat.ToLowerInvariant())
            {
                case "dwgzippackage":
                    accept = "application/octet-stream";
                    filetype = "zip";
                    break;
                case "geojson":
                    accept = "application/vnd.geo+json";
                    filetype = "json";
                    break;

                case "zip":
                    accept = "application/octet-stream";
                    filetype = "zip";
                    break;
                default:
                    accept = "application/json";
                    filetype = "json";
                    break;
            }
            client.AddDefaultHeader("Accept", accept);

            IRestResponse response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
                messages.Add(new MessageItem("Something went wrong while getting the resource from Azure Maps", MessageBarType.Error));

            // Read bytes
            byte[] fileBytes = response.RawBytes;

            string path = Path.Combine(Environment.WebRootPath, "Data");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName = Path.Combine(path, $"{item.udid}.{filetype}");

            File.WriteAllBytes(fileName, fileBytes);

            DownloadLink = Path.Combine("Data", $"{item.udid}.{filetype}");

            if (item.dataFormat.ToLowerInvariant() == "dwgzippackage")
            {
                using ZipArchive archive = ZipFile.OpenRead(fileName);
                ZipArchiveEntry entry = archive.GetEntry("manifest.json");

                if (entry != null)
                {
                    StreamReader reader = new(entry.Open());

                    string json = reader.ReadToEnd();
                    details = (MarkupString)json.Replace(" ", "&nbsp;").ReplaceLineEndings("<br />");

                    JsonSerializerOptions options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                    Manifest manifest = JsonSerializer.Deserialize<Manifest>(json, options);


                }
            }

            StateHasChanged();
        }

        public override void Dispose()
        {
            Selection.OnSelectionChanged -= Selection_OnSelectionChanged;
        }
    }
}