using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Briefcase.Utils
{
    public static class CollectionExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            foreach (var item in self)
                action(item);
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, TValue @default = default)
            => self.ContainsKey(key) ? self[key] : @default;

        public static bool AddIfNotContains<T>(this Collection<T> self, T item)
        {
            if (!self.Contains(item))
            {
                self.Add(item);
                return true;
            }

            return false;
        }
    }
}
