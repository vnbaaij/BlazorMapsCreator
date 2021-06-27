using System;
using System.Linq;
using System.Net;

using Azure;
using Azure.Maps.Creator;
using Azure.Maps.Creator.Models;

using BlazorFluentUI;

namespace BlazorMapsCreator.Pages.Lists
{
    public partial class ListDatasetsPage : ListPageBase<DatasetDetailInfo>
    {
        private void GetData()
        {
            DatasetClient client = new(Credential, Geography);

            itemList = client.List();
        }
        private void OnClick(DatasetDetailInfo item)
        {
            Console.WriteLine("Clicked!");
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Selection.GetKey = (item => item.DatasetId);
            Columns.Add(new DetailsRowColumn<DatasetDetailInfo>("DatasetUdid", x => x.DatasetId) { MaxWidth = 150, IsResizable = true, Index = 0 });
            Columns.Add(new DetailsRowColumn<DatasetDetailInfo>("Created", x => x.Created!) { Index = 1, MaxWidth = 150, IsResizable = true, OnColumnClick = OrderCreated });
            //Columns.Add(new DetailsRowColumn<DatasetDetailInfo>("Ontology", x => x.Ontology!) { Index = 2 });
            //Columns.Add(new DetailsRowColumn<DatasetDetailInfo>("Description", x => x.Description) { Index = 3 });
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