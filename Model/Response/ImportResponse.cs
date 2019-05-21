using Newtonsoft.Json;

namespace SouthernApi.Model.Response
{
    public class ImportResponse
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
        [JsonProperty("originalFilename", NullValueHandling = NullValueHandling.Ignore)]
        public string OriginalFilename { get; set; }
        public string Type { get; set; }
    }
}
