using System;

namespace SharpGrad
{
    public class Dimension
    {
        public static readonly Dimension Scalar = new();
        private Dimension()
        {
            Name = "Scalar";
            Size = 1;
        }

        public string Name { get; set; }
        public int Size { get; }

        public Dimension(string name, int size)
        {
            if (size < 2)
            {
                throw new ArgumentException($"Size must be greater than 1. Got {size}.");
            }
            Name = name;
            Size = size;
        }
    }
}