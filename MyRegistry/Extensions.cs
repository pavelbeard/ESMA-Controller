using System;

namespace MyLibrary
{
    public static class Extensions
    {
        public static double[] Copy(this Array a, double[] src, int start, int end)
        {
            int len = end - start;
            double[] dest = new double[len];

            for (int i = 0; i < len; i++)
            {
                dest[i] = src[start + i];
            }

            return dest;
        }
    }
}
