using System;

namespace DataFactory
{
    public static class ProjectileMotion
    {
        public static float GetDistance(float v, float a)
        {
            try
            {
                return (float)((Math.Pow(v, 2) / Constants.G) * Math.Sin(2 * a));
            }
            catch
            {
                //  invalid
                return float.NaN;
            }
        }

        public static float[] GetLaunchAngle(float x, float y, float v)
        {
            //  angles
            var u = Math.Pow(v, 4) - (Constants.G * ((Constants.G * Math.Pow(x, 2)) + (2 * y * Math.Pow(v, 2))));
            //  angle 1
            var a1 = GetAngle(v, x, u);
            var a2 = GetAngle(v, x, -u);
            return new float[] { a1, a2 };
        }

        private static float GetAngle(float v, float x, double u)
        {
            try
            {
                return (float)Math.Atan((Math.Pow(v, 2) + u) / (Constants.G * x));
            }
            catch
            {
                //  invalid
                return float.NaN;
            }
        }
    }
}
