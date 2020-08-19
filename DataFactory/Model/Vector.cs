using System;

namespace DataFactory.Model
{
    public class Vector
    {
        #region Constructors

        public Vector()
        {
        }

        public Vector(float length, float angle)
        {
            _Length = length;
            X = length * (float)Math.Cos(Math.Abs(angle));
            Y = length * (float)Math.Sin(angle);
            Angle = angle;
        }

        #endregion Constructors

        #region Properties

        public float X { get; set; }

        public float Y { get; set; }

        private float _Length = 0;
        public float Length
        {
            get
            {
                if (_Length > 0) return _Length;
                _Length = (float)Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
                return _Length;
            }
        }

        public float Angle { get; set; }

        #endregion Properties

        #region Operations

        public void Normalize()
        {
            //  already normalized
            if (Length == 1) return;
            X /= Length;
            Y /= Length;
        }

        #endregion Operations
    }
}
