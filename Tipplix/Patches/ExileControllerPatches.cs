using System;
using System.Linq;
using HarmonyLib;
using Tipplix.Enums;
using Tipplix.Extensions;

namespace Tipplix.Patches;

[HarmonyPatch(typeof(ExileController))]
public static class ExileControllerPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ExileController.Begin))]
    public static void BeginPostfix(ExileController __instance)
    {
        var role = __instance.exiled?.Role.GetCustomRole();
        if (role is null) return;
        
        var revealRole = role.RevealOnExile;
        switch (revealRole)
        {
            case ExileReveal.Never:
            case ExileReveal.Default when !PlayerControl.GameOptions.ConfirmImpostor: return;
            case ExileReveal.Always: break;
            default: throw new ArgumentOutOfRangeException();
        }
        
        __instance.completeString = $"{__instance.exiled!.PlayerName.TrimEnd()} was {GetRoleName(role.Name)}.";
    }

    private static string GetRoleName(string roleName)
    {
        var lower = roleName.ToLower().Trim();
        var needThe = lower.StartsWith("the");

        return $"{(needThe? "The " : "")}{roleName}".Trim();
    }
}