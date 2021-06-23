using System;

namespace BlazorMapsCreator.Models
{
    public record UploadStatusResponse
    {
        public string operationId { get; init; }
        public DateTime created { get; init; }
        public string status { get; init; }
        public Error error { get; init; }
    }

    public record Error
    {
        public string message { get; init; }
        public Detail[] details { get; init; }
    }

    public record Detail
    {
        public string code { get; init; }
        public string message { get; init; }
        public Detail1[] details { get; init; }
    }

    public record Detail1
    {
        public string message { get; init; }
    }



}
