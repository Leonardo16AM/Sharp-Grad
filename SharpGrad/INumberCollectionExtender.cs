using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SharpGrad
{
    public static class INumberCollectionExtender
    {
        public static T Sum<T>(this IEnumerable<T> @this)
            where T : INumber<T>
        {
            T sum = T.AdditiveIdentity;
            foreach (var item in @this)
                sum += item;
            return sum;
        }
        public static T Sum<T>(this T[] @this)
            where T : INumber<T> => Sum(@this.AsEnumerable());

        public static T Mean<T>(this IEnumerable<T> @this)
            where T : INumber<T>
        {
            T sum = T.AdditiveIdentity;
            T count = T.Zero;
            foreach (var item in @this)
            {
                sum += item;
                count++;
            }
            return sum / count;
        }
        public static T Mean<T>(this T[] @this)
            where T : INumber<T> => Mean(@this.AsEnumerable());

        public static T Var<T>(this IEnumerable<T> @this)
            where T : INumber<T>, IRootFunctions<T>
        {
            // Variance = E[(X - E[X])^2]
            T mean = @this.Mean();
            T sum = T.AdditiveIdentity;
            T count = T.Zero;

            foreach (var item in @this)
            {
                T diff = item - mean;
                sum += diff * diff;
                count++;
            }
            return sum / count;
        }
        public static T Var<T>(this T[] @this)
            where T : INumber<T>, IRootFunctions<T> => Var(@this.AsEnumerable());
        public static T Std<T>(this IEnumerable<T> @this)
            where T : INumber<T>, IRootFunctions<T>
        {
            // Standard deviation = sqrt( E[(X - E[X])^2] )
            return T.Sqrt(@this.Var());
        }
        public static T Std<T>(this T[] @this)
            where T : INumber<T>, IRootFunctions<T> => Std(@this.AsEnumerable());

    }
}