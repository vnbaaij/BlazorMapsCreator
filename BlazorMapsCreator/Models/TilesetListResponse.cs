using System.Collections.Generic;

namespace BlazorMapsCreator.Models
{
    public record TilesetListResponse
    {
        public List<TilesetDetailInfo> tilesets { get; set; }
    }

}
