using SharpGrad.DifEngine;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace SharpGrad.DifEngine
{
    public static class DimensionExtender
    {
        public static string GetString<T>(this IEnumerable<T> arr) => '[' + string.Join(", ", arr) + ']';
        public static string GetString(this Dimension[] dim) => GetString(dim.Select(d => d.Size));

        public static int Size(this IEnumerable<Dimension> @this)
            => @this.Aggregate(1, (acc, d) => acc * d.Size);

        public static bool IsScalar(this IEnumerable<Dimension> @this)
            => !@this.Any();

        public static bool IsVector(this IEnumerable<Dimension> @this)
            => @this.Count() == 1;
    }
}
