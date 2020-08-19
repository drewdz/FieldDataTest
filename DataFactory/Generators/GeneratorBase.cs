/*
 *      Humans walk at an average 0.833333-1.111111 m/s or 2.734033-3.645378 ft/s
 */

using DataFactory.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Xml;

namespace DataFactory.Generators
{
    public abstract class GeneratorBase
    {
        #region Constants

        protected const float WALK_MIN = 0.833333f;
        protected const float WALK_RANGE = 0.277778f;

        #endregion Constants

        #region Fields

        protected Random _Random = new Random();

        protected FieldActivity _Activity;

        #endregion Fields

        #region Operations

        public virtual void Init(FieldActivity activity)
        {
            if (activity == null) return;
            _Activity = activity;
        }

        public virtual List<EventData> Walk(long millis, string tag, List<PointF> points)
        {
            if ((points == null) || (points.Count < 2)) throw new Exception("Need at least 2 points to generate a walk");
            //  determine a speed for this walker
            var data = new List<EventData>();
            for (int i = 1; i < points.Count; i++)
            {
                var from = points[i - 1];
                data.AddRange(GenerateWalk(millis, tag, from, points[i]));
                millis = (data.Count == 0) ? millis : data.Max(d => d.Timestamp) + 100;
            }
            return data;
        }

        private List<EventData> GenerateWalk(long millis, string tag, PointF from, PointF to)
        {
            //  get initial velocity
            var s = (float)(WALK_MIN + (_Random.NextDouble() * WALK_RANGE));
            var variance = 0.0555556f;
            var direction = new Vector { X = to.X - from.X, Y = to.Y - from.Y };
            direction.Normalize();
            var dirX = (to.X >= from.X) ? 1 : -1;
            var dirY = (to.Y >= from.Y) ? 1 : -1;
            //  walk
            float x = 0, y = 0, t = 0;
            float x1 = to.X - from.X, y1 = to.Y - from.Y;
            var data = new List<EventData>();
            while (true)
            {
                //  check if we're there yet
                if ((((x1 - x) * dirX) < Constants.EPSILON) && (((y1 - y) * dirY) < Constants.EPSILON)) break;
                //  get location
                var v = s + ((float)_Random.NextDouble() * variance) - (variance / 2);
                x += direction.X * v * 0.1f;
                y += direction.Y * v * 0.1f;
                data.Add(new EventData
                {
                    TagId = tag,
                    Timestamp = millis,
                    V = s,
                    X = x + from.X,
                    Y = y + from.Y
                });
                t += 0.1f;
                millis += 100;
            }
            return data;
        }

        #endregion Operations

        #region Incremental

        public abstract EventData CreateSample(TrackingTag tag);

        #endregion Incremental
    }
}
