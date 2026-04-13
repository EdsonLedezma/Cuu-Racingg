using UnityEngine;
using UnityEngine.UI;

namespace CuuRacing.Mobile
{
    /// <summary>
    /// Botón mobile para encender/apagar luces del vehículo.
    /// 
    /// Nota: Requiere investigación en MVC para determinar cómo se controlan las luces.
    /// Por ahora, busca Light components en el auto y los alterna.
    /// 
    /// SETUP EN INSPECTOR:
    ///  - Asignar a un botón en el Canvas de la escena de carrera
    /// </summary>
    public class LightButtonMobile : MonoBehaviour
    {
        [Header("Button Reference")]
        [Tooltip("Botón que ejecuta toggle de luces")]
        public Button lightsButton;

        [Header("Debug")]
        public bool showDebugLogs = false;

        // ── Variables Privadas ───────────────────────────────────────

        private LightController _lightController;
        private bool _lightsOn = false;

        // ── Unity Lifecycle ──────────────────────────────────────────

        private void Start()
        {
            InitializeLights();
            BindButton();
        }

        // ── Initialize ───────────────────────────────────────────────

        private void InitializeLights()
        {
            // Buscar LightController
            _lightController = FindFirstObjectByType<LightController>();

            if (_lightController != null)
            {
                if (showDebugLogs)
                    Debug.Log("[LightButtonMobile] LightController encontrado");
            }
            else
            {
                Debug.LogWarning("[LightButtonMobile] LightController no encontrado en la escena");
            }
        }

        private void BindButton()
        {
            if (lightsButton != null)
                lightsButton.onClick.AddListener(OnLightsButtonClicked);
        }

        // ── Events ───────────────────────────────────────────────────

        private void OnLightsButtonClicked()
        {
            if (showDebugLogs)
                Debug.Log("[LightButtonMobile] Botón de luces presionado");

            ToggleLights();
        }

        // ── Toggle Lights ────────────────────────────────────────────

        private void ToggleLights()
        {
            if (_lightController == null)
            {
                Debug.LogWarning("[LightButtonMobile] No hay controlador de luces disponible");
                return;
            }

            _lightsOn = !_lightsOn;
            _lightController.SetLights(_lightsOn);

            if (showDebugLogs)
                Debug.Log($"[LightButtonMobile] Luces: {(_lightsOn ? "ON" : "OFF")}");
        }

        /// <summary>Forzar estado de luces</summary>
        public void SetLights(bool on)
        {
            _lightsOn = on;
            if (_lightController != null)
                _lightController.SetLights(on);
        }
    }
}
