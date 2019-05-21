using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SouthernApi.Model.Request
{

    public class ScheduleJobRequest
    {
        [JsonProperty("files", NullValueHandling = NullValueHandling.Ignore)]
        public List<Location> Files { get; set; }
        [JsonProperty("mapping", NullValueHandling = NullValueHandling.Ignore)]
        public Mapping Mapping { get; set; }
        [JsonProperty("entitySpecificData", NullValueHandling = NullValueHandling.Ignore)]
        public List<EntitySpecificData> EntitySpecificData { get; set; }
    }
}
