using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Azure;
using Azure.Maps.Creator;
using Azure.Maps.Creator.Models;

using BlazorFluentUI;

using Microsoft.AspNetCore.Components;

namespace BlazorMapsCreator.Pages.Management
{
    public partial class ListConversionsPage : ListPageBase<ConversionListDetailInfo>
    {
        private void GetData()
        {
            ConversionClient client = new(Credential, Geography);

            itemList = client.List();
        }
        private static void OnClick(ConversionListDetailInfo item)
        {
            Console.WriteLine("Clicked!");
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Selection.GetKey = (item => item.ConversionId);
            Columns.Add(new DetailsRowColumn<ConversionListDetailInfo>("Conversion ID", x => x.ConversionId) { MaxWidth = 150, IsResizable = true, Index = 0 });
            Columns.Add(new DetailsRowColumn<ConversionListDetailInfo>("Created", x => x.Created!) { Index = 1, MaxWidth = 150, IsResizable = true, OnColumnClick = OrderCreated });
            Columns.Add(new DetailsRowColumn<ConversionListDetailInfo>("Udid", x => x.Udid) { Index = 2 });

            GetData();
        }

        private void OrderCreated(IDetailsRowColumn<ConversionListDetailInfo> column)
        {
            //create new sorted list
            itemList = (Pageable<ConversionListDetailInfo>)(column.IsSorted ? itemList.OrderBy(x => x.Created) : itemList.OrderByDescending(x => x.Created));

            column.IsSorted = !column.IsSorted;
            StateHasChanged();
        }

        private void Delete()
        {
            messages.Clear();

            ConversionClient client = new(Credential, Geography);

            foreach (ConversionListDetailInfo item in Selection.GetSelection())
            {
                Response response = client.Delete(item.ConversionId);

                if (response.Status == (int)HttpStatusCode.NoContent)
                {
                    messages.Add($"Data with '{item.ConversionId}' has been deleted");
                }
            }

            Selection.ClearSelection();
            GetData();
            StateHasChanged();
        }

        private void Get()
        {
            StringBuilder sb = new();

            messages.Clear();

            ConversionListDetailInfo item = Selection.GetSelection()[0];

            sb.AppendLine($"Id: {item.ConversionId}");
            sb.AppendLine($"Created: {item.Created}");
            sb.AppendLine($"UdId: {item.Udid}");
            sb.AppendLine($"Ontology: {item.Ontology ?? "-"}");
            sb.AppendLine($"Description: {item.Description ?? "-"}");

            sb.AppendLine("Feature counts:");
            foreach (KeyValuePair<string, object> pair in (Dictionary<string, object>)item.FeatureCounts)
            {
                sb.AppendLine($"    {pair.Key}: {pair.Value}");
            }

            details = (MarkupString)sb.ToString().Replace(" ", "&nbsp;").Replace("\r\n", "<br />");
            StateHasChanged();
        }
    }
}