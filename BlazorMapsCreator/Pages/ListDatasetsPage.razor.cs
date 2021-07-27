using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

using BlazorFluentUI;

using BlazorMapsCreator.Models;

using RestSharp;

namespace BlazorMapsCreator.Pages
{
    public partial class ListDatasetsPage : PageBase<DatasetDetailInfo>
    {
        private DatasetListResponse datasetResponse;

        private void GetData()
        {
            RestClient client = new($"https://{Geography}.atlas.microsoft.com/datasets?subscription-key={SubscriptionKey}&api-version=2.0")
            {
                Timeout = -1
            };
            RestRequest request = new(Method.GET);

            IRestResponse response = client.Execute(request);


            if (response.IsSuccessful)
            {
                datasetResponse = JsonSerializer.Deserialize<DatasetListResponse>(response.Content);
                itemList = new List<DatasetDetailInfo>(datasetResponse.datasets.OrderBy(x => x.created));
            }

        }
        private void OnClick(DatasetDetailInfo item)
        {
            Console.WriteLine("Clicked!");
        }


        protected override void OnInitialized()
        {
            base.OnInitialized();

            Selection.GetKey = (item => item.datasetId);
            Columns.Add(new DetailsRowColumn<DatasetDetailInfo>("DatasetUdid", x => x.datasetId) { MaxWidth = 150, IsResizable = true, Index = 0 });
            Columns.Add(new DetailsRowColumn<DatasetDetailInfo>("Created", x => x.created!) { Index = 1, MaxWidth = 150, IsResizable = true, OnColumnClick = OrderCreated });
            Columns.Add(new DetailsRowColumn<DatasetDetailInfo>("Ontology", x => x.ontology!) { Index = 2 });
            Columns.Add(new DetailsRowColumn<DatasetDetailInfo>("Description", x => x.description) { Index = 3 });
            //Columns.Add(new DetailsRowColumn<DatasetDetailInfo>("Status", x => x.uploadStatus!) { Index = 3, MaxWidth = 100, IsResizable = true });
            //Columns.Add(new DetailsRowColumn<DatasetDetailInfo>("Size (kB)", x => x.sizeInBytes!) { Index = 4, MaxWidth = 100, IsResizable = true, OnColumnClick = OrderSize });
            //Columns.Add(new DetailsRowColumn<DatasetDetailInfo>("Format", x => x.dataFormat!) { Index = 5, MaxWidth = 100, IsResizable = true });
            //Columns.Add(new DetailsRowColumn<DatasetDetailInfo>("Location", x => x.location!) { Index = 6, MaxWidth = 450, IsResizable = true });

            GetData();
        }

        private void OrderCreated(IDetailsRowColumn<DatasetDetailInfo> column)
        {
            // since we're creating a new list, we need to make a copy of what was previously selected
            var selected = Selection.GetSelection();

            //create new sorted list
            itemList = new List<DatasetDetailInfo>(column.IsSorted ? itemList.OrderBy(x => x.created) : itemList.OrderByDescending(x => x.created));

            column.IsSorted = !column.IsSorted;
            StateHasChanged();
        }

        private void Delete()
        {
            messages.Clear();

            foreach (DatasetDetailInfo item in Selection.GetSelection())
            {
                RestClient client = new($"https://{Geography}.atlas.microsoft.com/datasets/{item.datasetId}?subscription-key={SubscriptionKey}&api-version=2.0")
                {
                    Timeout = -1
                };
                RestRequest request = new(Method.DELETE);

                IRestResponse response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    messages.Add($"Data with '{item.datasetId}' has been deleted");
                }
                itemList.Remove(item);
            }
            Selection.ClearSelection();
            StateHasChanged();
        }
    }
}