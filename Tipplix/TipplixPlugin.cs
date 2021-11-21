using System;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using InnerNet;
using Reactor;
using Tipplix.Attributes;
using Tipplix.Roles;
using Tipplix.TestRole;
using UnityEngine;
using Object = System.Object;
using Random = System.Random;

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
        
        Harmony.CreateAndPatchAll(typeof(DebugManager));
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
    
    public static class DebugManager
    {
        private static readonly Random Random = new((int) DateTime.Now.Ticks);
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
        public static void KJU()
        {
            if (!AmongUsClient.Instance.AmHost) return;

            // Spawn dummys
            if (Input.GetKeyDown(KeyCode.F3))
            {
                var playerControl = UnityEngine.Object.Instantiate(AmongUsClient.Instance.PlayerPrefab);
                _ = playerControl.PlayerId = (byte) GameData.Instance.GetAvailableId();

                GameData.Instance.AddPlayer(playerControl);
                AmongUsClient.Instance.Spawn(playerControl);

                playerControl.transform.position = PlayerControl.LocalPlayer.transform.position;
                playerControl.GetComponent<DummyBehaviour>().enabled = true;
                playerControl.NetTransform.enabled = false;
                playerControl.SetName("SideDummy " + playerControl.Data.PlayerId);
                playerControl.SetColor((byte) Random.Next(Palette.PlayerColors.Length));
                GameData.Instance.RpcSetTasks(playerControl.PlayerId, Array.Empty<byte>());
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SetRole))]
        public static void oosspo(PlayerControl targetPlayer, out RoleTypes roleType)
        {
            roleType = targetPlayer.PlayerId == 2? RoleTypes.Impostor : (RoleTypes) 6;
        }
    }
}