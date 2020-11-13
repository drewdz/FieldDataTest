/*      Generate Sample 40 Yard Dash Data
 * 
 *      - takes place in a demarkated lane
 *      - 40 yards long
 *      - runner will run in a more-or-less straight line from start to finish
 *      - people run at about 9-13.5 ft/s, eg: 10-15km/h
 */

using DataFactory.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace DataFactory.Generators
{
    public class DashGenerator : GeneratorBase, IGenerator
    {
        #region Constants

        /// <summary>
        /// Velocity variance for humans - 13.5-9 = 4.5
        /// </summary>
        private const float RANGE = 4.5f;
        /// <summary>
        /// Seconds to reach max velocity
        /// </summary>
        private const float MAX_V_IN = 4.5f;

        #endregion Constants

        #region Constructors

        public DashGenerator()
        {
        }

        #endregion Constructors

        #region Properties

        public string Name => "Dash";

        public string ActionName => "Do 40 Yard Dash";

        #endregion Properties

        #region Operations

        public override void SetQueues()
        {
            try
            {
                if (_Activity.Direction > 0)
                {
                    //  queue point
                    _Activity.QueuePoint = new BoundingBox { X0 = _Activity.X1 + 5, Y0 = _Activity.Y0 + ((_Activity.Y1 - _Activity.Y0) / 2) };
                    //  collect point
                    _Activity.CollectionPoint = new BoundingBox { X0 = _Activity.X0 - 5, Y0 = _Activity.Y0 + ((_Activity.Y1 - _Activity.Y0) / 2) };
                }
                else
                {
                    //  queue point
                    _Activity.QueuePoint = new BoundingBox { X0 = _Activity.X0 - 5, Y0 = _Activity.Y0 + ((_Activity.Y1 - _Activity.Y0) / 2) };
                    //  collect point
                    _Activity.CollectionPoint = new BoundingBox { X0 = _Activity.X1 + 5, Y0 = _Activity.Y0 + ((_Activity.Y1 - _Activity.Y0) / 2) };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not set queues for dash. Exception: {ex}");
            }
        }

        public List<EventData> Generate(DateTimeOffset startDate, int sampleCount)
        {
            Debug.WriteLine("Dash generator");
            if (sampleCount <= 0) sampleCount = 1;
            var data = new List<EventData>();
            long runTime = startDate.ToUnixTimeMilliseconds();
            for(int i = 0; i < sampleCount; i++)
            {
                data.AddRange(Generate(runTime));
                runTime = data.Max(r => r.Timestamp) + 100;
            }
            return data;
        }

        public List<EventData> Generate(long millis)
        {
            Debug.WriteLine("Dash generate");
            var tag = Guid.NewGuid().ToString();
            //  generate an average velocity for this runner
            var v = 9 + (RANGE * (float)_Random.NextDouble());
            //  generate a starting point
            var x = (_Activity.Direction == 0) ? _Activity.X0 : _Activity.X1;
            var y = _Activity.Y0 + ((_Activity.Y1 - _Activity.Y0) * (float)_Random.NextDouble());
            //  walk to starting point
            Debug.WriteLine($"Dash generate walk - q: {_Activity.QueuePoint.X0},{_Activity.QueuePoint.Y0}, to: {x},{y}, dir: {_Activity.Direction}");
            var data = Walk(millis, tag, new List<PointF> { new PointF(_Activity.QueuePoint.X0, _Activity.QueuePoint.Y0), new PointF(x, y)});
            millis = data.Max(w => w.Timestamp) + 100;
            //  run
            Debug.WriteLine($"Dash generate - points: {data.Count}, v: {v}, x: {x}, y: {y}, dir: {_Activity.Direction}");
            data.AddRange(GenerateRun(millis, tag, x, y, v, 0.65f));
            //  walk to collection point
            var last = data.OrderByDescending(d => d.Timestamp).FirstOrDefault();
            millis = last.Timestamp + 100;
            Debug.WriteLine($"Dash generate walk - points: {data.Count}, from: {last.X},{last.Y}, collect: {_Activity.CollectionPoint.X0},{_Activity.CollectionPoint.Y0}, dir: {_Activity.Direction}");
            data.AddRange(Walk(millis, tag, new List<PointF> { new PointF(last.X, last.Y), new PointF(_Activity.CollectionPoint.X0, _Activity.CollectionPoint.Y0) }));
            Debug.WriteLine($"Dash generate done - points: {data.Count}, dir: {_Activity.Direction}");
            return data;
        }

        private List<EventData> GenerateRun(long millis, string tag, float x, float y, float vMax, float variance)
        {
            var timeToMax = 6f / (MAX_V_IN + (float)_Random.NextDouble());
            float v = 0;
            float t = 0;
            bool accelarating = true;
            var data = new List<EventData>();
            var direction = (_Activity.Direction == 0) ? 1 : -1;
            while (true)
            {
                if (((direction >= 0) && (x >= _Activity.X1)) || ((direction < 0) && (x <= _Activity.X0))) break;
                //  get the velocity for this sample
                if (accelarating)
                {
                    v = Functions.Sigmoid2(t * timeToMax) * vMax;
                    if ((v + Constants.EPSILON) >= vMax)
                    {
                        v = vMax;
                        accelarating = false;
                    }
                }
                else
                {
                    v = vMax + ((float)_Random.NextDouble() * variance) - (variance / 2);
                }
                //  move
                x += (v * 0.1f) * direction;
                //  TODO: move y toward the centre
                data.Add(new EventData
                {
                    TagId = tag,
                    Timestamp = millis,
                    R = 0,
                    V = v,
                    X = x,
                    Y = y
                });
                t += 0.1f;
                millis += 100;
            }
            return data;
        }

        #endregion Operations

        #region Incremental

        public override EventData CreateSample(TrackingTag tag)
        {
            return null;
        }

        #endregion Incremental
    }
}
