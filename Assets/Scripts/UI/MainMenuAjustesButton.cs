using UnityEngine;
using UnityEngine.SceneManagement;

namespace CuuRacing.UI
{
    /// <summary>
    /// Script simple para el botón Ajustes en MainMenu.
    /// Carga la escena Ajustes.
    /// </summary>
    public class MainMenuAjustesButton : MonoBehaviour
    {
        [Tooltip("Nombre exacto de la escena Ajustes")]
        public string ajustesSceneName = "Ajustes";

        [Tooltip("Audio clip para click")]
        public AudioSource clickSound;

        public void OnAjustesClick()
        {
            if (clickSound != null)
                clickSound.Play();
            
            SceneManager.LoadScene(ajustesSceneName);
        }
    }
}
