using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CuuRacing.UI
{
    /// <summary>
    /// Reusable button handler to return to MainMenu from any scene.
    /// </summary>
    public class BackToMainMenuButton : MonoBehaviour
    {
        [Header("Scenes")]
        [Tooltip("Exact name of the MainMenu scene in Build Settings.")]
        public string mainMenuSceneName = "MainMenu";

        [Header("Audio")]
        [Tooltip("Optional click sound before scene change.")]
        public AudioSource clickSound;

        [Header("Loading")]
        [Tooltip("Use async scene load to avoid frame hitching.")]
        public bool useAsyncLoad = true;

        /// <summary>
        /// Assign this method to any UI Button OnClick to go back to MainMenu.
        /// </summary>
        public void OnBackToMainMenuClick()
        {
            if (clickSound != null)
            {
                clickSound.Play();
            }

            PlayerPrefs.Save();

            if (string.IsNullOrWhiteSpace(mainMenuSceneName))
            {
                Debug.LogWarning("[BackToMainMenuButton] MainMenu scene name is empty.");
                return;
            }

            if (useAsyncLoad)
            {
                StartCoroutine(LoadSceneAsync(mainMenuSceneName));
            }
            else
            {
                SceneManager.LoadScene(mainMenuSceneName);
            }
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            if (op == null)
            {
                Debug.LogError($"[BackToMainMenuButton] Scene '{sceneName}' was not found. Check Build Settings.");
                yield break;
            }

            while (!op.isDone)
            {
                yield return null;
            }
        }
    }
}
