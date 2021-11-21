using System;
using Reactor;

namespace Tipplix.Roles;

//[RegisterInIl2Cpp]
public sealed class PlixRole : RoleBehaviour
{
    public PlixRole(IntPtr ptr) : base(ptr) { }

    public override void Initialize(PlayerControl player)
    {
        
    }

    public override bool CanUse(IUsable console)
    {
        return false;
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return false;
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
    }

    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
    }

    public override void UseAbility()
    {
    }

    public override void OnMeetingStart()
    {
    }

    public override void OnVotingComplete()
    {
    }
    
    public override void SetUsableTarget(IUsable target)
    {
    }

    public override void SetPlayerTarget(PlayerControl target)
    {
    }

    public override void SetCooldown()
    {
    }
}