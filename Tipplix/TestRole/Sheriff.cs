using HarmonyLib;
using Tipplix.Attributes;
using Tipplix.Enums;
using Tipplix.Extensions;
using Tipplix.Options;
using Tipplix.Roles;
using UnityEngine;

namespace Tipplix.TestRole;

[RegisterCustomRoles]
public sealed class Sheriff : RoleExtension
{
    public override string Name => "Sheriff";
    public override string Description => "Kill The Impostors";
    public override string LongDescription => "Sheriff is a crew member that has ability to kill its opponent.";
    public override Color Color => new(255f, 255f, 0f);
    public override RoleTeamExtension TeamExtension => RoleTeamExtension.Crewmate;
    public override bool CanUseKillButton => true;
    public override bool CanVent => false;
    public override bool CanSabotage => false;
    public override bool AffectedByLightAffectors => true;
    public override bool TasksCountTowardProgress => true;
    public override int MaxPlayer => int.MaxValue;
    public override bool OptionVisible => true;
    public override RevealTypes? RevealOnExile => null;
    public override int? KillDistance => null;
    public override bool CanTarget(PlayerControl target) => true;


    [RegisterCustomOption]
    public static CustomNumberOption KillCooldown { get; } = new(
        title: "Kill Cooldown", 
        range: new FloatRange(15f, 60f), 
        increment: 5f, 
        defaultValue: 20f
    );
    
    [RegisterCustomOption]
    public static CustomToggleOption CanKillNeutrals { get; } = new(
        title: "Can Kill Neutrals"
    );
    
    [HarmonyPatch(typeof(PlayerControl))]
    private static class Patches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(PlayerControl.CmdCheckMurder))]
        private static void CmdMurderPrefix(PlayerControl __instance, ref PlayerControl target)
        {
            if (!__instance.Data.Role.Is<Sheriff>()) return;
            if (target.Data.Role.IsImpostor || CanKillNeutrals && target.Data.Role.IsTeam(RoleTeamExtension.Alone)) return;
            
            target = __instance;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(PlayerControl.CmdCheckMurder))]
        private static void MurderPrefix(PlayerControl __instance)
        {
            if (__instance.Data.Role.Is<Sheriff>())
                __instance.SetKillTimer(KillCooldown);
        }
    }
}