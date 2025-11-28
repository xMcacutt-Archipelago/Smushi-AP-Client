using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Rewired.Integration.UnityUI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.IO;
using UnityEngine.UI;

namespace Smushi_AP_Client
{
    public class APConsole : MonoBehaviour
    {
        private const float MessageHeight = 28f;
        private const float ConsoleHeight = 280f;

        private const float SlideInTime = 0.25f;
        private const float HoldTime = 3.0f;
        private const float FadeOutTime = 0.5f;

        private const float SlideInOffset = -50f;
        private const float FadeUpOffset = 20f;

        private const float PaddingX = 25f;
        private const float PaddingY = 25f;

        private const float MessageSpacing = 6f;
        
        public static TMP_FontAsset shumiFont;

        private static readonly Dictionary<string, string> KeywordColors = new Dictionary<string, string>()
        {
            {"red", "#fb0000"},
            {"blue", "#205aff"},
            {"green", "#35ff1c"},
            {"orange", "#ff8224"},
            {"purple", "#bd32ff"},
            {"cyan", "#33feb1"},
            {"white", "#ffffff"},
            {"black", "#48507a"},
            {"grey", "#898989"},
            {"gray", "#898989"},
            {"yellow", "#fff42b"},
            {"pink", "#ff5cfc"},
            {"archipelago", "#e597bd"},
            {"energy spore", "#ceff15"},
            {"wind essence", "#b679fa"},
            {"leaf glider", "#28b123"},
            {"essence of water", "#5590db"},
            {"Firestarter kit", "#f96136"},
            {"headlamp", "#f2ee5e"},
            {"container of light", "#dbdca5"},
            {"blade of power", "#ff1e7a"},
            {"garden", "#78dc75"},
            {"forest", "#ddb276"},
            {"lake", "#87d0e4"},
            {"grove", "#bca2e8"},
            {"garden of spring", "#78dc75"},
            {"forest of fall", "#ddb276"},
            {"lake of bloom", "#87d0e4"},
            {"grove of life", "#bca2e8"},
            {"elder's home", "#bca2e8"},
            {"snowy forest", "#cfddc7"},
            {"smushi's home", "#f3c2c2"},
            {"map", "#f3da9b"},
            {"mycology journal", "#f3da9b"},
            {"band of elasticity", "#ff8080"},
            {"ancient relic", "#957556"},
            {"tool of mining", "#8bb090"},
            {"tool of writing", "#d1c450"},
            {"hooks", "#b49285"},
            {"blueberry", "#583de9"},
            {"secret password", "#e00000"},
            {"secret opener", "#d3a94c"},
            {"old string", "#90775b"},
            {"screwdriver", "#a7bfd1"},
            {"band aid", "#fbc8b3"},
            {"ring of love", "#ace2ff"},
            {"ring of youth", "#7de54c"},
            {"ring of truth", "#3049f0"},
            {"ring of prosperity", "#e19a38"},
            {"ring of spirit", "#973ff1"},
            {"conch shell", "#d39c5e"},
            {"sacred streamer", "#eeeadf"},
            {"super spore", "#b5ff43"},
            {"super essence", "#be7ffe"},
            {"lotus flower", "#ffd7ec"},
            {"amethyst shroomie", "#d586ff"},
            {"strawberry shroomie", "#ff4e83"},
            {"flower shroomie", "#f5ed91"},
            {"sho bowl shroomie", "#f83737"},
            {"verdant shroomie", "#3ed851"},
            {"pelagic shroomie", "#55c6f5"},
            {"honey shroomie", "#ffdf4f"},
            {"sparkle shroomie", "#a5d0ea"},
            {"clavaria shroomie", "#414fed"},
            {"inkcap shroomie", "#60697e"},
            {"sharp shroomie", "#fdf3de"},
            {"precious shroomie", "#edd2d2"},
            {"veiled shroomie", "#faa853"},
            {"sacred shroomie", "#edc74a"},
            {"purple augmenter", "#d586ff"},
            {"strawberry augmenter", "#ff4e83"},
            {"flower augmenter", "#f5ed91"},
            {"secret augmenter", "#f83737"},
            {"verdant augmenter", "#3ed851"},
            {"pelagic augmenter", "#55c6f5"},
            {"honey augmenter", "#ffdf4f"},
            {"sparkle augmenter", "#a5d0ea"},
            {"clavaria augmenter", "#414fed"},
            {"ink augmenter", "#60697e"},
            {"sharp augmenter", "#fdf3de"},
            {"precious augmenter", "#edd2d2"},
            {"veiled augmenter", "#faa853"},
            {"sacred augmenter", "#edc74a"},
            {"mustache", "#ba4f00"},
            {"purple crystal", "#a88ded"},
            {"smushi", "#ff95b5"},
            {"chungy", "#d09efe"},
            {"capybara", "#f8af64"},
        };
        
        private readonly Queue<Image> _backgroundPool = new();

        private readonly Queue<LogEntry> _cachedEntries = new();

        private readonly Queue<TextMeshProUGUI> _textPool = new();
        private readonly List<LogEntry> _visibleEntries = [];
        private readonly List<LogEntry> _historyEntries = [];
        private GameObject _historyPanel;
        private RectTransform _historyContent;
        private bool _showHistory = false;
        private ScrollRect _historyScrollRect;
        private RectTransform _historyViewport;


        private Transform _messageParent;

        private bool _showConsole = true;

        public static APConsole Instance { get; private set; }

        private void Update()
        {
            UpdateMessages(Time.deltaTime);
            TryAddNewMessages();
            if (Input.GetKeyDown(KeyCode.F7))
                ToggleConsole();
            if (Input.GetKeyDown(KeyCode.F8))
                ToggleHistory();

            if (_showHistory)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public static void Create()
        {
            if (Instance != null)
                return;
            shumiFont = Resources.FindObjectsOfTypeAll<TMP_FontAsset>().First(x => x.name == "Shumi Font Final");
            var consoleObject = new GameObject("ArchipelagoConsoleUI");
            DontDestroyOnLoad(consoleObject);
            Instance = consoleObject.AddComponent<APConsole>();
            Instance.BuildUI();
            Instance.Log("Welcome to Smushi Come Home");
            // foreach (var word in KeywordColors.Keys)
            // {
            //     Instance.Log(word);
            // }
        }

        private void UpdateMessages(float delta)
        {
            for (var i = _visibleEntries.Count - 1; i >= 0; i--)
            {
                var e = _visibleEntries[i];
                var done = AnimateEntry(e, delta);

                if (done)
                {
                    RecycleEntry(e);
                    _visibleEntries.RemoveAt(i);
                    RecalculateBaseY();
                }
                else
                {
                    UpdateEntryVisual(e);
                }
            }
        }

        private void RecalculateBaseY()
        {
            var y = 0f;
            for (var i = _visibleEntries.Count - 1; i >= 0; i--)
            {
                var e = _visibleEntries[i];
                e.baseY = y;
                y += e.height + MessageSpacing;
            }
        }

        private bool AnimateEntry(LogEntry entry, float delta)
        {
            entry.stateTimer += delta;

            switch (entry.state)
            {
                case LogEntry.State.SlideIn:
                {
                    var t = Mathf.Clamp01(entry.stateTimer / SlideInTime);
                    entry.offsetY = Mathf.Lerp(SlideInOffset, 0f, EaseOutQuad(t));

                    if (t >= 1f)
                    {
                        entry.state = LogEntry.State.Hold;
                        entry.stateTimer = 0f;
                    }
                }
                    break;

                case LogEntry.State.Hold:
                {
                    entry.offsetY = 0f;
                    if (entry.stateTimer >= HoldTime)
                    {
                        entry.state = LogEntry.State.FadeOut;
                        entry.stateTimer = 0f;
                    }
                }
                    break;

                case LogEntry.State.FadeOut:
                {
                    var t = Mathf.Clamp01(entry.stateTimer / FadeOutTime);
                    entry.offsetY = Mathf.Lerp(0f, FadeUpOffset, t);
                    var alpha = 1f - t;
                    entry.text.color = new Color(1f, 1f, 1f, alpha);
                    entry.background.color = new Color(0f, 0f, 0f, 0.8f * alpha);

                    if (t >= 1f)
                        return true;
                }
                    break;
            }

            return false;
        }

        private static float EaseOutQuad(float x)
        {
            return 1f - (1f - x) * (1f - x);
        }

        private void TryAddNewMessages()
        {
            if (_showHistory)
                return; 
            
            if (!_cachedEntries.Any())
                return;

            var maxMessages = Mathf.FloorToInt(ConsoleHeight / MessageHeight);
            if (_visibleEntries.Count >= maxMessages)
                return;

            var entry = _cachedEntries.Dequeue();
            entry.state = LogEntry.State.SlideIn;
            entry.stateTimer = 0f;

            entry.offsetY = SlideInOffset;
            entry.animatedY = entry.baseY + entry.offsetY;

            CreateEntryVisual(entry);

            _visibleEntries.Add(entry);
            RecalculateBaseY();
            entry.animatedY = entry.baseY + entry.offsetY;
        }
        
        private void AddHistoryEntryVisual(LogEntry entry)
        {
            var bg = GetBackground();
            bg.transform.SetParent(_historyContent, false);

            var bgRect = bg.rectTransform;
            bgRect.anchorMin = new Vector2(0, 1);
            bgRect.anchorMax = new Vector2(1, 1);
            bgRect.pivot     = new Vector2(0.5f, 1);
            bgRect.anchoredPosition = Vector2.zero;
            bgRect.sizeDelta = new Vector2(600f, MessageHeight);

            var text = GetText();
            var tRect = text.rectTransform;
            tRect.SetParent(bg.transform, false);

            tRect.anchorMin = new Vector2(0, 0);
            tRect.anchorMax = new Vector2(1, 1);
            tRect.pivot     = new Vector2(0, 0.5f);
            tRect.offsetMin = new Vector2(8f, 4f);
            tRect.offsetMax = new Vector2(-8f, -4f);

            entry.text       = text;
            entry.background = bg;

            text.color = Color.white;
            bg.color   = new Color(0, 0, 0, 0.8f);

            text.text = Colorize(entry.message);

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(text.rectTransform);
            var height = Mathf.Max(MessageHeight, text.preferredHeight + 8f);
            var layoutElement = bg.GetComponent<LayoutElement>();
            if (!layoutElement)
                layoutElement = bg.gameObject.AddComponent<LayoutElement>();

            layoutElement.preferredHeight = height;
        }

        private void CreateEntryVisual(LogEntry entry)
        {
            var bg = GetBackground();
            bg.transform.SetParent(_messageParent, false);

            var bgRect = bg.rectTransform;
            bgRect.anchorMin = new Vector2(0, 0);
            bgRect.anchorMax = new Vector2(0, 0);
            bgRect.pivot = new Vector2(0, 0);
            bgRect.sizeDelta = new Vector2(600f, MessageHeight);

            var text = GetText();
            var tRect = text.rectTransform;
            tRect.SetParent(bg.transform, false);

            tRect.anchorMin = new Vector2(0, 0);
            tRect.anchorMax = new Vector2(1, 1);
            tRect.pivot = new Vector2(0, 0.5f);
            tRect.offsetMin = new Vector2(8f, 4f);
            tRect.offsetMax = new Vector2(-8f, -4f);

            entry.text = text;
            entry.background = bg;
            text.color = new Color(1, 1, 1, 1);
            bg.color = new Color(0, 0, 0, 0.8f);

            UpdateEntryVisual(entry);
        }

        private void UpdateEntryVisual(LogEntry entry)
        {
            entry.text.text = Colorize(entry.message);

            var bgRect = entry.background.rectTransform;
            var textHeight = entry.text.preferredHeight;
            entry.height = Mathf.Max(MessageHeight, textHeight + 8f);
            bgRect.sizeDelta = new Vector2(bgRect.sizeDelta.x, entry.height);

            var targetY = entry.baseY + entry.offsetY;
            entry.animatedY = Mathf.Lerp(entry.animatedY, targetY, Time.deltaTime * 12f);

            entry.background.rectTransform.anchoredPosition =
                new Vector2(0f, entry.animatedY);
        }

        private TextMeshProUGUI GetText()
        {
            if (_textPool.Count > 0)
            {
                var t = _textPool.Dequeue();
                t.gameObject.SetActive(true);
                return t;
            }

            var go = new GameObject("LogText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            var t2 = go.GetComponent<TextMeshProUGUI>();
            t2.fontSize = 19;
            t2.color = Color.white;
            t2.font = shumiFont;
            t2.wordSpacing = 20f;
            t2.alignment = TextAlignmentOptions.MidlineLeft;
            return t2;
        }

        private Image GetBackground()
        {
            if (_backgroundPool.Count > 0)
            {
                var img = _backgroundPool.Dequeue();
                img.gameObject.SetActive(true);
                return img;
            }

            var go = new GameObject("LogBG");
            var imgNew = go.AddComponent<Image>();
            imgNew.color = new Color(0, 0, 0, 0.8f);
            imgNew.type = Image.Type.Sliced;

            return imgNew;
        }

        private void RecycleEntry(LogEntry entry)
        {
            entry.text.gameObject.SetActive(false);
            entry.background.gameObject.SetActive(false);

            _textPool.Enqueue(entry.text);
            _backgroundPool.Enqueue(entry.background);
        }

        private string Colorize(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            List<string> tokens = Tokenize(input);
            ApplyMultiWordColoring(tokens);
            ApplySingleWordColoring(tokens);
            return string.Concat(tokens);
        }
        
        private List<string> Tokenize(string input)
        {
            var tokens = new List<string>();
            var sb = new StringBuilder();

            if (string.IsNullOrEmpty(input))
                return tokens;

            bool IsWordChar(char c) => char.IsLetterOrDigit(c) || c == '\'';
            var lastWasWord = IsWordChar(input[0]);

            foreach (var c in input)
            {
                var isWordChar = IsWordChar(c);
                if (isWordChar != lastWasWord)
                {
                    tokens.Add(sb.ToString());
                    sb.Clear();
                }
                sb.Append(c);
                lastWasWord = isWordChar;
            }

            if (sb.Length > 0)
                tokens.Add(sb.ToString());
            return tokens;
        }
        
        private void ApplySingleWordColoring(List<string> tokens)
        {
            var singleKeys = KeywordColors
                .Where(kvp => !kvp.Key.Contains(" "))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            for (var i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];

                if (!IsWord(token))
                    continue;

                var lower = token.ToLowerInvariant();

                if (singleKeys.TryGetValue(lower, out var color))
                {
                    tokens[i] = $"<color={color}>{token}</color>";
                }
            }
        }
        
        private void ApplyMultiWordColoring(List<string> tokens)
        {
            var multiKeys = KeywordColors
                .Where(kvp => kvp.Key.Contains(" "))
                .Select(kvp => new
                {
                    Words = kvp.Key.Split(' '),
                    Color = kvp.Value
                })
                .OrderByDescending(kvp => kvp.Words.Length) 
                .ToList();

            if (multiKeys.Count == 0 || tokens.Count == 0)
                return;
            
            var wordIndices = new List<int>();
            for (var i = 0; i < tokens.Count; i++)
                if (IsWord(tokens[i]))
                    wordIndices.Add(i);
            
            var wi = 0;
            while (wi < wordIndices.Count)
            {
                var matchedAny = false;

                foreach (var key in multiKeys)
                {
                    var wordCount = key.Words.Length;
                    if (wi + wordCount > wordIndices.Count)
                        continue;

                    var match = true;
                    for (var k = 0; k < wordCount; k++)
                    {
                        var token = tokens[wordIndices[wi + k]];
                        if (token.Equals(key.Words[k], StringComparison.InvariantCultureIgnoreCase)) 
                            continue;
                        match = false;
                        break;
                    }

                    if (!match)
                        continue;

                    var startTokenIndex = wordIndices[wi];
                    var endTokenIndex = wordIndices[wi + wordCount - 1];
                    var phraseText = string.Concat(tokens.Skip(startTokenIndex)
                        .Take(endTokenIndex - startTokenIndex + 1));
                    tokens[startTokenIndex] = $"<color={key.Color}>{phraseText}</color>";
                    for (var t = startTokenIndex + 1; t <= endTokenIndex; t++)
                        tokens[t] = string.Empty;
                    wi += wordCount;
                    matchedAny = true;
                    break;
                }

                if (!matchedAny)
                    wi++;
            }
        }
        
        private bool IsWord(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;
            var hasLetterOrDigit = false;
            for (var i = 0; i < token.Length; i++)
            {
                var c = token[i];
                if (char.IsLetterOrDigit(c))
                {
                    hasLetterOrDigit = true;
                    continue;
                }
                if (c != '\'' || i <= 0 || i >= token.Length - 1) 
                    return false;
                if (char.IsLetterOrDigit(token[i - 1]) && char.IsLetterOrDigit(token[i + 1]))
                    continue;
                return false;
            }
            return hasLetterOrDigit;
        }

        public void Log(string text)
        {
            var entry = new LogEntry(text);

            _cachedEntries.Enqueue(entry);

            _historyEntries.Add(new LogEntry(text));

            if (_historyPanel != null && _historyPanel.activeSelf)
                AddHistoryEntryVisual(new LogEntry(entry.message));
        }
        
        private void ToggleHistory()
        {
            _showHistory = !_showHistory;
            
            _messageParent.gameObject.SetActive(!_showHistory);

            _historyPanel.SetActive(_showHistory);

            if (_showHistory)
            {
                foreach (var e in _visibleEntries)
                {
                    if (e.text != null)      e.text.gameObject.SetActive(false);
                    if (e.background != null) e.background.gameObject.SetActive(false);

                    _textPool.Enqueue(e.text);
                    _backgroundPool.Enqueue(e.background);
                }

                _visibleEntries.Clear();
                _cachedEntries.Clear();

                RebuildHistory();
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = false;
                _messageParent.gameObject.SetActive(_showConsole);
            }
        }

        private void ToggleConsole()
        {
            _showConsole = !_showConsole;

            foreach (var e in _visibleEntries)
            {
                if (e.background != null)
                    e.background.gameObject.SetActive(_showConsole);
                if (e.text != null)
                    e.text.gameObject.SetActive(_showConsole);
            }

            _messageParent.gameObject.SetActive(_showConsole);

            if (_showConsole) 
                return;
            _showHistory = false;
            _historyPanel.SetActive(false);
        }

        private void BuildUI()
        {
            var canvasObject = new GameObject("APConsoleCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(transform);

            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 2000;

            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            var container = new GameObject("Messages", typeof(RectTransform));
            var rect = container.GetComponent<RectTransform>();
            rect.SetParent(canvasObject.transform, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0f, 0f);
            rect.anchoredPosition = new Vector2(PaddingX, PaddingY);
            _messageParent = container.transform;

            _historyPanel = new GameObject("HistoryPanel", typeof(RectTransform));
            var historyRect = _historyPanel.GetComponent<RectTransform>();
            historyRect.SetParent(canvasObject.transform, false);

            historyRect.anchorMin = new Vector2(0f, 0f);
            historyRect.anchorMax = new Vector2(0f, 0f);
            historyRect.pivot     = new Vector2(0f, 0f);
            historyRect.anchoredPosition = new Vector2(PaddingX, PaddingY);
            historyRect.sizeDelta = new Vector2(600f, ConsoleHeight);

            _historyPanel.SetActive(false);

            _historyScrollRect = _historyPanel.AddComponent<ScrollRect>();
            _historyScrollRect.horizontal = false;
            _historyScrollRect.vertical = true;
            _historyScrollRect.scrollSensitivity = 10f;
            _historyScrollRect.movementType = ScrollRect.MovementType.Clamped;

            var viewport = new GameObject("Viewport",
                typeof(RectTransform),
                typeof(Image),
                typeof(Mask));
            _historyViewport = viewport.GetComponent<RectTransform>();
            viewport.transform.SetParent(_historyPanel.transform, false);

            _historyViewport.anchorMin = Vector2.zero;
            _historyViewport.anchorMax = Vector2.one;
            _historyViewport.offsetMin = Vector2.zero;
            _historyViewport.offsetMax = Vector2.zero;

            var vpImage = viewport.GetComponent<Image>();
            vpImage.color = Color.white;
            vpImage.type = Image.Type.Simple;
            vpImage.raycastTarget = true;

            var mask = viewport.GetComponent<Mask>();
            mask.showMaskGraphic = false;  

            _historyScrollRect.viewport = _historyViewport;
            var content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            var contentRect = content.GetComponent<RectTransform>();
            contentRect.SetParent(viewport.transform, false);

            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot     = new Vector2(0.5f, 1);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(0, 0);

            var layout = content.GetComponent<VerticalLayoutGroup>();
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth  = true;
            layout.childControlHeight     = true;
            layout.childControlWidth      = true;
            layout.spacing                = 8;
            layout.childAlignment         = TextAnchor.UpperLeft;

            var fitter = content.GetComponent<ContentSizeFitter>();
            fitter.verticalFit   = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            _historyScrollRect.content = contentRect;
            _historyContent = contentRect;
        }
        
        private void RebuildHistory()
        {
            if (_historyContent == null)
                return;
            for (var i = _historyContent.childCount - 1; i >= 0; i--)
                Destroy(_historyContent.GetChild(i).gameObject);
            foreach (var entry in _historyEntries)
                AddHistoryEntryVisual(entry);
            
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_historyContent);
            Canvas.ForceUpdateCanvases();
            _historyScrollRect.verticalNormalizedPosition = 0f;
        }

        [Serializable]
        public class LogEntry
        {
            public enum State
            {
                SlideIn,
                Hold,
                FadeOut
            }

            public State state = State.SlideIn;

            public float stateTimer;
            public float offsetY;
            public float baseY;
            public float animatedY;

            public TextMeshProUGUI text;
            public Image background;

            public string message;
            public float height = MessageHeight;

            public LogEntry(string msg)
            {
                message = msg;
            }
        }
    }
}
