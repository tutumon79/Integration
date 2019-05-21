using Newtonsoft.Json;
using System.Collections.Generic;
namespace SouthernApi.Model.Response
{
    public class LabelLOVResponse : LOVResponse
    {
        [JsonProperty("entries", NullValueHandling = NullValueHandling.Ignore)]
        public List<LabelEntry> Entires { get; set; }
    }
}
