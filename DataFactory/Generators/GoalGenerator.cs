/*      Generate field goal kick test data
 *      
 *      The average NFL field goal distance for 2013 was 37.65 yards
 *      Since very few people would be expected to be able to kick this. The challenge will be kicked from 27 yards = 81 feet
 *      
 *      Initial Velocity: 10.3632-24.59736 m/s, range: 14.23416 (30-55 mph / 34.0-80.7 ft/s)
 *      Launch Angle: 27-42 deg / 0.471239-0.733038 rad  (range = 0.261799)
 *      RPM: 0-1400 rpm (range = 1400)
 *      Ball Weight: 0.91 lb / 0.413kg / 413g
 *      Gravity: 9.81m.s-2 
 *      Distance to posts: 24.6888 m /  27 yards
 */

using DataFactory.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace DataFactory.Generators
{
    public class GoalGenerator : GeneratorBase, IGenerator
    {
        #region Constants

        private const float VMIN = 34f;
        private const float VRANGE = 46.7f;
        private const float AMIN = 0.471239f;
        private const float ARANGE = 0.261799f;
        private const float RMAX = 1400;
        private const float DIST = 24.6888f; // 27 yards
        private const float POST_X = 24.6888f;
        private const float POST_Y = 3.3528f;
        private const float TENTH_DEGREE = 0.001745f;

        #endregion Constants

        #region Constructors

        public GoalGenerator()
        {
        }

        #endregion Constructors

        #region Properties

        public string Name => "Goal";

        public string ActionName => "Do Goal Kick";

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
            //  kick 1
            var tag = Guid.NewGuid().ToString();
            var tags = new List<string> { tag };
            Debug.WriteLine("Generating kick 1...");
            var data = Generate(millis, tag);
            //  3 second delay to next kick
            millis = data.Max(d => d.Timestamp) + 3000;
            //  kick 2
            tag = Guid.NewGuid().ToString();
            tags.Add(tag);
            Debug.WriteLine("Generating kick 2...");
            data.AddRange(Generate(millis, tag));
            millis = data.Max(d => d.Timestamp) + 3000;
            //  kick 3
            tag = Guid.NewGuid().ToString();
            tags.Add(tag);
            Debug.WriteLine("Generating kick 3...");
            data.AddRange(Generate(millis, tag));
            millis = data.Max(d => d.Timestamp) + 2000;
            //  collect balls and take to collection point - 1
            var last = data.Where(d => d.TagId.Equals(tags[0])).OrderByDescending(d => d.Timestamp).FirstOrDefault();
            data.AddRange(Walk(millis, tag, new List<PointF> { new PointF(last.X, last.Y), new PointF(_Activity.CollectionPoint.X0, _Activity.CollectionPoint.Y0) }));
            millis = data.Max(d => d.Timestamp) + 2000;
            //  collect - 2
            last = data.Where(d => d.TagId.Equals(tags[1])).OrderByDescending(d => d.Timestamp).FirstOrDefault();
            data.AddRange(Walk(millis, tag, new List<PointF> { new PointF(last.X, last.Y), new PointF(_Activity.CollectionPoint.X0, _Activity.CollectionPoint.Y0) }));
            millis = data.Max(d => d.Timestamp) + 2000;
            //  collect - 3
            last = data.Where(d => d.TagId.Equals(tags[1])).OrderByDescending(d => d.Timestamp).FirstOrDefault();
            data.AddRange(Walk(millis, tag, new List<PointF> { new PointF(last.X, last.Y), new PointF(_Activity.CollectionPoint.X0, _Activity.CollectionPoint.Y0) }));
            return data;
        }

        private List<EventData> Generate(long millis, string tag)
        {
            //  generate a starting point
            var x = (_Activity.Direction >= 0) ? _Activity.Bounds.X0 : _Activity.Bounds.X1;
            var y = _Activity.Bounds.Y0 + ((_Activity.Bounds.Y1 - _Activity.Bounds.Y0) * (float)_Random.NextDouble());
            //  walk to starting point
            var data = Walk(millis, tag, new List<PointF> { new PointF(_Activity.QueuePoint.X0, _Activity.QueuePoint.Y0), new PointF(x, y) });
            millis = data.Max(w => w.Timestamp) + 100;
            //  kick the ball
            Debug.WriteLine($"Generating kick - from: ({x},{y})...");
            data.AddRange(GenerateFlight(millis, tag, x, y, _Activity.Bounds));
            //  collect the ball and carry it to collection
            var last = data.OrderByDescending(d => d.Timestamp).FirstOrDefault();
            millis = last.Timestamp + 100;
            data.AddRange(Walk(millis, tag, new List<PointF> { new PointF(last.X, last.Y), new PointF(_Activity.CollectionPoint.X0, _Activity.CollectionPoint.Y0) }));
            return data;
        }

        private List<EventData> GenerateFlight(long millis, string tag, float x0, float y0, BoundingBox bounds)
        {
            //  get initial velocity
            var s = (float)(VMIN + (_Random.NextDouble() * VRANGE));
            var a = AMIN + (_Random.NextDouble() * ARANGE);
            var v = new Vector(s, (float)a);
            //  get initial rotational velocity
            var r = (float)_Random.NextDouble() * RMAX;
            //  kick the ball already!
            float x = 0, z = 0, t = 0;
            var data = new List<EventData>();
            while (true)
            {
                //  check if we've hit the ground
                if ((t > 0.1f) && ((z - Constants.EPSILON) <= 0)) break;
                //  get location
                x += (v.X * 0.1f) * _Activity.Direction;
                z += v.Y * 0.1f;
                //  decelerate by gravity
                var d = Constants.G * 0.1f;
                v = new Vector { X = v.X, Y = v.Y - d };
                var pt = new EventData
                {
                    R = r,
                    TagId = tag,
                    Timestamp = millis,
                    V = s,
                    X = x,
                    Y = 0,
                    Z = z
                };
                Debug.WriteLine($"Adding point: {pt}");
                data.Add(pt);
                t += 0.1f;
                millis += 100;
            }
            //  get the angle to the centre of the uprights
            var m = bounds.Y0 + ((bounds.Y1 - bounds.Y0) / 2);
            var aZ = (float)Math.Atan2(m - y0, bounds.X1 - bounds.X0);
            aZ += ((float)_Random.NextDouble() * ARANGE) - (ARANGE / 2);
            //  rotate the kick into the direction of the kick / kick space
            Debug.WriteLine($"Rotate to kick space: {aZ}");
            foreach (var p in data)
            {
                var n = new Vector(p.X, -aZ);
                p.X = n.X;
                p.Y = n.Y;
                p.X += x0;
                p.Y += y0;
                Debug.WriteLine($"Rotating point: {p}");
            }
            return data;
        }

        public override EventData CreateSample(TrackingTag tag)
        {
            throw new NotImplementedException();
        }

        public float CalculateDistance(List<EventData> data)
        {
            //  find max velocity
            float v = data.Max(d => d.V);
            //  calculate launch angles
            var aa = ProjectileMotion.GetLaunchAngle(POST_X, POST_Y, v);
            //  if both angles are nan - the kick failed
            if (aa.Length < 2) return 0;
            if ((aa[0] == float.NaN) && (aa[1] == float.NaN)) return 0;
            //  if either is nan then use the other
            if ((aa[0] == float.NaN) || (aa[1] == float.NaN))
            {
                var a = (aa[0] == float.NaN) ? aa[1] : aa[0];
                var d = ProjectileMotion.GetDistance(v, a);
                return (d == float.NaN) ? 0 : d;
            }
            else
            {
                float a = (float)Math.Round(Math.Min(aa[0], aa[1]), 1), aMax = (float)Math.Round(Math.Max(aa[0], aa[1]), 1);
                //  find max distance
                float dLast = 0;
                while (a < aMax)
                {
                    var d = ProjectileMotion.GetDistance(v, a);
                    if (d > dLast)
                    {
                        dLast = d;
                    }
                    if (d < dLast) return d;
                    a += TENTH_DEGREE;
                }
                return dLast;
            }
        }

        #endregion Operations
    }
}
