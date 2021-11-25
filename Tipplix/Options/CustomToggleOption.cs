using System;
using Prefabs = Tipplix.Options.OptionsManager.Prefabs;
using Il2CppSystem.IO;
using Object = UnityEngine.Object;

namespace Tipplix.Options;

public class CustomToggleOption : CustomOption
{
    public bool Value { get; set; }
    public event Action<bool>? OnValueChanged;
    
    internal override Action<OptionBehaviour> ValueChanged => behaviour =>
    {
        var value = behaviour.GetBool();

        Value = value;
        OnValueChanged?.Invoke(value);
        OptionsManager.SyncSettings();
    };

    public CustomToggleOption(string title, Action<bool> onValueChanged) : base(title)
    {
        OnValueChanged += onValueChanged;
    }
    
    public CustomToggleOption(string title) : base(title)
    { }

    public override OptionBehaviour Initialize()
    {
        var toggleOption = Object.Instantiate(Prefabs.ToggleOption, Prefabs.ToggleOption!.transform.parent)!;

        toggleOption.Title = Title;
        toggleOption.CheckMark.enabled = Value;

        return toggleOption;
    }
    
    public static implicit operator bool (CustomToggleOption _) => _.Value;

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(Value);
    }

    public override void Deserialize(BinaryReader reader)
    {
        Value = reader.ReadBoolean();
    }
}