using System;
using System.Linq;
using System.Net;

using Azure;
using Azure.Maps.Creator;
using Azure.Maps.Creator.Models;

using BlazorFluentUI;

namespace BlazorMapsCreator.Pages.Management
{
    public partial class ListAliasesPage : ListPageBase<AliasListItem>
    {
        private void GetData()
        {
            AliasClient client = new(Credential, Geography);

            itemList = client.List();
        }
        private static void OnClick(AliasListItem item)
        {
            Console.WriteLine("Clicked!");
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Selection.GetKey = (item => item.AliasId);
            Columns.Add(new DetailsRowColumn<AliasListItem>("Udid", x => x.AliasId) { MaxWidth = 150, IsResizable = true, Index = 0 });
            Columns.Add(new DetailsRowColumn<AliasListItem>("Created", x => x.CreatedTimestamp!) { Index = 1, MaxWidth = 150, IsResizable = true, OnColumnClick = OrderCreated });
            Columns.Add(new DetailsRowColumn<AliasListItem>("Updated", x => x.LastUpdatedTimestamp!) { Index = 2, MaxWidth = 150, IsResizable = true, OnColumnClick = OrderUpdated });

            GetData();
        }

        private void OrderCreated(IDetailsRowColumn<AliasListItem> column)
        {
            itemList = (Pageable<AliasListItem>)(column.IsSorted ? itemList.OrderBy(x => x.CreatedTimestamp) : itemList.OrderByDescending(x => x.CreatedTimestamp));

            column.IsSorted = !column.IsSorted;
            StateHasChanged();
        }

        private void OrderUpdated(IDetailsRowColumn<AliasListItem> column)
        {
            itemList = (Pageable<AliasListItem>)(column.IsSorted ? itemList.OrderBy(x => x.LastUpdatedTimestamp) : itemList.OrderByDescending(x => x.LastUpdatedTimestamp));

            column.IsSorted = !column.IsSorted;
            StateHasChanged();
        }

        private void Delete()
        {
            messages.Clear();

            AliasClient client = new(Credential, Geography);

            foreach (AliasListItem item in Selection.GetSelection())
            {
                Response response = client.Delete(item.AliasId);

                if (response.Status == (int)HttpStatusCode.NoContent)
                {
                    messages.Add($"Data with '{item.AliasId}' has been deleted");
                }
            }

            Selection.ClearSelection();
            GetData();
            StateHasChanged();
        }
    }
}