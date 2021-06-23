using System.Collections.Generic;

namespace BlazorMapsCreator.Models
{
    public record DatasetListResponse
    {
        public List<DatasetDetailInfo> datasets { get; set; }
    }

}
