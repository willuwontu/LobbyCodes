using System.Linq;
using Photon.Pun;
using UnboundLib;
using LobbyCodes.UI;
using LobbyCodes.Networking;

namespace LobbyCodes.Networking
{
    public class LobbyMonitor : MonoBehaviourPunCallbacks
    {
        public static LobbyMonitor instance {get; private set;}
        public const string hostOnlyPropKey = "LobbyCodes-OnlyHost";

        private void Awake()
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
                        LobbyCodes.instance.ExecuteAfterFrames(1, () => PhotonNetwork.LeaveRoom());
                        return;
                    }
                    else
                    {
                        // Remove room visibility if we are
                        PhotonNetwork.CurrentRoom.IsVisible = false;
                    }
                }

                // Force necessary UI into existance.
                var _ = LobbyUI.hostOnlyToggle;
                _ = LobbyUI.kickContainer;

                LobbyUI.UpdateStreamerModeSettings();
                LobbyUI.BG.SetActive(true);
                LobbyUI.UpdateLobbyCode(LobbyCodeHandler.GetCode());
            }
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            if (!newPlayer.WasInvitedByHost() && PhotonNetwork.MasterClient.OnlyHostCanInvite())
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    this.ForceKickPlayer(newPlayer);
                }
            }
            LobbyUI.UpdateKickList(PhotonNetwork.CurrentRoom.Players.Values.Where(p => !p.IsMasterClient).ToArray());
        }
        public override void OnPlayerLeftRoom(Photon.Realtime.Player newPlayer)
        {
            LobbyUI.UpdateKickList(PhotonNetwork.CurrentRoom.Players.Values.Where(p => !p.IsMasterClient).ToArray());
        }

        public void ForceKickPlayer(Photon.Realtime.Player player)
        {
            this.StartCoroutine(this.IForceKickPlayer(player));
        }
        private IEnumerator IForceKickPlayer(Photon.Realtime.Player player)
        {
            while (PhotonNetwork.CurrentRoom.Players.Values.Contains(player))
            {
                PhotonNetwork.CloseConnection(player);
                yield return new WaitForSecondsRealtime(0.5f);
            }
            yield break;
        }

        public override void OnLeftRoom()
        {
            LobbyUI.UpdateKickList(new Photon.Realtime.Player[] { });
            LobbyUI.BG.SetActive(false);
        }
    }
}
