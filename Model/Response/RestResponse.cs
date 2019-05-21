using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SouthernApi.Model.Response
{
    public class RESTResponse
    {
        [DataMember(EmitDefaultValue = false)]
        public dynamic Body { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int Status { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public Dictionary<string, string> Headers { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string RequestUri { get; set; }
    }
}
