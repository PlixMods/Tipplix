using System;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using Reactor;
using Tipplix.Attributes;
using Tipplix.Roles;
using Tipplix.TestRole;

namespace Tipplix;

[BepInAutoPlugin("api.tipplix")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class TipplixPlugin : BasePlugin
{
    public static TipplixPlugin Instance => PluginSingleton<TipplixPlugin>.Instance;
    private Harmony Harmony { get; } = new(Id);

    public override void Load()
    {
        RegisterCustomRolesAttribute.Initialize();
        
        // TODO: remove this when done messing up stuff
        RegisterCustomRolesAttribute.Register(Assembly.GetExecutingAssembly());
        
        Harmony.PatchAll();
    }

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static class MMMStart
    {
        private static bool _initialized;
        
        public static void Postfix()
        {
            if (_initialized) return;
            if (!CheckRolesExistance()) return;
            
            CustomRoleManagers.LoadRoles();
            
            Logger<TipplixPlugin>.Debug("Roles list: " + RoleManager.Instance.AllRoles.Select(x => x.NiceName).Aggregate((x,y) => x + ", " + y));
            
            _initialized = true;
        }

        public static bool CheckRolesExistance()
        {
            if (!RoleManager.Instance || !(RoleManager.Instance.AllRoles?.Any() ?? false)) return false;
            Logger<TipplixPlugin>.Debug("Roles indeed added/available on MainMenuManager.Start()..");
            return true;
        }
    }
}