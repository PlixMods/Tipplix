using System;
using System.Linq;
using System.Reflection;
using Reactor;
using Reactor.Extensions;
using UnityEngine;

namespace Tipplix.Utility;
public static class SpriteHelper
{
    public static Texture2D LoadTextureFromBytes(byte[] resource) 
    {
        var myTexture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
        ImageConversion.LoadImage(myTexture, resource, true);
        return myTexture;
    }

    public static Texture2D LoadTextureFromEmbeddedResources(string resource, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();
        var myTexture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
        var myStream = assembly.GetManifestResourceStream(resource);
        
        var image = new byte[myStream!.Length];
        myStream.Read(image, 0, (int) myStream.Length);
        ImageConversion.LoadImage(myTexture, image, true);
        return myTexture;
    }

    public static Sprite LoadSpriteFromTexture(Texture2D tex, float pixelsPerUnit)
    {
        return Sprite.Create(tex, 
            new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f),
            pixelsPerUnit);
    }

    public static Sprite LoadSpriteFromTexture(Texture2D tex)
    {
        return LoadSpriteFromTexture(tex, tex.width);
    }

    public static Sprite? FindOrNull(string fileName, float pixelsPerUnit, Assembly? assembly = null)
    {
        try
        {
            assembly ??= Assembly.GetCallingAssembly();
            var match = FindMatchingResource(fileName, assembly.GetManifestResourceNames()
                    .Where(x => x.EndsWith(".png")).ToArray(),
                assembly.GetName().Name);
            
            var texture = LoadTextureFromEmbeddedResources(match, assembly);
            var sprite = LoadSpriteFromTexture(texture, pixelsPerUnit);

            return sprite;
        }
        catch (Exception ex)
        { 
            Logger<TipplixPlugin>.Error($"Unable to find .png from \"{fileName}\": {ex}");
            return null;
        }
    }

    public static Sprite? FindOrNull(string fileName, Assembly? assembly = null)
    {
        try
        {
            assembly ??= Assembly.GetCallingAssembly();
            var match = FindMatchingResource(fileName, assembly.GetManifestResourceNames()
                .Where(x => x.EndsWith(".png")).ToArray(),
                assembly.GetName().Name);
            
            var texture = LoadTextureFromEmbeddedResources(match, assembly);
            var sprite = LoadSpriteFromTexture(texture);

            return sprite;
        }
        catch (Exception ex)
        { 
            Logger<TipplixPlugin>.Error($"Unable to find .png from \"{fileName}\": {ex}");
            return null;
        }
    }

    public static string FindMatchingResource(string fileName, string[] resources, string owner)
    {
        if (!resources.Any()) throw new Exception($"No any embedded .png found in {owner}");

        var possibleMatches = resources.Where(x => x.ToLower().EndsWith($"{fileName}")).ToArray();
        if (!possibleMatches.Any()) throw new Exception($"No matches found for \"{fileName}\"");
            
        var minLength = possibleMatches.Min(y=> y.Length);
        var shortest = possibleMatches.First(x=> x.Length == minLength);

        return shortest;
    }
    
    public static Sprite? CreateSprite(string image, float pixelsPerUnit = 128f)
    {
        try
        {
            var tex = GUIExtensions.CreateEmptyTexture();
            var myStream = Assembly.GetCallingAssembly().GetManifestResourceStream(image);
            ImageConversion.LoadImage(tex, myStream?.ReadFully(), false);
            
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