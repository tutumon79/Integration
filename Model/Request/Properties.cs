using Newtonsoft.Json;

namespace SouthernApi.Model.Request
{
    public class Properties
    {
        [JsonProperty("parent", NullValueHandling = NullValueHandling.Ignore)]
        public string Parent { get; set; }
        [JsonProperty("groupAssignmentMode", NullValueHandling = NullValueHandling.Ignore)]
        public string GroupAssignmentMode { get; set; }
    }
}