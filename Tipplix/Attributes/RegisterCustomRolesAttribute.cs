using System;
using System.Reflection;
using BepInEx.IL2CPP;
using HarmonyLib;
using Reactor;
using Tipplix.Roles;

namespace Tipplix.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class RegisterCustomRolesAttribute : Attribute
{
    public static void Register(Type roleType)
    {
        if (!roleType.IsSubclassOf(typeof(BaseRole)))
        {
            throw new InvalidOperationException($"Type {roleType.FullDescription()} has {nameof(RegisterCustomRolesAttribute)} but doesn't extend {nameof(BasePlugin)}.");
        }

        var newRole = (BaseRole) Activator.CreateInstance(roleType);
        CustomRoleManagers.Register(newRole);
    }
    
    public static void Register(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (type.GetCustomAttribute<RegisterCustomRolesAttribute>() != null) 
                Register(type);
        }
    }

    internal static void Initialize()
    {
        ChainloaderHooks.PluginLoad += plugin => Register(plugin.GetType().Assembly);
    }
}