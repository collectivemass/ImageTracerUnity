using System;
using System.Collections.Generic;

namespace CollectiveMass.ImageTracerUnity.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ForAsRange<T>(T initializer, Func<T, bool> condition, Func<T, T> iterator)
        {
            for (var i = initializer; condition(i); i = iterator(i))
            {
                yield return i;
            }
        }
    }
}
