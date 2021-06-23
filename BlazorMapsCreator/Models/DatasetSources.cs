using System.Collections.Generic;

namespace BlazorMapsCreator.Models
{
    public record DatasetSources
    {
        public List<string> conversionIds { get; init; }
        public string appendDatasetId { get; init; }
    }

}
