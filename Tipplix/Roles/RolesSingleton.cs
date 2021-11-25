using System.Linq;

namespace Tipplix.Roles;

public static class RoleSingleton<T> where T : RoleExtension
{
    private static T? _instance;
    public static T Instance => _instance ??= RoleExtensionManager.AllCustomRoles.OfType<T>().Single();
    
    public static RoleTypes Type => Instance.RoleType;
}