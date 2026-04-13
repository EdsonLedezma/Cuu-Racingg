using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CuuRacing.Mobile
{
    /// <summary>
    /// Carga las posiciones de botones guardadas desde Ajustes.unity
    /// Se ejecuta al iniciar DefaultScene-Mobile o PlainTestTrack-Mobile
    /// </summary>
    public class MobileHUDLayoutLoader : MonoBehaviour
    {
        [Header("Botones del HUD (en orden igual a Ajustes.unity)")]
        public Button[] buttonLayout;

        private const string LAYOUT_PREFS_KEY = "MobileButtonLayout";

        private void Start()
        {
            LoadLayout();
        }

        /// <summary>
        /// Lee las posiciones desde PlayerPrefs y las aplica a los botones
        /// </summary>
        private void LoadLayout()
        {
            if (buttonLayout == null || buttonLayout.Length == 0)
            {
                Debug.LogWarning("[MobileHUDLayoutLoader] No se asignaron botones");
                return;
            }

            string layoutJson = PlayerPrefs.GetString(LAYOUT_PREFS_KEY, "");
            if (string.IsNullOrEmpty(layoutJson))
            {
                Debug.Log("[MobileHUDLayoutLoader] No hay layout guardado. Usando posiciones por defecto.");
                return;
            }

            try
            {
                LayoutData layoutData = JsonUtility.FromJson<LayoutData>(layoutJson);
                
                if (layoutData.buttonPositions == null || layoutData.buttonPositions.Length == 0)
                {
                    Debug.LogWarning("[MobileHUDLayoutLoader] Layout vacío");
                    return;
                }

                // Aplicar posiciones guardadas
                for (int i = 0; i < buttonLayout.Length && i < layoutData.buttonPositions.Length; i++)
                {
                    if (buttonLayout[i] != null)
                    {
                        RectTransform rect = buttonLayout[i].GetComponent<RectTransform>();
                        if (rect != null)
                        {
                            rect.anchoredPosition = layoutData.buttonPositions[i];
                            Debug.Log($"[MobileHUDLayoutLoader] Botón {i} ({buttonLayout[i].name}) posición: {layoutData.buttonPositions[i]}");
                        }
                    }
                }

                Debug.Log("[MobileHUDLayoutLoader] Layout cargado correctamente ✅");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[MobileHUDLayoutLoader] Error al cargar layout: {ex.Message}");
            }
        }

        /// <summary>
        /// Estructura que coincide con AjustesController.LayoutData
        /// </summary>
        [System.Serializable]
        public class LayoutData
        {
            public Vector2[] buttonPositions;
            public string[] buttonNames;
        }
    }
}
