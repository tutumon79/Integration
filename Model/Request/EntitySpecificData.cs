using Newtonsoft.Json;

namespace SouthernApi.Model.Request
{
    public class EntitySpecificData
    {
        [JsonProperty("entityIdentifier", NullValueHandling = NullValueHandling.Ignore)]
        public string EntityIdentifier { get; set; }
        [JsonProperty("properties", NullValueHandling = NullValueHandling.Ignore)]
        public Properties Properties { get; set; }
    }
}