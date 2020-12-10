using Newtonsoft.Json;
using System;

namespace DataFactory.Model
{
    public class PlayerActivityData : NotifyBase
    {
        [JsonProperty("id")]
        public Guid? Id { get; set; }

        [JsonProperty("playerActivityId")]
        public string PlayerActivityId { get; set; }

        [JsonProperty("time", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(DateTimeConverter), "yyyy-MM-ddTHH:mm:ss.fffzz00")]
        public DateTimeOffset Time { get; set; }

        [JsonProperty("mediaMetaId")]
        public string MediaMetaId { get; set; }
    }
}
