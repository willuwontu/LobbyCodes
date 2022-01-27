using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using LobbyCodes.Utils;
using UnboundLib;
using System;
using System.Text;

namespace LobbyCodes.Networking
{
    public static class LobbyCodeHandler
    {
        private static string GetPureCode()
        {
            if (PhotonNetwork.OfflineMode || PhotonNetwork.CurrentRoom == null) { return ""; }
            return $"{PhotonNetwork.CloudRegion}:{PhotonNetwork.CurrentRoom.Name}";
        }
        public static string GetCode()
        {
            return ObfuscateJoinCode.Obfuscate(GetPureCode());
        }
        private static void PureConnectToRoom(string pureCode)
        {
            string[] reg_room = pureCode.Split(':');
            string region = reg_room[0];
            string room = reg_room[1];
            NetworkConnectionHandler.instance.ForceRegionJoin(region, room);
        }
        public static void ConnectToRoom(string obfuscatedCode)
        {
            PureConnectToRoom(ObfuscateJoinCode.DeObfuscate(obfuscatedCode));
        }

    }
}
