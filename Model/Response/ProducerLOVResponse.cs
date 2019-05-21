using Newtonsoft.Json;
using System.Collections.Generic;
namespace SouthernApi.Model.Response
{
    public class ProducerLOVResponse
    {
        [JsonProperty("entries", NullValueHandling = NullValueHandling.Ignore)]
        public List<ProducerEntry> Entires { get; set; }
    }
}
