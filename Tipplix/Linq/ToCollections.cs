using System;
using I = Il2CppSystem.Collections.Generic;
using S = System.Collections.Generic;

namespace Tipplix.Linq;

public static partial class LinqExtensions
{
    /// <summary>
    ///     Creates a new <see cref="I.List&lt;T&gt;"/> from <see cref="S.IEnumerable&lt;T&gt;"/> 
    /// </summary>
    public static I.List<T> ToIl2CppList<T>(this S.IEnumerable<T> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source) + " cannot be null");
        
        var newList = new I.List<T>();
        
        foreach (var item in source)
        {
            newList.Add(item);
        }

        return newList;
    }
}