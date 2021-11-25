using Prefabs = Tipplix.Options.OptionsManager.Prefabs;
using Il2CppSystem.IO;
using System.Linq;
using HarmonyLib;
using Tipplix.Extensions;

namespace Tipplix.Options;

[HarmonyPatch]
public static class Patches
{
    // TODO: Use custom RPC for sending custom game options
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.Serialize))]
    public static void GameOptionsData_SerializePostfix(BinaryWriter writer)
    {
        PlixOptions.Serialize(writer);
        if (!OptionsManager.AllOptions.Any()) return;

        foreach (var option in OptionsManager.AllOptions)
        {
            option.Serialize(writer);
        }
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.Deserialize), typeof(BinaryReader))]
    public static void GameOptionsData_DeserializePostfix(BinaryReader reader)
    {
        PlixOptions.Deserialize(reader);
        if (!OptionsManager.AllOptions.Any()) return;

        foreach (var option in OptionsManager.AllOptions)
        {
            option.Deserialize(reader);
        }
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.OnEnable))]
    public static void RolesSettingsMenu_OnEnablePrefix(RolesSettingsMenu __instance)
    {
        Prefabs.FindAndSet();
        Prefabs.FindAndSetRoleSettings(__instance);
        
        if (!OptionsManager.AllRoleOptions.Any()) return;
        
        foreach (var option in OptionsManager.AllRoleOptions.Where(_ => !_.InitializeRoleNum())) 
        {
            __instance.AllRoleSettings.Add(option.RoleSetting);
            __instance.AllAdvancedSettingTabs.Add(option.InitializeAdvancedTab());
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(RoleOptionSetting), nameof(RoleOptionSetting.ShowRoleDetails))]
    public static bool RoleOptionSetting_ShowRoleDetailsPrefix(RoleOptionSetting __instance)
    {
        if (!__instance.Role.HasExtension()) return true;
                
        GameSettingMenu.Instance.RoleName.text = __instance.Role.NiceName;
        GameSettingMenu.Instance.RoleBlurb.text = __instance.Role.BlurbLong;
        GameSettingMenu.Instance.RoleIcon.sprite = __instance.Role.GetExtensionOrDefault()?.Sprite;
                
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.ValueChanged))]
    public static bool RolesSettingsMenu_ValueChangedPrefix(RolesSettingsMenu __instance, OptionBehaviour obj)
    {
        if (OptionsManager.TitleToOption.TryGetValue(obj.Title, out var option))
        {
            option.ValueChanged(obj);
            return false;
        }
        
        return true;
    }
}