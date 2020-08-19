using System;

namespace DataFactory
{
    public class Line2
    {
        #region Constructors

        public Line2() { }

        public Line2(float x, float y)
        {
            M = ((x == 0) || (y == 0)) ? float.NaN : y / x;
        }

        #endregion Constructors

        #region Properties

        public float M { get; set; }

        #endregion Properties

        #region Operations

        public float GetY(float x)
        {
            try
            {
                return M * x;
            }
            catch
            {
                return float.NaN;
            }
        }

        public float GetX(float y)
        {
            try
            {
                return y / M;
            }
            catch
            {
                return float.NaN;
            }
        }

        #endregion Operations
    }
}
