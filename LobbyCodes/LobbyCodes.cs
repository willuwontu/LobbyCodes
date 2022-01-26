using System;
using System.Reflection;
using BepInEx;
using UnboundLib;
using UnityEngine;
using UnityEngine.UI;
using Jotunn.Utils;

namespace LobbyCodes
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class LobbyCodes : BaseUnityPlugin
    {
        private const string ModId = "com.roundsmoddingcommunity.rounds.LobbyCodes";
        private const string ModName = "Lobby Codes";
        public const string Version = "1.0.0"; // What version are we on (major.minor.patch)?

        AssetBundle assets;

        public static LobbyCodes instance { get; private set; }

        void Start()
        {
            instance = this;

            assets = AssetUtils.LoadAssetBundleFromResources("lobbycodes", typeof(LobbyCodes).Assembly);
        }
    }
}
