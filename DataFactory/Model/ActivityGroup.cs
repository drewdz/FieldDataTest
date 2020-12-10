using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DataFactory.Model
{
    public class ActivityGroup : NotifyBase
    {
        [JsonProperty("id")]
        public Guid? Id { get; set; }

        [JsonProperty("created")]
        [JsonConverter(typeof(DateTimeConverter), "yyyy-MM-ddTHH:mm:ss.fffzz00")]
        public DateTimeOffset Created { get; set; }

        private int _State = 0;
        [JsonProperty("state")]
        public int State
        {
            get => _State;
            set => SetProperty(ref _State, value);
        }

        [JsonProperty("activityId")]
        public Guid ActivityId { get; set; }

        private List<PlayerActivity> _PlayerActivities = null;
        [JsonProperty("playerActivities")]
        public List<PlayerActivity> PlayerActivities
        {
            get => _PlayerActivities;
            set => SetProperty(ref _PlayerActivities, value);
        }
    }
}
