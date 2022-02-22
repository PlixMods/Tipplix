using System;
using System.Reflection;
using HarmonyLib;
using Reactor;
using Tipplix.CustomGameOver;

namespace Tipplix.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class RegisterCustomOptionAttribute : Attribute
{

}