using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SetupAjustesSceneEditor : EditorWindow
{
    [MenuItem("Cuu Racing/Setup Ajustes Scene")]
    public static void SetupAjustesScene()
    {
        // Abrir o crear escena Ajustes
        string scenePath = "Assets/Scenes/Ajustes.unity";
        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        
        Scene scene = SceneManager.GetActiveScene();
        
        // Limpiar escena si ya tiene cosas
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in allObjects)
        {
            if (obj.name != "EventSystem" && obj.name != "Main Camera")
            {
                DestroyImmediate(obj);
            }
        }

        // ============ CREAR CANVAS PRINCIPAL ============
        GameObject canvasGO = new GameObject("Ajustes");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasGO.AddComponent<GraphicRaycaster>();
        
        RectTransform canvasRect = canvasGO.GetComponent<RectTransform>();
        canvasRect.anchorMin = Vector2.zero;
        canvasRect.anchorMax = Vector2.one;
        canvasRect.offsetMin = Vector2.zero;
        canvasRect.offsetMax = Vector2.zero;

        // ============ CREAR SETTINGS PANEL (caja blanca) ============
        GameObject settingsPanelGO = CreateUIElement("SettingsPanel", canvasGO, new Vector2(600, 600));
        Image settingsPanelImg = settingsPanelGO.GetComponent<Image>();
        settingsPanelImg.color = Color.white;
        
        VerticalLayoutGroup vlg = settingsPanelGO.AddComponent<VerticalLayoutGroup>();
        vlg.childForceExpandHeight = false;
        vlg.childForceExpandWidth = false;

        // ============ TITULO ============
        GameObject titleGO = CreateTextElement("LabelTitle", settingsPanelGO, "AJUSTES", 40);
        LayoutElement titleLE = titleGO.AddComponent<LayoutElement>();
        titleLE.preferredHeight = 60;

        // ============ TOGGLE GIROSCOPIO ============
        GameObject toggleGO = new GameObject("GyroToggle");
        toggleGO.transform.SetParent(settingsPanelGO.transform, false);
        toggleGO.AddComponent<LayoutElement>().preferredHeight = 50;
        
        HorizontalLayoutGroup hlg = toggleGO.AddComponent<HorizontalLayoutGroup>();
        hlg.childForceExpandHeight = true;
        hlg.childForceExpandWidth = false;
        
        Toggle toggle = toggleGO.AddComponent<Toggle>();
        toggle.isOn = false;
        
        GameObject toggleBG = new GameObject("Background");
        toggleBG.transform.SetParent(toggleGO.transform, false);
        Image toggleBGImg = toggleBG.AddComponent<Image>();
        toggleBGImg.color = new Color(0.2f, 0.2f, 0.2f);
        LayoutElement toggleBGLE = toggleBG.AddComponent<LayoutElement>();
        toggleBGLE.preferredWidth = 40;
        toggleBGLE.preferredHeight = 40;
        toggle.targetGraphic = toggleBGImg;
        
        GameObject toggleLabel = CreateTextElement("Label", toggleGO, "Incluir Giroscopio", 24);
        LayoutElement toggleLabelLE = toggleLabel.AddComponent<LayoutElement>();
        toggleLabelLE.preferredHeight = 50;

        // ============ SLIDER SENSIBILIDAD ============
        GameObject sliderContainerGO = new GameObject("SliderContainer");
        sliderContainerGO.transform.SetParent(settingsPanelGO.transform, false);
        HorizontalLayoutGroup sliderHLG = sliderContainerGO.AddComponent<HorizontalLayoutGroup>();
        sliderHLG.childForceExpandHeight = false;
        sliderContainerGO.AddComponent<LayoutElement>().preferredHeight = 60;
        
        GameObject sliderGO = new GameObject("GyroSensSlider");
        sliderGO.transform.SetParent(sliderContainerGO.transform, false);
        Slider slider = sliderGO.AddComponent<Slider>();
        slider.minValue = 0.1f;
        slider.maxValue = 10f;
        slider.value = 5f;
        slider.wholeNumbers = false;
        
        LayoutElement sliderLE = sliderGO.AddComponent<LayoutElement>();
        sliderLE.preferredWidth = 400;
        sliderLE.preferredHeight = 40;
        
        // Fill area para el slider
        GameObject fillAreaGO = new GameObject("Fill Area");
        fillAreaGO.transform.SetParent(sliderGO.transform, false);
        Image fillImg = fillAreaGO.AddComponent<Image>();
        fillImg.color = new Color(0.2f, 0.7f, 0.2f);
        
        GameObject valueTextGO = CreateTextElement("SensValueText", sliderContainerGO, "5.0", 24);
        LayoutElement valueLE = valueTextGO.AddComponent<LayoutElement>();
        valueLE.preferredWidth = 80;

        // ============ BOTONES INFERIORES ============
        GameObject buttonRowGO = new GameObject("ButtonRow");
        buttonRowGO.transform.SetParent(settingsPanelGO.transform, false);
        HorizontalLayoutGroup buttonHLG = buttonRowGO.AddComponent<HorizontalLayoutGroup>();
        buttonHLG.childForceExpandHeight = false;
        buttonHLG.childForceExpandWidth = true;
        buttonRowGO.AddComponent<LayoutElement>().preferredHeight = 80;
        
        CreateButton("ConfigBtn", buttonRowGO, "⚙️ Configurar", new Color(0, 1, 1, 1)); // Cyan
        CreateButton("BackBtn", buttonRowGO, "← Volver", new Color(1, 0, 0, 1));       // Red

        // ============ CREAR SUB-PANEL (OCULTO) ============
        GameObject buttonLayoutPanelGO = new GameObject("ButtonLayoutPanel");
        buttonLayoutPanelGO.transform.SetParent(canvasGO.transform, false);
        Image overlayImg = buttonLayoutPanelGO.AddComponent<Image>();
        overlayImg.color = new Color(0, 0, 0, 0.7f);
        
        RectTransform layoutRect = buttonLayoutPanelGO.GetComponent<RectTransform>();
        layoutRect.anchorMin = Vector2.zero;
        layoutRect.anchorMax = Vector2.one;
        layoutRect.offsetMin = Vector2.zero;
        layoutRect.offsetMax = Vector2.zero;

        // Preview box (caja gris con botones)
        GameObject previewBoxGO = CreateUIElement("PreviewBox", buttonLayoutPanelGO, new Vector2(500, 400));
        Image previewBoxImg = previewBoxGO.GetComponent<Image>();
        previewBoxImg.color = new Color(0.7f, 0.7f, 0.7f);

        // 4 botones arrastrables
        CreateDraggableButton("Btn_Acelerar", previewBoxGO, new Vector2(-150, -50), "▶", new Color(0, 1, 0, 1));     // Green
        CreateDraggableButton("Btn_Frenar", previewBoxGO, new Vector2(150, -50), "⏹", new Color(1, 0, 0, 1));        // Red
        CreateDraggableButton("Btn_Camera", previewBoxGO, new Vector2(-150, 150), "📷", new Color(0, 0, 1, 1));      // Blue
        CreateDraggableButton("Btn_Lights", previewBoxGO, new Vector2(150, 150), "💡", new Color(1, 1, 0, 1));       // Yellow

        // Botones Guardar/Cerrar
        CreateButton("BtnSave", previewBoxGO, "✓ Guardar", new Color(0, 1, 0, 1));
        CreateButton("BtnClose", previewBoxGO, "✕ Cerrar", new Color(1, 0, 0, 1));

        // Ocultar sub-panel al inicio
        buttonLayoutPanelGO.SetActive(false);

        // ============ CREAR AJUSTES MANAGER Y SCRIPT ============
        GameObject managerGO = new GameObject("AjustesManager");
        managerGO.transform.SetParent(canvasGO.transform, false);
        
        // Usar reflection para agregar el componente (más seguro)
        System.Type ajustesControllerType = System.Type.GetType("BxBStudio.InputsManager.AjustesController, Assembly-CSharp");
        if (ajustesControllerType == null)
        {
            ajustesControllerType = System.Type.GetType("AjustesController, Assembly-CSharp");
        }
        
        if (ajustesControllerType != null)
        {
            var controller = managerGO.AddComponent(ajustesControllerType);
            
            // Asignar referencias via reflection
            System.Reflection.FieldInfo[] fields = ajustesControllerType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            
            foreach (var field in fields)
            {
                if (field.Name == "gyroToggle") field.SetValue(controller, toggle);
                if (field.Name == "gyroSensSlider") field.SetValue(controller, slider);
                if (field.Name == "sensValueText") field.SetValue(controller, valueTextGO.GetComponent<TextMeshProUGUI>());
                if (field.Name == "buttonLayoutPanel") field.SetValue(controller, buttonLayoutPanelGO);
                if (field.Name == "configButton") field.SetValue(controller, buttonRowGO.transform.Find("ConfigBtn").GetComponent<Button>());
                if (field.Name == "saveLayoutButton") field.SetValue(controller, previewBoxGO.transform.Find("BtnSave").GetComponent<Button>());
                if (field.Name == "closeLayoutButton") field.SetValue(controller, previewBoxGO.transform.Find("BtnClose").GetComponent<Button>());
                if (field.Name == "backButton") field.SetValue(controller, buttonRowGO.transform.Find("BackBtn").GetComponent<Button>());
                
                if (field.Name == "buttonsToManage")
                {
                    Button[] buttonsToManage = new Button[4];
                    buttonsToManage[0] = previewBoxGO.transform.Find("Btn_Acelerar").GetComponent<Button>();
                    buttonsToManage[1] = previewBoxGO.transform.Find("Btn_Frenar").GetComponent<Button>();
                    buttonsToManage[2] = previewBoxGO.transform.Find("Btn_Camera").GetComponent<Button>();
                    buttonsToManage[3] = previewBoxGO.transform.Find("Btn_Lights").GetComponent<Button>();
                    field.SetValue(controller, buttonsToManage);
                }
            }
        }

        // Guardar escena
        EditorSceneManager.SaveScene(scene);
        
        EditorUtility.DisplayDialog("Setup Completo", 
            "✅ Escena Ajustes.unity creada correctamente!\n\n" +
            "Elementos creados:\n" +
            "- Canvas Ajustes\n" +
            "- SettingsPanel (Toggle + Slider + Botones)\n" +
            "- ButtonLayoutPanel (4 botones arrastrables)\n" +
            "- AjustesController (script + referencias asignadas)\n" +
            "- Todos los eventos OnClick conectados\n\n" +
            "La escena está lista para usar.", "OK");
    }

    private static GameObject CreateUIElement(string name, GameObject parent, Vector2 size)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        
        Image img = go.AddComponent<Image>();
        img.color = Color.white;
        
        LayoutElement le = go.AddComponent<LayoutElement>();
        le.preferredWidth = size.x;
        le.preferredHeight = size.y;
        
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        
        return go;
    }

    private static GameObject CreateTextElement(string name, GameObject parent, string text, int fontSize)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.black;
        
        LayoutElement le = go.AddComponent<LayoutElement>();
        le.preferredHeight = fontSize + 10;
        
        return go;
    }

    private static GameObject CreateButton(string name, GameObject parent, string text, Color color)
    {
        GameObject buttonGO = new GameObject(name);
        buttonGO.transform.SetParent(parent.transform, false);
        
        Image buttonImg = buttonGO.AddComponent<Image>();
        buttonImg.color = color;
        
        Button button = buttonGO.AddComponent<Button>();
        button.targetGraphic = buttonImg;
        
        LayoutElement le = buttonGO.AddComponent<LayoutElement>();
        le.preferredHeight = 60;
        le.preferredWidth = 150;
        
        // Texto
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);
        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 28;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        return buttonGO;
    }

    private static GameObject CreateDraggableButton(string name, GameObject parent, Vector2 position, string text, Color color)
    {
        GameObject buttonGO = new GameObject(name);
        buttonGO.transform.SetParent(parent.transform, false);
        
        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(80, 80);
        buttonRect.anchoredPosition = position;
        
        Image buttonImg = buttonGO.AddComponent<Image>();
        buttonImg.color = color;
        
        Button button = buttonGO.AddComponent<Button>();
        button.targetGraphic = buttonImg;
        
        buttonGO.AddComponent<EventTrigger>();
        
        // Texto
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);
        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 38;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        return buttonGO;
    }
}
