using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tipplix.Enums;
using UnityEngine;

namespace Tipplix.Options;

public static class OptionsManager
{
    private static readonly List<CustomOption> _allOptions = new();
    private static readonly List<CustomRoleOption> _allGameOptions = new();
    private static readonly List<CustomRoleOption> _allRoleOptions = new();
    
    internal static Dictionary<StringNames, CustomOption> TitleToOption = new();
    public static IReadOnlyCollection<CustomOption> AllOptions => _allOptions.AsReadOnly();
    public static IReadOnlyCollection<CustomRoleOption> AllGameOptions => _allGameOptions.AsReadOnly();
    public static IReadOnlyCollection<CustomRoleOption> AllRoleOptions => _allRoleOptions.AsReadOnly();
    
    internal static void RegisterOption(CustomOption customOption) => _allOptions.Add(customOption);

    public static CustomOption RegisterGameOption(CustomOption customOption)
    {
        _allOptions.Add(customOption);
        RegisterOption(customOption);
        return customOption;
    }
    
    public static CustomRoleOption RegisterRoleOption(CustomRoleOption roleOption)
    {
        _allRoleOptions.Add(roleOption);
        return roleOption;
    }

    public static void SyncSettings()
    {
        if (PlayerControl.LocalPlayer)
            PlayerControl.LocalPlayer.RpcSyncSettings(PlayerControl.GameOptions);
    }
    
    public static class Prefabs
    {
        public static bool AnyNull => !NumberOption || !ToggleOption || !StringOption || !RoleOptionSetting;
        
        public static NumberOption? NumberOption { get; private set; }
        public static ToggleOption? ToggleOption { get; private set; }
        public static StringOption? StringOption { get; private set; }
        public static RoleOptionSetting? RoleOptionSetting { get; private set; }
        public static GameObject? AdvancedTab { get; private set; }

        public static void FindAndSet()
        {
            if (!AnyNull) return;
            
            NumberOption = Object.FindObjectOfType<NumberOption>();
            ToggleOption = Object.FindObjectOfType<ToggleOption>();
            StringOption = Object.FindObjectOfType<StringOption>();
            RoleOptionSetting = Object.FindObjectOfType<RoleOptionSetting>();
        }

        public static void FindAndSetRoleSettings(RolesSettingsMenu instance)
        {
            if (AdvancedTab) return;
            AdvancedTab = instance.AllAdvancedSettingTabs.ToArray().FirstOrDefault()?.Tab;
        }
    }
}