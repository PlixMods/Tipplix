using System;
using System.Reflection;
using BepInEx.IL2CPP;
using HarmonyLib;
using Reactor;
using Reactor.Networking;
using Tipplix.Roles;

namespace Tipplix.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class RegisterCustomRolesAttribute : Attribute
{
    public static void Register(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            var attribute = type.GetCustomAttribute<RegisterCustomRolesAttribute>();
            if (attribute == null) continue;
            
            if (!type.IsSubclassOf(typeof(BaseRole)))
            {
                throw new InvalidOperationException($"Type {type.FullDescription()} has {nameof(RegisterCustomRolesAttribute)} but doesn't extend {nameof(BasePlugin)}.");
            }

            var newRole = (BaseRole) Activator.CreateInstance(type);
            CustomRoleManagers.Register(newRole);
        }
    }

    internal static void Initialize()
    {
        ChainloaderHooks.PluginLoad += plugin => Register(plugin.GetType().Assembly);
    }
}