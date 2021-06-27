using System;
using System.Net;
using System.Text;

using Azure;
using Azure.Maps.Creator;
using Azure.Maps.Creator.Models;

using BlazorFluentUI;

using Microsoft.AspNetCore.Components;

namespace BlazorMapsCreator.Pages.Lists
{
    public partial class ListStatesetsPage : ListPageBase<StatesetInfoObject>
    {
        private void GetData()
        {
            FeatureStateClient client = new(Credential, Geography);

            itemList = client.ListStateset();
        }
        private void OnClick(StatesetInfoObject item)
        {
            Console.WriteLine("Clicked!");
        }


        protected override void OnInitialized()
        {
            base.OnInitialized();
            Selection.GetKey = (item => item.StatesetId);
            Columns.Add(new DetailsRowColumn<StatesetInfoObject>("Stateset Id", x => x.StatesetId) { MaxWidth = 150, IsResizable = true, Index = 0 });
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
            FeatureStateClient client = new(Credential, Geography);

            foreach (StatesetInfoObject item in Selection.GetSelection())
            {
                Response response = client.DeleteStateset(item.StatesetId);

                if (response.Status == (int)HttpStatusCode.NoContent)
                {
                    messages.Add($"Data with '{item.StatesetId}' has been deleted");
                }
            }

            Selection.ClearSelection();
            GetData();
            StateHasChanged();
        }

        private void Get()
        {
            messages.Clear();

            StatesetInfoObject item = Selection.GetSelection()[0];

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Datasets:");
            foreach (string id in item.DatasetIds)
            {
                sb.AppendLine("  " + id);
            }
            sb.AppendLine("Styles:");
            foreach (StyleObject style in item.StatesetStyle.Styles)
            {
                sb.AppendLine("  Keyname: " + style.KeyName);

                //sb.AppendLine("  Type: " + style.Type);
                //sb.AppendLine("  Rules:");
                //foreach (Dictionary<string, string> rules in style.rules)
                //{
                //    foreach (KeyValuePair<string, string> rule in rules)
                //    {
                //        sb.AppendLine("    " + rule.Key + ": " + rule.Value);
                //    }
                //}
            }

            details = (MarkupString)sb.ToString().Replace(" ", "&nbsp;").Replace("\r\n", "<br />");

            StateHasChanged();
        }
    }
}