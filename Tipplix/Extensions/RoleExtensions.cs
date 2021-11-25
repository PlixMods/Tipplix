using System;
using Reactor;
using Tipplix.Enums;
using Tipplix.Roles;

namespace Tipplix.Extensions;

public static class RoleExtensions
{
    public static T? GetExtensionOrDefault<T>(this RoleBehaviour role) where T : RoleExtension
    {
        return RoleExtensionManager.GetOrDefault(role) as T;
    }
    
    public static T Get<T>(this RoleBehaviour role) where T : RoleExtension
    {
        return role.GetExtensionOrDefault<T>() ?? throw new Exception($"{nameof(T)} might not be registered");
    }

    public static bool Is<T>(this RoleBehaviour role) where T : RoleExtension
    {
        return role.GetExtensionOrDefault<T>() != null;
    }

    public static RoleExtension? GetExtensionOrDefault(this RoleBehaviour role)
    {
        return RoleExtensionManager.GetOrDefault(role);
    }
    
    public static bool HasExtension(this RoleBehaviour role)
    {
        return role.GetExtensionOrDefault() != null;
    }
    
    public static bool IsTeam(this RoleBehaviour role, RoleTeamExtension teamExtension)
    {
        return role.TeamType == (RoleTeamTypes) teamExtension;
    }
}