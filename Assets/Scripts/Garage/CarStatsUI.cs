using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CuuRacing.Garage
{
    /// <summary>
    /// Actualiza la UI de especificaciones cuando cambia el auto seleccionado.
    /// Los Sliders deben tener Interactable = false en el Inspector.
    /// </summary>
    public class CarStatsUI : MonoBehaviour
    {
        [Header("Nombre del auto")]
        public TMP_Text carNameText;

        [Header("Barras de especificaciones (Interactable = OFF en Inspector)")]
        public Slider accelerationBar;
        public Slider topSpeedBar;
        public Slider handlingBar;
        public Slider brakingBar;
        public Slider weightBar;

        [Header("Etiquetas de valor (opcional)")]
        public TMP_Text accelerationValue;
        public TMP_Text topSpeedValue;
        public TMP_Text handlingValue;
        public TMP_Text brakingValue;
        public TMP_Text weightValue;

        private void Awake()
        {
            // Aseguramos que los sliders no sean interactivos en runtime
            SetNonInteractable(accelerationBar);
            SetNonInteractable(topSpeedBar);
            SetNonInteractable(handlingBar);
            SetNonInteractable(brakingBar);
            SetNonInteractable(weightBar);
        }

        /// <summary>
        /// Actualiza toda la UI con los datos del auto recibido.
        /// </summary>
        public void UpdateStats(CarData data)
        {
            if (data == null) return;

            // Nombre
            if (carNameText != null)
                carNameText.text = data.carName;

            // Barras
            SetBar(accelerationBar, accelerationValue, data.acceleration);
            SetBar(topSpeedBar,     topSpeedValue,     data.topSpeed);
            SetBar(handlingBar,     handlingValue,     data.handling);
            SetBar(brakingBar,      brakingValue,      data.braking);
            SetBar(weightBar,       weightValue,       data.weight);
        }

        // ── Helpers ──────────────────────────────────────────────────────

        private void SetBar(Slider bar, TMP_Text label, float value)
        {
            if (bar != null)
                bar.value = value;

            if (label != null)
                label.text = Mathf.RoundToInt(value).ToString();
        }

        private void SetNonInteractable(Slider bar)
        {
            if (bar != null)
                bar.interactable = false;
        }
    }
}
