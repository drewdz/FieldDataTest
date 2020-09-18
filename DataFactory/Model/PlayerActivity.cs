using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;

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
        [JsonConverter(typeof(DateTimeConverter), "yyyy-MM-ddTHH:mm:ss.fffzz00")]
        public DateTimeOffset? Scanned { get; set; }

        [JsonProperty("startTime", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(DateTimeConverter), "yyyy-MM-ddTHH:mm:ss.fffzz00")]
        public DateTimeOffset? StartTime { get; set; }

        [JsonProperty("endTime", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(DateTimeConverter), "yyyy-MM-ddTHH:mm:ss.fffzz00")]
        public DateTimeOffset? EndTime { get; set; }

        [JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
        public int State { get; set; }

        [JsonProperty("age")]
        public int Age { get; set; } = 0;

        [JsonProperty("processed")]
        public int Processed { get; set; } = 0;
    }

    public class DateTimeConverter : IsoDateTimeConverter
    {
        public DateTimeConverter(string format)
        {
            DateTimeFormat = format;
        }
    }
}
