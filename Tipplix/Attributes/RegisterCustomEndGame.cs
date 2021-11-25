using System;
using System.Reflection;
using HarmonyLib;
using Reactor;
using Tipplix.CustomGameOver;

namespace Tipplix.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class RegisterCustomEndGameAttribute : Attribute
{
    public static void Register(Type type)
    {
        if (!type.IsSubclassOf(typeof(CustomGameOverReason)))
        {
            throw new InvalidOperationException($"Type {type.FullDescription()} has {nameof(RegisterCustomEndGameAttribute)} but doesn't extend {nameof(CustomGameOverReason)}.");
        }

        var newReason = (CustomGameOverReason) Activator.CreateInstance(type);
        CustomGameOverManager.Register(newReason);
    }
    
    public static void Register(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (type.GetCustomAttribute<RegisterCustomEndGameAttribute>() != null) 
                Register(type);
        }
    }

    internal static void Initialize()
    {
        ChainloaderHooks.PluginLoad += plugin => Register(plugin.GetType().Assembly);
    }
}