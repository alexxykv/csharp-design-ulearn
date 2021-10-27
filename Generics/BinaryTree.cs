using System;
using System.Collections;
using System.Collections.Generic;

namespace Generics.BinaryTrees
{
    public class BinaryTree<T> : IEnumerable<T>
        where T : IComparable
    {
        public T Value { get; private set; }
        public BinaryTree<T> Left { get; private set; }
        public BinaryTree<T> Right { get; private set; }
        public bool IsEmpty { get; private set; } = true;

        public void Add(T value)
        {
            if (IsEmpty)
            {
                Value = value;
                IsEmpty = false;
                return;
            }

            if (value.CompareTo(Value) > 0)
            {
                Right = Right ?? new BinaryTree<T>();
                Right.Add(value);
            }
            else
            {
                Left = Left ?? new BinaryTree<T>();
                Left.Add(value);
            }
        }

        private IEnumerable<T> InorderTraversal()
        {
            if (Left != null)
            {
                var leftSubtree = Left.InorderTraversal();
                foreach (var value in leftSubtree)
                {
                    yield return value;
                }
            }

            yield return Value;

            if (Right != null)
            {
                var rightSubtree = Right.InorderTraversal();
                foreach (var value in rightSubtree)
                {
                    yield return value;
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (IsEmpty) yield break;

            foreach (var value in InorderTraversal())
            {
                yield return value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static class BinaryTree
    {
        public static BinaryTree<T> Create<T>(params T[] values)
            where T : IComparable
        {
            var tree = new BinaryTree<T>();
            foreach (var value in values)
            {
                tree.Add(value);
            }
            return tree;
        }
    }
}
