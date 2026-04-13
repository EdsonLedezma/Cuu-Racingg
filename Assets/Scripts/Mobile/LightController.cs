using UnityEngine;

namespace CuuRacing.Mobile
{
    /// <summary>
    /// Controlador de luces del vehículo.
    /// 
    /// Este es un placeholder que busca Light components en el vehículo.
    /// Debe ser revisado/actualizado basándose en cómo MVC implementa las luces.
    /// 
    /// SETUP EN INSPECTOR:
    ///  - Asignar a una GameObject en el vehículo o en la escena
    ///  - Opcionalmente asignar HeadLights[] para tener mejor control
    /// </summary>
    public class LightController : MonoBehaviour
    {
        [Header("Lights Setup")]
        [Tooltip("Luces delanteras (faros)")]
        public Light[] headLights;

        [Tooltip("Luces traseras (frenos)")]
        public Light[] brakeLights;

        [Header("Debug")]
        public bool showDebugLogs = false;

        // ── Variables Privadas ───────────────────────────────────────

        private bool _lightsActive = false;

        // ── Unity Lifecycle ──────────────────────────────────────────

        private void Start()
        {
            AutoFindLights();
        }

        // ── Auto Find Lights ────────────────────────────────────────

        private void AutoFindLights()
        {
            // Si no están asignadas, buscar en el vehículo o padre
            if ((headLights == null || headLights.Length == 0) && 
                (brakeLights == null || brakeLights.Length == 0))
            {
                // Buscar todos los Light components en este objeto y sus hijos
                Light[] allLights = GetComponentsInChildren<Light>();
                
                if (allLights.Length > 0)
                {
                    headLights = allLights;
                    if (showDebugLogs)
                        Debug.Log($"[LightController] Se encontraron {allLights.Length} luces automáticamente");
                }
                else
                {
                    Debug.LogWarning("[LightController] No se encontraron Light components en el vehículo");
                }
            }
        }

        // ── Set Lights ───────────────────────────────────────────────

        /// <summary>Activar o desactivar todas las luces</summary>
        public void SetLights(bool active)
        {
            _lightsActive = active;

            // Activar/desactivar faros
            if (headLights != null)
            {
                foreach (Light light in headLights)
                {
                    if (light != null)
                        light.enabled = active;
                }
            }

            // Activar/desactivar frenos
            if (brakeLights != null)
            {
                foreach (Light light in brakeLights)
                {
                    if (light != null)
                        light.enabled = active;
                }
            }

            if (showDebugLogs)
                Debug.Log($"[LightController] Luces: {(active ? "ACTIVADAS" : "DESACTIVADAS")}");
        }

        /// <summary>Alternar estado de las luces</summary>
        public void ToggleLights()
        {
            SetLights(!_lightsActive);
        }

        /// <summary>Obtener estado actual de las luces</summary>
        public bool AreLightsActive()
        {
            return _lightsActive;
        }

        // ── Specific Light Control ──────────────────────────────────

        /// <summary>Controlar solo los faros</summary>
        public void SetHeadLights(bool active)
        {
            if (headLights != null)
            {
                foreach (Light light in headLights)
                {
                    if (light != null)
                        light.enabled = active;
                }
            }
        }

        /// <summary>Controlar solo las luces traseras</summary>
        public void SetBrakeLights(bool active)
        {
            if (brakeLights != null)
            {
                foreach (Light light in brakeLights)
                {
                    if (light != null)
                        light.enabled = active;
                }
            }
        }
    }
}
