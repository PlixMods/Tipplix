using System;
using System.Linq;
using Il2CppSystem.IO;
using Reactor;
using Prefabs = Tipplix.Options.OptionsManager.Prefabs;
using static UnityEngine.Object;

namespace Tipplix.Options;

public class CustomStringOption : CustomOption
{
    public int Value { get; set; }
    public StringNames[] Values { get; set; }
    
    internal override Action<OptionBehaviour> ValueChanged => behaviour => 
    {
        var value = behaviour.GetInt();

        Value = value;
        OnValueChanged?.Invoke(value);
            
        OptionsManager.SyncSettings();
    };
    
    public event Action<int>? OnValueChanged;
    
    public CustomStringOption(string title, Action<int> onValueChanged, params string[] values) : this(title, values)
    {
        OnValueChanged += onValueChanged;
    }
    
    public CustomStringOption(string title, params string[] values) : base(title)
    {
        Values = values.Select(x => (StringNames) CustomStringName.Register(x)).ToArray();
    }

    public override OptionBehaviour Initialize()
    {
        Prefabs.FindAndSet();
        var stringOption = Instantiate(Prefabs.StringOption!, Prefabs.StringOption!.transform.parent);
                    
        stringOption.Title = Title;
        stringOption.Value = Value;
        stringOption.Values = Values;

        return stringOption;
    }
    
    public static implicit operator int (CustomStringOption _) => _.Value;

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(Value);
    }

    public override void Deserialize(BinaryReader reader)
    {
        Value = reader.ReadInt32();
    }
}