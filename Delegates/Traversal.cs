using System;
using System.Collections.Generic;
using System.Linq;

namespace Delegates.TreeTraversal
{
    public static class Traversal
    {
        private static IEnumerable<TOut> Do<TIn, TOut>(
            TIn root,
            Func<TIn, bool> condition,
            Func<TIn, IEnumerable<TIn>> childrenList,
            Func<TIn, IEnumerable<TOut>> resultsList
            )
        {
            if (condition(root))
            {
                foreach (var value in resultsList(root))
                {
                    yield return value;
                }
            }

            foreach (var child in childrenList(root))
            {
                var values = Do(child, condition, childrenList, resultsList);
                foreach (var value in values)
                {
                    yield return value;
                }
            }
        }

        public static IEnumerable<Product> GetProducts(ProductCategory root)
        {
            return Do(
                root,
                x => x.Products.Count > 0,
                y => y.Categories,
                z => z.Products
                );
        }

        public static IEnumerable<Job> GetEndJobs(Job root)
        {
            return Do(
                root,
                x => x.Subjobs.Count == 0,
                y => y.Subjobs,
                z => new List<Job> { z }
                );
        }

        public static IEnumerable<T> GetBinaryTreeValues<T>(BinaryTree<T> root)
        {
            return Do(
                root,
                x => x.Left == null && x.Right == null,
                y => new List<BinaryTree<T>> { y.Left, y.Right }.Where(x => x != null),
                z => new List<T> { z.Value }
                );
        }
    }
}
