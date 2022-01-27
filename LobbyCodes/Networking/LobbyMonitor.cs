using System;
using System.Collections.Generic;
using Photon.Pun;
using UnboundLib;
using UnboundLib.Networking;
using ExitGames.Client.Photon;
using LobbyCodes.UI;

namespace LobbyCodes.Networking
{
    class LobbyMonitor : MonoBehaviourPunCallbacks
    {
        private void Start()
        {

        }

        public override void OnJoinedRoom()
        {
            LobbyUI.BG.SetActive(true);
            LobbyUI.UpdateLobbyCode("Code");
            LobbyCodes.instance.ExecuteAfterSeconds(5f, () => LobbyUI.BG.transform.SetAsLastSibling());
        }

        public override void OnLeftRoom()
        {
            LobbyUI.BG.SetActive(false);
        }
    }
}
