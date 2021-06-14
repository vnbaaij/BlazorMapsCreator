using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMapsCreator
{
    public class UploadStatusResponse
    {
        public string operationId { get; set; }
        public DateTime created { get; set; }
        public string status { get; set; }
        public Error error { get; set; }
    }

    public class Error
    {
        public string message { get; set; }
        public Detail[] details { get; set; }
    }

    public class Detail
    {
        public string code { get; set; }
        public string message { get; set; }
        public Detail1[] details { get; set; }
    }

    public class Detail1
    {
        public string message { get; set; }
    }



}
