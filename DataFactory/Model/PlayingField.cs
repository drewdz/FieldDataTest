using System;
using System.Collections.Generic;

namespace DataFactory.Model
{
    public class PlayingField
    {
        #region Fields

        private long _Millis;

        #endregion Fields

        #region Properties

        public BoundingBox Padding { get; set; } = new BoundingBox();

        public List<FieldActivity> Activities { get; set; }

        #endregion Properties

        #region Initialize

        public void Init(DateTimeOffset time)
        {
            //_Millis = time.Subtract(Constants.UnixEpoch).Milliseconds;
            _Millis = time.ToUnixTimeMilliseconds();
            foreach (var activity in Activities)
            {
                activity.Init(_Millis);
            }
        }

        public List<EventData> CreateSamples(bool waitScan)
        {
            var data = new List<EventData>();
            foreach (var activity in Activities)
            {
                data.AddRange(activity.CreateSamples(_Millis, waitScan));
            }
            _Millis++;
            return data;
        }

        #endregion Initialize
    }
}
