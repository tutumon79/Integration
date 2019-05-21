using Newtonsoft.Json;

namespace SouthernApi.Model.Response
{
    public class Supplier
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
        [JsonProperty("label", NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; }
        [JsonProperty("entityId", NullValueHandling = NullValueHandling.Ignore)]
        public string EntityId { get; set; }
    }
}