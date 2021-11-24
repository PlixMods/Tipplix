using System;
using Il2CppSystem.IO;
using static UnityEngine.Object;
using Prefabs = Tipplix.Options.OptionsManager.Prefabs;

namespace Tipplix.Options;

public class CustomNumberOption : CustomOption
{
    public FloatRange Range { get; set; }
    public float Increment { get; set; }
    public event Action<float>? OnValueChanged;
    
    internal override Action<OptionBehaviour> ValueChanged => behaviour => 
    {
        var value = Range.Clamp(behaviour.GetFloat());
            
        Value = value;
        OnValueChanged?.Invoke(value);
        OptionsManager.SyncSettings();
    };

    public float Value { get; set; }
    
    public CustomNumberOption(string title, FloatRange range, float increment, float defaultValue) : base(title)
    {
        Range = range;
        Value = defaultValue;
        Increment = increment;
    }
    
    public CustomNumberOption(string title, FloatRange range, float increment, float defaultValue, Action<float> onValueChanged) 
        : this(title, range, increment, defaultValue)
    {
        OnValueChanged += onValueChanged;
    }

    public override OptionBehaviour Initialize()
    {
        Prefabs.FindAndSet();
        var numberOption = Instantiate(Prefabs.NumberOption, Prefabs.NumberOption!.transform.parent)!;
        
        numberOption.Title = Title;
        numberOption.Value = Value;
        numberOption.ValidRange = Range;
        numberOption.Increment = Increment;
        numberOption.SuffixType = NumberSuffixes.None;

        return numberOption;
    }

    public static implicit operator float (CustomNumberOption _) => _.Value;

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(Value);
    }

    public override void Deserialize(BinaryReader reader)
    {
        Value = reader.ReadSingle();
    }
}