using System.Linq;
using Reactor;
using Reactor.Extensions;
using Tipplix.Enums;
using Tipplix.Options;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tipplix.Roles;

public abstract partial class BaseRole
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract string LongDescription { get; }
    public abstract string MedDescription { get; }
    public abstract Color Color { get; }
    public abstract RoleTeam Team { get; }
    public abstract bool CanUseKillButton { get; }
    public abstract bool CanVent { get; }
    public abstract bool TasksCountTowardProgress { get; }
    public abstract int MaxPlayer { get; }
        
    public virtual Sprite? GetSprite { get; set; }
    public Sprite? Sprite => _sprite ? _sprite : _sprite = GetSprite;
    private Sprite? _sprite;

    public RoleBehaviour? Behaviour { get; set; }
    public RoleTypes RoleType { get; private set; }
    public CustomRoleOptions? RoleOptions { get; set; }

    public void Initialize()
    {
        RoleOptions = CustomRoleOptions.Register(Behaviour = CreateInstance());
    }

    public RoleBehaviour CreateInstance()
    {
        var prefab = RoleManager.Instance.AllRoles.First(x => x);
        Behaviour = Object.Instantiate(prefab).DontDestroyOnLoad();
        Behaviour.name = $"{Name}Role";

        Behaviour.StringName = CustomStringName.Register(Name);
        Behaviour.BlurbName = CustomStringName.Register(Description);
        Behaviour.BlurbNameMed = CustomStringName.Register(MedDescription);
        Behaviour.BlurbNameLong = CustomStringName.Register(LongDescription);
        Behaviour.NameColor = Color;
            
        RoleType = Behaviour.Role = (RoleTypes) CustomRoleManagers.GetAvailableRoleId();
        Behaviour.TeamType = (RoleTeamTypes) Team;
        Behaviour.CanUseKillButton = CanUseKillButton;
        Behaviour.CanVent = CanVent;
        Behaviour.TasksCountTowardProgress = TasksCountTowardProgress;
        Behaviour.MaxCount = MaxPlayer;

        return Behaviour;
    }
}