using System.Collections.Generic;
using HarmonyLib;
using Reactor;
using Tipplix.Attributes;
using Tipplix.CustomGameOver;
using Tipplix.Enums;
using Tipplix.Extensions;
using Tipplix.Options;
using Tipplix.Roles;
using Tipplix.Utility;
using UnityEngine;

namespace Tipplix.TestRole;

[RegisterCustomRoles]
public sealed class Jester : RoleExtension
{
    public override string Name => "Jester";
    public override string Description => "Get voted out";
    public override string LongDescription => "Just get voted out and win, that's all, seriously that's all, but this is a long description it has to be long since it's a long description.. but yeah just get voted tht's the goal kbye.";
    public override string MedDescription => "Be suspicious and get voted out";
    public override Color Color => Color.magenta;
    public override RoleTeamExtension TeamExtension => RoleTeamExtension.Alone;
    public override bool CanUseKillButton => false;
    public override bool CanVent => false;
    public override bool CanSabotage => false;
    public override bool AffectedByLightAffectors => !Settings.CanSeeInDark;
    public override bool TasksCountTowardProgress => true;
    public override int MaxPlayer => 1;
    public override bool OptionVisible => true;
    public override RevealTypes? RevealOnExile => null;
    public override int? KillDistance => null;
    public override bool CanTarget(PlayerControl target) => false;
    
    public override Sprite Sprite => _sprite != null ? _sprite : _sprite = SpriteHelper.FindOrNull("jester.png")!;
    public override CustomOption[] Options => new CustomOption[] {
        Settings.CanCallMeeting,
        Settings.CanSeeInDark
    };

    private Sprite _sprite = null!;
    public static class Settings
    {
        public static readonly CustomToggleOption CanCallMeeting = new("Can Call Emergency Meeting");
        public static readonly CustomToggleOption CanSeeInDark = new("Can See In The Dark");
    }
    
    internal class EndGame : CustomGameOverReason
    {
        public override string WinText => YouWon ? "You won" : "Jester won";
        public override string DescriptionText => $"{(YouWon ? "You won" : "Jester won")} by getting voted";
        public override Color Color => Color.magenta;
        public override bool ShowName => true;

        public override EndGameStingers Stinger =>
            YouWon ? EndGameStingers.CrewStinger : EndGameStingers.ImpostorStinger;
    }
    
    [HarmonyPatch]
    public static class Patches
    {
        [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.TryAssignRoleOnDeath))]
        public static bool Prefix(RoleManager __instance, PlayerControl player)
        {
            if (player.Data.Role.Is<Jester>())
            {
                CustomGameOverManager.RpcEndGame<EndGame>(new List<GameData.PlayerInfo> { player.Data });
            }

            return !player.Data.Role.Is<Jester>();
        }
    }
}