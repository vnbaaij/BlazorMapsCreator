using System;

namespace BlazorMapsCreator.Models
{
    public record DatasetDetailInfo
    {
        public DateTime created { get; init; }
        public string datasetId { get; init; }
        public string description { get; init; } = "";
        public DatasetSources datasetSources { get; init; }
        public Featurecounts featureCounts { get; init; }
        public string ontology { get; init; }
    }

}
