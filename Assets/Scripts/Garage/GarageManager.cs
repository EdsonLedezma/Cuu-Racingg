using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace CuuRacing.Garage
{
    /// <summary>
    /// Controlador principal de la escena Garage.
    /// 
    /// SETUP EN INSPECTOR:
    ///  - Cars[]           : arrastra aquí todos los ScriptableObjects CarData.
    ///  - ExhibitionPoint  : Transform vacío donde aparecerá el auto 3D.
    ///  - StatsUI          : el componente CarStatsUI del Canvas.
    ///  - ClickSound       : AudioSource con SFX_Click_Mechanical (para botones).
    ///  - ScrollSound      : AudioSource con SFX_Click_Whoosh (para navegar autos).
    ///  - DefaultRaceScene : nombre de la escena de carrera (fallback si CarData no tiene una).
    /// </summary>
    public class GarageManager : MonoBehaviour
    {
        [Header("Autos Disponibles")]
        [Tooltip("Lista de ScriptableObjects CarData, uno por vehículo")]
        public CarData[] cars;

        [Header("Exhibición 3D")]
        [Tooltip("Transform donde se instancia el auto seleccionado")]
        public Transform exhibitionPoint;

        [Tooltip("Offset de rotación inicial del auto al mostrarse (euler)")]
        public Vector3 carDisplayRotation = new Vector3(0f, 160f, 0f);

        [Tooltip("Escala a la que aparecerá el auto (aumentar si se ve pequeño)")]
        public float carDisplayScale = 8f;

        [Tooltip("Velocidad a la que el auto gira sobre su eje (0 = no gira)")]
        public float carRotationSpeed = 15f;

        [Header("UI")]
        public CarStatsUI statsUI;

        [Header("Audio")]
        [Tooltip("SFX_Click_Mechanical — para clicks de botones")]
        public AudioSource clickSound;

        [Tooltip("SFX_Click_Whoosh — para navegación entre autos")]
        public AudioSource scrollSound;

        [Header("Escena de carrera (fallback)")]
        [Tooltip("Se usa si el CarData del auto seleccionado no tiene escena propia")]
        public string defaultRaceSceneName = "DefaultScene - Mobile";

        [Header("Nombre de la escena del Menú Principal")]
        public string mainMenuSceneName = "MainMenu";

        // ── Estado interno ───────────────────────────────────────────────

        private int _currentIndex = 0;
        private GameObject _currentCarInstance;

        // Swipe tracking
        private Vector2 _startTouchPosition;
        private Vector2 _endTouchPosition;
        private bool _isSwiping = false;

        // ── Unity Lifecycle ──────────────────────────────────────────────

        private void Start()
        {
            if (cars == null || cars.Length == 0)
            {
                Debug.LogWarning("[GarageManager] No hay autos en la lista. " +
                                 "Agrega CarData assets al array 'Cars'.");
                return;
            }

            ShowCar(_currentIndex);
        }

        private void Update()
        {
            // Hace que el auto rote automáticamente sobre su eje
            if (_currentCarInstance != null && carRotationSpeed > 0f)
            {
                _currentCarInstance.transform.Rotate(Vector3.up * carRotationSpeed * Time.deltaTime, Space.World);
            }

            // Detectar scroll / swipe táctil y de mouse
            DetectSwipe();
        }

        private void DetectSwipe()
        {
            bool pointerDown = false;
            bool pointerUp = false;
            Vector2 currentPointerPos = Vector2.zero;

            // 1. Detectar Touch (Móviles)
            if (Touchscreen.current != null)
            {
                var touch = Touchscreen.current.primaryTouch;
                if (touch.press.wasPressedThisFrame)
                {
                    pointerDown = true;
                    currentPointerPos = touch.position.ReadValue();
                }
                else if (touch.press.wasReleasedThisFrame)
                {
                    pointerUp = true;
                    currentPointerPos = touch.position.ReadValue();
                }
            }
            // 2. Detectar Mouse (PC)
            else if (Mouse.current != null)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    pointerDown = true;
                    currentPointerPos = Mouse.current.position.ReadValue();
                }
                else if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    pointerUp = true;
                    currentPointerPos = Mouse.current.position.ReadValue();
                }
            }

            // Procesar el Swipe
            if (pointerDown)
            {
                _startTouchPosition = currentPointerPos;
                _isSwiping = true;
            }
            
            if (pointerUp && _isSwiping)
            {
                _endTouchPosition = currentPointerPos;
                _isSwiping = false;

                // Calcular la distancia horizontal del deslizamiento
                float swipeDistance = _endTouchPosition.x - _startTouchPosition.x;

                // Si se deslizó más de 100 píxeles a la izquierda o derecha
                if (Mathf.Abs(swipeDistance) > 100f)
                {
                    if (swipeDistance > 0)
                    {
                        // Deslizó hacia la derecha -> Ver auto anterior
                        OnPrevCar();
                    }
                    else
                    {
                        // Deslizó hacia la izquierda -> Ver auto siguiente
                        OnNextCar();
                    }
                }
            }
        }


        // ── Botones de navegación ————————————————————————────────————————

        /// <summary>Llamar desde el botón «  (Anterior)</summary>
        public void OnPrevCar()
        {
            PlayScroll();
            _currentIndex--;
            if (_currentIndex < 0) _currentIndex = cars.Length - 1;
            ShowCar(_currentIndex);
        }

        /// <summary>Llamar desde el botón » (Siguiente)</summary>
        public void OnNextCar()
        {
            PlayScroll();
            _currentIndex++;
            if (_currentIndex >= cars.Length) _currentIndex = 0;
            ShowCar(_currentIndex);
        }

        // ── Botón Jugar ──────────────────────────────────────────────────

        /// <summary>Llamar desde el botón ¡JUGAR!</summary>
        public void OnPlayClick()
        {
            PlayClick();

            if (cars == null || cars.Length == 0) return;

            CarData selected = cars[_currentIndex];

            // Guardamos el nombre del auto o el prefab seleccionado para leerlo en la escena de carrera
            PlayerPrefs.SetString("SelectedCarName", selected.carName);
            PlayerPrefs.SetInt("SelectedCarIndex", _currentIndex);
            PlayerPrefs.Save(); // Asegurar que se guarde el auto antes de cargar la escena

            // Obtener la pista seleccionada (desde TrackSelector)
            string scene = TrackSelector.GetSelectedTrack();

            StartCoroutine(LoadSceneAsync(scene));
        }

        // ── Botón Volver ─────────────────────────────────────────────────

        /// <summary>Llamar desde el botón Volver / Back</summary>
        public void OnBackClick()
        {
            PlayClick();
            SceneManager.LoadScene(mainMenuSceneName);
        }

        private void ShowCar(int index)
        {
            // Destruir el auto actual si existe
            if (_currentCarInstance != null)
                Destroy(_currentCarInstance);

            CarData data = cars[index];
            if (data == null)
            {
                Debug.LogWarning($"[GarageManager] CarData en índice {index} es null.");
                return;
            }

            // Instanciar el prefab del auto en el punto de exhibición
            if (data.carPrefab != null && exhibitionPoint != null)
            {
                _currentCarInstance = Instantiate(
                    // Desactivar físicas al aparecer en el garage
                    data.carPrefab,
                    exhibitionPoint.position,
                    Quaternion.Euler(carDisplayRotation),
                    exhibitionPoint
                );
                
                // Ajustamos la escala para que no se vea microscópico
                _currentCarInstance.transform.localScale = Vector3.one * carDisplayScale;

                // Desactivamos físicas para que no se caiga
                Rigidbody rb = _currentCarInstance.GetComponent<Rigidbody>();
                if (rb != null) {
                    rb.useGravity = false;
                    rb.isKinematic = true; // Esto lo deja clavado en el aire
                }
            }
            else
            {
                Debug.LogWarning($"[GarageManager] '{data.carName}' no tiene prefab " +
                                 "o ExhibitionPoint no está asignado.");
            }

            // Actualizar UI de stats
            if (statsUI != null)
                statsUI.UpdateStats(data);
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning("[GarageManager] El nombre de la escena está vacío.");
                yield break;
            }

            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            if (op == null)
            {
                Debug.LogError($"[GarageManager] No se encontró la escena '{sceneName}'. " +
                               "Verifica que esté en File > Build Settings.");
                yield break;
            }

            while (!op.isDone)
                yield return null;
        }

        // ── Audio helpers ────────────────────────────────────────────────

        private void PlayClick()
        {
            if (clickSound != null) clickSound.Play();
        }

        private void PlayScroll()
        {
            if (scrollSound != null) scrollSound.Play();
        }
    }
}
