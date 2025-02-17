namespace GamaEdtech.Backend.Common.Core.Extensions.Collections.Generic
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public static class DictionaryExtensions
    {
        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="dictionary">Dictionary to check and get.</param>
        /// <param name="key">Key to find the value.</param>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
        public static TValue? GetOrDefault<TKey, TValue>([NotNull] this Dictionary<TKey, TValue?> dictionary, TKey key)
            where TKey : notnull => dictionary.TryGetValue(key, out var obj) ? obj : default;

        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="dictionary">Dictionary to check and get.</param>
        /// <param name="key">Key to find the value.</param>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
        public static TValue? GetOrDefault<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary, TKey key) => dictionary.TryGetValue(key, out var obj) ? obj : default;

        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="dictionary">Dictionary to check and get.</param>
        /// <param name="key">Key to find the value.</param>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
        public static TValue? GetOrDefault<TKey, TValue>([NotNull] this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key) => dictionary.TryGetValue(key, out var obj) ? obj : default;

        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="dictionary">Dictionary to check and get.</param>
        /// <param name="key">Key to find the value.</param>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
        public static TValue? GetOrDefault<TKey, TValue>([NotNull] this ConcurrentDictionary<TKey, TValue?> dictionary, TKey key)
            where TKey : notnull => dictionary.TryGetValue(key, out var obj) ? obj : default;

        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="dictionary">Dictionary to check and get.</param>
        /// <param name="key">Key to find the value.</param>
        /// <param name="factory">A factory method used to create the value if not found in the dictionary.</param>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
#pragma warning disable CS8777 // Parameter must have a non-null value when exiting.
        public static TValue GetOrAdd<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary, [NotNull] TKey key, [NotNull] Func<TKey, TValue> factory) => dictionary.TryGetValue(key, out var obj) ? obj : (dictionary[key] = factory(key));
#pragma warning restore CS8777 // Parameter must have a non-null value when exiting.

        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="dictionary">Dictionary to check and get.</param>
        /// <param name="key">Key to find the value.</param>
        /// <param name="factory">A factory method used to create the value if not found in the dictionary.</param>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
        public static TValue GetOrAdd<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> factory) => dictionary.GetOrAdd(key, k => factory());

        /// <summary>
        /// This method is used to try to get a value in a dictionary if it does exists.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="dictionary">The collection object.</param>
        /// <param name="key">Key.</param>
        /// <param name="value">Value of the key (or default value if key not exists).</param>
        /// <returns>True if key does exists in the dictionary.</returns>
        internal static bool TryGetValue<T>([NotNull] this IDictionary<string, object?> dictionary, string key, out T? value)
        {
            if (dictionary.TryGetValue(key, out var valueObj) && valueObj is T t)
            {
                value = t;
                return true;
            }

            value = default;
            return false;
        }
    }
}
