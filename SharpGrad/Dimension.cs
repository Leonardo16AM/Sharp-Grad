using System;

namespace SharpGrad.DifEngine
{
    public class Dimension
    {
        public static readonly Dimension Scalar = new();

        public int Size { get; }

        private Dimension() { Size = 1; }
        public Dimension(int size)
        {
            if (size < 2)
            {
                throw new ArgumentException($"Size must be greater than 1. Got {size}.");
            }
            Size = size;
        }
    }
}