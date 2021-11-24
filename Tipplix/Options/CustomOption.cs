using System;
using Il2CppSystem.IO;
using Reactor;

namespace Tipplix.Options;

public abstract class CustomOption
{
    public StringNames Title { get; }

    protected CustomOption(string title)
    {
        Title = CustomStringName.Register(title);
        OptionsManager.TitleToOption.Add(Title, this);
    }

    internal abstract Action<OptionBehaviour> ValueChanged { get; }
    
    public abstract OptionBehaviour Initialize();
    public abstract void Serialize(BinaryWriter writer);
    public abstract void Deserialize(BinaryReader reader);
}