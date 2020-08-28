using System;

namespace DataFactory
{
    public static class Constants
    {
        #region Static Members

        public static DateTime UnixEpoch { get; } = new DateTime(1970, 1, 1);

        #endregion Static Members

        #region Constants

        public const float EPSILON = 0.002f;
        public const float FEET_TO_METERS = 0.3048f;
        public const float G = 32.17405f;

        #endregion Constants
    }
}
