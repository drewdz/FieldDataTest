/*      Generate test data for the Passing Challenge
 *      
 *      User would get to throw three balls at targets
 *      
 *      Target 1 - 10 feet,
 *      Target 2 - 20 feet,
 *      Target 3 - 30 feet
 *      
 *      Speed: 13.41-22.35 m/s (30-50 mph) = 8.94 variance (43.99606-73.32677 = 29.33071 ft/s)
 *      Angle: 0-35 deg = 0-0.610865 rad 
 */

using DataFactory.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DataFactory.Generators
{
    public class PassGenerator : GeneratorBase, IGenerator
    {
        #region Constants

        private const float VMIN = 43.99606f;
        private const float VRANGE = 29.33071f;
        private const float ARANGE = 0.261799f;
        private const float RMAX = 750;
        private const float TWO_FEET = 2f;

        #endregion Constants

        #region Constructors

        public PassGenerator()
        {
        }

        #endregion Constructors

        #region Properties

        public string Name => "Pass";

        public string ActionName => "Do Passing Challenge";

        #endregion Properties

        #region Operations

        public List<EventData> Generate(DateTime startDate, int sampleCount)
        {
            if (sampleCount <= 0) sampleCount = 1;
            var data = new List<EventData>();
            long runTime = startDate.Subtract(Constants.UnixEpoch).Ticks / 10000;
            for (int i = 0; i < sampleCount; i++)
            {
                data.AddRange(Generate(runTime));
                runTime = data.Max(s => s.Timestamp) + 100;
            }
            return data;
        }

        public List<EventData> Generate(long millis)
        {
            var data = new List<EventData>();
            var tags = new List<string>();
            foreach (var target in _Activity.Targets)
            {
                var tag = Guid.NewGuid().ToString();
                tags.Add(tag);
                //  generate the start point
                var x0 = _Activity.Bounds.X0 + (TWO_FEET / 2);
                x0 += ((float)_Random.NextDouble() * TWO_FEET) - (TWO_FEET / 2);
                var y0 = _Activity.Bounds.Y0 + ((_Activity.Bounds.Y1 - _Activity.Bounds.Y0) / 2);
                y0 += ((float)_Random.NextDouble() * TWO_FEET) - (TWO_FEET / 2);
                //  walk to starting point
                data.AddRange(Walk(millis, tag, new List<PointF> { new PointF(_Activity.QueuePoint.X0, _Activity.QueuePoint.Y0), new PointF(x0, y0) }));
                millis = data.Max(w => w.Timestamp) + 1000;     //   1 second to aim
                //  get the angle to the target
                var aZ = (float)Math.Atan2(target.Bounds.Y0 - y0, target.Bounds.X0 - x0);
                aZ += ((float)_Random.NextDouble() * ARANGE) - (ARANGE / 2);
                //  generate initial velocity and throw
                var v = VMIN + ((float)_Random.NextDouble() * VRANGE);
                data.AddRange(GenerateThrow(millis, tag, x0, y0, v, aZ, _Activity.Bounds));
                var last = data.OrderByDescending(d => d.Timestamp).FirstOrDefault();
                //  1 second delay between throws
                millis = last.Timestamp + 1000;
            }
            //  collect balls and return to collection
            foreach (var tag in tags)
            {
                var last = data.Where(d => d.TagId.Equals(tag)).OrderByDescending(d => d.Timestamp).FirstOrDefault();
                //  collect the ball and take it to collection
                data.AddRange(Walk(millis, tag, new List<PointF> { new PointF(last.X, last.Y), new PointF(_Activity.CollectionPoint.X0, _Activity.CollectionPoint.Y0) }));
                millis = data.Max(r => r.Timestamp) + 2000;
            }
            return data;
        }

        private List<EventData> GenerateThrow(long millis, string tag, float x0, float y0, float s, float aZ, BoundingBox bounds)
        {
            var a = (_Random.NextDouble() * ARANGE);
            var v = new Vector(s, (float)a);
            //  get initial rotational velocity
            var r = (float)_Random.NextDouble() * RMAX;
            //  throw the ball already!
            float x = 0, z = 0, t = 0;
            var data = new List<EventData>();
            while (true)
            {
                //  check if we've hit the ground
                if ((t > 0.1f) && ((z - Constants.EPSILON) <= 0)) break;
                //  get location
                x += v.X * 0.1f;
                z += v.Y * 0.1f;
                //  decellarate by gravity
                var d = Constants.G * 0.1f;
                v = new Vector { X = v.X, Y = v.Y - d };
                data.Add(new EventData
                {
                    R = r,
                    TagId = tag,
                    Timestamp = millis,
                    V = s,
                    X = x,
                    Y = 0,
                    Z = z
                });
                t += 0.1f;
                millis += 100;
            }
            //  rotate the kick into the direction of the kick / kick space
            foreach (var p in data)
            {
                if (p.X > 0)
                {
                    var n = new Vector(p.X, aZ);
                    p.X = n.X;
                    p.Y = n.Y;
                }
                p.X += x0;
                p.Y += y0;
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
