using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using UnboundLib.GameModes;
using UnityEngine;
using Jotunn.Utils;
using LobbyImprovements.Networking;
using LobbyImprovements.UI;
using UnboundLib.Utils.UI;
using UnboundLib;
using UnboundLib.Networking;
using Photon.Pun;
using HarmonyLib;

namespace LobbyImprovements
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class LobbyImprovements : BaseUnityPlugin
    {
        private const string ModId = "com.roundsmoddingcommunity.rounds.LobbyImprovements";
        private const string ModName = "Lobby Improvements";
        private static readonly string CompatibilityModName = ModName.Replace(" ","");
        public const string Version = "1.0.0"; // What version are we on (major.minor.patch)?

        internal GameObject hostOnlyConfigToggle;

        internal AssetBundle assets = null;

        public List<AudioClip> click;
        public List<AudioClip> hover;

        private static Harmony harmony;
        public static LobbyImprovements instance { get; private set; }

#if DEBUG
        internal static bool DEBUG = true;
#else
        internal static bool DEBUG = false;
#endif

        internal static void Log(object message)
        {
            if (DEBUG) { UnityEngine.Debug.Log($"[{ModName}] {message}"); }
        }

        void Awake()
        {
            harmony = new Harmony(ModId);
            harmony.PatchAll();
        }
        void Start()
        {
            instance = this;

            this.gameObject.AddComponent<LobbyMonitor>();

            assets = AssetUtils.LoadAssetBundleFromResources("lobbycodes", typeof(LobbyImprovements).Assembly);
            click = assets.LoadAllAssets<AudioClip>().ToList().Where(clip => clip.name.Contains("UI_Button_Click")).ToList();
            hover = assets.LoadAllAssets<AudioClip>().ToList().Where(clip => clip.name.Contains("UI_Button_Hover")).ToList();

            JoinUI.Init();

            Unbound.RegisterClientSideMod(ModId);

            Unbound.RegisterCredits(ModName, new string[] {"willuwontu (Project Creation, UI)", "Pykess (Backend)" }, new string[] {"Support willuwontu", "Support Pykess" }, new string[] {"https://ko-fi.com/willuwontu", "https://ko-fi.com/pykess" });

            Unbound.RegisterMenu(ModName, () => { }, GUI, null, false);

            GameModeManager.AddHook(GameModeHooks.HookGameStart, GameStart);
        }

        public static string GetConfigKey(string key) => $"{LobbyImprovements.CompatibilityModName}_{key}";

        public static bool StreamerMode
        {
            get
            {
                return PlayerPrefs.GetInt(GetConfigKey("StreamerMode"), 0) == 1;
            }
            internal set
            {
                PlayerPrefs.SetInt(GetConfigKey("StreamerMode"), value ? 1 : 0);
            }
        }
        public static bool OnlyHostCanInvite
        {
            get
            {
                return PlayerPrefs.GetInt(GetConfigKey("OnlyHostCanInvite"), 0) == 1;
            }
            internal set
            {
                PlayerPrefs.SetInt(GetConfigKey("OnlyHostCanInvite"), value ? 1 : 0);
            }
        }

        private IEnumerator GameStart(IGameModeHandler gm)
        {
            LobbyUI.BG.SetActive(false);

            yield break;
        }

        private void GUI(GameObject menu)
        {
            MenuHandler.CreateText(ModName, menu, out var _);
            MenuHandler.CreateText(" ", menu, out var _, 30);
            MenuHandler.CreateToggle(StreamerMode, "Enable Streamer Mode", menu, (bool val) => { StreamerMode = val; JoinUI.UpdateStreamerModeSettings(); LobbyUI.UpdateStreamerModeSettings(); });
            MenuHandler.CreateText(" ", menu, out var _, 30);
            hostOnlyConfigToggle = MenuHandler.CreateToggle(OnlyHostCanInvite, "Only host can invite", menu, (bool val) => { OnlyHostCanInvite = val; });
        }

        [UnboundRPC]
        internal static void RPCS_Kick(int actorID)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber != actorID) { return; }
            Unbound.Instance.StartCoroutine((IEnumerator)NetworkConnectionHandler.instance.InvokeMethod("DoDisconnect", "KICKED", "YOU HAVE BEEN KICKED FROM THE LOBBY"));
        }
    }
}
