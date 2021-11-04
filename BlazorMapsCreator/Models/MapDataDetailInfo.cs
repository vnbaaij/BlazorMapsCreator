using System;

namespace BlazorMapsCreator.Models
{

    public record MapDataDetailInfo
    {
        public string udid { get; init; }
        public string location { get; init; }
        public string description { get; init; }
        public DateTime created { get; init; }
        public DateTime updated { get; init; }
        public int sizeInBytes { get; init; }
        public string uploadStatus { get; init; }
        public string dataFormat { get; init; }
    }
}
