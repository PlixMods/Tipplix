using System;
using System.Linq;
using TMPro;
using UnhollowerBaseLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tipplix.Extensions
{
    public static class PlayerControlExtensions
    {
        public static string GetPlayerVanillaRoles(this PlayerControl player, bool showTasks = false, bool visible = true)
        {
            if (!visible) return string.Empty;
            
            if (player.Data.Role.IsImpostor)
                return "Impostor".WithColor(Palette.ImpostorRed);

            var (completedTasks, allTasks) = player.GetPlayerTasks();
            return showTasks
                ? $"Tasks ({completedTasks}/{allTasks})".WithColor(Color.green)
                : "Crewmate";
        }

        public static (int, int) GetPlayerTasks(this PlayerControl player)
        {
            var allTasks = player.Data.Tasks.Count;
            var completedTasks = player.Data.Tasks.ToArray().Count(x => x.Complete);

            return (completedTasks, allTasks);
        }

        public static void DefineRole(this PlayerControl player, string info)
        {
            player.DefineName(info);
            player.DefineVoteName(info);
        }

        public static void DefineName(this PlayerControl player, string info)
        {
            var playerInfoTransform = player.nameText.transform.parent.FindChild("Info");
            var playerInfo = playerInfoTransform?.GetComponent<TextMeshPro>();

            if (!playerInfo)
            {
                playerInfo = Object.Instantiate(player.nameText, player.nameText.transform.parent);
                playerInfo.transform.localPosition += Vector3.up * 0.5f;
                playerInfo.gameObject.name = "Info";
                playerInfo.fontSize *= 0.75f;
            }

            playerInfo.text = info;
            playerInfo.gameObject.SetActive(player.Visible);
        }

        public static void DefineVoteName(this PlayerControl player, string info)
        {
            var playerVoteArea = MeetingHud.Instance?.playerStates?
                .FirstOrDefault(x => x.TargetPlayerId == player.PlayerId);

            if (!playerVoteArea) return;
            info = MeetingHud.Instance.state != MeetingHud.VoteStates.Results ? info : string.Empty;
            info = $"<size=1.6>{info}</size>";
            playerVoteArea.NameText.text = $"{player.nameText.text}\n{info}";
            playerVoteArea.NameText.color = player.nameText.color;
        }
        
        public static void AddTasksToTab(this PlayerControl player)
        {
            if (player != PlayerControl.LocalPlayer || player.Data.Role.IsImpostor ||
                !DestroyableSingleton<TaskPanelBehaviour>.InstanceExists) return;

            var (completed, allTasks) = player.GetPlayerTasks();
            var tabText = DestroyableSingleton<TaskPanelBehaviour>.Instance.tab.transform
                .FindChild("TabText_TMP").GetComponent<TextMeshPro>();

            tabText.SetText($"Tasks <color=green>({completed}/{allTasks})</color>");
        }
    }
}