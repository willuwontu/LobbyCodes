using Photon.Pun;
using UnboundLib;
using LobbyCodes.UI;
using LobbyCodes.Networking;

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
            LobbyUI.UpdateLobbyCode(LobbyCodeHandler.GetCode());
            LobbyCodes.instance.ExecuteAfterSeconds(5f, () => LobbyUI.BG.transform.SetAsLastSibling());
        }

        public override void OnLeftRoom()
        {
            LobbyUI.BG.SetActive(false);
        }
    }
}
