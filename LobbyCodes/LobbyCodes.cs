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

namespace LobbyCodes
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class LobbyCodes : BaseUnityPlugin
    {
        private const string ModId = "com.roundsmoddingcommunity.rounds.LobbyCodes";
        private const string ModName = "Lobby Codes";
        private readonly string CompatibilityModName = ModName.Replace(" ","");
        public const string Version = "1.0.0"; // What version are we on (major.minor.patch)?

        internal AssetBundle assets = null;

        public List<AudioClip> click;
        public List<AudioClip> hover;

        public static LobbyCodes instance { get; private set; }

        void Start()
        {
            instance = this;

            this.gameObject.AddComponent<LobbyMonitor>();

            assets = AssetUtils.LoadAssetBundleFromResources("lobbycodes", typeof(LobbyCodes).Assembly);
            click = assets.LoadAllAssets<AudioClip>().ToList().Where(clip => clip.name.Contains("UI_Button_Click")).ToList();
            hover = assets.LoadAllAssets<AudioClip>().ToList().Where(clip => clip.name.Contains("UI_Button_Hover")).ToList();

            JoinUI.Init();

            GameModeManager.AddHook(GameModeHooks.HookGameStart, GameStart);
        }

        private IEnumerator GameStart(IGameModeHandler gm)
        {
            LobbyUI.BG.SetActive(false);

            yield break;
        }
    }
}
