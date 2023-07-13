using System;
using System.Collections.Generic;

namespace TWJ.TWJApp.TWJService.Common.Extensions
{
    public static class EnumerableExtension
    {
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }
    }
}
