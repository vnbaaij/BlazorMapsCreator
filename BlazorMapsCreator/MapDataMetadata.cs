using System;

namespace BlazorMapsCreator
{

    public record MapDataMetadata
    {
        public string udid { get; set; }
        public string location { get; set; }
        public DateTime created { get; set; }
        public DateTime updated { get; set; }
        public int sizeInBytes { get; set; }
        public string uploadStatus { get; set; }
        public string dataFormat { get; set; }
    }
}
