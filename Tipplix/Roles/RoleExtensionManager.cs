using System.Collections.Generic;
using System.Linq;

namespace Tipplix.Roles;

public static class RoleExtensionManager
{
    private static List<RoleExtension> _allCustomRoles = new();
    public static IReadOnlyCollection<RoleExtension> AllCustomRoles => _allCustomRoles.AsReadOnly();
    internal static Dictionary<ushort, RoleExtension> ExtensionAddresses { get; } = new();

    private static ushort _currentId = 6;
    private static ushort GetAvailableRoleId() => _currentId++;

    public static void Register(RoleExtension roleExtension)
    {
        roleExtension.RoleType = (RoleTypes) GetAvailableRoleId();
        ExtensionAddresses[(ushort) roleExtension.RoleType] = roleExtension;
        _allCustomRoles.Add(roleExtension);
    }

    public static void LoadRoles()
    {
        _allCustomRoles.ForEach(x => x.Initialize());
        RoleManager.Instance.AllRoles = RoleManager.Instance.AllRoles
            .Concat(_allCustomRoles.Select(x => x.Behaviour))
            .ToArray();
    }

    public static RoleExtension? GetOrDefault(RoleBehaviour roleBehaviour)
    {
        Reactor.Logger<TipplixPlugin>.Info("Is castable: " + roleBehaviour.TryCast<PlixRole>()?.RoleExtension?.GetType());
        return roleBehaviour.TryCast<PlixRole>()?.RoleExtension ?? GetOrDefault(roleBehaviour.Role);
    }
        
    public static RoleExtension? GetOrDefault(RoleTypes roleType)
    {
        return ExtensionAddresses.TryGetValue((ushort) roleType, out var val) ? 
            val : _allCustomRoles.FirstOrDefault(x => x.RoleType == roleType);
    }
}