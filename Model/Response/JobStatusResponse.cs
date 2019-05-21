using Newtonsoft.Json;
using System.Collections.Generic;

namespace SouthernApi.Model.Response
{
    public class JobStatusResponse
    {
        [JsonProperty("cacheId", NullValueHandling = NullValueHandling.Ignore)]
        public string CacheId { get; set; }
        [JsonProperty("entityIdentifier", NullValueHandling = NullValueHandling.Ignore)]
        public string EntityIdentifier { get; set; }
        [JsonProperty("totalSize", NullValueHandling = NullValueHandling.Ignore)]
        public string TotalSize { get; set; }
        [JsonProperty("startIndex", NullValueHandling = NullValueHandling.Ignore)]
        public string StartIndex { get; set; }
        [JsonProperty("pageSize", NullValueHandling = NullValueHandling.Ignore)]
        public string PageSize { get; set; }
        [JsonProperty("rowCount", NullValueHandling = NullValueHandling.Ignore)]
        public string RowCount { get; set; }
        [JsonProperty("columnCount", NullValueHandling = NullValueHandling.Ignore)]
        public string ColumnCount { get; set; }
        [JsonProperty("columns", NullValueHandling = NullValueHandling.Ignore)]
        public object[] Columns { get; set; }
        [JsonProperty("rows", NullValueHandling = NullValueHandling.Ignore)]
        public Row[] Rows { get; set; }
    }

    public class Row
    {
        [JsonProperty("object", NullValueHandling = NullValueHandling.Ignore)]
        public Supplier Supplier { get; set; }
        [JsonProperty("values", NullValueHandling = NullValueHandling.Ignore)]
        public List<object> Values { get;set; }
    }
}
