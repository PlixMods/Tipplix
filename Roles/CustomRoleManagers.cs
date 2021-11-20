using System.Collections.Generic;
using System.Linq;

namespace Tipplix.Roles
{
    public static class CustomRoleManagers
    {
        private static List<BaseRole> _allCustomRoles = new();
        public static IReadOnlyCollection<BaseRole> AllCustomRoles => _allCustomRoles.AsReadOnly();

        private static ushort _currentId = 6;
        public static ushort GetAvailableRoleId() => _currentId++;
        public static void Register(BaseRole role) => _allCustomRoles.Add(role);

        public static void LoadRoles()
        {
            _allCustomRoles.ForEach(_ => _.Initialize());
            RoleManager.Instance.AllRoles = RoleManager.Instance.AllRoles.Concat(_allCustomRoles.Select(_ => _.Behaviour)).ToArray();
        }

        public static BaseRole GetOrDefault(RoleBehaviour roleBehaviour)
        {
            return GetOrDefault(roleBehaviour.Role);
        }
        
        public static BaseRole GetOrDefault(RoleTypes roleType)
        {
            var pos = _allCustomRoles.FindIndex(x => x.RoleType == roleType);
            return pos != -1? _allCustomRoles[pos] : null;
        }
    }
}