using System.Linq;
using HarmonyLib;
using Tipplix.Roles;
using UnityEngine;

namespace Tipplix.Patches;

[HarmonyPatch(typeof(PlayerControl))]
public static class PlayerControlPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(PlayerControl.SetRole))]
    public static void SetRolePrefix(PlayerControl __instance)
    {
        __instance.roleAssigned = false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(PlayerControl.CheckMurder))]
    public static bool CheckMurderPrefix(PlayerControl __instance, PlayerControl target)
    {
        if (AmongUsClient.Instance.IsGameOver || !AmongUsClient.Instance.AmHost) return false;
        if (target.Data is null || target.Data.IsDead || target.inVent) return false;
        
        __instance.RpcMurderPlayer(target);
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(PlayerControl.FixedUpdate))]
    public static void FixedUpdatePostfix(PlayerControl __instance)
    {
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(PlayerControl.FindClosestTarget))]
    public static bool FindClosestPlayerPrefix(PlayerControl __instance, bool protecting, ref PlayerControl __result)
    {
        if (protecting) return true;
        if (__instance.Data.Role is not PlixRole role || role.RoleExtension is null) return true;
        
        PlayerControl playerControl = null!;

        var killDistance = GameOptionsData.KillDistances[Mathf.Clamp(role.RoleExtension.KillDistance ?? PlayerControl.GameOptions.KillDistance, 0, 2)];
        if (!ShipStatus.Instance) __result = playerControl;
        
        var truePosition = __instance.GetTruePosition();
        var allPlayers = GameData.Instance.AllPlayers.ToArray();

        foreach (var player in allPlayers.Where(_ => !_.Disconnected && _.PlayerId != __instance.PlayerId && 
                                                     !_.IsDead && role.RoleExtension.CanTarget(_.Object) && !_.Object.inVent))
        {
            var @object = player.Object;
            if (!@object || !@object.Collider.enabled) continue;
            
            var vector = @object.GetTruePosition() - truePosition;
            var magnitude = vector.magnitude;
            
            if (magnitude <= killDistance && !PhysicsHelpers.AnyNonTriggersBetween(
                    truePosition, vector.normalized, magnitude, Constants.ShipAndObjectsMask))
            {
                playerControl = @object;
                killDistance = magnitude;
            }
        }

        __result = playerControl;
        return false;
    }

}