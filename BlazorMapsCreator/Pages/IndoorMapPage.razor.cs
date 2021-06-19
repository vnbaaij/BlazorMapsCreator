using System;
using System.Threading.Tasks;

using AzureMapsControl.Components.Map;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;

namespace BlazorMapsCreator.Pages
{
    public partial class IndoorMapPage
    {

        [Inject] AzureMapsControl.Components.Indoor.IIndoorService IndoorService { get; set; }
        [Inject] IConfiguration Configuration { get; set; }
        [Inject] Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }

        public async Task OnMapReadyAsync(MapEventArgs eventArgs)
        {
            var levelControl = new AzureMapsControl.Components.Indoor.LevelControl(new AzureMapsControl.Components.Indoor.LevelControlOptions
            {
                Position = AzureMapsControl.Components.Controls.ControlPosition.TopRight
            });

            var statesetId = await LocalStorage.GetItemAsync<string>("stateset-udid");

            var options = new AzureMapsControl.Components.Indoor.IndoorManagerOptions
            {
                Geography = Configuration["AzureMaps:Geography"],
                LevelControl = levelControl,
                StatesetId = statesetId,
                TilesetId = await LocalStorage.GetItemAsync<string>("tileset-udid")
            };

            var indoorManager = await IndoorService.CreateIndoorManagerAsync(options, AzureMapsControl.Components.Indoor.IndoorManagerEventActivationFlags.All());

            indoorManager.OnFacilityChanged += eventArgs =>
            {
                Console.WriteLine("OnFacilityChanged");
                Console.WriteLine($"Switched facility from {eventArgs.PrevFacilityId} to {eventArgs.FacilityId}");
                Console.WriteLine($"Switched level from {eventArgs.PrevLevelNumber} to {eventArgs.LevelNumber}");
            };

            indoorManager.OnLevelChanged += eventArgs =>
            {
                Console.WriteLine("OnLevelChanged");
                Console.WriteLine($"Switched facility from {eventArgs.PrevFacilityId} to {eventArgs.FacilityId}");
                Console.WriteLine($"Switched level from {eventArgs.PrevLevelNumber} to {eventArgs.LevelNumber}");
            };

            if (!string.IsNullOrWhiteSpace(statesetId))
            {
                await indoorManager.SetDynamicStylingAsync(true);
            }
        }
    }
}