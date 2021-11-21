using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Reactor;
using Reactor.Extensions;
using Tipplix.Extensions;
using Tipplix.Utility;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tipplix.Options;

public class CustomRoleOptions
{
    private static List<CustomRoleOptions> _roleOptions { get; } = new();
    public static IReadOnlyList<CustomRoleOptions> RoleOptions => _roleOptions.AsReadOnly();
        
    public RoleOptionSetting? RoleSetting { get; set; }
        
    public RoleBehaviour? Role { get; }
    public AdvancedRoleSettingsButton? Tab { get; set; }

    public CustomRoleOptions(RoleBehaviour role)
    {
        Role = role;
    }

    public static CustomRoleOptions Register(RoleBehaviour role)
    {
        var roleOptions = new CustomRoleOptions(role);
        _roleOptions.Add(roleOptions);
        return roleOptions;
    }
        
    public bool InitializeChance(RoleOptionSetting? prefab)
    {
        if (RoleSetting) return true;
            
        RoleSetting = Object.Instantiate(prefab, prefab!.transform.parent);
        RoleSetting!.Role = Role;
        RoleSetting.name = Role!.NiceName;

        return false;
    }
        
    public AdvancedRoleSettingsButton InitializeAdvancedTab(AdvancedRoleSettingsButton? prefab)
    {
        if (Tab != null && Tab.Tab) return Tab;

        Tab = new AdvancedRoleSettingsButton {
            Tab = Object.Instantiate(prefab!.Tab, prefab.Tab.transform.parent),
            Type = Role!.Role
        };

        Tab.Tab.name = $"{Role.NiceName} Settings";
            
        var textGameObject = Tab.Tab.GetComponentInChildren<TextTranslatorTMP>();
        if (textGameObject) textGameObject.TargetText = Role.StringName;

        foreach (var optionBehaviour in Tab.Tab.GetComponentsInChildren<OptionBehaviour>())
        {
            optionBehaviour.gameObject.Destroy();
        }
            
        return Tab;
    }

    [HarmonyPatch]
    public static class Patches
    {
        private static RoleOptionSetting? _roleOptionsPrefab;
        private static AdvancedRoleSettingsButton? _buttonPrefab;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.OnEnable))]
        public static void OnEnablePrefix(RolesSettingsMenu __instance)
        {
            if (!RoleOptions.Any()) return;
            TrySetPrefab(__instance);
                
            foreach (var option in RoleOptions.Where(_ => !_.InitializeChance(_roleOptionsPrefab))) 
            {
                __instance.AllRoleSettings.Add(option.RoleSetting);
                __instance.AllAdvancedSettingTabs.Add(option.InitializeAdvancedTab(_buttonPrefab));
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(RoleOptionSetting), nameof(RoleOptionSetting.ShowRoleDetails))]
        public static bool ShowRoleDetailsPrefix(RoleOptionSetting __instance)
        {
            if (!__instance.Role.IsCustom()) return true;
                
            GameSettingMenu.Instance.RoleName.text = __instance.Role.NiceName;
            GameSettingMenu.Instance.RoleBlurb.text = __instance.Role.BlurbLong;
            GameSettingMenu.Instance.RoleIcon.sprite = __instance.Role.GetCustomRole()?.Sprite;
                
            return false;
        }

        public static void TrySetPrefab(RolesSettingsMenu menu)
        {
            if (!_roleOptionsPrefab) _roleOptionsPrefab = Object.FindObjectOfType<RoleOptionSetting>();
            if (_buttonPrefab is null || !_buttonPrefab.Tab) 
                _buttonPrefab = menu.AllAdvancedSettingTabs.ToArray().First();
        }
    }
}