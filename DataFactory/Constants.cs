using System;
using System.ComponentModel;

namespace DataFactory
{
    public static class Constants
    {
        #region Static Members

        //public static DateTimeOffset UnixEpoch { get; private set; }

        #endregion Static Members

        #region Constructors

        static Constants()
        {
            //  = new DateTime(1970, 1, 1);
            //UnixEpoch = DateTimeOffset.Parse("1970-01-01T00:00:00+00:00");
        }

        #endregion Constructors

        #region Constants

        public const float EPSILON = 0.002f;
        public const float FEET_TO_METERS = 0.3048f;
        public const float G = 32.17405f;

        #endregion Constants
    }
}
