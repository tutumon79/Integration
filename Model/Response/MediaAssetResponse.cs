using Newtonsoft.Json;

namespace SouthernApi.Model.Response
{
    public class MediaAssetResponse
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
    }
}
