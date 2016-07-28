using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityExtension
{
    public static class HashSetExtension
    {
        public static bool ContainsAll<T>(this HashSet<T> set, ICollection<T> subset)
        {
            foreach (T item in subset)
            {
                if (!set.Contains(item))
                    return false;
            }

            return true;
        }
    }
}
