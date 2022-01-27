using UnboundLib.Utils.UI;
using UnityEngine;
using UnityEngine.UI;
using LobbyCodes.Networking;
using UnityEngine.SceneManagement;
using UnboundLib;
using TMPro;

namespace LobbyCodes.UI
{
    static class JoinUI
    {
        static string currentCode = "";
        internal static TextMeshProUGUI StreamerModeText;
        static void SetupUI(bool firstTime)
        {
            Unbound.Instance.ExecuteAfterSeconds(firstTime ? 0.2f : 0f, () =>
            {
                var onlineGo = GameObject.Find("/Game/UI/UI_MainMenu/Canvas/ListSelector/Online");
                var spaceGo = onlineGo?.transform?.Find("Space")?.transform;
                var inviteGo = onlineGo?.transform?.Find("Invite friend")?.transform;
                int siblingIndex = spaceGo != null ? spaceGo.GetSiblingIndex() + 1 : inviteGo != null ? inviteGo.GetSiblingIndex() : 4;
                var joinMenu = MenuHandler.CreateMenu("JOIN LOBBY", () => { }, onlineGo.gameObject, 60, true, false, null, true, siblingIndex);
                MenuHandler.CreateText("ENTER LOBBY CODE", joinMenu, out var _);
                MenuHandler.CreateText(LobbyCodes.StreamerMode ? "STREAMER MODE ENABLED" : "", joinMenu, out StreamerModeText, 30, color: new Color32(145, 70, 255, 255));
                StreamerModeText.fontStyle = FontStyles.Bold;
                MenuHandler.CreateText(" ", joinMenu, out TextMeshProUGUI status, 30, color: Color.red);
                MenuHandler.CreateText(" ", joinMenu, out TextMeshProUGUI _, 30);
                var inputField = MenuHandler.CreateInputField("LOBBY CODE", 60, joinMenu, (string str) => JoinUI.currentCode = str);
                inputField.transform.localScale = 2f * Vector3.one;
                foreach (TMP_Text text in inputField.GetComponentsInChildren<TMP_Text>())
                {
                    text.alignment = TextAlignmentOptions.Center;
                }
                MenuHandler.CreateText(" ", joinMenu, out TextMeshProUGUI _, 30);
                void DoConnect()
                {
                    LobbyCodeHandler.ExitCode exit = LobbyCodeHandler.ConnectToRoom(JoinUI.currentCode);
                    switch (exit)
                    {
                        case LobbyCodeHandler.ExitCode.Success:
                            status.text = " ";
                            break;
                        case LobbyCodeHandler.ExitCode.Invalid:
                            status.text = "INVALID CODE";
                            break;
                        case LobbyCodeHandler.ExitCode.UnknownError:
                            status.text = "UNKNOWN ERROR";
                            break;
                        case LobbyCodeHandler.ExitCode.Empty:
                            status.text = " ";
                            break;
                        default:
                            status.text = " ";
                            break;
                    }
                }
                MenuHandler.CreateButton("JOIN", joinMenu, DoConnect, 60);
            });
        }

        internal static void Init()
        {
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                if (scene.name == "Main")
                {
                    SetupUI(false);
                }
            };
            SetupUI(true);
        }
    }
}
