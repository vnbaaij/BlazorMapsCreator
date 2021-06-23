using System;

namespace BlazorMapsCreator.Models
{
    public record ConversionListDetailInfo
    {
        public string conversionId { get; init; }
        public string udid { get; init; }
        public DateTime created { get; init; }
        public string description { get; init; } = "";
        public string ontology { get; init; } = "";
        public Featurecounts featureCounts { get; init; }
    }

}
