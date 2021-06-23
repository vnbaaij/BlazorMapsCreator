namespace BlazorMapsCreator.Models
{
    public record ODataErrorResponse
    {
        public ODataError error { get; init; }
    }

    public record ODataError
    {
        public string code { get; init; }
        public string message { get; init; }
        public string target { get; init; }
        public ODataError[] details { get; init; }
    }

}
