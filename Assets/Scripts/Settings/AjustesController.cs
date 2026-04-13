using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace CuuRacing.Settings
{
    /// <summary>
    /// Controlador principal de la escena Ajustes.
    /// Gestiona: Giroscopio (toggle + sensibilidad), Configuración de botones, etc.
    /// 
    /// SETUP EN INSPECTOR (Ajustes.unity):
    ///  - GyroToggle              : Toggle para activar/desactivar giroscopio
    ///  - GyroSensSlider          : Slider (0-10) para sensibilidad del giroscopio
    ///  - SensTextValue           : TMP_Text para mostrar valor actual de sensibilidad
    ///  - ConfigButtonsButton     : Botón "Configurar Posición de Botones"
        ///  - ButtonLayoutPanel       : GameObject/Panel donde aparecen botones arrastrables
    ///  - ButtonsToManage[]       : Array con los botones que se pueden mover
    ///  - SaveLayoutButton        : Botón para guardar layout
    ///  - CloseLayoutButton       : Botón para cerrar el configurador
    ///  - BackButton              : Botón para volver al Garage
    /// </summary>
    public class AjustesController : MonoBehaviour
    {
        [Header("Gyroscope UI")]
        [Tooltip("Toggle para activar/desactivar giroscopio")]
        public Toggle gyroToggle;

        [Tooltip("Slider para ajustar sensibilidad (0-10)")]
        public Slider gyroSensSlider;

        [Tooltip("Texto para mostrar el valor actual de sensibilidad")]
        public TextMeshProUGUI sensTextValue;

        [Header("Button Layout Configuration")]
        [Tooltip("Panel con los botones que se pueden reposicionar")]
        public GameObject buttonLayoutPanel;

        [Tooltip("Botones que se pueden mover")]
        public Button[] buttonsToManage;

        [Tooltip("Botón para guardar el layout")]
        public Button saveLayoutButton;

        [Tooltip("Botón para cerrar el configurador")]
        public Button closeLayoutButton;

        [Header("Navigation")]
        [Tooltip("Botón para volver al Garage")]
        public Button backButton;

        [Tooltip("Nombre exacto de la escena Garage")]
        public string garageSceneName = "Garage";

        [Header("Audio")]
        public AudioSource clickSound;

        // ── Constantes de PlayerPrefs ────────────────────────────────

        private const string GYRO_ENABLED_KEY = "GyroEnabled";
        private const string GYRO_SENSITIVITY_KEY = "GyroSensitivity";
        private const string LAYOUT_PREFS_KEY = "MobileButtonLayout";
        private const float MIN_SENSITIVITY = 0.1f;
        private const float MAX_SENSITIVITY = 10f;

        // ── Controles Dragging ───────────────────────────────────────

        private Dictionary<Button, Vector2> _initialPositions = new Dictionary<Button, Vector2>();
        private Button _currentDragging;
        private RectTransform _draggedRectTrans;
        private Vector2 _dragOffset;

        // ── Unity Lifecycle ──────────────────────────────────────────

        private void Start()
        {
            LoadSettings();
            BindUI();
            
            if (buttonLayoutPanel != null)
                buttonLayoutPanel.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_currentDragging != null)
                UpdateDrag();
        }

        // ── Cargar Ajustes ──────────────────────────────────────────

        private void LoadSettings()
        {
            // Cargar giroscopio (default: false)
            bool gyroEnabled = PlayerPrefs.GetInt(GYRO_ENABLED_KEY, 0) == 1;
            if (gyroToggle != null)
                gyroToggle.isOn = gyroEnabled;

            // Cargar sensibilidad (default: 5)
            float sensitivity = PlayerPrefs.GetFloat(GYRO_SENSITIVITY_KEY, 5f);
            sensitivity = Mathf.Clamp(sensitivity, MIN_SENSITIVITY, MAX_SENSITIVITY);
            
            if (gyroSensSlider != null)
                gyroSensSlider.value = sensitivity;

            UpdateSensitivityText(sensitivity);
        }

        // ── Bindings UI ──────────────────────────────────────────────

        private void BindUI()
        {
            if (gyroToggle != null)
                gyroToggle.onValueChanged.AddListener(OnGyroToggleChanged);

            if (gyroSensSlider != null)
                gyroSensSlider.onValueChanged.AddListener(OnSensitivitySliderChanged);

            if (backButton != null)
                backButton.onClick.AddListener(OnBackClick);

            // Botones del layout
            if (saveLayoutButton != null)
                saveLayoutButton.onClick.AddListener(OnSaveLayoutClick);

            if (closeLayoutButton != null)
                closeLayoutButton.onClick.AddListener(OnCloseLayoutClick);

            // Botón para abrir configurador
            var configBtn = GetComponent<Button>();
            if (configBtn == null)
            {
                // Buscar botón en UI
                Button[] allButtons = GetComponentsInChildren<Button>();
                foreach (Button btn in allButtons)
                {
                    if (btn.name.Contains("Config") || btn.name.Contains("Buttons"))
                    {
                        btn.onClick.AddListener(OnConfigButtonClick);
                        break;
                    }
                }
            }

            // Agregar EventTriggers a botones arrastrables
            if (buttonsToManage != null)
            {
                foreach (Button btn in buttonsToManage)
                {
                    if (btn != null)
                    {
                        EventTrigger trigger = btn.GetComponent<EventTrigger>();
                        if (trigger == null)
                            trigger = btn.gameObject.AddComponent<EventTrigger>();

                        // PointerDown
                        EventTrigger.Entry entryDown = new EventTrigger.Entry();
                        entryDown.eventID = EventTriggerType.PointerDown;
                        entryDown.callback.AddListener((data) => OnPointerDown(btn));
                        trigger.triggers.Add(entryDown);

                        // PointerUp
                        EventTrigger.Entry entryUp = new EventTrigger.Entry();
                        entryUp.eventID = EventTriggerType.PointerUp;
                        entryUp.callback.AddListener((data) => OnPointerUp(btn));
                        trigger.triggers.Add(entryUp);

                        // Guardar posición inicial
                        RectTransform rect = btn.GetComponent<RectTransform>();
                        if (rect != null)
                            _initialPositions[btn] = rect.anchoredPosition;
                    }
                }
            }
        }

        // ── Eventos Giroscopio ──────────────────────────────────────

        private void OnGyroToggleChanged(bool isOn)
        {
            PlayerPrefs.SetInt(GYRO_ENABLED_KEY, isOn ? 1 : 0);
            Debug.Log($"[AjustesController] Giroscopio: {(isOn ? "ACTIVADO" : "DESACTIVADO")}");
            PlayClick();
        }

        private void OnSensitivitySliderChanged(float value)
        {
            float sensitivity = Mathf.Clamp(value, MIN_SENSITIVITY, MAX_SENSITIVITY);
            PlayerPrefs.SetFloat(GYRO_SENSITIVITY_KEY, sensitivity);
            UpdateSensitivityText(sensitivity);
            Debug.Log($"[AjustesController] Sensibilidad del giroscopio: {sensitivity:F1}");
            PlayClick();
        }

        private void UpdateSensitivityText(float sensitivity)
        {
            if (sensTextValue != null)
                sensTextValue.text = $"{sensitivity:F1}";
        }

        // ── Eventos Layout ──────────────────────────────────────────

        public void OnConfigButtonClick()
        {
            Debug.Log("[AjustesController] Abriendo configurador de posición de botones...");
            if (buttonLayoutPanel != null)
                buttonLayoutPanel.gameObject.SetActive(true);
        }

        public void OnSaveLayoutClick()
        {
            SaveLayout();
            OnCloseLayoutClick();
        }

        public void OnCloseLayoutClick()
        {
            if (buttonLayoutPanel != null)
                buttonLayoutPanel.gameObject.SetActive(false);
        }

        public void OnBackClick()
        {
            PlayClick();
            SceneManager.LoadScene(garageSceneName);
        }

        // ── Drag & Drop ──────────────────────────────────────────────

        private void OnPointerDown(Button btn)
        {
            _currentDragging = btn;
            _draggedRectTrans = btn.GetComponent<RectTransform>();
            
            if (_draggedRectTrans != null && buttonLayoutPanel != null)
            {
                Canvas canvas = buttonLayoutPanel.GetComponentInParent<Canvas>();
                Camera cam = canvas != null ? canvas.worldCamera : null;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    (RectTransform)buttonLayoutPanel.transform,
                    Input.mousePosition,
                    cam,
                    out Vector2 localPos
                );
                _dragOffset = localPos - _draggedRectTrans.anchoredPosition;
            }
        }

        private void OnPointerUp(Button btn)
        {
            if (_currentDragging == btn)
                _currentDragging = null;
        }

        private void UpdateDrag()
        {
            if (_draggedRectTrans == null || buttonLayoutPanel == null) return;

            Canvas canvas = buttonLayoutPanel.GetComponentInParent<Canvas>();
            Camera cam = canvas != null ? canvas.worldCamera : null;
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)buttonLayoutPanel.transform,
                Input.mousePosition,
                cam,
                out Vector2 localPos
            );

            _draggedRectTrans.anchoredPosition = localPos - _dragOffset;
        }

        // ── Guardar/Cargar Layout ───────────────────────────────────

        private void SaveLayout()
        {
            if (buttonsToManage == null || buttonsToManage.Length == 0)
                return;

            LayoutData layoutData = new LayoutData();
            layoutData.buttonPositions = new Vector2[buttonsToManage.Length];
            layoutData.buttonNames = new string[buttonsToManage.Length];

            for (int i = 0; i < buttonsToManage.Length; i++)
            {
                if (buttonsToManage[i] != null)
                {
                    RectTransform rect = buttonsToManage[i].GetComponent<RectTransform>();
                    if (rect != null)
                    {
                        layoutData.buttonPositions[i] = rect.anchoredPosition;
                        layoutData.buttonNames[i] = buttonsToManage[i].name;
                    }
                }
            }

            string json = JsonUtility.ToJson(layoutData, true);
            PlayerPrefs.SetString(LAYOUT_PREFS_KEY, json);
            PlayerPrefs.Save();

            Debug.Log("[AjustesController] Layout guardado correctamente");
            PlayClick();
        }

        /// <summary>Cargar layout guardado (usar en escenas de carrera)</summary>
        public static void LoadLayout(Button[] buttons)
        {
            if (!PlayerPrefs.HasKey(LAYOUT_PREFS_KEY))
                return;

            if (buttons == null || buttons.Length == 0)
                return;

            string json = PlayerPrefs.GetString(LAYOUT_PREFS_KEY, "");
            if (string.IsNullOrEmpty(json))
                return;

            try
            {
                LayoutData layoutData = JsonUtility.FromJson<LayoutData>(json);
                
                for (int i = 0; i < buttons.Length && i < layoutData.buttonPositions.Length; i++)
                {
                    if (buttons[i] != null)
                    {
                        RectTransform rect = buttons[i].GetComponent<RectTransform>();
                        if (rect != null)
                            rect.anchoredPosition = layoutData.buttonPositions[i];
                    }
                }

                Debug.Log("[AjustesController] Layout cargado correctamente");
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[AjustesController] Error cargando layout: {ex.Message}");
            }
        }

        // ── Audio ────────────────────────────────────────────────────

        private void PlayClick()
        {
            if (clickSound != null)
                clickSound.Play();
        }

        // ── Static Helpers ───────────────────────────────────────────

        /// <summary>Obtener si el giroscopio está habilitado</summary>
        public static bool IsGyroEnabled()
        {
            return PlayerPrefs.GetInt(GYRO_ENABLED_KEY, 0) == 1;
        }

        /// <summary>Obtener sensibilidad del giroscopio (0.1 - 10.0)</summary>
        public static float GetGyroSensitivity()
        {
            return PlayerPrefs.GetFloat(GYRO_SENSITIVITY_KEY, 5f);
        }

        // ── Data Structure para JSON ────────────────────────────────

        [System.Serializable]
        public class LayoutData
        {
            public Vector2[] buttonPositions;
            public string[] buttonNames;
        }
    }
}
