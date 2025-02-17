namespace GamaEdtech.Backend.Common.Core.Extensions.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using GamaEdtech.Backend.Common.Core.Extensions;

    public static class ListExtensions
    {
        public static int FindIndex<T>([NotNull] this IList<T> source, [NotNull] Predicate<T> selector)
        {
            for (var i = 0; i < source.Count; ++i)
            {
                if (selector(source[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public static void AddFirst<T>([NotNull] this IList<T> source, T item) => source.Insert(0, item);

        public static void AddLast<T>([NotNull] this IList<T> source, T item) => source.Insert(source.Count, item);

        public static void InsertAfter<T>([NotNull] this IList<T> source, T existingItem, T item)
        {
            var index = source.IndexOf(existingItem);
            if (index < 0)
            {
                source.AddFirst(item);
                return;
            }

            source.Insert(index + 1, item);
        }

        public static void InsertAfter<T>(this IList<T> source, Predicate<T> selector, T item)
        {
            var index = source.FindIndex(selector);
            if (index < 0)
            {
                source.AddFirst(item);
                return;
            }

            source.Insert(index + 1, item);
        }

        public static void InsertBefore<T>([NotNull] this IList<T> source, T existingItem, T item)
        {
            var index = source.IndexOf(existingItem);
            if (index < 0)
            {
                source.AddLast(item);
                return;
            }

            source.Insert(index, item);
        }

        public static void InsertBefore<T>(this IList<T> source, Predicate<T> selector, T item)
        {
            var index = source.FindIndex(selector);
            if (index < 0)
            {
                source.AddLast(item);
                return;
            }

            source.Insert(index, item);
        }

        public static void ReplaceWhile<T>([NotNull] this IList<T> source, [NotNull] Predicate<T> selector, T item)
        {
            for (var i = 0; i < source.Count; i++)
            {
                if (selector(source[i]))
                {
                    source[i] = item;
                }
            }
        }

        public static void ReplaceWhile<T>([NotNull] this IList<T> source, [NotNull] Predicate<T> selector, [NotNull] Func<T, T> itemFactory)
        {
            for (var i = 0; i < source.Count; i++)
            {
                var item = source[i];
                if (selector(item))
                {
                    source[i] = itemFactory(item);
                }
            }
        }

        public static void ReplaceOne<T>([NotNull] this IList<T> source, [NotNull] Predicate<T> selector, T item)
        {
            for (var i = 0; i < source.Count; i++)
            {
                if (selector(source[i]))
                {
                    source[i] = item;
                    return;
                }
            }
        }

        public static void ReplaceOne<T>([NotNull] this IList<T> source, [NotNull] Predicate<T> selector, [NotNull] Func<T, T> itemFactory)
        {
            for (var i = 0; i < source.Count; i++)
            {
                var item = source[i];
                if (selector(item))
                {
                    source[i] = itemFactory(item);
                    return;
                }
            }
        }

        public static void ReplaceOne<T>([NotNull] this IList<T> source, T item, T replaceWith)
        {
            for (var i = 0; i < source.Count; i++)
            {
                if (Comparer<T>.Default.Compare(source[i], item) == 0)
                {
                    source[i] = replaceWith;
                    return;
                }
            }
        }

        public static T GetOrAdd<T>([NotNull] this IList<T> source, Func<T, bool> selector, [NotNull] Func<T> factory)
        {
            var item = source.FirstOrDefault(selector);

            if (item is null)
            {
                item = factory();
                source.Add(item);
            }

            return item;
        }

        public static IReadOnlyList<T> SortByDependencies<T>([NotNull] this IEnumerable<T> source, [NotNull] Func<T, IEnumerable<T>> getDependencies)
        {
            var sorted = new List<T>();
            var visited = new Dictionary<T, bool>();

            foreach (var item in source)
            {
                SortByDependenciesVisit(item, getDependencies, sorted, visited);
            }

            return sorted;
        }

        public static T? Find<T>([NotNull] this IReadOnlyList<T> list, Predicate<T> match)
        {
            ArgumentNullException.ThrowIfNull(match, nameof(match));

            for (var i = 0; i < list.Count; i++)
            {
                if (match(list[i]))
                {
                    return list[i];
                }
            }

            return default;
        }

        public static bool Exists<T>([NotNull] this IReadOnlyList<T> list, Predicate<T> match)
        {
            ArgumentNullException.ThrowIfNull(match, nameof(match));

            for (var i = 0; i < list.Count; i++)
            {
                if (match(list[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private static void SortByDependenciesVisit<T>(T item, Func<T, IEnumerable<T>> getDependencies, List<T> sorted, Dictionary<T, bool> visited)
        {
            var alreadyVisited = visited.TryGetValue(item, out var inProcess);

            if (alreadyVisited)
            {
                if (inProcess)
                {
                    throw new ArgumentException("Cyclic dependency found! Item: " + item);
                }
            }
            else
            {
                visited[item] = true;

                var dependencies = getDependencies(item);
                if (dependencies is not null)
                {
                    foreach (var dependency in dependencies)
                    {
                        SortByDependenciesVisit(dependency, getDependencies, sorted, visited);
                    }
                }

                visited[item] = false;
                sorted.Add(item);
            }
        }
    }
}
