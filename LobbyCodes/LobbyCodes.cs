using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using UnboundLib.GameModes;
using UnityEngine;
using Jotunn.Utils;
using LobbyCodes.Networking;
using LobbyCodes.UI;
using UnboundLib.Utils.UI;
using UnboundLib;

namespace LobbyCodes
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class LobbyCodes : BaseUnityPlugin
    {
        private const string ModId = "com.roundsmoddingcommunity.rounds.LobbyCodes";
        private const string ModName = "Lobby Codes";
        private static readonly string CompatibilityModName = ModName.Replace(" ","");
        public const string Version = "1.0.0"; // What version are we on (major.minor.patch)?

        internal AssetBundle assets = null;

        public List<AudioClip> click;
        public List<AudioClip> hover;

        public static LobbyCodes instance { get; private set; }

#if DEBUG
        internal static bool DEBUG = true;
#else
        internal static bool DEBUG = false;
#endif

        internal static void Log(object message)
        {
            if (DEBUG) { UnityEngine.Debug.Log($"[{ModName}] {message}"); }
        }

        void Start()
        {
            instance = this;

            this.gameObject.AddComponent<LobbyMonitor>();

            assets = AssetUtils.LoadAssetBundleFromResources("lobbycodes", typeof(LobbyCodes).Assembly);
            click = assets.LoadAllAssets<AudioClip>().ToList().Where(clip => clip.name.Contains("UI_Button_Click")).ToList();
            hover = assets.LoadAllAssets<AudioClip>().ToList().Where(clip => clip.name.Contains("UI_Button_Hover")).ToList();

            JoinUI.Init();

            Unbound.RegisterMenu(ModName, () => { }, GUI, null, false);

            GameModeManager.AddHook(GameModeHooks.HookGameStart, GameStart);
        }

        public static string GetConfigKey(string key) => $"{LobbyCodes.CompatibilityModName}_{key}";

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

        private IEnumerator GameStart(IGameModeHandler gm)
        {
            LobbyUI.BG.SetActive(false);

            yield break;
        }

        private void GUI(GameObject menu)
        {
            MenuHandler.CreateText(ModName, menu, out var _);
            MenuHandler.CreateText(" ", menu, out var _, 30);
            MenuHandler.CreateToggle(StreamerMode, "Enable Streamer Mode", menu, (bool val) => { StreamerMode = val; JoinUI.StreamerModeText.text = val ? "STREAMER MODE ENABLED" : ""; });
        }
    }
}
