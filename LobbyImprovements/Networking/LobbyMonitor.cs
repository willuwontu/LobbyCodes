using System.Linq;
using Photon.Pun;
using UnboundLib;
using LobbyImprovements.UI;
using LobbyImprovements.Networking;
using ExitGames.Client.Photon;
using LobbyImprovements.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnboundLib.Networking;

namespace LobbyImprovements.Networking
{
    public class LobbyMonitor : MonoBehaviourPunCallbacks
    {
        public static LobbyMonitor instance {get; private set;}
        private List<string> kickedUserIDs = new List<string>() { };

        private void Awake()
        {
            instance = this;
        }

        public override void OnCreatedRoom()
        {
            if (PhotonNetwork.IsMasterClient)
            {
            }
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
                        LobbyImprovements.instance.ExecuteAfterFrames(1, () => PhotonNetwork.LeaveRoom());
                        LobbyImprovements.instance.ExecuteAfterFrames(3, () => LobbyUI.BG.SetActive(false));
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
                LobbyUI.CodesContainer.SetActive(true);
                LobbyUI.hostOnlyToggle.GetComponent<UnityEngine.UI.Toggle>().interactable = PhotonNetwork.LocalPlayer.IsMasterClient;

                // Check to see if host only is enabled.

                if (PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    LobbyUI.hostOnlyToggle.GetComponent<UnityEngine.UI.Toggle>().interactable = true;
                    LobbyUI.kickContainer.SetActive(true);
                    PhotonNetwork.LocalPlayer.SetOnlyHostCanInvite(LobbyImprovements.OnlyHostCanInvite);
                }
                else
                {
                    LobbyUI.hostOnlyToggle.GetComponent<UnityEngine.UI.Toggle>().interactable = false;
                    LobbyUI.kickContainer.SetActive(false);
                    LobbyUI.CodesContainer.SetActive(!PhotonNetwork.MasterClient.OnlyHostCanInvite());
                }

                // clear the kickedUserIDs
                this.kickedUserIDs = new List<string>() { };

                LobbyUI.BG.SetActive(true);
                LobbyUI.UpdateLobbyCode(LobbyCodeHandler.GetCode());
                this.ExecuteAfterSeconds(1f, () =>
                {
                    LobbyUI.UpdateKickList(PhotonNetwork.CurrentRoom.Players.Values.Where(player => player != PhotonNetwork.MasterClient).ToArray());
                });
            }
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            if ((!newPlayer.WasInvitedByHost() && PhotonNetwork.MasterClient.OnlyHostCanInvite()) || this.kickedUserIDs.Contains(newPlayer.UserId))
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    this.ForceKickPlayer(newPlayer, false);
                }
            }
            this.ExecuteAfterSeconds(1f, () =>
            {
                LobbyUI.UpdateKickList(PhotonNetwork.CurrentRoom.Players.Values.Where(player => player != PhotonNetwork.MasterClient).ToArray());
            });
        }
        public override void OnPlayerLeftRoom(Photon.Realtime.Player newPlayer)
        {
            LobbyUI.UpdateKickList(PhotonNetwork.CurrentRoom.Players.Values.Where(p => !p.IsMasterClient).ToArray());
        }

        public void ForceKickPlayer(Photon.Realtime.Player player, bool addToKickedUsers)
        {
            if (addToKickedUsers && !this.kickedUserIDs.Contains(player.UserId))
            {
                this.kickedUserIDs.Add(player.UserId);
            }
            this.StartCoroutine(this.IForceKickPlayer(player));
        }
        private IEnumerator IForceKickPlayer(Photon.Realtime.Player player)
        {
            while (PhotonNetwork.CurrentRoom.Players.Values.Contains(player))
            {
                NetworkingManager.RPC(typeof(LobbyImprovements), nameof(LobbyImprovements.RPCS_Kick), new Photon.Realtime.RaiseEventOptions { TargetActors = new int[] { player.ActorNumber } }, player.ActorNumber);
                NetworkingManager.RPC(typeof(Unbound), nameof(Unbound.BuildInfoPopup), new Photon.Realtime.RaiseEventOptions { TargetActors = new int[] { player.ActorNumber } }, "You have been kicked from the lobby.");
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

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            // look to see if the host's host only invite properties have changed.
            if (targetPlayer.IsMasterClient && !PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                LobbyUI.hostOnlyToggle.GetComponent<UnityEngine.UI.Toggle>().isOn = (bool) PhotonNetwork.MasterClient.OnlyHostCanInvite();

                LobbyUI.CodesContainer.SetActive(!(bool)PhotonNetwork.MasterClient.OnlyHostCanInvite());
            }

        }
    }
}
