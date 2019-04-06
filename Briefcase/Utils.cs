using System;
using System.Collections.Generic;
using System.Linq;

namespace Briefcase
{
    public static class Utils
    {
        public static int? IndexOf<T>(this IEnumerable<T> @this, Predicate<T> predicate)
            => @this.Select((item, index) => new { item, index }).FirstOrDefault(p => predicate(p.item))?.index;
    }
}
