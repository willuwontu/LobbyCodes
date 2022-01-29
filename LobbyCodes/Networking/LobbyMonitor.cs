using System.Linq;
using Photon.Pun;
using UnboundLib;
using LobbyCodes.UI;
using LobbyCodes.Networking;
using ExitGames.Client.Photon;

namespace LobbyCodes.Networking
{
    public class LobbyMonitor : MonoBehaviourPunCallbacks
    {
        public static LobbyMonitor instance {get; private set;}
        public const string hostOnlyPropKey = "LobbyCodes-OnlyHost";

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
                        LobbyCodes.instance.ExecuteAfterFrames(1, () => PhotonNetwork.LeaveRoom());
                        return;
                    }
                    else
                    {
                        // Remove room visibility if we are
                        PhotonNetwork.CurrentRoom.IsVisible = false;
                    }
                }

                LobbyUI.UpdateStreamerModeSettings();
                LobbyUI.CodesContainer.SetActive(true);

                // Force necessary UI into existance.
                var _ = LobbyUI.hostOnlyToggle;
                _ = LobbyUI.kickContainer;

                // Check to see if host only is enabled.

                ExitGames.Client.Photon.Hashtable customProperties;
                if (PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    LobbyUI.hostOnlyToggle.GetComponent<UnityEngine.UI.Toggle>().interactable = true;
                    LobbyUI.kickContainer.SetActive(true);
                    // Get the current custom properties of the local photon player object.
                    customProperties = PhotonNetwork.LocalPlayer.CustomProperties;

                    // Record the ping, we don't care if we override anything.
                    customProperties[hostOnlyPropKey] = LobbyCodes.instance.onlyHostCanInviteConfig.Value;

                    // Send out the update to their properties.
                    PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties, null, null);
                }
                else
                {
                    LobbyUI.hostOnlyToggle.GetComponent<UnityEngine.UI.Toggle>().interactable = true;
                    LobbyUI.kickContainer.SetActive(false);
                    customProperties = PhotonNetwork.MasterClient.CustomProperties;

                    if (customProperties.TryGetValue(hostOnlyPropKey, out var prop))
                    {
                        if ((bool)prop)
                        {
                            LobbyUI.CodesContainer.SetActive(false);
                        }
                    }
                }

                LobbyUI.BG.SetActive(true);
                LobbyUI.UpdateLobbyCode(LobbyCodeHandler.GetCode());
                LobbyUI.UpdateKickList(PhotonNetwork.CurrentRoom.Players.Values.Where((player) => player != PhotonNetwork.MasterClient).ToArray());
            }
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            LobbyUI.UpdateKickList(PhotonNetwork.CurrentRoom.Players.Values.ToArray());
        }

        public override void OnLeftRoom()
        {
            LobbyUI.UpdateKickList(new Photon.Realtime.Player[] { });
            LobbyUI.BG.SetActive(false);
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            // look to see if the host's host only invite properties have changed.
            if (targetPlayer.IsMasterClient && !PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                if (changedProps.TryGetValue(hostOnlyPropKey, out var hostOnlyState))
                {
                    LobbyUI.hostOnlyToggle.GetComponent<UnityEngine.UI.Toggle>().isOn = (bool) hostOnlyState;

                    LobbyUI.CodesContainer.SetActive(!(bool)hostOnlyState);
                }
            }
        }
    }
}
