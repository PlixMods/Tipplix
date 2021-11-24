using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using I = Il2CppSystem.Collections.Generic;
using Reactor;
using Reactor.Networking;
using Tipplix.Enums;
using Tipplix.Extensions;
using Tipplix.Linq;
using UnityEngine;
using static RoleManager;

namespace Tipplix.Roles;

public static class Patches
{
	[HarmonyPatch(typeof(RoleBehaviour))]
	private static class RoleBehaviourPatches
	{
		[HarmonyPrefix]
		[HarmonyPatch(nameof(RoleBehaviour.TeamColor), MethodType.Getter)]
		private static bool TeamColorPrefix(RoleBehaviour __instance, ref Color __result)
		{
			var role = __instance.GetCustomRole();
			if (role is null) return true;

			__result = role.Color;
			return false;
		}
	}

	[HarmonyPatch(typeof(TaskAddButton))]
	private static class TaskAdderPatches
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(TaskAddButton.Role), MethodType.Setter)]
		private static void TeamColorPrefix(TaskAddButton __instance, RoleBehaviour value)
		{
			if (!value.IsCustom()) return;
			
			__instance.FileImage.color = value.TeamColor;
			__instance.RolloverHandler.OutColor = value.TeamColor;
		}
	}

	[HarmonyPatch(typeof(RoleManager))]
	private static class RoleManagerPatches
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(RoleManager.SelectRoles))]
		public static void SelectRolesPatch()
		{
			var roleOptions = PlayerControl.GameOptions.RoleOptions;
			var allAloneRoles = RoleManager.Instance.AllRoles.Where(role => role.TeamType > (RoleTeamTypes) 1 && !IsGhostRole(role.Role));
			var allCrews = GameData.Instance.AllPlayers.ToArray().Where(_ => !_.Disconnected &&!_.IsDead && _.Role.IsSimpleRole && !_.Role.IsImpostor).ToList();
			var toAssign = allAloneRoles
				.Select(role => role.Role)
				.Where(role => roleOptions.GetChancePerGame(role) == 100 || HashRandom.Next(101) < roleOptions.GetChancePerGame(role))
				.Shuffle().ToList();

			foreach (var role in toAssign)
			{
				for (var i = 1; i < roleOptions.GetNumPerGame(role); i++)
					toAssign.Add(role);
			}

			allCrews = allCrews.Shuffle().ToList();
			toAssign = toAssign.Shuffle().ToList();

			while (true)
			{
				if (!toAssign.Any()) break;
				var randomRole = toAssign[HashRandom.Next(toAssign.Count)];
				var randomPlayer = allCrews[HashRandom.Next(allCrews.Count)];
				
				if (allCrews.Remove(randomPlayer) && toAssign.Remove(randomRole))
					randomPlayer.Object.RpcSetRole(randomRole);
			}
		}
	}
}