using Tipplix.Roles;

namespace Tipplix.Extensions;

public static class BaseRoleExtensions
{
    public static bool IsCustom(this RoleBehaviour roleBehaviour)
    {
        return (int) roleBehaviour.Role >= 6;
    }
        
    public static bool Is<T>(this PlayerControl playerControl) where T : BaseRole
    {
        return playerControl.Data.Is<T>();
    }
        
    public static bool Is<T>(this GameData.PlayerInfo playerInfo) where T : BaseRole
    {
        return GetCustomRole(playerInfo.Role) is T;
    }
    
    public static BaseRole? GetCustomRole(this PlayerControl player)
    {
        return CustomRoleManagers.GetOrDefault(player.Data.Role);
    }
    
    public static BaseRole? GetCustomRole(this RoleBehaviour roleBehaviour)
    {
        return CustomRoleManagers.GetOrDefault(roleBehaviour);
    }
}