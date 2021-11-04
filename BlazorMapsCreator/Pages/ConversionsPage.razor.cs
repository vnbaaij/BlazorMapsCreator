using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BlazorFluentUI;

using BlazorMapsCreator.Models;
using Microsoft.AspNetCore.Components;
using RestSharp;
namespace BlazorMapsCreator.Pages
{
    public partial class ConversionsPage : PageBase<ConversionListDetailInfo>
    {

        [Inject] Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }

        private bool convertButtonDisabled = true;

        private string geography;
        private string subscriptionkey;
        private string statusUrl;
        private string uploadUdid;
        private string conversionUdid;
        private ConversionListResponse conversionListResponse;

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
        private void ListConversions()
        {

            RestClient client = new($"https://{Geography}.atlas.microsoft.com/conversions?subscription-key={SubscriptionKey}&api-version=2.0")
            {
                Timeout = -1
            };
            RestRequest request = new(Method.GET);

            IRestResponse response = client.Execute(request);


            if (response.IsSuccessful)
            {
                conversionListResponse = JsonSerializer.Deserialize<ConversionListResponse>(response.Content);
                itemList = new List<ConversionListDetailInfo>(conversionListResponse.conversions.OrderBy(x => x.created));
            }

        }
        private void OnClick(ConversionListDetailInfo item)
        {
            Console.WriteLine("Clicked!");
        }


        protected override void OnInitialized()
        {
            base.OnInitialized();
            Selection.GetKey = (item => item.conversionId);
            Columns.Add(new DetailsRowColumn<ConversionListDetailInfo>("Conversion Id", x => x.conversionId) { MaxWidth = 150, IsResizable = true, Index = 0 });
            Columns.Add(new DetailsRowColumn<ConversionListDetailInfo>("Created", x => x.created!) { Index = 1, MaxWidth = 150, IsResizable = true, OnColumnClick = OrderCreated });
            Columns.Add(new DetailsRowColumn<ConversionListDetailInfo>("Upload Id", x => x.udid) { Index = 2 });
            Columns.Add(new DetailsRowColumn<ConversionListDetailInfo>("Ontology", x => x.ontology) { Index = 3 });
            //Columns.Add(new DetailsRowColumn<ConversionListDetailInfo>("Description", x => x.description) { Index = 4 });
            //Columns.Add(new DetailsRowColumn<ConversionListDetailInfo>("FeatureCounts", x => x.featureCounts.) { Index = 4 });

            ListConversions();
        }

        private void OrderCreated(IDetailsRowColumn<ConversionListDetailInfo> column)
        {
            // since we're creating a new list, we need to make a copy of what was previously selected
            var selected = Selection.GetSelection();

            //create new sorted list
            itemList = new List<ConversionListDetailInfo>(column.IsSorted ? itemList.OrderBy(x => x.created) : itemList.OrderByDescending(x => x.created));

            column.IsSorted = !column.IsSorted;
            StateHasChanged();
        }


        private void Delete()
        {
            messages.Clear();

            foreach (ConversionListDetailInfo item in Selection.GetSelection())
            {
                RestClient client = new($"https://{Geography}.atlas.microsoft.com/conversions/{item.udid}?subscription-key={SubscriptionKey}&api-version=2.0")
                {
                    Timeout = -1
                };
                RestRequest request = new(Method.DELETE);

                IRestResponse response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    messages.Add(new MessageItem($"Data with '{item.udid}' has been deleted"));
                }
                itemList.Remove(item);
            }
            Selection.ClearSelection();
            StateHasChanged();
        }
    }
}