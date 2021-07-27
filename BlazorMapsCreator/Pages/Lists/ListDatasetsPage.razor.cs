using System;
using System.Linq;
using System.Net;

using Azure;
using Azure.Maps.Creator;
using Azure.Maps.Creator.Models;

using BlazorFluentUI;

namespace BlazorMapsCreator.Pages.Management
{
    public partial class ListDatasetsPage : ListPageBase<DatasetDetailInfo>
    {
        private void GetData()
        {
            DatasetClient client = new(Credential, Geography);

            itemList = client.List();
        }
        private static void OnClick(DatasetDetailInfo item)
        {
            Console.WriteLine("Clicked!");
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Selection.GetKey = (item => item.DatasetId);
            Columns.Add(new DetailsRowColumn<DatasetDetailInfo>("DatasetUdid", x => x.DatasetId) { MaxWidth = 150, IsResizable = true, Index = 0 });
            Columns.Add(new DetailsRowColumn<DatasetDetailInfo>("Created", x => x.Created!) { Index = 1, MaxWidth = 150, IsResizable = true, OnColumnClick = OrderCreated });

            GetData();
        }

        private void OrderCreated(IDetailsRowColumn<DatasetDetailInfo> column)
        {
            //create new sorted list
            itemList = (Pageable<DatasetDetailInfo>)(column.IsSorted ? itemList.OrderBy(x => x.Created) : itemList.OrderByDescending(x => x.Created));

            column.IsSorted = !column.IsSorted;
            StateHasChanged();
        }

        private void Delete()
        {
            messages.Clear();

            DatasetClient client = new(Credential, Geography);

            foreach (DatasetDetailInfo item in Selection.GetSelection())
            {
                Response response = client.Delete(item.DatasetId);

                if (response.Status == (int)HttpStatusCode.NoContent)
                {
                    messages.Add($"Data with '{item.DatasetId}' has been deleted");
                }
            }

            Selection.ClearSelection();
            GetData();
            StateHasChanged();
        }
    }
}