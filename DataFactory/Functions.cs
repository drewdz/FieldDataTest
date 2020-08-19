using System;

namespace DataFactory
{
    public static class Functions
    {
        /// <summary>
        /// Sigmoid function 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float Sigmoid(float x)
        {
            return (float)(1f / (1 + Math.Pow(Math.E, x * -1)));
        }

        public static float Sigmoid2(float x)
        {
            return (float)((1f / (1 + Math.Pow(Math.E, x * -1))) - 0.5f) * 2;
        }
    }
}
