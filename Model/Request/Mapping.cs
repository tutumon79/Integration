using Newtonsoft.Json;

namespace SouthernApi.Model.Request
{
    public class Mapping
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
    }
}