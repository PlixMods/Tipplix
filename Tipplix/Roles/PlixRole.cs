using System;
using System.Linq;
using Reactor;
using Tipplix.Enums;
using UnhollowerBaseLib.Attributes;

namespace Tipplix.Roles;

[RegisterInIl2Cpp]
public class PlixRole : RoleBehaviour
{
    public PlixRole(IntPtr ptr) : base(ptr) { }
    
    public RoleExtension? RoleExtension { get; internal set; }

    public void Start()
    {
        RoleExtension ??= RoleExtensionManager.AllCustomRoles.Single(x => x.RoleType == Role);
    }

    public override void Initialize(PlayerControl player)
    {
        Player = player;
        if (!player.AmOwner) return;
        
        if (CanUseKillButton)
        {
            DestroyableSingleton<HudManager>.Instance.KillButton.Show();
            player.SetKillTimer(10f);
        }
        else
        {
            DestroyableSingleton<HudManager>.Instance.KillButton.Hide();
        }

        if (RoleExtension != null)
        {
            if (RoleExtension.CanVent) DestroyableSingleton<HudManager>.Instance.ImpostorVentButton.Show();
            else DestroyableSingleton<HudManager>.Instance.ImpostorVentButton.Hide();

            if (RoleExtension.CanSabotage) DestroyableSingleton<HudManager>.Instance.SabotageButton.Show();
            else DestroyableSingleton<HudManager>.Instance.SabotageButton.Hide();
        }
    }
}