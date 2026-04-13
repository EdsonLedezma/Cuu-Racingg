using UnityEngine;

namespace CuuRacing.Mobile
{
    /// <summary>
    /// Controlador del giroscopio para mobile.
    /// Detecta input del acelerómetro y genera un valor de dirección adicional.
    /// 
    /// Por defecto se integra con VehicleController, pero puede usarse como Input provider.
    /// 
    /// SETUP EN INSPECTOR:
    ///  - Asignar a una GameObject en la escena de carrera
    ///  - (Opcional) Referenciar el controlador del vehículo si está accesible
    /// </summary>
    public class GyroscopeController : MonoBehaviour
    {
        [Header("Gyroscope Settings")]
        [Tooltip("Si true, usa Input.acceleration. Si false, desactiva")]
        public bool useGyroscope = true;

        [Tooltip("Amplitud de influencia del giroscopio (-1 a 1)")]
        [Range(-1f, 1f)]
        public float gyroInfluence = 0.5f;

        [Header("Debug")]
        public bool showDebugInfo = false;

        // ── Variables Privadas ───────────────────────────────────────

        private float _gyroSteerInput;
        private bool _gyroAvailable = false;

        // ── Unity Lifecycle ──────────────────────────────────────────

        private void Start()
        {
            DetectGyroscope();
            UpdateSettings();
        }

        private void Update()
        {
            if (!_gyroAvailable || !useGyroscope)
                return;

            ReadGyroscopeInput();
        }

        // ── Detect Gyroscope ────────────────────────────────────────

        private void DetectGyroscope()
        {
            _gyroAvailable = SystemInfo.supportsAccelerometer;
            
            if (_gyroAvailable)
                Debug.Log("[GyroscopeController] Acelerómetro detectado ✓");
            else
                Debug.LogWarning("[GyroscopeController] Acelerómetro NO disponible en este dispositivo");
        }

        // ── Read Input ───────────────────────────────────────────────

        private void ReadGyroscopeInput()
        {
            // Leer aceleración del dispositivo (x = izquierda/derecha)
            Vector3 acceleration = Input.acceleration;
            
            // El eje X corresponde a left/right tilt
            // Rango típico: -1 a 1
            float tilt = Mathf.Clamp(acceleration.x, -1f, 1f);
            
            // Aplicar sensibilidad desde AjustesController
            float sensitivity = Settings.AjustesController.GetGyroSensitivity();
            
            // Mapear sensibilidad (1-10) a multiplicador (0.1-1.0)
            float sensMultiplier = (sensitivity / 10f);
            
            _gyroSteerInput = tilt * sensMultiplier * gyroInfluence;

            if (showDebugInfo)
                Debug.Log($"[GyroscopeController] Gyro Input: {_gyroSteerInput:F2} | Accel: {tilt:F2} | Sens: {sensitivity:F1}");
        }

        // ── Update Settings ────────────────────────────────────────

        /// <summary>Recargar ajustes desde AjustesController</summary>
        private void UpdateSettings()
        {
            useGyroscope = Settings.AjustesController.IsGyroEnabled();
            Debug.Log($"[GyroscopeController] Gyroscope: {(useGyroscope ? "ENABLED" : "DISABLED")}");
        }

        // ── Getters ──────────────────────────────────────────────────

        /// <summary>Obtener el input actual del giroscopio (-1 a 1)</summary>
        public float GetGyroSteerInput()
        {
            return useGyroscope && _gyroAvailable ? _gyroSteerInput : 0f;
        }

        /// <summary>Verificar si el giroscopio está activo</summary>
        public bool IsGyroActive()
        {
            return useGyroscope && _gyroAvailable;
        }

        /// <summary>Recargar ajustes (si el usuario cambió en Settings)</summary>
        public void ReloadSettings()
        {
            UpdateSettings();
        }
    }
}
