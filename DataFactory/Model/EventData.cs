using System;

namespace DataFactory.Model
{
    public class EventData
    {
        #region Properties

        public long Timestamp { get; set; }

        public string TagId { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public float V { get; set; }

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
            return $"{Timestamp},{TagId},{(int)((X / Constants.FEET_TO_METERS) * 10)},{(int)((Y / Constants.FEET_TO_METERS) * 10)},{(int)((V / Constants.FEET_TO_METERS) * 10)},{(int)R}";
        }

        public EventData Copy()
        {
            return new EventData
            {
                Timestamp = Timestamp,
                TagId = TagId,
                X = X,
                Y = Y,
                Z = Z,
                V = V,
                R = R
            };
        }

        #endregion Helpers
    }
}
