using Newtonsoft.Json;
using System;

namespace DataFactory.Model
{
    public class PlayerActivity
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("activityId", NullValueHandling = NullValueHandling.Ignore)]
        public string ActivityId { get; set; }

        [JsonProperty("lanyardId", NullValueHandling = NullValueHandling.Ignore)]
        public string LanyardId { get; set; }

        [JsonProperty("scanned", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Scanned { get; set; }

        [JsonProperty("startTime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? StartTime { get; set; }

        [JsonProperty("endTime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? EndTime { get; set; }

        [JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
        public int State { get; set; }
    }
}
