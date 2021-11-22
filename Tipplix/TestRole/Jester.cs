using HarmonyLib;
using Tipplix.Attributes;
using Tipplix.Enums;
using Tipplix.Extensions;
using Tipplix.Roles;
using Tipplix.Utility;
using UnityEngine;

namespace Tipplix.TestRole;

[RegisterCustomRoles]
public sealed class Jester : BaseRole
{
    public override string Name => "Jester";
    public override string Description => "Get voted out";
    public override string LongDescription => "Just get voted out and win, that's all, seriously that's all, but this is a long description it has to be long since it's a long description.. but yeah just get voted tht's the goal kbye.";
    public override string MedDescription => "Be suspicious and get voted out";
    public override Color Color => Color.magenta;
    public override RoleTeam Team => RoleTeam.Alone;
    public override bool CanUseKillButton => false;
    public override bool CanVent => false;
    public override bool TasksCountTowardProgress => true;
    public override int MaxPlayer => 1;
    public override Sprite? GetSprite => SpriteHelper.CreateSprite("Tipplix.Resources.jester.png", 190f);

    [HarmonyPatch]
    public static class Patches
    {
        [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.TryAssignRoleOnDeath))]
        public static bool Prefix(RoleManager __instance, PlayerControl player)
        {
            if (player.Is<Jester>())
            {
                ShipStatus.RpcEndGame((GameOverReason) 2, false);
            }

            return !player.Is<Jester>();
        }
    }
}