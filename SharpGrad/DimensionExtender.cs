using SharpGrad.DifEngine;
using System.Collections.Generic;
using System.Linq;

namespace SharpGrad
{
    public static class DimensionExtender
    {
        public static int Size(this IEnumerable<Dimension> @this)
        {
            return @this.Aggregate(1, (acc, d) => acc * d.Size);
        }
    }
}
