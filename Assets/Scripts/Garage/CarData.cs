using UnityEngine;

namespace CuuRacing.Garage
{
    /// <summary>
    /// ScriptableObject que contiene todos los datos de un auto.
    /// Crea uno por vehículo desde el menú:
    ///   Assets > Create > CuuRacing > Car Data
    /// </summary>
    [CreateAssetMenu(menuName = "CuuRacing/Car Data", fileName = "NewCarData")]
    public class CarData : ScriptableObject
    {
        [Header("Identidad")]
        [Tooltip("Nombre del auto que se mostrará en la UI")]
        public string carName = "Auto";

        [Tooltip("Prefab 3D del auto (debe estar en la carpeta Prefabs o Resources)")]
        public GameObject carPrefab;

        [Header("Especificaciones (0 – 100)")]
        [Range(0f, 100f)]
        [Tooltip("Qué tan rápido alcanza la velocidad máxima")]
        public float acceleration = 50f;

        [Range(0f, 100f)]
        [Tooltip("Velocidad máxima que puede alcanzar")]
        public float topSpeed = 50f;

        [Range(0f, 100f)]
        [Tooltip("Qué tan bien toma las curvas")]
        public float handling = 50f;

        [Range(0f, 100f)]
        [Tooltip("Qué tan rápido frena")]
        public float braking = 50f;

        [Range(0f, 100f)]
        [Tooltip("Peso del vehículo (100 = muy pesado)")]
        public float weight = 50f;

        [Header("Escena de Carrera")]
        [Tooltip("Nombre exacto de la escena a cargar al presionar ¡JUGAR! con este auto. " +
                 "Déjalo vacío para usar la escena predeterminada.")]
        public string raceSceneName = "";
    }
}
