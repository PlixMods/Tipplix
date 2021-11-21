using System;
using System.IO;
using System.Reflection;
using Reactor;
using Reactor.Extensions;
using UnityEngine;

namespace Tipplix.Utility;
public static class SpriteHelper
{
    public static Sprite? CreateSprite(string image, float pixelsPerUnit = 128f)
    {
        try
        {
            var tex = GUIExtensions.CreateEmptyTexture();
            var myStream = Assembly.GetCallingAssembly().GetManifestResourceStream(image);
            ImageConversion.LoadImage(tex, myStream?.ReadFully(), false);
            
            Logger<TipplixPlugin>.Debug("Created a sprite");

            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f),
                pixelsPerUnit);
        }
        catch (Exception ex)
        {
            Logger<TipplixPlugin>.Error("Unable to create sprite: \n" + ex);
            return null;
        }
    }
}