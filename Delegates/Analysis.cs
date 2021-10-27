using System;
using System.Collections.Generic;
using System.Linq;

namespace Delegates.PairsAnalysis
{
    public static class Analysis
    {
        public static int FindMaxPeriodIndex(params DateTime[] data)
        {
            return data
                .Pairs()
                .Select(x => (x.Item2 - x.Item1).TotalSeconds)
                .MaxIndex();
        }

        public static double FindAverageRelativeDifference(params double[] data)
        {
            return data
                .Pairs()
                .Select(x => (x.Item2 - x.Item1) / x.Item1)
                .Max();
        }
    }

    public static class IEnumerableExtension
    {
        public static IEnumerable<Tuple<T, T>> Pairs<T>(this IEnumerable<T> collection)
        {
            var enumerator = collection.GetEnumerator();
            if (!enumerator.MoveNext()) throw new InvalidOperationException();

            var queue = new Queue<T>();
            queue.Enqueue(enumerator.Current);

            while (enumerator.MoveNext())
            {
                queue.Enqueue(enumerator.Current);
                if (queue.Count == 2)
                {
                    yield return Tuple.Create(queue.Dequeue(), queue.Peek());
                }
            }
        }

        public static int MaxIndex<T>(this IEnumerable<T> collection)
            where T : IComparable<T>
        {
            var enumerator = collection.GetEnumerator();
            if (!enumerator.MoveNext()) throw new InvalidOperationException();

            var maxElement = (Index: 0, Value: enumerator.Current);
            var currentIndex = 0;

            while (enumerator.MoveNext())
            {
                currentIndex++;
                if (maxElement.Value.CompareTo(enumerator.Current) < 0)
                {
                    maxElement = (currentIndex, enumerator.Current);
                }
            }

            return maxElement.Index;
        }
    }
}
