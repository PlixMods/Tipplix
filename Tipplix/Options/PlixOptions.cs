using Il2CppSystem.IO;
using Tipplix.Enums;

namespace Tipplix.Options;

public static class PlixOptions
{
    public static RevealTypes ExileRevealType { get; internal set; }

    internal static void Serialize(BinaryWriter writer)
    {
        writer.Write((byte) ExileRevealType);
    }
        
    internal static void Deserialize(BinaryReader reader)
    {
        ExileRevealType = (RevealTypes) reader.ReadByte();
    }
}