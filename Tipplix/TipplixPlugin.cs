using System;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
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
    public static ManualLogSource Logger => Logger<TipplixPlugin>.Instance;
    private Harmony Harmony { get; } = new(Id);

    public override void Load()
    {
        RegisterCustomRolesAttribute.Initialize();
        
        // TODO: remove this when done messing up stuff
        RegisterCustomRolesAttribute.Register(typeof(Jester));
        RegisterCustomRolesAttribute.Register(typeof(Sheriff));
        
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
            
            _initialized = true;
        }

        private static bool CheckRolesExistance()
        {
            return RoleManager.Instance && (RoleManager.Instance.AllRoles?.Any() ?? false);
        }
    }
    
    [HarmonyPatch]
    public static class DebugManager
    {
        private static readonly Random Random = new((int) DateTime.Now.Ticks);
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
        public static void KJU()
        {
            // Spawn dummys
            if (Input.GetKeyDown(KeyCode.F3) && AmongUsClient.Instance.AmHost)
            {
                var dummy = UnityEngine.Object.Instantiate(AmongUsClient.Instance.PlayerPrefab);
                dummy.PlayerId = (byte) GameData.Instance.GetAvailableId();

                GameData.Instance.AddPlayer(dummy);
                AmongUsClient.Instance.Spawn(dummy);

                dummy.transform.position = PlayerControl.LocalPlayer.transform.position;
                dummy.GetComponent<DummyBehaviour>().enabled = true;
                dummy.NetTransform.enabled = false;
                dummy.SetName("SideDummy " + dummy.Data.PlayerId);
                dummy.SetColor((byte) Random.Next(Palette.PlayerColors.Length));
                GameData.Instance.RpcSetTasks(dummy.PlayerId, Array.Empty<byte>());
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                PlayerControl.AllPlayerControls.ToArray().ToList().ForEach(p =>
                {
                    p.nameText.text = p.Data.PlayerName + "\n" + p.Data.Role.NiceName + "\n ";
                    p.nameText.color = p.Data.Role.TeamColor;
                });
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                Logger<TipplixPlugin>.Debug("Jester can call meeting:" + (bool) Jester.Settings.CanCallMeeting);
            }
        }
    }
}