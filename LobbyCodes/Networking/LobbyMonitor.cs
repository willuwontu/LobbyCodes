using Photon.Pun;
using UnboundLib;
using LobbyCodes.UI;
using LobbyCodes.Networking;

namespace LobbyCodes.Networking
{
    class LobbyMonitor : MonoBehaviourPunCallbacks
    {
        public static LobbyMonitor instance {get; private set;}

        private void Start()
        {
            instance = this;
        }

        public override void OnJoinedRoom()
        {
            LobbyUI.BG.SetActive(false);
            // If we're offline, we don't care whether codes are visible or the room we join
            if (!PhotonNetwork.OfflineMode)
            {
                // If our room is not private, we're entering an unmodded room.
                if (PhotonNetwork.CurrentRoom.IsVisible)
                {
                    // See if we're the master client
                    if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        // Leave room if we're not
                        PhotonNetwork.LeaveRoom();
                        return;
                    }
                    else
                    {
                        // Remove room visibility if we are
                        PhotonNetwork.CurrentRoom.IsVisible = false;
                    }
                }

                LobbyUI.UpdateStreamerModeSettings();
                LobbyUI.BG.SetActive(true);
                LobbyUI.UpdateLobbyCode(LobbyCodeHandler.GetCode());
                LobbyCodes.instance.ExecuteAfterSeconds(5f, () => LobbyUI.BG.transform.SetAsLastSibling());
            }
        }

        public override void OnLeftRoom()
        {
            LobbyUI.BG.SetActive(false);
        }
    }
}
