using UnityEngine;

namespace Tipplix.Extensions;

public static class StringExtensions
{
    public static string WithColor(this string text, Color32 color)
    {
        var hex = $"{color.r:X2}{color.g:X2}{color.b:X2}";
        return $"<color=#{hex}>{text}</color>";
    }
}