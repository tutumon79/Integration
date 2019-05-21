using Newtonsoft.Json;

namespace SouthernApi.Model.Request
{
    public class Options
    {
        [JsonProperty("createNewObjects", NullValueHandling = NullValueHandling.Ignore)]
        public bool createNewObjects { get; set; }
        [JsonProperty("updateExistingObjects", NullValueHandling = NullValueHandling.Ignore)]
        public bool UpdateExistingObjects { get; set; }
        [JsonProperty("dryRunBeforeImport", NullValueHandling = NullValueHandling.Ignore)]
        public bool DryRunBeforeImport { get; set; }
        [JsonProperty("dryRunOnly", NullValueHandling = NullValueHandling.Ignore)]
        public bool DryRunOnly { get; set; }
        [JsonProperty("errorMode", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorMode { get; set; }
        [JsonProperty("numberOfThreads", NullValueHandling = NullValueHandling.Ignore)]
        public int NumberOfThreads { get; set; }
    }
}