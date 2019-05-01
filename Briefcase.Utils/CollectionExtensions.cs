using System.Collections.ObjectModel;

namespace Briefcase.Utils
{
    public static class CollectionExtensions
    {
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
