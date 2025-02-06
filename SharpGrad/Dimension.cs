using System;

namespace SharpGrad.DifEngine
{
    public class Dimension
    {
        public int Size { get; }

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