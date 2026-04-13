using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using CuuRacing.UI;

namespace CuuRacing.Editor
{
    public static class MainMenuBuilder
    {
        // ── Paths to SlimUI assets ───────────────────────────────────
        const string PATH_BUTTON_SPRITE  = "Assets/SlimUI/Modern Menu 1/Graphics/Buttons/Button Fram 256px.png";
        const string PATH_BUTTON_HOVER   = "Assets/SlimUI/Modern Menu 1/Graphics/Buttons/Button Frame Hover 256px.png";
        const string PATH_BUTTON_PRESS   = "Assets/SlimUI/Modern Menu 1/Graphics/Buttons/Button Frame Press 256px.png";
        const string PATH_FONT_BOLD      = "Assets/SlimUI/Modern Menu 1/Fonts/RUBIK-BOLD SDF.asset";
        const string PATH_FONT_LIGHT     = "Assets/SlimUI/Modern Menu 1/Fonts/RUBIK-LIGHT SDF.asset";
        const string PATH_SFX_HOVER      = "Assets/SlimUI/Modern Menu 1/Audio/Clicks/SFX_Click_Mechanical.mp3";
        const string PATH_SFX_CLICK      = "Assets/SlimUI/Modern Menu 1/Audio/Clicks/SFX_Click_Punch.ogg";
        const string PATH_SCENE          = "Assets/Scenes/MainMenu.unity";
        const string PATH_GAME_SCENE     = "Assets/BxB Studio/MVC Getting Started - Mobile/Scenes/DefaultScene - Mobile.unity";
        const string RACE_SCENE_NAME     = "DefaultScene - Mobile";

        // ── Colors ───────────────────────────────────────────────────
        static readonly Color COLOR_CYAN        = new Color(0f,    0.898f, 1f,    0.75f);
        static readonly Color COLOR_CYAN_HOVER  = new Color(0f,    0.898f, 1f,    1f);
        static readonly Color COLOR_CYAN_PRESS  = new Color(0f,    0.6f,   0.8f,  1f);
        static readonly Color COLOR_BG_FALLBACK = new Color(0.05f, 0.05f,  0.12f, 1f);
        static readonly Color COLOR_SETTINGS_BG = new Color(0.05f, 0.05f,  0.12f, 0.96f);
        static readonly Color COLOR_TRANSPARENT  = new Color(0,    0,      0,     0);
        static readonly Color COLOR_BLACK_FULL   = Color.black;
        static readonly Color COLOR_WHITE        = Color.white;

        [MenuItem("CuuRacing/Build Main Menu Scene")]
        public static void BuildMainMenuScene()
        {
            // ── 1. New scene ─────────────────────────────────────────
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // ── 2. Load external assets ──────────────────────────────
            var spriteBtn   = AssetDatabase.LoadAssetAtPath<Sprite>(PATH_BUTTON_SPRITE);
            var spriteHover = AssetDatabase.LoadAssetAtPath<Sprite>(PATH_BUTTON_HOVER);
            var spritePress = AssetDatabase.LoadAssetAtPath<Sprite>(PATH_BUTTON_PRESS);
            var fontBold    = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(PATH_FONT_BOLD);
            var fontLight   = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(PATH_FONT_LIGHT);
            var sfxHover    = AssetDatabase.LoadAssetAtPath<AudioClip>(PATH_SFX_HOVER);
            var sfxClick    = AssetDatabase.LoadAssetAtPath<AudioClip>(PATH_SFX_CLICK);

            // Fallback to LiberationSans if SlimUI fonts not found
            if (fontBold == null)
                fontBold = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                    "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
            if (fontLight == null)
                fontLight = fontBold;

            // ── 3. Camera ────────────────────────────────────────────
            var camGO = new GameObject("Main Camera");
            var cam = camGO.AddComponent<Camera>();
            cam.clearFlags      = CameraClearFlags.SolidColor;
            cam.backgroundColor = COLOR_BG_FALLBACK;
            cam.orthographic    = true;
            camGO.tag = "MainCamera";
            AddAudioListener(camGO);

            // ── 4. Canvas root ───────────────────────────────────────
            var canvasGO = new GameObject("Canvas");
            var canvas   = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode          = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution  = new Vector2(1920, 1080);
            scaler.screenMatchMode      = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight   = 0.5f;

            canvasGO.AddComponent<GraphicRaycaster>();

            // ── 5. Background image ──────────────────────────────────
            var bg      = CreateUIObject("Img_Background", canvasGO.transform);
            var bgImage = bg.AddComponent<Image>();
            bgImage.color = COLOR_BG_FALLBACK;          // replaced by real sprite later
            SetStretch(bg.GetComponent<RectTransform>());

            // ── 6. Logo text ─────────────────────────────────────────
            var logoGO  = CreateUIObject("Txt_Logo", canvasGO.transform);
            var logoRT  = logoGO.GetComponent<RectTransform>();
            logoRT.anchorMin = new Vector2(0.5f, 1f);
            logoRT.anchorMax = new Vector2(0.5f, 1f);
            logoRT.pivot     = new Vector2(0.5f, 1f);
            logoRT.sizeDelta = new Vector2(900f, 150f);
            logoRT.anchoredPosition = new Vector2(0f, -60f);
            var logoTMP = logoGO.AddComponent<TextMeshProUGUI>();
            logoTMP.text      = "CUURACING";
            logoTMP.font      = fontBold;
            logoTMP.fontSize  = 110f;
            logoTMP.alignment = TextAlignmentOptions.Center;
            logoTMP.color     = COLOR_WHITE;
            logoTMP.fontStyle = FontStyles.Bold;

            // ── 7. Panel_Main (transparent full-screen container) ────
            var panelMain   = CreateUIObject("Panel_Main", canvasGO.transform);
            var panelMainRT = panelMain.GetComponent<RectTransform>();
            SetStretch(panelMainRT);
            var panelMainImg = panelMain.AddComponent<Image>();
            panelMainImg.color = COLOR_TRANSPARENT;

            // ── 8. Panel_Buttons ─────────────────────────────────────
            var panelBtns   = CreateUIObject("Panel_Buttons", panelMain.transform);
            var panelBtnsRT = panelBtns.GetComponent<RectTransform>();
            panelBtnsRT.anchorMin        = new Vector2(0.5f, 0f);
            panelBtnsRT.anchorMax        = new Vector2(0.5f, 0f);
            panelBtnsRT.pivot            = new Vector2(0.5f, 0f);
            panelBtnsRT.sizeDelta        = new Vector2(900f, 75f);
            panelBtnsRT.anchoredPosition = new Vector2(0f,  45f);

            var hlg = panelBtns.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing           = 30f;
            hlg.childAlignment    = TextAnchor.MiddleCenter;
            hlg.childForceExpandWidth  = false;
            hlg.childForceExpandHeight = false;
            hlg.childControlWidth      = false;
            hlg.childControlHeight     = false;

            // ── 9. Three buttons ─────────────────────────────────────
            CreateMenuButton("Btn_Jugar",   "JUGAR",   panelBtns.transform, spriteBtn, spriteHover, spritePress, fontBold);
            CreateMenuButton("Btn_Garage",  "GARAGE",  panelBtns.transform, spriteBtn, spriteHover, spritePress, fontBold);
            CreateMenuButton("Btn_Ajustes", "AJUSTES", panelBtns.transform, spriteBtn, spriteHover, spritePress, fontBold);

            // ── 10. Panel_Settings ───────────────────────────────────
            var panelSettings   = CreateUIObject("Panel_Settings", canvasGO.transform);
            var panelSettingsRT = panelSettings.GetComponent<RectTransform>();
            SetStretch(panelSettingsRT);
            var panelSettingsImg = panelSettings.AddComponent<Image>();
            panelSettingsImg.color = COLOR_SETTINGS_BG;

            // Settings title
            var settTitleGO  = CreateUIObject("Txt_SettingsTitle", panelSettings.transform);
            var settTitleRT  = settTitleGO.GetComponent<RectTransform>();
            settTitleRT.anchorMin        = new Vector2(0.5f, 1f);
            settTitleRT.anchorMax        = new Vector2(0.5f, 1f);
            settTitleRT.pivot            = new Vector2(0.5f, 1f);
            settTitleRT.sizeDelta        = new Vector2(700f, 100f);
            settTitleRT.anchoredPosition = new Vector2(0f, -80f);
            var settTitleTMP = settTitleGO.AddComponent<TextMeshProUGUI>();
            settTitleTMP.text      = "AJUSTES";
            settTitleTMP.font      = fontBold;
            settTitleTMP.fontSize  = 60f;
            settTitleTMP.alignment = TextAlignmentOptions.Center;
            settTitleTMP.color     = COLOR_WHITE;

            // Settings placeholder note
            var settNoteGO  = CreateUIObject("Txt_Placeholder", panelSettings.transform);
            var settNoteRT  = settNoteGO.GetComponent<RectTransform>();
            settNoteRT.anchorMin        = new Vector2(0.5f, 0.5f);
            settNoteRT.anchorMax        = new Vector2(0.5f, 0.5f);
            settNoteRT.pivot            = new Vector2(0.5f, 0.5f);
            settNoteRT.sizeDelta        = new Vector2(800f, 80f);
            settNoteRT.anchoredPosition = new Vector2(0f, 0f);
            var settNoteTMP = settNoteGO.AddComponent<TextMeshProUGUI>();
            settNoteTMP.text      = "Próximamente: Volumen, Gráficas, Controles";
            settNoteTMP.font      = fontLight;
            settNoteTMP.fontSize  = 30f;
            settNoteTMP.alignment = TextAlignmentOptions.Center;
            settNoteTMP.color     = new Color(1f, 1f, 1f, 0.5f);

            // Back button
            var btnBack = CreateMenuButton("Btn_Back", "← VOLVER", panelSettings.transform,
                                           spriteBtn, spriteHover, spritePress, fontBold);
            var btnBackRT = btnBack.GetComponent<RectTransform>();
            btnBackRT.anchorMin        = new Vector2(0.5f, 0f);
            btnBackRT.anchorMax        = new Vector2(0.5f, 0f);
            btnBackRT.pivot            = new Vector2(0.5f, 0f);
            btnBackRT.anchoredPosition = new Vector2(0f, 45f);

            panelSettings.SetActive(false);

            // ── 11. Panel_Loading ────────────────────────────────────
            var panelLoading   = CreateUIObject("Panel_Loading", canvasGO.transform);
            var panelLoadingRT = panelLoading.GetComponent<RectTransform>();
            SetStretch(panelLoadingRT);
            var panelLoadingImg = panelLoading.AddComponent<Image>();
            panelLoadingImg.color = COLOR_BLACK_FULL;

            var loadSliderGO = new GameObject("Slider_Loading");
            loadSliderGO.transform.SetParent(panelLoading.transform, false);
            var loadSliderRT = loadSliderGO.AddComponent<RectTransform>();
            loadSliderRT.anchorMin        = new Vector2(0.5f, 0f);
            loadSliderRT.anchorMax        = new Vector2(0.5f, 0f);
            loadSliderRT.pivot            = new Vector2(0.5f, 0f);
            loadSliderRT.sizeDelta        = new Vector2(800f, 20f);
            loadSliderRT.anchoredPosition = new Vector2(0f, 80f);
            var loadSlider = loadSliderGO.AddComponent<Slider>();
            loadSlider.minValue = 0f;
            loadSlider.maxValue = 1f;
            loadSlider.value    = 0f;
            // Style the fill cyan
            StyleSlider(loadSliderGO, loadSlider);

            var loadTextGO  = CreateUIObject("Txt_Loading", panelLoading.transform);
            var loadTextRT  = loadTextGO.GetComponent<RectTransform>();
            loadTextRT.anchorMin        = new Vector2(0.5f, 0.5f);
            loadTextRT.anchorMax        = new Vector2(0.5f, 0.5f);
            loadTextRT.pivot            = new Vector2(0.5f, 0.5f);
            loadTextRT.sizeDelta        = new Vector2(700f, 80f);
            loadTextRT.anchoredPosition = new Vector2(0f, 0f);
            var loadTMP = loadTextGO.AddComponent<TextMeshProUGUI>();
            loadTMP.text      = "Cargando...";
            loadTMP.font      = fontLight;
            loadTMP.fontSize  = 36f;
            loadTMP.alignment = TextAlignmentOptions.Center;
            loadTMP.color     = COLOR_WHITE;

            panelLoading.SetActive(false);

            // ── 12. EventSystem (New Input System) ───────────────────
            var esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            // Use InputSystemUIInputModule (New Input System) instead of
            // StandaloneInputModule which uses the legacy UnityEngine.Input API.
            esGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

            // ── 13. MenuManager + CuuRacingMenu ─────────────────────
            var managerGO = new GameObject("MenuManager");
            var menuScript = managerGO.AddComponent<CuuRacingMenu>();

            var audioHover = managerGO.AddComponent<AudioSource>();
            audioHover.playOnAwake = false;
            if (sfxHover != null) audioHover.clip = sfxHover;

            var audioClick = managerGO.AddComponent<AudioSource>();
            audioClick.playOnAwake = false;
            if (sfxClick != null) audioClick.clip = sfxClick;

            menuScript.mainPanel     = panelMain;
            menuScript.settingsPanel = panelSettings;
            menuScript.loadingPanel  = panelLoading;
            menuScript.loadingBar    = loadSlider;
            menuScript.loadingText   = loadTMP;
            menuScript.hoverSound    = audioHover;
            menuScript.clickSound    = audioClick;
            menuScript.raceSceneName = RACE_SCENE_NAME;

            // ── 14. Wire up button OnClick events ────────────────────
            WireButton(panelBtns.transform.Find("Btn_Jugar")?.gameObject,
                       managerGO, "OnJugarClick");
            WireButton(panelBtns.transform.Find("Btn_Garage")?.gameObject,
                       managerGO, "OnGarageClick");
            WireButton(panelBtns.transform.Find("Btn_Ajustes")?.gameObject,
                       managerGO, "OnAjustesClick");
            WireButton(panelSettings.transform.Find("Btn_Back")?.gameObject,
                       managerGO, "OnAjustesBack");

            // ── 15. Configure Build Settings (MainMenu = 0, Game = 1) ─
            var buildScenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene(PATH_SCENE,      true),
                new EditorBuildSettingsScene(PATH_GAME_SCENE, true),
            };
            EditorBuildSettings.scenes = buildScenes;

            // ── 16. Save scene ───────────────────────────────────────
            EditorSceneManager.SaveScene(scene, PATH_SCENE);
            AssetDatabase.Refresh();

            Debug.Log("[CuuRacing] ✅ Escena MainMenu creada en: " + PATH_SCENE);
            EditorUtility.DisplayDialog("CuuRacing",
                "Escena MainMenu.unity creada exitosamente en Assets/Scenes/\n\n" +
                "Recuerda:\n" +
                "• Agrega la escena en File > Build Settings\n" +
                "• Asigna tu imagen de fondo a Img_Background\n" +
                "• Crea el TMP Font Asset de Tokyo Drifter para el logo",
                "OK");
        }

        // ────────────────────────────────────────────────────────────
        // Helpers
        // ────────────────────────────────────────────────────────────

        static GameObject CreateUIObject(string name, Transform parent)
        {
            var go = new GameObject(name);
            go.AddComponent<RectTransform>();
            go.transform.SetParent(parent, false);
            return go;
        }

        static void SetStretch(RectTransform rt)
        {
            rt.anchorMin   = Vector2.zero;
            rt.anchorMax   = Vector2.one;
            rt.offsetMin   = Vector2.zero;
            rt.offsetMax   = Vector2.zero;
        }

        static void AddAudioListener(GameObject go)
        {
            if (go.GetComponent<AudioListener>() == null)
                go.AddComponent<AudioListener>();
        }

        static GameObject CreateMenuButton(string name, string label, Transform parent,
                                           Sprite spriteNormal, Sprite spriteHover,
                                           Sprite spritePress,  TMP_FontAsset font)
        {
            var btnGO = new GameObject(name);
            btnGO.transform.SetParent(parent, false);
            var btnRT = btnGO.AddComponent<RectTransform>();
            btnRT.sizeDelta = new Vector2(260f, 65f);

            var btnImage = btnGO.AddComponent<Image>();
            if (spriteNormal != null)
            {
                btnImage.sprite    = spriteNormal;
                btnImage.type      = Image.Type.Sliced;
            }
            btnImage.color = new Color(0f, 0.898f, 1f, 0.75f);

            var btn = btnGO.AddComponent<Button>();
            var cb  = btn.colors;
            cb.normalColor      = new Color(0f, 0.898f, 1f, 0.75f);
            cb.highlightedColor = new Color(0f, 0.898f, 1f, 1f);
            cb.pressedColor     = new Color(0f, 0.6f,   0.8f, 1f);
            cb.selectedColor    = cb.normalColor;
            cb.fadeDuration     = 0.1f;
            btn.colors          = cb;
            btn.targetGraphic   = btnImage;
            btn.navigation      = Navigation.defaultNavigation;
            btn.navigation      = new Navigation { mode = Navigation.Mode.None };

            // TMP label (no child Image needed — text only)
            var textGO  = new GameObject("Text");
            textGO.transform.SetParent(btnGO.transform, false);
            var textRT = textGO.AddComponent<RectTransform>();
            textRT.anchorMin   = Vector2.zero;
            textRT.anchorMax   = Vector2.one;
            textRT.offsetMin   = new Vector2(8f, 0f);
            textRT.offsetMax   = new Vector2(-8f, 0f);

            var tmp = textGO.AddComponent<TextMeshProUGUI>();
            tmp.text      = label;
            tmp.font      = font;
            tmp.fontSize  = 26f;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color     = Color.white;
            tmp.fontStyle = FontStyles.Bold;

            return btnGO;
        }

        static void StyleSlider(GameObject sliderGO, Slider slider)
        {
            // Background
            var bgGO  = new GameObject("Background");
            bgGO.transform.SetParent(sliderGO.transform, false);
            var bgRT  = bgGO.AddComponent<RectTransform>();
            bgRT.anchorMin   = Vector2.zero;
            bgRT.anchorMax   = Vector2.one;
            bgRT.offsetMin   = Vector2.zero;
            bgRT.offsetMax   = Vector2.zero;
            var bgImg = bgGO.AddComponent<Image>();
            bgImg.color = new Color(1f, 1f, 1f, 0.1f);

            // Fill Area
            var fillAreaGO = new GameObject("Fill Area");
            fillAreaGO.transform.SetParent(sliderGO.transform, false);
            var fillAreaRT = fillAreaGO.AddComponent<RectTransform>();
            fillAreaRT.anchorMin   = Vector2.zero;
            fillAreaRT.anchorMax   = Vector2.one;
            fillAreaRT.offsetMin   = Vector2.zero;
            fillAreaRT.offsetMax   = Vector2.zero;

            var fillGO  = new GameObject("Fill");
            fillGO.transform.SetParent(fillAreaGO.transform, false);
            var fillRT  = fillGO.AddComponent<RectTransform>();
            fillRT.anchorMin   = Vector2.zero;
            fillRT.anchorMax   = new Vector2(0f, 1f);
            fillRT.offsetMin   = Vector2.zero;
            fillRT.offsetMax   = Vector2.zero;
            var fillImg = fillGO.AddComponent<Image>();
            fillImg.color = new Color(0f, 0.898f, 1f, 1f);

            slider.fillRect = fillRT;
        }

        static void WireButton(GameObject btnGO, GameObject target, string methodName)
        {
            if (btnGO == null) return;
            var btn    = btnGO.GetComponent<Button>();
            if (btn == null) return;
            var script = target.GetComponent<CuuRacingMenu>();
            if (script == null) return;

            UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(
                btn.onClick,
                System.Delegate.CreateDelegate(
                    typeof(UnityEngine.Events.UnityAction),
                    script,
                    typeof(CuuRacingMenu).GetMethod(methodName)) as UnityEngine.Events.UnityAction
            );
        }
    }
}
