using System.Collections.Generic;
using System.Linq;
using Reactor;
using Reactor.Networking;
using Reactor.Networking.MethodRpc;
using Tipplix.Networking;

namespace Tipplix.CustomGameOver;

public static class CustomGameOverManager
{
    private static List<CustomGameOverReason> _allReasons { get; set; } = new();
    public static IReadOnlyCollection<CustomGameOverReason> AllReasons => _allReasons.AsReadOnly();
    public static List<WinningPlayerData> Winners { get; set; } = new();
    
    private static int _current = 7;
    public static void Register(CustomGameOverReason reason)
    {
        _allReasons.Add(reason);
        reason.Id = _current++;
    }

    public static CustomGameOverReason? GetReasonOrDefault(GameOverReason reason)
    {
        return AllReasons.FirstOrDefault(x => x.Id == (int) reason);
    }

    public static void RpcEndGame<T>(IEnumerable<GameData.PlayerInfo> winners) where T : CustomGameOverReason
    {
        var reason = AllReasons.OfType<T>().Single();
        if (PlayerControl.LocalPlayer)
        {
            Rpc<RpcCustomEndReason>.Instance.Send((winners.Select(x => x.PlayerId).ToArray(), (byte) reason.Id));
        }
    }
}