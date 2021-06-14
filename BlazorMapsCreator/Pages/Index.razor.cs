using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorMapsCreator.Pages
{
    public partial class Index
    {
        private readonly List<string> _geographies = new() { "us", "eu"};
        private string geography;
        private string subscriptionkey;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                geography = await localStorage.GetItemAsync<string>("geography");
                subscriptionkey = await localStorage.GetItemAsync<string>("subscriptionkey");
                StateHasChanged();
            }
        }

        private async Task Save()
        {
            await localStorage.SetItemAsync("geography", geography);
            await localStorage.SetItemAsync("subscriptionkey", subscriptionkey);
        }
    }
}