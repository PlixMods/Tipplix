using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using Reactor;
using Reactor.Networking;
using Tipplix.Enums;
using UnityEngine;

namespace Tipplix.Networking
{
    [RegisterCustomRpc((uint) RpcEnum.CustomEndGame)]
    public class RpcCustomEndReason : PlayerCustomRpc<TipplixPlugin, (byte[], byte)>
    {
        public RpcCustomEndReason(TipplixPlugin plugin, uint id) : base(plugin, id) { }

        public override RpcLocalHandling LocalHandling => RpcLocalHandling.After;

        public override void Write(MessageWriter writer, (byte[], byte) data)
        {
            if (data.Item1 == null) throw new ArgumentNullException(nameof(data));
            
            writer.Write(data.Item2);
            writer.Write(data.Item1.Length);
            writer.Write(data.Item1);
        }

        public override (byte[], byte) Read(MessageReader reader)
        {
            return (reader.ReadBytes(reader.ReadInt32()), reader.ReadByte());
        }

        public override void Handle(PlayerControl innerNetObject, (byte[], byte) data)
        {
            if (data.Item1 == null) throw new ArgumentNullException(nameof(data));
            
            CustomGameOver.CustomGameOverManager.Winners = data.Item1
                .Select(x => GameData.Instance.GetPlayerById(x))
                .Select(x => new WinningPlayerData(x)).ToList();
            
            if (AmongUsClient.Instance.AmHost)
            {
                ShipStatus.RpcEndGame((GameOverReason) data.Item2, !SaveManager.BoughtNoAds);
            }
        }
    }
}