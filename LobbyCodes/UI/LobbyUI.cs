using System;
using UnboundLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

namespace LobbyCodes.UI
{
    public class LobbyUI
    {
        private static GameObject _BG = null;

        public static GameObject BG
        {
            get
            {
                if (LobbyUI._BG != null) { return LobbyUI._BG; }

                LobbyUI._BG = new GameObject("LobbyCode", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));
                LobbyUI._BG.transform.parent = UnityEngine.GameObject.Find("/Game/UI/UI_Game/Canvas/").transform;

                // We want to dock it in the top right corner
                var rect = LobbyUI._BG.GetComponent<RectTransform>();
                rect.localScale = new Vector3(1, 1, 1);
                rect.anchorMin = new Vector2(1, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(1, 1);
                rect.offsetMax = new Vector2(-10, -10);
                rect.sizeDelta = new Vector2(380, 75);

                var image = LobbyUI._BG.GetComponent<Image>();
                image.color = new Color(1, 1, 1, 0.05f);

                var group = LobbyUI._BG.GetComponent<HorizontalLayoutGroup>();
                group.childAlignment = TextAnchor.MiddleRight;
                group.childControlHeight = true;
                group.childForceExpandHeight = true;
                group.childControlWidth = false;
                group.childForceExpandWidth = false;
                group.spacing = 5f;
                group.padding = new RectOffset(5, 5, 5, 5);

                var sizeFitter = _BG.GetComponent<ContentSizeFitter>();
                sizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                sizeFitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;

                LobbyCodes.instance.ExecuteAfterFrames(1, LobbyUI.SortChildren);

                return LobbyUI._BG;
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
                GameObject _ = LobbyUI.BG;

                LobbyUI._input = new GameObject("LobbyCodeInput", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(TMP_InputField));
                LobbyUI._input.transform.parent = LobbyUI.BG.transform;

                TMP_InputField inputField = LobbyUI._input.GetComponent<TMP_InputField>();
                inputField.readOnly = true;

                RectTransform rect = null;
                rect = LobbyUI._input.GetComponent<RectTransform>();
                rect.localScale = new Vector3(1, 1, 1);

                var image = LobbyUI._input.GetComponent<Image>();
                image.color = new Color(0, 0, 0, 0.1f);

                var textView = new GameObject("Text Area", typeof(RectTransform), typeof(RectMask2D));
                textView.transform.parent = LobbyUI._input.transform;
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
                placeholderText.transform.parent = textView.transform;
                {
                    rect = placeholderText.GetComponent<RectTransform>();
                    rect.localScale = new Vector3(1, 1, 1);
                    rect.anchorMin = new Vector2(0, 0);
                    rect.anchorMax = new Vector2(1, 1);
                    rect.pivot = new Vector2(0.5f, 0.5f);
                    rect.offsetMin = new Vector2(0, 0);
                    rect.offsetMax = new Vector2(0, 0);

                    var text = placeholderText.GetComponent<TextMeshProUGUI>();
                    text.text = "You shouldn't see this ...";
                    text.enableAutoSizing = true;
                }

                inputField.placeholder = placeholderText.GetComponent<TextMeshProUGUI>();

                var inputText = new GameObject("Input Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
                inputText.transform.parent = textView.transform;
                {
                    rect = inputText.GetComponent<RectTransform>();
                    rect.localScale = new Vector3(1, 1, 1);
                    rect.anchorMin = new Vector2(0, 0);
                    rect.anchorMax = new Vector2(1, 1);
                    rect.pivot = new Vector2(0.5f, 0.5f);
                    rect.offsetMin = new Vector2(0, 0);
                    rect.offsetMax = new Vector2(0, 0);

                    LobbyCodes.instance.ExecuteAfterFrames(5, () =>
                    {
                        rect = inputText.GetComponent<RectTransform>();
                        rect.anchorMin = new Vector2(0, 0);
                        rect.anchorMax = new Vector2(0.5f, 0.5f);
                        rect.pivot = new Vector2(0.5f, 0.5f);
                        rect.offsetMin = new Vector2(0, 0);
                        rect.offsetMax = new Vector2(0, 0);

                        LobbyCodes.instance.ExecuteAfterFrames(5, () =>
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
                    text.enableAutoSizing = true;
                    text.fontSizeMin = 2f;
                }

                inputField.textComponent = inputText.GetComponent<TextMeshProUGUI>();
                inputField.text = "@Pykess#5527";

                inputField.onSelect.AddListener((str) => UnityEngine.Debug.Log($"{str}"));

                rect = LobbyUI._input.GetComponent<RectTransform>();
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(280, 80);

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
                GameObject _ = LobbyUI.BG;

                LobbyUI._text = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
                LobbyUI._text.transform.parent = LobbyUI.BG.transform;

                RectTransform rect = LobbyUI._text.GetComponent<RectTransform>();
                rect.localScale = new Vector3(1, 1, 1);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(200, 80);

                var text = LobbyUI._text.GetComponent<TextMeshProUGUI>();
                text.text = "Lobby Code:";
                text.enableAutoSizing = true;
                text.alignment = TextAlignmentOptions.Right;

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
                GameObject _ = LobbyUI.BG;

                Sprite clipboardIcon = LobbyCodes.instance.assets.LoadAsset<Sprite>("copy-regular");

                // The button object.
                LobbyUI._copyButton = new GameObject("Copy Button", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
                LobbyUI._copyButton.transform.parent = LobbyUI.BG.transform;

                RectTransform rect = LobbyUI._copyButton.GetComponent<RectTransform>();
                rect.localScale = new Vector3(1, 1, 1);
                rect.pivot = new Vector2(0.5f, 0.5f);
                var yheight = LobbyUI.BG.GetComponent<RectTransform>().sizeDelta.y - LobbyUI.BG.GetComponent<HorizontalLayoutGroup>().padding.top - LobbyUI.BG.GetComponent<HorizontalLayoutGroup>().padding.bottom-10;
                rect.sizeDelta = new Vector2(yheight/clipboardIcon.rect.size.y*clipboardIcon.rect.size.x+10, yheight+10);

                Image image = LobbyUI._copyButton.GetComponent<Image>();
                image.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);

                Button button = LobbyUI._copyButton.GetComponent<Button>();
                button.targetGraphic = image;
                button.image = image;

                var interact = LobbyUI._copyButton.AddComponent<ButtonInteraction>();
                interact.mouseClick.AddListener(() => input.GetComponent<TMP_InputField>().text.CopyToClipboard());

                var icon = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                icon.transform.parent = LobbyUI._copyButton.transform;
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
                    image.color = new Color(1, 1, 1, 0.8f);
                }

                return LobbyUI._copyButton;
            }
        }

        private static void SortChildren()
        {
            LobbyUI.text.transform.SetSiblingIndex(0);
            LobbyUI.input.transform.SetSiblingIndex(1);
            LobbyUI.copyButton.transform.SetSiblingIndex(2);
        }

        private class ButtonInteraction : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
        {
            public UnityEvent mouseClick = new UnityEvent();
            public UnityEvent mouseEnter = new UnityEvent();
            public UnityEvent mouseExit = new UnityEvent();
            public Button button;
            public AudioSource source;
            public static ButtonInteraction instance;

            private System.Random random = new System.Random();

            private void Start()
            {
                instance = this;
                button = gameObject.GetComponent<Button>();
                source = gameObject.GetOrAddComponent<AudioSource>();

                mouseEnter.AddListener(OnEnter);
                mouseExit.AddListener(OnExit);
                mouseClick.AddListener(OnClick);
            }

            public void OnEnter()
            {
                source.PlayOneShot(LobbyCodes.instance.hover[random.Next(LobbyCodes.instance.hover.Count)]);
            }

            public void OnExit()
            {
                source.PlayOneShot(LobbyCodes.instance.hover[random.Next(LobbyCodes.instance.hover.Count)]);
            }

            public void OnClick()
            {
                source.PlayOneShot(LobbyCodes.instance.click[random.Next(LobbyCodes.instance.click.Count)]);
                EventSystem.current.SetSelectedGameObject(null);
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
