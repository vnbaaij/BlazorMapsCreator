using System.Collections.Generic;

namespace BlazorMapsCreator.Models
{
    public record ConversionListResponse
    {
        public List<ConversionListDetailInfo> conversions { get; set; }
    }

}
