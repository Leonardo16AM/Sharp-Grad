using System;
using System.Collections.Generic;

namespace SharpGrad.DifEngine
{
    /// <summary>
    /// Represents a collection of <see cref="Dimension"/> and <see cref="Index"/> pairs.
    /// </summary>
    public class Dimdices
    {
        /// <summary>
        /// The shape of the <see cref="Dimdices"/>.
        /// </summary>
        public readonly Dimension[] Shape;
        /// <summary>
        /// The indices of the <see cref="Dimdices"/>.
        /// </summary>
        public IReadOnlyList<Index> Indices;

        /// <summary>
        /// Get the <see cref="Index"/> of the specified <see cref="Dimension"/>.
        /// </summary>
        /// <param name="dim">The <see cref="Dimension"/> to get the <see cref="Index"/> of.</param>
        /// <returns>The <see cref="Index"/> of the specified <see cref="Dimension"/>.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when the specified <see cref="Dimension"/> is not found in the shape.
        /// </exception>
        public Index this[Dimension dim]
        {
            get
            {
                for (int i = 0; i < Shape.Length; i++)
                {
                    if (Shape[i] == dim)
                    {
                        return Indices[i];
                    }
                }
                throw new KeyNotFoundException($"Dimension {dim} not found in the shape.");
            }
        }

        /// <summary>
        /// Create a new instance of <see cref="Dimdices"/>.
        /// </summary>
        /// <param name="shape">The shape of the <see cref="Dimdices"/>.</param>
        /// <param name="indices">The indices of the <see cref="Dimdices"/>.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the shape size is not equal to the indices length.
        /// </exception>
        public Dimdices(Dimension[] shape, Index[] indices)
        {
            if(shape.Size() != indices.Length)
            {
                throw new ArgumentException($"The shape size {shape.Size()} is not equal to the indices length {indices.Length}");
            }
            Shape = shape;
            Indices = indices;
        }

        /// <summary>
        /// Get the linear index of the <see cref="Dimdices"/>.
        /// </summary>
        /// <returns>The linear index of the <see cref="Dimdices"/>.</returns>
        public long GetLinearIndex()
        {
            long index = 0;
            long stride = 1;
            for (int i = 0; i < Shape.Length; i++)
            {
                index += Indices[i].Value * stride;
                stride *= Shape[i].Size;
            }
            return index;
        }
    }
}