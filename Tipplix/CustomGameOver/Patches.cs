using System.Linq;
using HarmonyLib;
using Tipplix.Enums;
using UnityEngine;
using static UnityEngine.Object;
using static Tipplix.CustomGameOver.CustomGameOverManager;

namespace Tipplix.CustomGameOver;

[HarmonyPatch(typeof(EndGameManager))]
public static class Patches
{
    private static readonly int Color = Shader.PropertyToID("_Color");

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    private static void AmongUsClient_OnGameEndPrefix(EndGameResult endGameResult)
    {
        TempData.EndReason = endGameResult.GameOverReason;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(EndGameManager.SetEverythingUp))]
    public static bool SetEverythingUpPrefix(EndGameManager __instance)
    {
        var reason = GetReasonOrDefault(TempData.EndReason);
        if (reason is null) return true;
        
        reason.YouWon = Winners.Any(x => x.IsYou);
        
        __instance.WinText.SetText(reason.WinText);
        __instance.WinText.color = reason.Color;
        __instance.BackgroundBar.material.SetColor(Color, reason.BackgroundColor);

        if (!string.IsNullOrWhiteSpace(reason.DescriptionText))
        {
            var desc = Instantiate(__instance.WinText, __instance.WinText.transform.parent);
            desc.name = nameof(reason.GetType) + "Desc";
            desc.SetText(reason.DescriptionText);
            desc.fontSizeMin = desc.fontSizeMax = desc.fontSize *= 0.3f;
            desc.color = reason.DescColor;
        }
        
        SoundManager.Instance.PlayDynamicSound("Stinger", reason.Stinger switch 
        {
            EndGameStingers.Default => reason.CustomStinger,
            EndGameStingers.CrewStinger => __instance.CrewStinger,
            EndGameStingers.DisconnectStinger => __instance.DisconnectStinger,
            EndGameStingers.ImpostorStinger => __instance.ImpostorStinger,
            _ => __instance.DisconnectStinger
        }, 
            false, 
            (DynamicSound.GetDynamicsFunction) __instance.GetStingerVol);

        var list = Winners.OrderBy(b => b.IsYou ? -1 : 0).ToArray();
        var num = Mathf.CeilToInt(7.5f);
        
        for (var i = 0; i < list.Length; i++)
        {
            var winningPlayerData2 = list[i];
            var num2 = i % 2 == 0 ? -1 : 1;
            var num3 = (i + 1) / 2;
            var num4 = num3 / (float) num;
            var num5 = Mathf.Lerp(1f, 0.75f, num4);
            var num6 = (float) (i == 0 ? -8 : -1);
            
            var poolablePlayer = Instantiate(__instance.PlayerPrefab, __instance.transform);
            poolablePlayer.transform.localPosition = new Vector3(
                1f * num2 * num3 * num5, 
                FloatRange.SpreadToEdges(-1.125f, 0f, num3, num),
                num6 + num3 * 0.01f) * 0.9f;
            
            var num7 = Mathf.Lerp(1f, 0.65f, num4) * 0.9f;
            var vector = new Vector3(num7, num7, 1f);
            poolablePlayer.transform.localScale = vector;
            poolablePlayer.UpdateFromPlayerOutfit(winningPlayerData2, winningPlayerData2.IsDead);
            
            if (winningPlayerData2.IsDead)
            {
                poolablePlayer.Body.sprite = __instance.GhostSprite;
                poolablePlayer.SetDeadFlipX(i % 2 == 0);
            }
            else
            {
                poolablePlayer.SetFlipX(i % 2 == 0);
            }
            
            poolablePlayer.NameText.text = winningPlayerData2.PlayerName;
            poolablePlayer.NameText.transform.position += Vector3.down / 2;
            poolablePlayer.NameText.gameObject.SetActive(reason.ShowName);
        }
        
        return false;
    }
}