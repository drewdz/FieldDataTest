using Newtonsoft.Json;
using System;

namespace DataFactory.Model
{
    public class EventData
    {
        #region Properties

        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public long Timestamp { get; set; }

        [JsonProperty("chipId")]
        public string TagId { get; set; }

        [JsonProperty("x")]
        public float X { get; set; }

        [JsonProperty("y")]
        public float Y { get; set; }

        public float Z { get; set; }

        [JsonProperty("velocity")]
        public float V { get; set; }

        [JsonProperty("rpm")]
        public float R { get; set; }

        #endregion Properties

        #region Helpers

        public static EventData FromCsv(string csv)
        {
            if (string.IsNullOrWhiteSpace(csv)) return null;
            //  tokenize
            var parts = csv.Split(new char[] { ',' });
            if (parts.Length < 6) return null;
            return new EventData
            {
                Timestamp = long.Parse(parts[0]),
                TagId = parts[1],
                X = float.Parse(parts[2]) / 10,
                Y = float.Parse(parts[3]) / 10,
                V = float.Parse(parts[4]) / 10,
                R = float.Parse(parts[5])
            };
        }

        public override string ToString()
        {
            return $"{Timestamp},{TagId},{(int)(X * 10)},{(int)(Y * 10)},{(int)(V * 10)},{(int)R}";
        }

        public EventData Copy()
        {
            return new EventData
            {
                Id = Id,
                Timestamp = Timestamp,
                TagId = TagId,
                X = X,
                Y = Y,
                Z = Z,
                V = V,
                R = R
            };
        }

        public EventData CopyToFeet()
        {
            return new EventData
            {
                Timestamp = Timestamp,
                TagId = TagId,
                X = X / Constants.FEET_TO_METERS,
                Y = Y / Constants.FEET_TO_METERS,
                Z = Z / Constants.FEET_TO_METERS,
                V = V / Constants.FEET_TO_METERS,
                R = R
            };
        }

        #endregion Helpers
    }
}
