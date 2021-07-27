using System.Collections.Generic;

using Azure;
using Azure.Maps.Creator.Models;

using BlazorFluentUI;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;

namespace BlazorMapsCreator.Pages
{
    public class PageBase<TItem> : ComponentBase
    {
        [Inject] protected IConfiguration Configuration { get; set; }

        private string subscriptionkey;
        protected MarkupString details;

        protected List<string> messages = new();

        protected Pageable<TItem> itemList;
        protected List<IDetailsRowColumn<TItem>> Columns = new();
        protected Selection<TItem> Selection = new();

        protected AzureKeyCredential Credential { get; set; }
        protected Geography Geography { get; set; }

        protected override void OnInitialized()
        {
            Geography = Configuration["AzureMaps:Geography"];

            subscriptionkey = Configuration["AzureMaps:SubscriptionKey"];
            Credential = new(subscriptionkey);

            base.OnInitialized();
        }
    }
}

