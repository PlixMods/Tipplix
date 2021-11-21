using HarmonyLib;
using Reactor;
using Tipplix.Enums;
using Tipplix.Extensions;
using UnityEngine;

namespace Tipplix.Roles;

public static class Patches
{
    [HarmonyPatch(typeof(RoleBehaviour))]
    private static class RoleBehaviourPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(RoleBehaviour.TeamColor), MethodType.Getter)]
        private static bool TeamColorPrefix(RoleBehaviour __instance, ref Color __result)
        {
            var role = __instance.GetCustomRole();
            if (role is null or { Team: not RoleTeam.Alone }) return true;

            __result = role.Color;
            return false;
        }
    }
        
    [HarmonyPatch(typeof(TaskAddButton))]
    private static class TaskAdderPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(TaskAddButton.Role), MethodType.Setter)]
        private static void TeamColorPrefix(TaskAddButton __instance, RoleBehaviour value)
        {
            if (!value.IsCustom()) return;
            __instance.FileImage.color = value.TeamColor;
            __instance.RolloverHandler.OutColor = value.TeamColor;
        }
    }
}