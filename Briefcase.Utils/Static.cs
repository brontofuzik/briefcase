using System;
using System.Collections.Generic;
using System.Linq;

namespace Briefcase.Utils
{
    public static class Static
    {
        public static int? IndexOf<T>(this IEnumerable<T> @this, Predicate<T> predicate)
            => @this.Select((item, index) => new { item, index }).FirstOrDefault(p => predicate(p.item))?.index;

        // Eager
        public static TValue Switch<TControl, TValue>(this TControl @this, TValue @default, params (TControl label, TValue value)[] branches)
            => branches.FirstOrDefault<(TControl label, TValue value)>(b => @this.Equals(b.label), (@this, @default)).value;

        // Lazy
        public static TValue Switch<TControl, TValue>(this TControl @this, Func<TValue> @default, params (TControl label, Func<TValue> value)[] branches)
            => branches.FirstOrDefault<(TControl label, Func<TValue> value)>(b => @this.Equals(b.label), (@this, @default)).value();

        public static T FirstOrDefault<T>(this IEnumerable<T> source, Predicate<T> predicate, T @default)
        {
            foreach (T item in source)
                if (predicate(item)) return item;
            return @default;
        }
    }
}
