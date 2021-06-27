using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

using BlazorFluentUI;

using BlazorMapsCreator.Models;

using RestSharp;

namespace BlazorMapsCreator.Pages.Lists
{
    public partial class ListConversionsPage : ListPageBase<ConversionListDetailInfo>
    {
        private ConversionListResponse conversionListResponse;

        private void GetData()
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
            Columns.Add(new DetailsRowColumn<ConversionListDetailInfo>("Conversion ID", x => x.conversionId) { MaxWidth = 150, IsResizable = true, Index = 0 });
            Columns.Add(new DetailsRowColumn<ConversionListDetailInfo>("Created", x => x.created!) { Index = 1, MaxWidth = 150, IsResizable = true, OnColumnClick = OrderCreated });
            Columns.Add(new DetailsRowColumn<ConversionListDetailInfo>("Udid", x => x.udid) { Index = 2 });
            Columns.Add(new DetailsRowColumn<ConversionListDetailInfo>("Ontology", x => x.ontology) { Index = 3 });
            //Columns.Add(new DetailsRowColumn<ConversionListDetailInfo>("Description", x => x.description) { Index = 4 });
            //Columns.Add(new DetailsRowColumn<ConversionListDetailInfo>("FeatureCounts", x => x.featureCounts.) { Index = 4 });

            GetData();
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
                    messages.Add($"Data with '{item.udid}' has been deleted");
                }
                itemList.Remove(item);
            }
            Selection.ClearSelection();
            StateHasChanged();
        }
    }
}