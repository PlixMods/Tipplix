namespace Tipplix.Roles;

public abstract partial class BaseRole
{
    public override string ToString() => Name;

    public static explicit operator RoleBehaviour(BaseRole _) => _.Behaviour;
    public static explicit operator RoleTypes(BaseRole _) => _.RoleType;
        
    public static explicit operator BaseRole(RoleBehaviour _) => CustomRoleManagers.GetOrDefault(_);
    public static explicit operator BaseRole(RoleTypes _) => CustomRoleManagers.GetOrDefault(_);
}