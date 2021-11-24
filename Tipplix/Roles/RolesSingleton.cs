using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tipplix.Roles;

public static class RoleSingleton<T> where T : BaseRole
{
    private static T? _instance;
    public static T Instance => _instance ??= CustomRoleManagers.AllCustomRoles.OfType<T>().Single();
    
    public static RoleTypes Type => Instance.RoleType;
}