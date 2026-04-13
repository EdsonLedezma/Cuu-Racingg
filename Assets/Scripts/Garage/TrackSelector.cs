using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CuuRacing.Garage
{
    /// <summary>
    /// Script para el dropdown de selección de pista en el Garage.
    /// Permite elegir entre "Default Scene - Mobile" y "Plain Test Track - Mobile".
    /// 
    /// SETUP EN INSPECTOR:
    ///  - TrackDropdown : TMP_Dropdown con las opciones de pista
    /// </summary>
    public class TrackSelector : MonoBehaviour
    {
        [Header("Dropdown")]
        [Tooltip("Dropdown para seleccionar la pista")]
        public TMP_Dropdown trackDropdown;

        [Tooltip("Audio para cambio de selección")]
        public AudioSource selectSound;

        // ── Escenas De Carreras ──────────────────────────────────────

        private const string DEFAULT_TRACK = "DefaultScene - Mobile";
        private const string PLAIN_TEST_TRACK = "PlainTestTrack - Mobile";

        // ── Unity Lifecycle ──────────────────────────────────────────

        private void Start()
        {
            InitializeDropdown();
        }

        // ── Initialize Dropdown ──────────────────────────────────────

        private void InitializeDropdown()
        {
            if (trackDropdown == null)
            {
                Debug.LogError("[TrackSelector] TrackDropdown no asignado en inspector");
                return;
            }

            // Agregar opciones
            trackDropdown.ClearOptions();
            var options = new System.Collections.Generic.List<TMP_Dropdown.OptionData>
            {
                new TMP_Dropdown.OptionData("Pista Clásica"),
                new TMP_Dropdown.OptionData("Pista Prueba")
            };
            trackDropdown.AddOptions(options);

            // Cargar selección anterior (default: 0 = Pista Clásica)
            int savedIndex = PlayerPrefs.GetInt("SelectedTrackIndex", 0);
            trackDropdown.value = savedIndex;

            // Bindings
            trackDropdown.onValueChanged.AddListener(OnTrackSelected);

            Debug.Log("[TrackSelector] Dropdown inicializado");
        }

        // ── Eventos ──────────────────────────────────────────────────

        private void OnTrackSelected(int index)
        {
            if (selectSound != null)
                selectSound.Play();

            string trackName = index == 0 ? DEFAULT_TRACK : PLAIN_TEST_TRACK;
            PlayerPrefs.SetString("SelectedTrack", trackName);
            PlayerPrefs.SetInt("SelectedTrackIndex", index);

            Debug.Log($"[TrackSelector] Pista seleccionada: {trackName}");
        }

        /// <summary>Obtener la pista actualmente seleccionada</summary>
        public static string GetSelectedTrack()
        {
            return PlayerPrefs.GetString("SelectedTrack", DEFAULT_TRACK);
        }
    }
}
