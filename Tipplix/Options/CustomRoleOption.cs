using System.Linq;
using Reactor.Extensions;
using UnityEngine;
using Prefabs = Tipplix.Options.OptionsManager.Prefabs;
using Object = UnityEngine.Object;

namespace Tipplix.Options;

public class CustomRoleOption
{
    public RoleBehaviour? Role { get; }
    public CustomOption[]? CustomOptions { get; }
    public RoleOptionSetting? RoleSetting { get; set; }
    public AdvancedRoleSettingsButton? Tab { get; set; }

    public CustomRoleOption(RoleBehaviour role) => Role = role;

    public CustomRoleOption(RoleBehaviour role, params CustomOption[]? options) : this(role)
    {
        CustomOptions = options;
        if (options is null) return;
        
        foreach (var option in options)
        {
            OptionsManager.RegisterOption(option);
        }
    }
        
    public bool InitializeRoleNum()
    {
        if (RoleSetting) return true;

        var prefab = Prefabs.RoleOptionSetting;
        RoleSetting = Object.Instantiate(prefab, prefab!.transform.parent)!;
        RoleSetting.Role = Role;
        RoleSetting.name = Role!.NiceName;

        return false;
    }
        
    public AdvancedRoleSettingsButton InitializeAdvancedTab()
    {
        if (Tab != null && Tab.Tab) return Tab;
        
        var prefab = Prefabs.AdvancedTab;
        Tab = new AdvancedRoleSettingsButton {
            Tab = Object.Instantiate(prefab, prefab!.transform.parent)!,
            Type = Role!.Role
        };

        Tab.Tab.name = $"{Role.NiceName} Settings";
            
        var textGameObject = Tab.Tab.GetComponentInChildren<TextTranslatorTMP>();
        if (textGameObject) textGameObject.TargetText = Role.StringName;

        foreach (var optionBehaviour in Tab.Tab.GetComponentsInChildren<OptionBehaviour>())
        {
            optionBehaviour.gameObject.Destroy();
        }

        if (CustomOptions is null || !CustomOptions.Any())
        {
            // destroy adv. button
            return Tab;
        }

        const float yStart = 0.06f;
        const float yOffset = 0.56f;
        
        for (var i = 0; i < CustomOptions.Length; i++)
        {
            var behaviour = CustomOptions[i].Initialize();
            var transform = behaviour.transform;
            
            transform.SetParent(Tab.Tab.transform);
            transform.localPosition = new Vector3(-1.25f, yStart - i * yOffset, 0f);
            behaviour.name = behaviour.GetType().ToString();
        }
        
        return Tab;
    }
}