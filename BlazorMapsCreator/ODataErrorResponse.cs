using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMapsCreator
{
    public class ODataErrorResponse
    {
            public ODataError error { get; set; }
    }

    public class ODataError
    {
        public string code { get; set; }
        public string message { get; set; }
        public string target { get; set; }
        public ODataError[] details { get; set; }
    }

}
