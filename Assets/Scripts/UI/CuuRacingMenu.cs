using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

namespace CuuRacing.UI
{
    public class CuuRacingMenu : MonoBehaviour
    {
        [Header("Scenes")]
        [Tooltip("Nombre exacto de la escena de carrera")]
        public string raceSceneName = "DefaultScene - Mobile";
        [Tooltip("Nombre exacto de la escena de garage")]
        public string garageSceneName = "Garage";
        [Tooltip("Nombre exacto de la escena de ajustes")]
        public string settingsSceneName = "Ajustes";

        [Header("Panels")]
        public GameObject mainPanel;
        public GameObject settingsPanel;
        public GameObject loadingPanel;

        [Header("Loading")]
        public Slider loadingBar;
        public TMP_Text loadingText;
        public bool waitForInput = false;

        [Header("Audio")]
        public AudioSource hoverSound;
        public AudioSource clickSound;

        void Start()
        {
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (loadingPanel != null)  loadingPanel.SetActive(false);
            if (mainPanel != null)     mainPanel.SetActive(true);
        }

        // ── Botones principales ──────────────────────────────────────

        public void OnJugarClick()
        {
            PlayClick();
            StartCoroutine(LoadSceneAsync(raceSceneName));
        }

        public void OnGarageClick()
        {
            PlayClick();
            StartCoroutine(LoadSceneAsync(garageSceneName));
        }

        public void OnAjustesClick()
        {
            PlayClick();
            StartCoroutine(LoadSceneAsync(settingsSceneName));
        }

        public void OnAjustesBack()
        {
            PlayClick();
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (mainPanel != null)     mainPanel.SetActive(true);
        }

        // ── Hover sound (conectar al EventTrigger PointerEnter) ──────

        public void OnHover()
        {
            if (hoverSound != null) hoverSound.Play();
        }

        // ── Loading async ────────────────────────────────────────────

        IEnumerator LoadSceneAsync(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning("[CuuRacingMenu] Nombre de escena vacío.");
                yield break;
            }

            if (mainPanel != null)    mainPanel.SetActive(false);
            if (loadingPanel != null) loadingPanel.SetActive(true);
            if (loadingText != null)  loadingText.text = "Cargando...";

            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

            if (op == null)
            {
                Debug.LogError($"[CuuRacingMenu] No se encontró la escena '{sceneName}'. " +
                               "Verifica que esté agregada en File > Build Settings.");
                if (loadingPanel != null) loadingPanel.SetActive(false);
                if (mainPanel != null)    mainPanel.SetActive(true);
                yield break;
            }

            op.allowSceneActivation = false;

            while (!op.isDone)
            {
                float progress = Mathf.Clamp01(op.progress / 0.9f);
                if (loadingBar != null) loadingBar.value = progress;

                if (op.progress >= 0.9f)
                {
                    if (loadingBar != null) loadingBar.value = 1f;

                    if (waitForInput)
                    {
                        if (loadingText != null)
                            loadingText.text = "Toca o presiona para continuar";

                        // New Input System — compatible con teclado y touch
                        bool anyKey = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
                        bool touch  = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
                        if (anyKey || touch)
                            op.allowSceneActivation = true;
                    }
                    else
                    {
                        op.allowSceneActivation = true;
                    }
                }

                yield return null;
            }
        }

        void PlayClick()
        {
            if (clickSound != null) clickSound.Play();
        }
    }
}
