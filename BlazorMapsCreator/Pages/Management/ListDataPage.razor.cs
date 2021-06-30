using System;
using System.Collections.Generic;

using Azure.Maps.Creator;
using Azure.Maps.Creator.Models;

using BlazorFluentUI;

namespace BlazorMapsCreator.Pages.Management
{
    public partial class ListDataPage : ListPageBase<MapDataDetailInfo>
    {
        private IList<MapDataDetailInfo> dataList;
        private void GetData()
        {
            DataClient client = new(Credential, Geography);

            MapDataListResponse response = client.ListPreview();

            dataList = (IList<MapDataDetailInfo>)response.MapDataList;
        }

        private static void OnClick(MapDataDetailInfo item)
        {
            Console.WriteLine("Clicked!");
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Selection.GetKey = (item => item.Udid);
            Columns.Add(new DetailsRowColumn<MapDataDetailInfo>("Udid", x => x.Udid) { MaxWidth = 150, IsResizable = true, Index = 0 });
            Columns.Add(new DetailsRowColumn<MapDataDetailInfo>("Status", x => x.UploadStatus!) { Index = 3, MaxWidth = 100, IsResizable = true });
            Columns.Add(new DetailsRowColumn<MapDataDetailInfo>("Size (kB)", x => x.SizeInBytes!) { Index = 4, MaxWidth = 100, IsResizable = true });
            Columns.Add(new DetailsRowColumn<MapDataDetailInfo>("Format", x => x.DataFormat!) { Index = 5, MaxWidth = 100, IsResizable = true });
            Columns.Add(new DetailsRowColumn<MapDataDetailInfo>("Location", x => x.Location!) { Index = 6, MaxWidth = 450, IsResizable = true });
            //Columns.Add(new DetailsRowColumn<MapDataDetailInfo>("Description", x => x.Description) { Index = 7 });

            GetData();
        }

        private void Delete()
        {
            messages.Clear();
            DataClient client = new(Credential, Geography);

            foreach (MapDataDetailInfo item in Selection.GetSelection())
            {
                client.DeletePreview(item.Udid);
            }
            GetData();
            Selection.ClearSelection();
            StateHasChanged();
        }
    }
}