using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpGrad.DifEngine
{
    /// <summary>
    /// Represents a collection of <see cref="Dimension"/> and index pairs.
    /// </summary>
    public class Dimdices
    {
        public static readonly Dimdices Scalar = new();
        public static readonly int[] ScalarIntIndices = [0];

        /// <summary>
        /// The shape of the <see cref="Dimdices"/>.
        /// </summary>
        public readonly Dimension[] Shape;
        /// <summary>
        /// The indices of the <see cref="Dimdices"/>.
        /// </summary>
        public IReadOnlyList<int> Indices;

        /// <summary>
        /// Get if the <see cref="Dimdices"/> is a scalar.
        /// </summary>
        public bool IsScalar => Shape.Length == 0;

        /// <summary>
        /// Get the index of the specified <see cref="Dimension"/>.
        /// </summary>
        /// <param name="dim">The <see cref="Dimension"/> to get the index of.</param>
        /// <returns>The index of the specified <see cref="Dimension"/>.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when the specified <see cref="Dimension"/> is not found in the shape.
        /// </exception>
        public int this[Dimension dim]
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

        private Dimdices()
        {
            Shape = [];
            Indices = [];
        }

        // Convert Index[] to int[]
        private static int[] ToInts(Dimension[] shape, Index[] indices)
        {
            int[] idxs = new int[indices.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                if (indices[i].IsFromEnd)
                {
                    idxs[i] = shape[i].Size - indices[i].Value;
                }
                else
                {
                    idxs[i] = indices[i].Value;
                }
            }
            return idxs;
        }

        /// <summary>
        /// Create a new instance of <see cref="Dimdices"/>.
        /// </summary>
        /// <param name="shape">The shape of the <see cref="Dimdices"/>.</param>
        /// <param name="indices">The indices of the <see cref="Dimdices"/>.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the shape have no dimensions or its dimensions count is not equal to the indices length.
        /// </exception>
        public Dimdices(Dimension[] shape, int[] indices)
        {
            if (shape.Length == 0)
            {
                throw new ArgumentException("Shape must have at least one dimension.");
            }
            if (shape.Length != indices.Length)
            {
                throw new ArgumentException($"The shape size {shape.Size()} is not equal to the indices length {indices.Length}");
            }
            Shape = shape;
            Indices = indices;
        }

        /// <summary>
        /// Create a new instance of <see cref="Dimdices"/>.
        /// </summary>
        /// <param name="shape">The shape of the <see cref="Dimdices"/>.</param>
        /// <param name="indices">The indices of the <see cref="Dimdices"/>.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the shape have no dimensions or its dimensions count is not equal to the indices length.
        /// </exception>
        public Dimdices(Dimension[] shape, Index[] indices)
            : this(shape, ToInts(shape, indices))
        { }


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
                index += Indices[i] * stride;
                stride *= Shape[i].Size;
            }
            return index;
        }
    }

    /// <summary>
    /// Represents an indexer for <see cref="Dimdices"/>.
    /// Starts from the first index in each dimension and ends at the last index in each dimension.
    /// </summary>
    public class Dimdexer(Dimension[] shape) : IEnumerable<Dimdices>
    {
        public readonly Dimension[] Shape = shape;
        public int[] Indices = new int[shape.Length];

        public IEnumerator<Dimdices> GetEnumerator()
        {
            if (Shape.IsScalar())
            {
                yield return Dimdices.Scalar;
                yield break;
            }
            Array.Fill(Indices, 0);
            yield return new Dimdices(Shape, Indices);

            while (true)
            {
                int i = 0;
                while (i < Shape.Length)
                {
                    Indices[i]++;
                    if (Indices[i] == Shape[i].Size)
                    {
                        Indices[i] = 0;
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (i == Shape.Length)
                {
                    break;
                }
                yield return new Dimdices(Shape, Indices);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}