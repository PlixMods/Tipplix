using Reactor;
using Reactor.Extensions;
using Tipplix.Enums;
using Tipplix.Options;
using UnityEngine;

namespace Tipplix.Roles;

public abstract class RoleExtension
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public virtual string MedDescription => Description;
    public virtual string LongDescription => Description;
    public abstract Color Color { get; }
    public abstract RoleTeamExtension TeamExtension { get; }
    public abstract bool TasksCountTowardProgress { get; }
    public abstract int MaxPlayer { get; }
    public abstract bool OptionVisible { get; }
    
    /// <summary>
    /// Type of reveal on exile, return null to use <see cref="PlixOptions"/>
    /// </summary>
    public abstract RevealTypes? RevealOnExile { get; }
    
    public abstract int? KillDistance { get; }
    public abstract bool CanTarget(PlayerControl target);
    
    public abstract bool CanUseKillButton { get; }
    public abstract bool CanVent { get; }
    public abstract bool CanSabotage { get; }

    public abstract bool AffectedByLightAffectors { get; }

    public virtual CustomOption[]? Options => null;
    public virtual Sprite? Sprite { get; } = null;

    public PlixRole? Behaviour { get; set; }
    public RoleTypes RoleType { get; internal set; }
    public CustomRoleOption? RoleOptions { get; set; }

    public void Initialize()
    {
        Behaviour = CreateInstance();

        if (OptionVisible)
        {
            var roleOption = new CustomRoleOption(Behaviour, Options);
            RoleOptions = OptionsManager.RegisterRoleOption(roleOption);
        }
    }

    public PlixRole CreateInstance()
    {
        Behaviour = new GameObject($"{Name}Role").AddComponent<PlixRole>().DontDestroyOnLoad();

        Behaviour.StringName = CustomStringName.Register(Name);
        Behaviour.BlurbName = CustomStringName.Register(Description);
        Behaviour.BlurbNameMed = CustomStringName.Register(MedDescription);
        Behaviour.BlurbNameLong = CustomStringName.Register(LongDescription);
        
        Behaviour.Role = RoleType;
        Behaviour.TeamType = (RoleTeamTypes) TeamExtension;
        Behaviour.CanUseKillButton = CanUseKillButton;
        Behaviour.CanVent = CanVent;
        Behaviour.TasksCountTowardProgress = TasksCountTowardProgress;
        Behaviour.MaxCount = MaxPlayer;
        
        Behaviour.AffectedByLightAffectors = AffectedByLightAffectors;
        Behaviour.RoleExtension = this;
        
        return Behaviour;
    }
}