using System;
using HarmonyLib;
using Tipplix.Options;
using UnityEngine;

namespace Tipplix.Patches;

[HarmonyPatch(typeof(GameOptionsData))]
public static class GameOptionsDataPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(GameOptionsData.ToHudString))]
    public static bool ToHudStringPrefix(GameOptionsData __instance, int numPlayers, out string __result)
    {
        AppendVanilla(__instance, numPlayers);
        
		__result =  __instance.settings.ToString();
		return false;
    }
    
    private static void AppendRoles(GameOptionsData __instance) {
	    
    }

    private static void AppendVanilla(GameOptionsData __instance, int numPlayers)
    {
	    numPlayers = Mathf.Clamp(numPlayers, 0, GameOptionsData.MaxImpostors.Length);
	    __instance.settings.Length = 0;
	    
	    try
	    {
		    var num = GameOptionsData.MaxImpostors[numPlayers];
		    var num2 = __instance.MapId == 0 && Constants.ShouldFlipSkeld() ? 3 : __instance.MapId;
		    var value = Constants.MapNames[num2];

		    __instance.AppendItem(__instance.settings, StringNames.GameMapName, value);
		    __instance.settings.Append($"{GetString(StringNames.GameNumImpostors)}: {__instance.NumImpostors}");

		    if (__instance.NumImpostors > num)
		    {
			    __instance.settings.Append($" ({GetString(StringNames.Limit)}: {num})");
		    }

		    __instance.settings.AppendLine();
		    __instance.AppendItem(__instance.settings, $"Reveal On Exile: {PlixOptions.ExileRevealType}");
		    __instance.AppendItem(__instance.settings, StringNames.GameNumMeetings, __instance.NumEmergencyMeetings);
		    __instance.AppendItem(__instance.settings, StringNames.GameAnonymousVotes, __instance.AnonymousVotes);
		    __instance.AppendItem(__instance.settings, StringNames.GameEmergencyCooldown, GetString(StringNames.GameSecondsAbbrev, __instance.EmergencyCooldown));
		    __instance.AppendItem(__instance.settings, StringNames.GameDiscussTime, GetString(StringNames.GameSecondsAbbrev, __instance.DiscussionTime));

		    __instance.AppendItem(__instance.settings, StringNames.GameVotingTime,
			    __instance.VotingTime > 0
				    ? GetString(StringNames.GameSecondsAbbrev, __instance.VotingTime)
				    : GetString(StringNames.GameSecondsAbbrev, "∞"));

		    __instance.AppendItem(__instance.settings, StringNames.GamePlayerSpeed, __instance.PlayerSpeedMod, "x");
		    __instance.AppendItem(__instance.settings, StringNames.GameCrewLight, __instance.CrewLightMod, "x");
		    __instance.AppendItem(__instance.settings, StringNames.GameImpostorLight, __instance.ImpostorLightMod, "x");
		    __instance.AppendItem(__instance.settings, StringNames.GameKillCooldown, GetString(StringNames.GameSecondsAbbrev, __instance.KillCooldown));
		    __instance.AppendItem(__instance.settings, StringNames.GameKillDistance, GetString(StringNames.SettingShort + __instance.KillDistance));
		    __instance.AppendItem(__instance.settings, StringNames.GameTaskBarMode, GetString(StringNames.SettingNormalTaskMode + (int) __instance.TaskBarMode));
		    __instance.AppendItem(__instance.settings, StringNames.GameVisualTasks, __instance.VisualTasks);
		    __instance.AppendItem(__instance.settings, StringNames.GameCommonTasks, __instance.NumCommonTasks);
		    __instance.AppendItem(__instance.settings, StringNames.GameLongTasks, __instance.NumLongTasks);
		    __instance.AppendItem(__instance.settings, StringNames.GameShortTasks, __instance.NumShortTasks);

	    }
	    catch
	    {
		    // ignored
	    }
    }

    private static string GetString(StringNames stringName)
    {
	    return GetString(stringName, Array.Empty<object>());
    }
    
    private static string GetString(StringNames stringName, params object[] objs)
    {
	    return string.Format(DestroyableSingleton<TranslationController>.Instance
		    .GetString(stringName, Array.Empty<Il2CppSystem.Object>()), objs);
    }
}