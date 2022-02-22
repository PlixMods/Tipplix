using HarmonyLib;
using Tipplix.Enums;
using Tipplix.Extensions;
using Tipplix.Options;
using UnhollowerBaseLib;

namespace Tipplix.Patches;

[HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>))]
public static class ExileControllerPatches
{
    [HarmonyPrefix]
    public static bool Prefix(StringNames id, ref string __result)
    {
        // Reveal custom role
        if (ExileController.Instance is var controller && controller 
            && id is StringNames.ExileTextPN or StringNames.ExileTextSN or StringNames.ExileTextPP 
            or StringNames.ExileTextSP)
        {
            var role = controller.exiled?.Role.GetExtensionOrDefault();
            if (role is null) return true;

            var playerName = controller.exiled!.PlayerName.Trim();
            var revealRole = role.RevealOnExile ?? PlixOptions.ExileRevealType;
            var roleString = revealRole switch {
                RevealTypes.None => "was ejected",
                RevealTypes.Team => GetTeamString(role.TeamExtension),
                RevealTypes.Role => GetRoleString(role.Name),
                _ => "was unknown"
            };

            __result = $"{playerName} {roleString}.";
            return false;
        }
        
        // Never reveal how many impostor left
        if (controller && id is StringNames.ImpostorsRemainP or StringNames.ImpostorsRemainS)
        {
            __result = string.Empty;
            return false;
        }

        return true;
    }

    private static string GetTeamString(RoleTeamExtension teamExtensionTypes)
    {
        return teamExtensionTypes switch {
            RoleTeamExtension.Crewmate => "was a crewmate",
            RoleTeamExtension.Impostor => "was an impostor",
            RoleTeamExtension.Alone => "was a neutral",
            _ => "role is unknown (invalid)"
        };
    }

    private static string GetRoleString(string roleName)
    {
        var lower = roleName.ToLower().Trim();
        var needThe = lower.StartsWith("the");

        return $"was {(needThe? "The " : "")}{roleName}".Trim();
    }
}