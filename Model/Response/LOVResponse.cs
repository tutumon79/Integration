using Newtonsoft.Json;
using System.Collections.Generic;

namespace SouthernApi.Model.Response
{
    public class LOVResponse
    {
        [JsonProperty("Identifier", NullValueHandling = NullValueHandling.Ignore)]
        public string Identifier { get; set; }
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        [JsonProperty("dataType", NullValueHandling = NullValueHandling.Ignore)]
        public string DataType { get; set; }       
    }

    public class LabelEntry
    {
        [JsonProperty("label", NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; }
        [JsonProperty("key", NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }
        [JsonProperty("externalCode", NullValueHandling = NullValueHandling.Ignore)]
        public string ExternalCode { get; set; }
        [JsonProperty("synonyms", NullValueHandling = NullValueHandling.Ignore)]
        public object[] Synonyms { get; set; }
    }

    public class ProducerEntry
    {
        [JsonProperty("label", NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; }
        [JsonProperty("key", NullValueHandling = NullValueHandling.Ignore)]
        public EntryKey Key { get; set; }
        [JsonProperty("externalCode", NullValueHandling = NullValueHandling.Ignore)]
        public string ExternalCode { get; set; }
        [JsonProperty("synonyms", NullValueHandling = NullValueHandling.Ignore)]
        public object[] Synonyms { get; set; }
    }

    public class EntryKey
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
        [JsonProperty("label", NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; }
    }
}
