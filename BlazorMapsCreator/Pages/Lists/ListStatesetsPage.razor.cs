using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

using BlazorFluentUI;

using BlazorMapsCreator.Models;

using Microsoft.AspNetCore.Components;

using RestSharp;

namespace BlazorMapsCreator.Pages.Lists
{
    public partial class ListStatesetsPage : ListPageBase<StatesetInfoObject>
    {
        private StatesetListResponse statesetResponse;

        private void GetData()
        {

            RestClient client = new($"https://{Geography}.atlas.microsoft.com/featureStateSets?subscription-key={SubscriptionKey}&api-version=2.0")
            {
                Timeout = -1
            };
            RestRequest request = new(Method.GET);

            IRestResponse response = client.Execute(request);


            if (response.IsSuccessful)
            {
                statesetResponse = JsonSerializer.Deserialize<StatesetListResponse>(response.Content);
                itemList = new List<StatesetInfoObject>(statesetResponse.statesets);
            }

        }
        private void OnClick(StatesetInfoObject item)
        {
            Console.WriteLine("Clicked!");
        }


        protected override void OnInitialized()
        {
            base.OnInitialized();

            Selection.GetKey = (item => item.statesetId);
            Columns.Add(new DetailsRowColumn<StatesetInfoObject>("Stateset Id", x => x.statesetId) { MaxWidth = 150, IsResizable = true, Index = 0 });
            //Columns.Add(new DetailsRowColumn<StatesetInfoObject>("Created", x => x.created!) { Index = 1, MaxWidth = 150, IsResizable = true, OnColumnClick = OrderCreated });
            //Columns.Add(new DetailsRowColumn<StatesetInfoObject>("Updated", x => x.updated!) { Index = 2, MaxWidth = 150, IsResizable = true, OnColumnClick = OrderUpdated });
            //Columns.Add(new DetailsRowColumn<StatesetInfoObject>("Status", x => x.uploadStatus!) { Index = 3, MaxWidth = 100, IsResizable = true });
            //Columns.Add(new DetailsRowColumn<StatesetInfoObject>("Size (kB)", x => x.sizeInBytes!) { Index = 4, MaxWidth = 100, IsResizable = true, OnColumnClick = OrderSize });
            //Columns.Add(new DetailsRowColumn<StatesetInfoObject>("Format", x => x.dataFormat!) { Index = 5, MaxWidth = 100, IsResizable = true });
            //Columns.Add(new DetailsRowColumn<StatesetInfoObject>("Location", x => x.location!) { Index = 6, MaxWidth = 450, IsResizable = true });
            //Columns.Add(new DetailsRowColumn<StatesetInfoObject>("Description", x => x.description) { Index = 7 });

            GetData();
        }

        private void Delete()
        {
            messages.Clear();

            foreach (StatesetInfoObject item in Selection.GetSelection())
            {
                RestClient client = new($"https://{Geography}.atlas.microsoft.com/featureStateSets/{item.statesetId}?subscription-key={SubscriptionKey}&api-version=2.0")
                {
                    Timeout = -1
                };
                RestRequest request = new(Method.DELETE);

                IRestResponse response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    messages.Add($"Data with '{item.statesetId}' has been deleted");
                }
                itemList.Remove(item);
            }
            Selection.ClearSelection();
            StateHasChanged();
        }

        private void Get()
        {
            messages.Clear();

            StatesetInfoObject item = Selection.GetSelection()[0];

            RestClient client = new($"https://{Geography}.atlas.microsoft.com/featureStateSets/{item.statesetId}?subscription-key={SubscriptionKey}&api-version=2.0")
            {
                Timeout = -1
            };

            RestRequest request = new(Method.GET);

            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful)
            {
                StatesetGetResponse statesetGetResponse = JsonSerializer.Deserialize<StatesetGetResponse>(response.Content);
                if (statesetGetResponse != null)
                {
                    StringBuilder sb = new StringBuilder(statesetGetResponse.description);
                    sb.AppendLine("Datasets:");
                    foreach (string id in statesetGetResponse.datasetIds)
                    {
                        sb.AppendLine("  " + id);
                    }
                    sb.AppendLine("Styles:");
                    foreach (Style style in statesetGetResponse.statesetStyle.styles)
                    {
                        sb.AppendLine("  Keyname: " + style.keyName);
                        sb.AppendLine("  Type: " + style.type);
                        sb.AppendLine("  Rules:");
                        foreach (Dictionary<string, string> rules in style.rules)
                        {
                            foreach (KeyValuePair<string, string> rule in rules)
                            {
                                sb.AppendLine("    " + rule.Key + ": " + rule.Value);
                            }
                        }
                    }

                    details = (MarkupString)sb.ToString().Replace(" ", "&nbsp;").Replace("\r\n", "<br />");
                }
            }
            StateHasChanged();
        }
    }
}