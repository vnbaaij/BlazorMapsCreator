using System.Collections.Generic;

using BlazorFluentUI;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;

namespace BlazorMapsCreator.Pages.Lists
{
    public class ListPageBase<TItem> : ComponentBase
    {
        [Inject] protected IConfiguration Configuration { get; set; }


        protected MarkupString details;

        protected List<string> messages = new();

        protected List<TItem> itemList;
        protected List<IDetailsRowColumn<TItem>> Columns = new();
        protected Selection<TItem> Selection = new();

        protected string Geography { get; set; }
        protected string SubscriptionKey { get; set; }

        protected override void OnInitialized()
        {
            Geography = Configuration["AzureMaps:Geography"];

            SubscriptionKey = Configuration["AzureMaps:SubscriptionKey"];

            base.OnInitialized();
        }
    }
}

