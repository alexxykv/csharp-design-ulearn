using System;

namespace Incapsulation.Weights
{
	public class Indexer
    {
        private readonly double[] array;
        private readonly int start;
        public readonly int Length;

        public Indexer(double[] array, int start, int length)
        {
            CheckRangeValid(array, start, length);
            this.array = array;
            this.start = start;
            Length = length;
        }

        public double this[int index]
        {
            get { return CheckIndexValid(index); }
            set
            {
                CheckIndexValid(index);
                array[start + index] = value;
            }
        }

        private void CheckRangeValid(double[] array, int start, int length)
        {
            if (start < 0 || length < 0 || start + length > array.Length)
                throw new ArgumentException();
        }

        private double CheckIndexValid(int index)
        {
            if (index < 0 || index > Length - 1)
                throw new IndexOutOfRangeException();
            return array[start + index];
        }
    }
}
