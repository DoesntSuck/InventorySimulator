using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityExtension
{
    public static class ListExtension
    {
        public static T Pop<T>(this List<T> list)
        {
            T element = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);

            return element;
        }

        public static T PopAt<T>(this List<T> list, int index)
        {
            T element = list[index];
            list.RemoveAt(index);

            return element;
        }
    }
}
