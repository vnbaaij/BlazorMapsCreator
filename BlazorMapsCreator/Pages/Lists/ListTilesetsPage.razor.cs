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
    public partial class ListTilesetsPage : ListPageBase<TilesetDetailInfo>
    {

        private void GetData()
        {
            TilesetClient client = new(Credential, Geography);

            itemList = client.List();
        }
        private void OnClick(TilesetDetailInfo item)
        {
            Console.WriteLine("Clicked!");
        }


        protected override void OnInitialized()
        {
            base.OnInitialized();
            Selection.GetKey = (item => item.TilesetId);
            Columns.Add(new DetailsRowColumn<TilesetDetailInfo>("Tileset Id", x => x.TilesetId) { MaxWidth = 150, IsResizable = true, Index = 0 });
            Columns.Add(new DetailsRowColumn<TilesetDetailInfo>("Dataset Id", x => x.DatasetId) { Index = 1, MaxWidth = 150, IsResizable = true });
            //Columns.Add(new DetailsRowColumn<TilesetDetailInfo>("Min zoom", x => x.minZoom!) { Index = 2, MaxWidth = 150, IsResizable = true });
            //Columns.Add(new DetailsRowColumn<TilesetDetailInfo>("Max zoom", x => x.maxZoom!) { Index = 3, MaxWidth = 100, IsResizable = true });
            //Columns.Add(new DetailsRowColumn<TilesetDetailInfo>("Ontology", x => x.ontology) { Index = 4, MaxWidth = 100, IsResizable = true });
            //Columns.Add(new DetailsRowColumn<TilesetDetailInfo>("Description", x => x.description) { Index = 7 });

            GetData();


        }


        private void Delete()
        {
            messages.Clear();
            TilesetClient client = new(Credential, Geography);

            foreach (TilesetDetailInfo item in Selection.GetSelection())
            {
                Response response = client.Delete(item.TilesetId);

                if (response.Status == (int)HttpStatusCode.NoContent)
                {
                    messages.Add($"Data with '{item.TilesetId}' has been deleted");
                }
            }

            Selection.ClearSelection();
            GetData();
            StateHasChanged();
        }

        private void Get()
        {
            messages.Clear();

            TilesetDetailInfo item = Selection.GetSelection()[0];

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Dataset: {item.DatasetId}");

            sb.AppendLine("Bounding box:");
            sb.AppendLine($"  Min lon/lat: {item.Bbox[0]}, {item.Bbox[1]}");
            sb.AppendLine($"  Max lon/lat: {item.Bbox[2]}, {item.Bbox[3]}");
            sb.AppendLine($"Min zoom: {item.MinZoom}");
            sb.AppendLine($"Max zoom: {item.MaxZoom}");
            sb.AppendLine($"Ontology: {item.Ontology}");
            sb.AppendLine($"Description: {item.Description ?? "-"}");

            details = (MarkupString)sb.ToString().Replace(" ", "&nbsp;").Replace("\r\n", "<br />");

            StateHasChanged();
        }
    }
}