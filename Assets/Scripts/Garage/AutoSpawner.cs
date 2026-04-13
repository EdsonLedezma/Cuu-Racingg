using UnityEngine;
using CuuRacing.Garage; // Para poder usar la clase CarData

namespace CuuRacing.Gameplay
{
    /// <summary>
    /// Este script recibe el auto seleccionado desde el Garage y lo hace aparecer 
    /// en la pista de carreras.
    /// 
    /// INSTRUCCIONES:
    /// 1. Pon este script en un objeto vacío de tu escena de carreras ("DefaultScene - Mobile").
    /// 2. Llama a ese objeto "SpawnPoint".
    /// 3. Coloca el objeto "SpawnPoint" exactamente donde quieres que inicie el coche (en la línea de meta).
    /// 4. En el Inspector, arrastra la lista de tus archivos 'CarData' (los mismos del Garage) a la lista 'availableCars'.
    /// </summary>
    [DefaultExecutionOrder(-100)] // ¡VITAL! Fuerza a este script a ejecutarse ANTES que cualquier otro (MVC, etc)
    public class AutoSpawner : MonoBehaviour
    {
        [Header("No es necesario instanciar prefabs")]
        [Tooltip("Esta vez, el script buscará los autos directamente en la escena y activará el correcto.")]
        public bool useSceneCars = true;

        private void Awake()
        {
            // 1. Obtener el nombre del auto desde PlayerPrefs
            string selectedCarName = PlayerPrefs.GetString("SelectedCarName", "");
            if (string.IsNullOrEmpty(selectedCarName))
            {
                Debug.LogWarning("[AutoSpawner] No hay auto seleccionado, eligiendo uno por defecto.");
            }

            // 2. Buscar del grupo de vehículos en la escena
            GameObject drivableParent = GameObject.Find("Drivable");
            if (drivableParent == null)
            {
                Debug.LogError("[AutoSpawner] ¡No se encontró el objeto 'Drivable' en la jerarquía! Asegúrate de que los autos estén dentro de Vehicles > Drivable.");
                return;
            }

            // 3. Evaluar e identificar el mejor auto que coincida
            GameObject targetCar = null;
            int maxScore = -1;

            foreach (Transform child in drivableParent.transform)
            {
                string childName = child.name.ToLower();
                string garageName = selectedCarName.ToLower();
                int score = 0;

                // Detectamos similitudes de palabras clave grandes 
                if (garageName.Contains("porsche") && childName.Contains("porsche")) score += 10;
                if (garageName.Contains("lexus") && childName.Contains("lexus")) score += 10;
                if (garageName.Contains("supra") && childName.Contains("supra")) score += 10;
                if (garageName.Contains("golf") && childName.Contains("golf")) score += 10;
                if (garageName.Contains("bmw") && childName.Contains("bmw")) score += 10;

                // Buscamos coincidencia exacta por palabras
                string[] words = garageName.Split(new char[] { ' ', '-' }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (string w in words)
                {
                    if (w.Length > 2 && childName.Contains(w)) score++;
                }

                if (score > maxScore)
                {
                    maxScore = score;
                    targetCar = child.gameObject;
                }
            }

            // Fallback por index
            if (maxScore <= 0 && drivableParent.transform.childCount > 0)
            {
                int index = PlayerPrefs.GetInt("SelectedCarIndex", 0);
                if (index >= 0 && index < drivableParent.transform.childCount)
                {
                    targetCar = drivableParent.transform.GetChild(index).gameObject;
                }
                else
                {
                    targetCar = drivableParent.transform.GetChild(0).gameObject;
                }
            }

            // 4. Desactivar los otros autos (NO destruir, para que MVC no colapse)
            foreach (Transform child in drivableParent.transform)
            {
                if (child.gameObject != targetCar)
                {
                    child.gameObject.SetActive(false);
                }
                else
                {
                    child.gameObject.SetActive(true);
                }
            }

            // 5. ¡Teletransportar el auto elegido!
            if (targetCar != null)
            {
                // Reubicar el coche al SpawnPoint (esta misma posición)
                targetCar.transform.position = transform.position;
                targetCar.transform.rotation = transform.rotation;

                // Asegurar que las fisicas despierten o no acumulen inercia vieja
                Rigidbody rb = targetCar.GetComponentInChildren<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                // Dejamos que el paquete MVC asigne la cámara Automáticamente a través del único vehículo que dejamos vivo.
                Debug.Log($"[AutoSpawner] Activando vehículo físico que ya está en escena: {targetCar.name}!");
            }
        }
    }
}
