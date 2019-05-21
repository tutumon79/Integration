using Newtonsoft.Json;

namespace SouthernApi.Model.Request
{
    public class Location
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
    }
}