//using DataMonitor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace DataFactory.Model
{
    public class TrackingTag
    {
        #region Constants

        private const float METERS_TO_FEET = 3.28084f;

        #endregion Constants

        #region Fields

        private Timer _Timer;
        //private DataMonitor.Monitor _Monitor;

        #endregion Fields

        #region Properties

        [JsonProperty("chipId")]
        public string TagId { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty]
        public bool Used { get; set; } = false;

        [JsonProperty]
        public bool InUse { get; set; } = false;

        [JsonProperty]
        public bool IsBall { get; set; }

        [JsonProperty]
        public long Timestamp { get; set; }

        [JsonProperty]
        public float X { get; set; }

        [JsonProperty]
        public int XOut
        {
            get { return (int)(X * 10 * METERS_TO_FEET); }
        }

        [JsonProperty]
        public float Y { get; set; }

        [JsonProperty]
        public int YOut
        {
            get { return (int)(Y * 10 * METERS_TO_FEET); }
        }

        [JsonProperty]
        public float Z { get; set; }

        [JsonProperty]
        public float V { get; set; }

        [JsonProperty]
        public int VOut
        {
            get { return (int)(V * METERS_TO_FEET); }
        }

        [JsonProperty]
        public float R { get; set; }

        [JsonProperty]
        public int ROut
        {
            get { return (int)(R * METERS_TO_FEET); }
        }

        #endregion Properties

        #region Operations

        public void StepSample(EventData data)
        {
            X = data.X;
            Y = data.Y;
            V = data.V;
            R = data.R;
            Timestamp = data.Timestamp;
        }

        //public void StartTimer(DataMonitor.Monitor monitor)
        //{
        //    _Monitor = monitor;
        //    _Timer = new Timer(Tick, null, 0, 100);
        //}

        //public void StopTimer()
        //{
        //    if (_Timer == null) return;
        //    _Timer.Dispose();
        //    _Timer = null;
        //}

        public override string ToString()
        {
            return $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()},{TagId},{XOut},{YOut},{VOut},{ROut}";
        }

        public EventData ToEventData(long millis)
        {
            return new EventData { Timestamp = millis, TagId = TagId, X = X, Y = Y, V = V, R = R, Z = Z };
        }


        #endregion Operations

        #region Timer

        //private void Tick(object o)
        //{
        //    if (_Monitor == null) return;
        //    _Monitor.AddData(ToString());
        //}

        #endregion Timer
    }
}
