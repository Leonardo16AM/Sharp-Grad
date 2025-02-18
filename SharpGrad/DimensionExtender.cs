using SharpGrad.DifEngine;
using System;
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

        public static long GetLinearIndex(this Dimension[] shape, int[] indices)
        {
            if (shape.Length == 0 && indices.Length == 1)
                return 0;
            if(shape.Length != indices.Length)
            {
                throw new ArgumentException($"The shape size {shape.Size()} is not equal to the indices length {indices.Length}");
            }
            long index = 0;
            long stride = 1;
            for (int i = shape.Length - 1; i >= 0; i--)
            {
                index += indices[i] * stride;
                stride *= shape[i].Size;
            }
            return index;
        }
    }
}
