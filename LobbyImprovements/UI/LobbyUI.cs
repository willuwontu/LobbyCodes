using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using Photon.Pun;
using LobbyImprovements.Networking;
using LobbyImprovements.Extensions;

namespace LobbyImprovements.UI
{
    public class LobbyUI
    {

        internal static void UpdateStreamerModeSettings()
        {
            if (!PhotonNetwork.InRoom)
            {
                LobbyUI.BG.GetComponent<RectTransform>().anchorMax = new Vector2(2, 2); 
            }
            LobbyUI.input.SetActive(!LobbyImprovements.StreamerMode);
            LobbyUI.text.GetComponent<TextMeshProUGUI>().text = LobbyImprovements.StreamerMode ? "STREAMER MODE" : "Lobby Code:";
            LobbyUI.text.GetComponent<TextMeshProUGUI>().color = LobbyImprovements.StreamerMode ? new Color32(145, 70, 255, 255) : new Color32(255, 255, 255, (int) (0.8f * 255));
            LobbyUI.text.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            LobbyUI.text.SetActive(LobbyImprovements.StreamerMode);
            LobbyImprovements.instance.ExecuteAfterFrames(3, () =>
            {
                if (!PhotonNetwork.InRoom)
                {
                    LobbyUI.BG.SetActive(false);
                }
                LobbyUI.BG.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            });
        }
        internal static void UpdateHostOnlySettings()
        {
            if (!PhotonNetwork.InRoom)
            {
                LobbyUI.BG.GetComponent<RectTransform>().anchorMax = new Vector2(2, 2);
            }
            LobbyUI.hostOnlyToggle.GetComponent<Toggle>().isOn = LobbyImprovements.OnlyHostCanInvite;
            LobbyImprovements.instance.ExecuteAfterFrames(3, () =>
            {
                if (!PhotonNetwork.InRoom)
                {
                    LobbyUI.BG.SetActive(false);
                    LobbyUI.BG.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
                }
            });
        }

        private static GameObject uiCanvas
        {
            get
            {
                return UnityEngine.GameObject.Find("/Game/UI/UI_Game/Canvas/");
            }
        }

        private static GameObject _BG = null;

        public static GameObject BG
        {
            get
            {
                if (!uiCanvas.GetComponent<BringBGToTop>())
                {
                    uiCanvas.AddComponent<BringBGToTop>();
                }

                if (LobbyUI._BG != null) { return LobbyUI._BG; }

                LobbyUI._BG = new GameObject("LobbyImprovementsBG", typeof(RectTransform), typeof(VerticalLayoutGroup));
                LobbyUI._BG.transform.SetParent(LobbyUI.uiCanvas.transform);

                RectTransform rect = LobbyUI._BG.GetComponent<RectTransform>();
                rect.localScale = new Vector3(1, 1, 1);
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(1f, 1f);
                rect.offsetMin = new Vector2(0, 0);
                rect.offsetMax = new Vector2(0, 0);

                var group = LobbyUI._BG.GetComponent<VerticalLayoutGroup>();
                group.childAlignment = TextAnchor.UpperRight;
                group.childControlHeight = false;
                group.childForceExpandHeight = false;
                group.childControlWidth = false;
                group.childForceExpandWidth = false;
                group.spacing = 10f;
                group.padding = new RectOffset(10, 10, 40, 10);

                LobbyImprovements.instance.ExecuteAfterFrames(1, LobbyUI.SortChildren);
                LobbyImprovements.instance.ExecuteAfterFrames(5, LobbyUI.SortChildren);

                return LobbyUI._BG;
            }
        }

        private static GameObject _hostOnlyContainer = null;

        public static GameObject hostOnlyContainer
        {
            get
            {
                if (LobbyUI._hostOnlyContainer != null) { return LobbyUI._hostOnlyContainer; }

                // Get BG to make sure it exists.
                GameObject _ = LobbyUI.BG;

                LobbyUI._hostOnlyContainer = new GameObject("Kick Container", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));
                LobbyUI._hostOnlyContainer.transform.SetParent(LobbyUI.BG.transform);

                RectTransform rect = LobbyUI._hostOnlyContainer.GetComponent<RectTransform>();
                rect.localScale = new Vector3(1, 1, 1);
                rect.anchorMin = new Vector2(1, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(1, 1);
                rect.sizeDelta = new Vector2(300, 40);

                var group = LobbyUI._hostOnlyContainer.GetComponent<HorizontalLayoutGroup>();
                group.childAlignment = TextAnchor.UpperRight;
                group.childControlHeight = false;
                group.childForceExpandHeight = false;
                group.childControlWidth = false;
                group.childForceExpandWidth = false;
                group.spacing = 5f;
                group.padding = new RectOffset(5, 5, 5, 5);

                var image = LobbyUI._hostOnlyContainer.GetComponent<Image>();
                image.color = new Color(1, 1, 1, 0.05f);

                var sizeFitter = _hostOnlyContainer.GetComponent<ContentSizeFitter>();
                sizeFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
                sizeFitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;

                return LobbyUI._hostOnlyContainer;
            }
        }

        private static GameObject _hostOnlyText = null;

        public static GameObject hostOnlyText
        {
            get
            {
                if (LobbyUI._hostOnlyText != null) { return LobbyUI._hostOnlyText; }

                GameObject localGo = UnityEngine.GameObject.Find("/Game/UI/UI_MainMenu/Canvas/ListSelector/Main/Group/Local/Text");
                TMP_FontAsset font = localGo.GetComponent<TextMeshProUGUI>().font;
                Material[] fontMaterials = localGo.GetComponent<TextMeshProUGUI>().fontMaterials;

                // Get BG to make sure it exists.
                GameObject _ = LobbyUI.hostOnlyContainer;

                LobbyUI._hostOnlyText = new GameObject("Host Only Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
                LobbyUI._hostOnlyText.transform.SetParent(LobbyUI.hostOnlyContainer.transform);

                RectTransform rect = LobbyUI._hostOnlyText.GetComponent<RectTransform>();
                rect.localScale = Vector3.one;
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.offsetMin = new Vector2(0, 0);
                rect.offsetMax = new Vector2(0, 0);
                rect.sizeDelta = new Vector2(318.125f, 40);

                var text = LobbyUI._hostOnlyText.GetComponent<TextMeshProUGUI>();
                text.font = font;
                text.fontMaterials = fontMaterials;
                text.color = new Color(1, 1, 1, 0.8f);
                text.text = "Only host can invite:".ToUpper();
                text.enableAutoSizing = true;
                text.alignment = TextAlignmentOptions.Right;

                return LobbyUI._hostOnlyText;
            }
        }

        private static GameObject _hostOnlyToggle = null;

        public static GameObject hostOnlyToggle
        {
            get
            {
                if (LobbyUI._hostOnlyToggle != null) { return LobbyUI._hostOnlyToggle; }

                // Get BG to make sure it exists.
                GameObject _ = LobbyUI.hostOnlyContainer;

                var unboundToggle = UnboundLib.Utils.UI.MenuHandler.CreateToggle(LobbyImprovements.OnlyHostCanInvite, "Only host can invite:", BG, null, 30, false, null, null, null, TextAlignmentOptions.Right);

                var bgImage = unboundToggle.transform.GetChild(0).gameObject.GetComponent<Image>();
                var checkImage = unboundToggle.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>();

                LobbyUI._hostOnlyToggle = new GameObject("HostOnly Toggle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Mask), typeof(Toggle), typeof(ButtonInteraction));
                LobbyUI._hostOnlyToggle.transform.SetParent(LobbyUI.hostOnlyContainer.transform);

                RectTransform rect = LobbyUI._hostOnlyToggle.GetComponent<RectTransform>();
                rect.localScale = Vector3.one;
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.offsetMin = new Vector2(0, 0);
                rect.offsetMax = new Vector2(0, 0);
                rect.sizeDelta = new Vector2(40f, 40f);

                Image image = LobbyUI._hostOnlyToggle.GetComponent<Image>();
                image.sprite = bgImage.sprite;
                image.color = bgImage.color;
                image.material = bgImage.material;

                var mask = LobbyUI._hostOnlyToggle.GetComponent<Mask>();
                mask.showMaskGraphic = true;

                var toggle = LobbyUI._hostOnlyToggle.GetComponent<Toggle>();
                toggle.targetGraphic = image;

                // Checkmark
                var check = new GameObject("Checkmark", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                check.transform.SetParent(LobbyUI._hostOnlyToggle.transform);

                rect = check.GetComponent<RectTransform>();
                rect.localScale = Vector3.one;
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.offsetMin = new Vector2(5, 5);
                rect.offsetMax = new Vector2(-5, -5);
                rect.Rotate(0, 0, 45);

                image = check.GetComponent<Image>();
                image.sprite = checkImage.sprite;
                image.color = checkImage.color;
                image.material = checkImage.material;

                toggle.graphic = image;
                toggle.isOn = LobbyImprovements.OnlyHostCanInvite;

                toggle.onValueChanged.AddListener((value) =>
                {
                    LobbyImprovements.OnlyHostCanInvite = value;
                    LobbyImprovements.instance.hostOnlyConfigToggle.GetComponent<Toggle>().isOn = value;
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        PhotonNetwork.LocalPlayer.SetOnlyHostCanInvite(value);
                    }
                });

                UnityEngine.GameObject.Destroy(unboundToggle);

                return LobbyUI._hostOnlyToggle;
            }
        }
        private static GameObject _CodesContainer = null;

        public static GameObject CodesContainer
        {
            get
            {
                if (LobbyUI._CodesContainer != null) { return LobbyUI._CodesContainer; }

                LobbyUI._CodesContainer = new GameObject("LobbyCodeContainer", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));
                LobbyUI._CodesContainer.transform.SetParent(LobbyUI.BG.transform);

                // We want to dock it in the top right corner
                RectTransform rect = LobbyUI._CodesContainer.GetComponent<RectTransform>();
                rect.localScale = new Vector3(1, 1, 1);
                rect.anchorMin = new Vector2(1, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(1, 1);
                rect.offsetMax = new Vector2(-10, -40);
                rect.sizeDelta = new Vector2(380, 75);

                var image = LobbyUI._CodesContainer.GetComponent<Image>();
                image.color = new Color(1, 1, 1, 0.05f);

                var group = LobbyUI._CodesContainer.GetComponent<HorizontalLayoutGroup>();
                group.childAlignment = TextAnchor.MiddleRight;
                group.childControlHeight = true;
                group.childForceExpandHeight = true;
                group.childControlWidth = false;
                group.childForceExpandWidth = false;
                group.spacing = 5f;
                group.padding = new RectOffset(5, 5, 5, 5);

                var sizeFitter = _CodesContainer.GetComponent<ContentSizeFitter>();
                sizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                sizeFitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;

                return LobbyUI._BG;
            }
        }

        private static GameObject _kickContainer = null;

        public static GameObject kickContainer
        {
            get
            {
                if (LobbyUI._kickContainer != null) { return LobbyUI._kickContainer; }

                // Get BG to make sure it exists.
                GameObject _ = LobbyUI.BG;

                LobbyUI._kickContainer = new GameObject("Kick Container", typeof(RectTransform), typeof(CanvasRenderer), typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));
                LobbyUI._kickContainer.transform.SetParent(LobbyUI.BG.transform);

                RectTransform rect = LobbyUI._kickContainer.GetComponent<RectTransform>();
                rect.localScale = new Vector3(1, 1, 1);
                rect.anchorMin = new Vector2(1, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(1, 1);
                rect.sizeDelta = new Vector2(300, 40);

                var group = LobbyUI._kickContainer.GetComponent<HorizontalLayoutGroup>();
                group.childAlignment = TextAnchor.UpperRight;
                group.childControlHeight = false;
                group.childForceExpandHeight = false;
                group.childControlWidth = false;
                group.childForceExpandWidth = false;
                group.spacing = 5f;
                group.padding = new RectOffset(0, 0, 0, 0);

                var sizeFitter = _kickContainer.GetComponent<ContentSizeFitter>();
                sizeFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
                sizeFitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;

                return LobbyUI._kickContainer;
            }
        }
        private static GameObject _kicklist = null;
        private static TMP_Dropdown _dropdown = null;

        public static GameObject kicklist
        {
            get
            {
                if (LobbyUI._kicklist != null) { return LobbyUI._kicklist; }

                // Get BG to make sure it exists.
                GameObject _ = LobbyUI.kickContainer;

                // Stuff for the dropdown menu
                GameObject localGo = UnityEngine.GameObject.Find("/Game/UI/UI_MainMenu/Canvas/ListSelector/Main/Group/Local/Text");
                TMP_FontAsset font = localGo.GetComponent<TextMeshProUGUI>().font;
                Material[] fontMaterials = localGo.GetComponent<TextMeshProUGUI>().fontMaterials;
                Sprite dropdownArrow = LobbyImprovements.instance.assets.LoadAsset<Sprite>("DropdownArrow");
                Sprite dropdownCheckMark = LobbyImprovements.instance.assets.LoadAsset<Sprite>("DropdownCheckmark");

                // The dropdown item
                var kick = LobbyUI._kicklist = new GameObject("Kicklist", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(TMP_Dropdown), typeof(ButtonInteraction));
                kicklist.transform.SetParent(LobbyUI.kickContainer.transform);

                RectTransform rect = LobbyUI._kicklist.GetComponent<RectTransform>();
                rect.localScale = Vector3.one;
                rect.anchorMin = new Vector2(1f, 1f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.pivot = new Vector2(1f, 1f);
                rect.sizeDelta = new Vector2(268.125f, 40);


                Image image = LobbyUI._kicklist.GetComponent<Image>();
                image.color = new Color(0.35f, 0.35f, 0.35f, 0.7f);

                TMP_Dropdown dropdown = LobbyUI._kicklist.GetComponent<TMP_Dropdown>();
                dropdown.image = image;
                LobbyUI._dropdown = dropdown;

                // Label for the dropdown
                var label = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
                label.transform.SetParent(kick.transform);

                rect = label.GetComponent<RectTransform>();
                rect.localScale = Vector3.one;
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.offsetMin = new Vector2(5f, 5f);
                rect.offsetMax = new Vector2(-37f, -5f);

                var text = label.GetComponent<TextMeshProUGUI>();
                text.fontMaterials = fontMaterials;
                text.font = font;
                text.enableAutoSizing = true;
                text.alignment = TextAlignmentOptions.Left;
                text.color = new Color(1, 1, 1, 0.8f);

                dropdown.captionText = text;

                var arrow = new GameObject("Arrow", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                arrow.transform.SetParent(kick.transform);

                image = arrow.GetComponent<Image>();
                image.color = new Color(1, 1, 1, 0.8f);
                image.sprite = dropdownArrow;

                rect = arrow.GetComponent<RectTransform>();
                rect.localScale = Vector3.one;
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchorMin = new Vector2(1f, 0.5f);
                rect.anchorMax = new Vector2(1f, 0.5f);
                rect.offsetMax = new Vector2(-5f, 15f);
                rect.offsetMin = new Vector2(-35f, -15f);

                // Now we start building the scroll template used when the dropdown is clicked
                // Template
                var template = new GameObject("Template", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(ScrollRect));
                template.transform.SetParent(dropdown.transform);

                rect = template.GetComponent<RectTransform>();
                rect.localScale = Vector3.one;
                rect.pivot = new Vector2(0.5f, 1f);
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(1f, 0f);
                rect.offsetMax = new Vector2(0, -2);
                rect.offsetMin = new Vector2(0, 0);
                rect.sizeDelta = new Vector2(1, 150);

                image = template.GetComponent<Image>();
                image.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);

                ScrollRect scrollRect = template.GetComponent<ScrollRect>();
                scrollRect.inertia = false;
                scrollRect.horizontal = false;
                scrollRect.vertical = true;
                scrollRect.movementType = ScrollRect.MovementType.Clamped;
                scrollRect.scrollSensitivity = 50f;

                template.SetActive(false);

                // Viewport
                var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Mask));
                viewport.transform.SetParent(template.transform);

                rect = viewport.GetComponent<RectTransform>();
                rect.localScale = Vector3.one;
                rect.pivot = new Vector2(0f, 1f);
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.offsetMax = new Vector2(0, 0);
                rect.offsetMin = new Vector2(0, 0);

                var mask = viewport.GetComponent<Mask>();
                mask.showMaskGraphic = false;

                scrollRect.viewport = rect;

                // Content Container
                var content = new GameObject("Content", typeof(RectTransform));
                content.transform.SetParent(viewport.transform);

                rect = content.GetComponent<RectTransform>();
                rect.localScale = Vector3.one;
                rect.pivot = new Vector2(0.5f, 1f);
                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.offsetMax = new Vector2(0, 0);
                rect.offsetMin = new Vector2(0, 0);
                rect.sizeDelta = new Vector2(0, 30);

                scrollRect.content = rect;

                // Template item
                var item = new GameObject("Item", typeof(RectTransform), typeof(CanvasRenderer), typeof(Toggle), typeof(ButtonInteraction));
                item.transform.SetParent(content.transform);

                rect = item.GetComponent<RectTransform>();
                rect.localScale = Vector3.one;
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchorMin = new Vector2(0f, 0.5f);
                rect.anchorMax = new Vector2(1f, 0.5f);
                rect.offsetMax = new Vector2(0, 15);
                rect.offsetMin = new Vector2(0, -15);

                var toggle = item.GetComponent<Toggle>();

                // Item Background
                var itemBG = new GameObject("Item Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                itemBG.transform.SetParent(item.transform);

                rect = itemBG.GetComponent<RectTransform>();
                rect.localScale = Vector3.one;
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.offsetMax = new Vector2(0, 0);
                rect.offsetMin = new Vector2(0, 0);

                image = itemBG.GetComponent<Image>();
                image.color = new Color(0f, 0f, 0f, 0.7f);

                toggle.targetGraphic = image;

                // Item Checkmark
                var itemCheck = new GameObject("Item Checkmark", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                itemCheck.transform.SetParent(itemBG.transform);

                image = itemCheck.GetComponent<Image>();
                image.color = new Color(1, 1, 1, 0.8f);
                image.sprite = dropdownCheckMark;

                rect = itemCheck.GetComponent<RectTransform>();
                rect.localScale = Vector3.one;
                rect.pivot = new Vector2(0f, 0.5f);
                rect.anchorMin = new Vector2(0f, 0.5f);
                rect.anchorMax = new Vector2(0f, 0.5f);
                rect.offsetMax = new Vector2(35, 15);
                rect.offsetMin = new Vector2(5, -15);

                toggle.graphic = image;

                // Item Label
                var itemLabel = new GameObject("Item Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
                itemLabel.transform.SetParent(itemBG.transform);

                rect = itemLabel.GetComponent<RectTransform>();
                rect.localScale = Vector3.one;
                rect.pivot = new Vector2(1f, 0.5f);
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.offsetMax = new Vector2(-2, -2);
                rect.offsetMin = new Vector2(45, 2);

                text = itemLabel.GetComponent<TextMeshProUGUI>();
                text.font = font;
                text.fontMaterials = fontMaterials;
                text.enableAutoSizing = true;
                text.color = new Color(1, 1, 1, 0.8f);
                text.alignment = TextAlignmentOptions.Left;

                dropdown.itemText = text;

                dropdown.template = template.GetComponent<RectTransform>();
                var flipper = kick.AddComponent<FlipArrowOnExpansion>();
                flipper.arrow = arrow;

                dropdown.AddOptions(new List<TMP_Dropdown.OptionData> { });

                return LobbyUI._kicklist;
            }
        }

        private static Photon.Realtime.Player[] playerKickList;

        internal static void UpdateKickList(Photon.Realtime.Player[] players)
        {
            // Make Sure that the kicklist and dropdown fields exist
            var _ = kicklist;

            playerKickList = players;

            var wasExpanded = LobbyUI._dropdown.IsExpanded;

            LobbyUI._dropdown.Hide();
            LobbyUI._dropdown.ClearOptions();
            LobbyUI._dropdown.AddOptions(playerKickList.Select((player) => new TMP_Dropdown.OptionData(player.NickName)).ToList());
            LobbyUI._dropdown.RefreshShownValue();

            if (wasExpanded)
            {
                LobbyUI._dropdown.Show();
            }

            LobbyUI._dropdown.RefreshShownValue();
        }

        public static Photon.Realtime.Player GetSelectedKickListPlayer()
        {
            if (!(playerKickList.Count() > 0))
            {
                return null;
            }

            return playerKickList[LobbyUI._dropdown.value];
        }

        private static GameObject _kickButton = null;

        /// <summary>
        /// The action run when the kick button is pressed. The input parameter is the player selected.
        /// </summary>
        //public static Action<Photon.Realtime.Player> kickButtonPressed = null;

        public static GameObject kickButton
        {
            get
            {
                if (LobbyUI._kickButton != null) { return LobbyUI._kickButton; }

                GameObject localGo = UnityEngine.GameObject.Find("/Game/UI/UI_MainMenu/Canvas/ListSelector/Main/Group/Local/Text");
                TMP_FontAsset font = localGo.GetComponent<TextMeshProUGUI>().font;
                Material[] fontMaterials = localGo.GetComponent<TextMeshProUGUI>().fontMaterials;

                // Get BG to make sure it exists.
                GameObject _ = LobbyUI.kickContainer;

                // The button object.
                var kickbutton = LobbyUI._kickButton = new GameObject("Kick Button", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
                kickbutton.transform.SetParent(LobbyUI.kickContainer.transform);

                RectTransform rect = kickbutton.GetComponent<RectTransform>();
                rect.localScale = new Vector3(1, 1, 1);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(100, 40);

                Image image = kickbutton.GetComponent<Image>();
                image.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);

                Button button = kickbutton.GetComponent<Button>();
                button.targetGraphic = image;
                button.image = image;

                var interact = kickbutton.AddComponent<ButtonInteraction>();
                interact.mouseClick.AddListener(() =>
                {
                    if (playerKickList.Count() > 0 && PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        Unbound.BuildModal()
                            .Title("Kick Player")
                            .Message($"Kick {playerKickList[LobbyUI._dropdown.value].NickName} from the lobby?")
                            .ConfirmButton("Kick", () =>
                            {
                                if (playerKickList.Count() > 0)
                                {
                                    LobbyMonitor.instance.ForceKickPlayer(playerKickList[LobbyUI._dropdown.value], true);
                                }
                            })
                            .CancelButton("Cancel", () => { })
                            .Show();
                    }
                });

                // Text object
                var textobj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
                textobj.transform.SetParent(kickbutton.transform);

                rect = textobj.GetComponent<RectTransform>();
                rect.localScale = Vector3.one;
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.offsetMax = new Vector2(-5, -5);
                rect.offsetMin = new Vector2(5, 5);

                var text = textobj.GetComponent<TextMeshProUGUI>();
                text.font = font;
                text.fontMaterials = fontMaterials;
                text.color = new Color(1, 1, 1, 0.8f);
                text.alignment = TextAlignmentOptions.Center;
                text.text = "KICK";

                return LobbyUI._kickButton;
            }
        }



        private class FlipArrowOnExpansion : MonoBehaviour
        {
            private TMP_Dropdown dropdown = null;
            public GameObject arrow = null;
            private bool wasExpanded = false;

            private void Start()
            {
                dropdown = gameObject.GetComponent<TMP_Dropdown>();

                if (!dropdown || !arrow)
                {
                    UnityEngine.GameObject.Destroy(this);
                }
            }

            private void FixedUpdate()
            {
                if (dropdown.IsExpanded && !wasExpanded)
                {
                    arrow.transform.Rotate(0,0,180);
                    wasExpanded = true;
                }
                else if (!dropdown.IsExpanded && wasExpanded)
                {
                    arrow.transform.Rotate(0, 0, -180);
                    wasExpanded = false;
                }
            }
        }

        public static void UpdateLobbyCode(string code)
        {
            LobbyUI.input.GetComponent<TMP_InputField>().text = code;
        }

        private static GameObject _input = null;

        public static GameObject input
        {
            get
            {
                if (LobbyUI._input != null) { return LobbyUI._input; }

                // Get BG to make sure it exists.
                GameObject _ = LobbyUI.CodesContainer;

                GameObject localGo = UnityEngine.GameObject.Find("/Game/UI/UI_MainMenu/Canvas/ListSelector/Main/Group/Local/Text");
                var font = localGo.GetComponent<TextMeshProUGUI>().font;
                var fontMaterials = localGo.GetComponent<TextMeshProUGUI>().fontMaterials;

                LobbyUI._input = new GameObject("LobbyCode Input", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(TMP_InputField));
                LobbyUI._input.transform.SetParent(LobbyUI.CodesContainer.transform);

                TMP_InputField inputField = LobbyUI._input.GetComponent<TMP_InputField>();
                inputField.readOnly = true;

                RectTransform rect = null;
                rect = LobbyUI._input.GetComponent<RectTransform>();
                rect.localScale = new Vector3(1, 1, 1);

                var image = LobbyUI._input.GetComponent<Image>();
                image.color = new Color(0, 0, 0, 0.1f);

                var textView = new GameObject("Text Area", typeof(RectTransform), typeof(RectMask2D));
                textView.transform.SetParent(LobbyUI._input.transform);
                {
                    rect = textView.GetComponent<RectTransform>();
                    rect.localScale = new Vector3(1, 1, 1);
                    rect.anchorMin = new Vector2(0, 0);
                    rect.anchorMax = new Vector2(1, 1);
                    rect.pivot = new Vector2(0.5f, 0.5f);
                    rect.offsetMin = new Vector2(5, 5);
                    rect.offsetMax = new Vector2(-5, -5);
                }

                inputField.textViewport = rect;

                var placeholderText = new GameObject("Placeholder", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
                placeholderText.transform.SetParent(textView.transform);
                {
                    rect = placeholderText.GetComponent<RectTransform>();
                    rect.localScale = new Vector3(1, 1, 1);
                    rect.anchorMin = new Vector2(0, 0);
                    rect.anchorMax = new Vector2(1, 1);
                    rect.pivot = new Vector2(0.5f, 0.5f);
                    rect.offsetMin = new Vector2(0, 0);
                    rect.offsetMax = new Vector2(0, 0);

                    var text = placeholderText.GetComponent<TextMeshProUGUI>();
                    if (localGo)
                    {
                        text.font = font;
                        text.fontMaterials = fontMaterials;
                    }
                    text.text = "";
                    text.enableAutoSizing = true;
                    text.alignment = TextAlignmentOptions.Right;
                }

                inputField.placeholder = placeholderText.GetComponent<TextMeshProUGUI>();

                var inputText = new GameObject("Input Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
                inputText.transform.SetParent(textView.transform);
                {
                    rect = inputText.GetComponent<RectTransform>();
                    rect.localScale = new Vector3(1, 1, 1);
                    rect.anchorMin = new Vector2(0, 0);
                    rect.anchorMax = new Vector2(1, 1);
                    rect.pivot = new Vector2(0.5f, 0.5f);
                    rect.offsetMin = new Vector2(0, 0);
                    rect.offsetMax = new Vector2(0, 0);

                    LobbyImprovements.instance.ExecuteAfterFrames(5, () =>
                    {
                        rect = inputText.GetComponent<RectTransform>();
                        rect.anchorMin = new Vector2(0, 0);
                        rect.anchorMax = new Vector2(0.5f, 0.5f);
                        rect.pivot = new Vector2(0.5f, 0.5f);
                        rect.offsetMin = new Vector2(0, 0);
                        rect.offsetMax = new Vector2(0, 0);

                        LobbyImprovements.instance.ExecuteAfterFrames(5, () =>
                        {
                            rect = inputText.GetComponent<RectTransform>();
                            rect.anchorMin = new Vector2(0, 0);
                            rect.anchorMax = new Vector2(1f, 1f);
                            rect.pivot = new Vector2(0.5f, 0.5f);
                            rect.offsetMin = new Vector2(0, 0);
                            rect.offsetMax = new Vector2(0, 0);
                        });
                    });

                    var text = inputText.GetComponent<TextMeshProUGUI>();
                    if (localGo)
                    {
                        text.font = font;
                        text.fontMaterials = fontMaterials;
                    }
                    text.enableAutoSizing = true;
                    text.fontSizeMin = 2f;
                    text.alignment = TextAlignmentOptions.Right;
                }

                inputField.textComponent = inputText.GetComponent<TextMeshProUGUI>();
                inputField.text = "";
                inputField.textComponent.color = new Color(0.9f, 0.9f, 0.9f, 0.8f);
                inputField.selectionColor = new Color(0.5f, 0.5f, 0.5f, 0.7f);

                inputField.onSelect.AddListener((str) => LobbyImprovements.Log($"{str}"));

                // Blip the input field to make it recognize that we've hooked things up now.
                LobbyImprovements.instance.ExecuteAfterFrames(5, () =>
                { 
                    LobbyUI.input.SetActive(false);
                    LobbyImprovements.instance.ExecuteAfterFrames(5, () =>
                    {
                        LobbyUI.input.SetActive(!LobbyImprovements.StreamerMode);
                    });
                });


                rect = LobbyUI._input.GetComponent<RectTransform>();
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(300, 80);

                return LobbyUI._input;
            }
        }

        private static GameObject _text = null;

        public static GameObject text
        {
            get
            {
                if (_text != null) { return _text; }

                // Get BG to make sure it exists.
                GameObject _ = LobbyUI.CodesContainer;

                GameObject localGo = UnityEngine.GameObject.Find("/Game/UI/UI_MainMenu/Canvas/ListSelector/Main/Group/Local/Text");
                var font = localGo.GetComponent<TextMeshProUGUI>().font;
                var fontMaterials = localGo.GetComponent<TextMeshProUGUI>().fontMaterials;

                LobbyUI._text = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
                LobbyUI._text.transform.SetParent(LobbyUI.CodesContainer.transform);

                RectTransform rect = LobbyUI._text.GetComponent<RectTransform>();
                rect.localScale = new Vector3(1, 1, 1);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(300, 80);

                var text = LobbyUI._text.GetComponent<TextMeshProUGUI>();
                if (localGo)
                {
                    text.font = font;
                    text.fontMaterials = fontMaterials;
                }
                text.text = "Lobby Code:";
                text.color = new Color(0.9f, 0.9f, 0.9f, 0.8f);
                text.enableAutoSizing = true;
                text.alignment = TextAlignmentOptions.Right;

                LobbyUI._text.SetActive(false);

                return _text;
            }
        }

        private static GameObject _copyButton = null;

        public static GameObject copyButton
        {
            get
            {
                if (LobbyUI._copyButton != null) { return LobbyUI._copyButton; }

                // Get BG to make sure it exists.
                GameObject _ = LobbyUI.CodesContainer;

                Sprite clipboardIcon = LobbyImprovements.instance.assets.LoadAsset<Sprite>("copy-regular");

                // The button object.
                LobbyUI._copyButton = new GameObject("Copy Button", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
                LobbyUI._copyButton.transform.SetParent(LobbyUI.CodesContainer.transform);

                RectTransform rect = LobbyUI._copyButton.GetComponent<RectTransform>();
                rect.localScale = new Vector3(1, 1, 1);
                rect.pivot = new Vector2(0.5f, 0.5f);
                var yheight = LobbyUI.CodesContainer.GetComponent<RectTransform>().sizeDelta.y - LobbyUI.CodesContainer.GetComponent<HorizontalLayoutGroup>().padding.top - LobbyUI.CodesContainer.GetComponent<HorizontalLayoutGroup>().padding.bottom-10;
                rect.sizeDelta = new Vector2(yheight/clipboardIcon.rect.size.y*clipboardIcon.rect.size.x+10, yheight+10);

                Image image = LobbyUI._copyButton.GetComponent<Image>();
                image.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);

                Button button = LobbyUI._copyButton.GetComponent<Button>();
                button.targetGraphic = image;
                button.image = image;

                _ = LobbyUI.popover;

                var interact = LobbyUI._copyButton.AddComponent<ButtonInteraction>();
                interact.mouseClick.AddListener(() => 
                { 
                    input.GetComponent<TMP_InputField>().text.CopyToClipboard();
                    LobbyUI.popover.GetComponent<FadePopover>().Go();
                });

                var icon = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                icon.transform.SetParent(LobbyUI._copyButton.transform);
                {
                    rect = icon.GetComponent<RectTransform>();
                    rect.localScale = new Vector3(1, 1, 1);
                    rect.anchorMin = new Vector2(0, 0);
                    rect.anchorMax = new Vector2(1, 1);
                    rect.pivot = new Vector2(0.5f, 0.5f);
                    rect.offsetMin = new Vector2(5, 5);
                    rect.offsetMax = new Vector2(-5, -5);

                    image = icon.GetComponent<Image>();
                    image.sprite = clipboardIcon;
                    image.color = new Color(0.9f, 0.9f, 0.9f, 0.8f);
                }

                return LobbyUI._copyButton;
            }
        }

        private static GameObject _popover = null;

        private static GameObject popover
        {
            get
            {
                if (LobbyUI._popover != null) { return LobbyUI._popover; }

                Sprite popoverIcon = LobbyImprovements.instance.assets.LoadAsset<Sprite>("Popover4");
                GameObject localGo = UnityEngine.GameObject.Find("/Game/UI/UI_MainMenu/Canvas/ListSelector/Main/Group/Local/Text");
                var font = localGo.GetComponent<TextMeshProUGUI>().font;
                var fontMaterials = localGo.GetComponent<TextMeshProUGUI>().fontMaterials;

                // Get BG to make sure it exists.
                GameObject _ = LobbyUI.CodesContainer;

                LobbyUI._popover = new GameObject("Copy Popover", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                LobbyUI._popover.transform.SetParent(LobbyUI._copyButton.transform);
                RectTransform rect = LobbyUI._popover.GetComponent<RectTransform>();
                {
                    rect.localScale = new Vector3(1, 1, 1);
                    rect.anchorMin = new Vector2(0.5f, 0.9f);
                    rect.anchorMax = new Vector2(0.5f, 0.9f);
                    rect.pivot = new Vector2(0.5f, 0f);
                    rect.sizeDelta = new Vector2(80, 50);
                }

                var image = LobbyUI._popover.GetComponent<Image>();
                image.sprite = popoverIcon;
                image.color = new Color(0.25f, 0.25f, 0.25f, 0.8f);

                var textobj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
                textobj.transform.SetParent(LobbyUI._popover.transform);

                rect = textobj.GetComponent<RectTransform>();
                {
                    rect.localScale = new Vector3(1, 1, 1);
                    rect.anchorMin = new Vector2(0f, 0f);
                    rect.anchorMax = new Vector2(1f, 1f);
                    rect.offsetMax = new Vector2(-5f, -5f);
                    rect.offsetMin = new Vector2(5f, 5);
                }

                var text = textobj.GetComponent<TextMeshProUGUI>();
                text.font = font;
                text.fontMaterials = fontMaterials;
                text.text = "Copied";
                text.enableAutoSizing = true;
                text.color = new Color(1, 1, 1, 0.8f);

                var fade = LobbyUI._popover.AddComponent<FadePopover>();
                fade.text = text;
                fade.image = image;

                LobbyUI._popover.SetActive(false);

                return LobbyUI._popover;
            }
        }

        private class FadePopover : MonoBehaviour
        {
            Coroutine fadeCoroutine = null;
            float fadeDuration = 0.5f;
            internal TextMeshProUGUI text = null;
            internal Image image = null;

            internal void Go()
            {
                if (fadeCoroutine != null)
                {
                    LobbyImprovements.instance.StopCoroutine(fadeCoroutine);
                }

                LobbyUI.popover.SetActive(true);
                ResetColors();

                fadeCoroutine = LobbyImprovements.instance.StartCoroutine(FadePopup());
            }

            private IEnumerator FadePopup()
            {
                yield return new WaitForSecondsRealtime(1.25f);

                var timeremaining = fadeDuration;

                while (timeremaining > 0f)
                {
                    image.color = new Color(0.25f, 0.25f, 0.25f, 0.8f * timeremaining/ fadeDuration);
                    text.color = new Color(1, 1, 1, 0.8f * timeremaining / fadeDuration);

                    timeremaining -= Time.deltaTime;
                    yield return null;
                }
                LobbyUI.popover.SetActive(false);
                ResetColors();

                yield break;
            }

            private void ResetColors()
            {
                image.color = new Color(0.25f, 0.25f, 0.25f, 0.8f);
                text.color = new Color(1, 1, 1, 0.8f);
            }
        }

        private static void SortChildren()
        {
            LobbyUI.CodesContainer.transform.SetSiblingIndex(0);
            LobbyUI.text.transform.SetSiblingIndex(0);
            LobbyUI.input.transform.SetSiblingIndex(1);
            LobbyUI.copyButton.transform.SetSiblingIndex(2);

            LobbyUI.hostOnlyContainer.transform.SetSiblingIndex(1);
            LobbyUI.hostOnlyText.transform.SetSiblingIndex(0);
            LobbyUI.hostOnlyToggle.transform.SetSiblingIndex(1);

            LobbyUI.kickContainer.transform.SetSiblingIndex(2);
            LobbyUI.kicklist.transform.SetSiblingIndex(0);
            LobbyUI.kickButton.transform.SetSiblingIndex(1);
        }

        private class BringBGToTop : MonoBehaviour
        {
            private void OnTransformChildrenChanged()
            {
                this.ExecuteAfterFrames(1, () => LobbyUI.BG.transform.SetAsLastSibling());
            }
        }

        private class ButtonInteraction : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
        {
            public UnityEvent mouseClick = new UnityEvent();
            public UnityEvent mouseEnter = new UnityEvent();
            public UnityEvent mouseExit = new UnityEvent();
            public Selectable selectable;
            public AudioSource source;
            public static ButtonInteraction instance;

            private System.Random random = new System.Random();

            private void Start()
            {
                instance = this;
                selectable = gameObject.GetComponent<Selectable>();
                source = gameObject.GetOrAddComponent<AudioSource>();

                mouseEnter.AddListener(OnEnter);
                mouseExit.AddListener(OnExit);
                mouseClick.AddListener(OnClick);
            }

            public void OnEnter()
            {
                if (selectable.interactable)
                {
                    source.PlayOneShot(LobbyImprovements.instance.hover[random.Next(LobbyImprovements.instance.hover.Count)]);
                }
            }

            public void OnExit()
            {
                if (selectable.interactable)
                {
                    source.PlayOneShot(LobbyImprovements.instance.hover[random.Next(LobbyImprovements.instance.hover.Count)]);
                }
            }

            public void OnClick()
            {
                if (selectable.interactable)
                {
                    source.PlayOneShot(LobbyImprovements.instance.click[random.Next(LobbyImprovements.instance.click.Count)]);
                    EventSystem.current.SetSelectedGameObject(null);
                }
            }

            public void OnPointerEnter(PointerEventData eventData)
            {
                mouseEnter?.Invoke();
            }
            public void OnPointerExit(PointerEventData eventData)
            {
                mouseExit?.Invoke();
            }

            public void OnPointerClick(PointerEventData eventData)
            {
                mouseClick?.Invoke();
            }
        }
    }
}
