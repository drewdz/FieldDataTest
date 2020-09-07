/*      Generate test data for the Shuttle Run / 3 Cone run
 *      
 *      Player runs from Point A either to the left or the right to point B - 15 feet (5 yards)
 *      then they run from point B back past A to C - 30 feet (10 yards)
 *      then back to A
 *      
 *      Average times for high school trials are 7.5 - 8.5 seconds
 */

using DataFactory.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DataFactory.Generators
{
    public class ShuttleGenerator : GeneratorBase, IGenerator
    {
        #region Constants

        /// <summary>
        /// Velocity variance for humans - 13.5 - 9 = 4.5
        /// </summary>
        private const float RANGE = 14.76378f;
        /// <summary>
        /// Seconds to reach max velocity
        /// </summary>
        private const float MAX_V_IN = 4.5f;

        #endregion Constants

        #region Constructors

        public ShuttleGenerator()
        {
        }

        #endregion Constructors

        #region Properties

        public string Name => "Shuttle";

        public string ActionName => "Do Shuttle Run";

        #endregion Properties

        #region Operations

        public List<EventData> Generate(DateTimeOffset startDate, int sampleCount)
        {
            if (sampleCount <= 0) sampleCount = 1;
            var data = new List<EventData>();
            //long runTime = startDate.Subtract(Constants.UnixEpoch).Ticks / 10000;
            long runTime = startDate.ToUnixTimeMilliseconds();
            for (int i = 0; i < sampleCount; i++)
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
            var x = _Activity.Bounds.X0 + ((_Activity.Bounds.X1 - _Activity.Bounds.X0) / 2f);
            var y = _Activity.Bounds.Y0 + ((_Activity.Bounds.Y1 - _Activity.Bounds.Y0) * (float)_Random.NextDouble());
            //  walk to starting point
            var data = Walk(millis, tag, new List<PointF> { new PointF(_Activity.QueuePoint.X0, _Activity.QueuePoint.Y0), new PointF(x, y) });
            millis = data.Max(w => w.Timestamp) + 100;
            data.AddRange(GenerateRun(millis, tag, x, y, v, 0.656168f));
            //  walk to collection point
            var last = data.OrderByDescending(d => d.Timestamp).FirstOrDefault();
            millis = last.Timestamp + 100;
            data.AddRange(Walk(millis, tag, new List<PointF> { new PointF(last.X, last.Y), new PointF(_Activity.CollectionPoint.X0, _Activity.CollectionPoint.Y0) }));
            return data;
        }

        private List<EventData> GenerateRun(long millis, string tag, float x, float y, float vMax, float variance)
        {
            var data = new List<EventData>();
            //  run to the right -> 15 ft to max X
            data.AddRange(GenerateRunPart(millis, tag, x, y, vMax, variance, _Activity.Bounds.X1));
            //  run to the left -> 30 ft to min x
            var from = data[data.Count - 1];
            data.AddRange(GenerateRunPart(from.Timestamp + 100, tag, from.X, from.Y, vMax, variance, _Activity.Bounds.X0));
            //  run to the start -> 15 ft to half way
            from = data[data.Count - 1];
            data.AddRange(GenerateRunPart(from.Timestamp + 100, tag, from.X, from.Y, vMax, variance, x));
            return data;
        }

        private List<EventData> GenerateRunPart(long millis, string tag, float x, float y, float vMax, float variance, float xMax)
        {
            var timeToMax = 6f / (MAX_V_IN + (float)_Random.NextDouble());
            float v = 0;
            float t = 0;
            bool accelarating = true;
            var data = new List<EventData>();
            float dir = ((xMax - x) > 0) ? 1 : -1;

            while (((xMax - x) * dir) > 0)
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
                x += v * 0.1f * dir;
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

        public override EventData CreateSample(TrackingTag tag)
        {
            throw new NotImplementedException();
        }

        #endregion Operations
    }
}
