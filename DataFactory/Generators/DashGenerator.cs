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
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;

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

        public List<EventData> Generate(DateTime startDate, int sampleCount)
        {
            if (sampleCount <= 0) sampleCount = 1;
            var data = new List<EventData>();
            long runTime = startDate.Subtract(Constants.UnixEpoch).Ticks / 10000;
            for(int i = 0; i < sampleCount; i++)
            {
                data.AddRange(Generate(runTime));
                runTime = data.Max(r => r.Timestamp) + 100;
            }
            return data;
        }

        public List<EventData> Generate(long millis)
        {
            var tag = Guid.NewGuid().ToString();
            //  generate an average velocity for this runner
            var v = 9 + (RANGE * (float)_Random.NextDouble());
            //  generate a starting point
            var x = _Activity.Bounds.X0;
            var y = _Activity.Bounds.Y0 + ((_Activity.Bounds.Y1 - _Activity.Bounds.Y0) * (float)_Random.NextDouble());
            //  walk to starting point
            var data = Walk(millis, tag, new List<PointF> { new PointF(_Activity.QueuePoint.X0, _Activity.QueuePoint.Y0), new PointF(x, y)});
            millis = data.Max(w => w.Timestamp) + 100;
            //  run
            data.AddRange(GenerateRun(millis, tag, x, y, v, 0.65f, _Activity.Bounds));
            //  walk to collection point
            var last = data.OrderByDescending(d => d.Timestamp).FirstOrDefault();
            millis = last.Timestamp + 100;
            data.AddRange(Walk(millis, tag, new List<PointF> { new PointF(last.X, last.Y), new PointF(_Activity.CollectionPoint.X0, _Activity.CollectionPoint.Y0) }));
            return data;
        }

        private List<EventData> GenerateRun(long millis, string tag, float x, float y, float vMax, float variance, BoundingBox bounds)
        {
            var timeToMax = 6f / (MAX_V_IN + (float)_Random.NextDouble());
            float v = 0;
            float t = 0;
            bool accelarating = true;
            var data = new List<EventData>();
            while (x < bounds.X1)
            {
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
                x += v * 0.1f;
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
