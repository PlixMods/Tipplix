using HarmonyLib;
using Tipplix.Enums;
using Tipplix.Extensions;
using UnityEngine;
using I = Il2CppSystem.Collections.Generic;

namespace Tipplix.Intro;

[HarmonyPatch(typeof(IntroCutscene))]
public static class Patches
{
    private static readonly int Color = Shader.PropertyToID("_Color");

    [HarmonyPrefix]
    [HarmonyPatch(nameof(IntroCutscene.BeginCrewmate))]
    public static void BeginCrewmatePrefix(ref I.List<PlayerControl> yourTeam)
    {
        var localPlayer = PlayerControl.LocalPlayer;
        var exists = localPlayer && localPlayer.Data != null && localPlayer.Data.Role;
        if (!exists) return;

        if (localPlayer.Data!.Role.TeamType != (RoleTeamTypes) RoleTeam.Alone) return;
        
        yourTeam.Clear();
        yourTeam.Add(PlayerControl.LocalPlayer);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(IntroCutscene.BeginCrewmate))]
    public static void BeginCrewmatePostfix(IntroCutscene __instance)
    {
        var localRoleData = PlayerControl.LocalPlayer!.Data!.Role;
        if (localRoleData!.TeamType != (RoleTeamTypes) RoleTeam.Alone) return;
        
        __instance.TeamTitle.SetText(localRoleData.NiceName);
        __instance.TeamTitle.color = localRoleData.TeamColor;
        __instance.ImpostorText.SetText(localRoleData.Blurb);
        __instance.ImpostorText.color = localRoleData.TeamColor;
        __instance.BackgroundBar.material.SetColor(Color, localRoleData.TeamColor);
    }

    [HarmonyPrefix] [HarmonyPostfix]
    [HarmonyPatch(nameof(IntroCutscene.SetUpRoleText))]
    public static void SetUpRoleTextPrefix(IntroCutscene __instance)
    { } // Patching here because the game crashes if I don't?
}