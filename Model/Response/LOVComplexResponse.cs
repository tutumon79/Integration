using Newtonsoft.Json;
using System.Collections.Generic;

namespace SouthernApi.Model.Response
{
    public class LOVComplexResponse
    {
        [JsonProperty("cacheId", NullValueHandling = NullValueHandling.Ignore)]
        public string CacheId { get; set; }
        [JsonProperty("entityIdentifier", NullValueHandling = NullValueHandling.Ignore)]
        public string EntityIdentifier { get; set; }
        [JsonProperty("totalSize", NullValueHandling = NullValueHandling.Ignore)]
        public int TotalSize { get; set; }
        [JsonProperty("startIndex", NullValueHandling = NullValueHandling.Ignore)]
        public int StartIndex { get; set; }
        [JsonProperty("pageSize", NullValueHandling = NullValueHandling.Ignore)]
        public int PageSize { get; set; }
        [JsonProperty("rowCount", NullValueHandling = NullValueHandling.Ignore)]
        public int RowCount { get; set; }
        [JsonProperty("columnCount", NullValueHandling = NullValueHandling.Ignore)]
        public int ColumnCount { get; set; }
        [JsonProperty("columns", NullValueHandling = NullValueHandling.Ignore)]
        public object[] Columns { get; set; }
        [JsonProperty("rows", NullValueHandling = NullValueHandling.Ignore)]
        public List<ComplexData> Rows { get; set; }
    }
    public class ComplexData
    {
        [JsonProperty("object", NullValueHandling = NullValueHandling.Ignore)]
        public KeyObject Key { get; set; }
        [JsonProperty("values", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Values { get; set; }
    }

    public class KeyObject
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
        [JsonProperty("entityId", NullValueHandling = NullValueHandling.Ignore)]
        public int EntityId { get; set; }
    }
}
