namespace BlazorMapsCreator.Models
{
    public record TilesetDetailInfo
    {
        public string tilesetId { get; init; }
        public string datasetId { get; init; }
        public string description { get; init; } = "";
        public string ontology { get; init; } = "";
        public int minZoom { get; init; }
        public int maxZoom { get; init; }
        public float[] bbox { get; init; }
    }

}
