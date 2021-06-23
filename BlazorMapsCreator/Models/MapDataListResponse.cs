using System.Collections.Generic;

namespace BlazorMapsCreator.Models
{
    public record MapDataListResponse
    {
        public List<MapDataDetailInfo> mapDataList { get; set; }
    }
}
