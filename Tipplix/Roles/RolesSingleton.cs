using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tipplix.Roles;

public static class RoleSingleton<T> where T : BaseRole
{
    private static T? _instance;
    public static T Instance => _instance ??= CustomRoleManagers.AllCustomRoles.OfType<T>().Single();

    public static RoleTypes Type => Instance.RoleType;
    
    // Bad performance, will change later
    public static IEnumerable<byte> PlayerIds => GameData.Instance.AllPlayers.ToArray().Where(x => x.Role.Role == Instance.RoleType).Select(x => x.PlayerId);
    
    public static IEnumerable<PlayerControl> Players => PlayerIds.Select(x => GameData.Instance.GetPlayerById(x).Object);
    public static IEnumerable<GameData.PlayerInfo> PlayerDatas => PlayerIds.Select(x => GameData.Instance.GetPlayerById(x));
    
    internal static void Register(BaseRole role)
    {
        role.GetType().MakeGenericType(typeof(RoleSingleton<>))!
            .GetField(nameof(_instance), BindingFlags.Static | BindingFlags.NonPublic)!
            .SetValue(null, role);
    }
}