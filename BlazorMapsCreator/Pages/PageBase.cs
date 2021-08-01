using System;
using System.Collections.Generic;
using BlazorFluentUI;
using BlazorMapsCreator.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;

namespace BlazorMapsCreator.Pages
{
    public class PageBase<TItem> : ComponentBase, IDisposable
    {
        [Inject] protected IConfiguration Configuration { get; set; }

        protected MarkupString details;

        protected List<MessageItem> messages = new();

        protected List<TItem> itemList;
        protected List<IDetailsRowColumn<TItem>> Columns = new();
        protected Selection<TItem> Selection = new();

        protected string baseUrl;

        protected string Geography { get; set; }
        protected string SubscriptionKey { get; set; }

        protected override void OnInitialized()
        {
            Geography = Configuration["AzureMaps:Geography"];
            SubscriptionKey = Configuration["AzureMaps:SubscriptionKey"];

            baseUrl = $"https://{Geography}.atlas.microsoft.com";

            base.OnInitialized();
        }

        public virtual void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}

