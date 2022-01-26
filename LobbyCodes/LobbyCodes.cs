using System;
using System.Reflection;
using BepInEx;
using UnboundLib;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LobbyCodes
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class LobbyCodes : BaseUnityPlugin
    {
        private const string ModId = "com.willuwontu.rounds.MapObjectsEditor";
        private const string ModName = "Will's Wacky Map Objects Editor";
        public const string Version = "1.0.0"; // What version are we on (major.minor.patch)?

        public static LobbyCodes instance { get; private set; }

        void Start()
        {
            instance = this;
        }
    }


    public class LobbyUI
    {
        private static GameObject _BG = null;

        public static GameObject BG
        {
            get
            {
                if (LobbyUI._BG != null) { return LobbyUI._BG; }

                LobbyUI._BG = new GameObject("LobbyCode", typeof(HorizontalLayoutGroup), typeof(Image));
                LobbyUI._BG.transform.parent = UnityEngine.GameObject.Find("/Game/UI/UI_Game/Canvas/").transform;

                // We want to dock it in the top right corner
                var rect = LobbyUI._BG.AddComponent<RectTransform>();
                rect.localScale = new Vector3(1, 1, 1);
                rect.anchorMin = new Vector2(1, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(1, 1);
                rect.offsetMin = new Vector2(-390, -70);
                rect.offsetMax = new Vector2(-10, -10);
                rect.sizeDelta = new Vector2(380, 60);

                var image = LobbyUI._BG.GetOrAddComponent<Image>();
                image.color = new Color(1, 1, 1, 0.05f);

                var group = LobbyUI._BG.GetOrAddComponent<HorizontalLayoutGroup>();
                group.childAlignment = TextAnchor.MiddleRight;
                group.childControlHeight = true;
                group.childForceExpandHeight = true;
                group.childControlWidth = false;
                group.childForceExpandWidth = false;
                group.spacing = 10f;
                group.padding = new RectOffset(5, 5, 5, 5);

                var sizeFitter = _BG.AddComponent<ContentSizeFitter>();
                sizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                sizeFitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;

                return LobbyUI._BG;
            }
        }

        private static GameObject _input = null;

        public static GameObject input
        {
            get
            {
                if (LobbyUI._input != null) { return LobbyUI._input; }

                LobbyUI._input = new GameObject("LobbyCodeInput", typeof(TMP_InputField));
                LobbyUI._input.transform.parent = LobbyUI.BG.transform;
                return LobbyUI._input;
            }
        }
    }
}
