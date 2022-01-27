using Photon.Pun;
using UnboundLib;
using LobbyCodes.UI;
using LobbyCodes.Networking;

namespace LobbyCodes.Networking
{
    class LobbyMonitor : MonoBehaviourPunCallbacks
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

                // Check to see if host only is enabled.

                ExitGames.Client.Photon.Hashtable customProperties;
                if (PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    // Get the current custom properties of the local photon player object.
                    customProperties = PhotonNetwork.LocalPlayer.CustomProperties;

                    // Record the ping, we don't care if we override anything.
                    customProperties[hostOnlyPropKey] = LobbyCodes.instance.onlyHostCanInviteConfig.Value;

                    // Send out the update to their properties.
                    PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties, null, null);
                }
                else
                {
                    customProperties = PhotonNetwork.MasterClient.CustomProperties;

                    if (customProperties.TryGetValue(hostOnlyPropKey, out var prop))
                    {
                        if ((bool)prop)
                        {
                            return;
                        }
                    }
                }

                LobbyUI.BG.SetActive(true);
                LobbyUI.UpdateLobbyCode(LobbyCodeHandler.GetCode());
            }
        }

        public override void OnLeftRoom()
        {
            LobbyUI.BG.SetActive(false);
        }
    }
}
