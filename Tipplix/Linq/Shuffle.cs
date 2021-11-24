using System;
using System.Collections.Generic;
using Enumerable = System.Linq.Enumerable;
using Random = UnityEngine.Random;

namespace Tipplix.Linq;

public static partial class LinqExtensions
{
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));
        
        var enumerable = list as T[] ?? Enumerable.ToArray(list);
        var count = enumerable.Length;
        var last = count - 1;
        
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count);
            (enumerable[i], enumerable[r]) = (enumerable[r], enumerable[i]);
        }

        return enumerable;
    }
    
}