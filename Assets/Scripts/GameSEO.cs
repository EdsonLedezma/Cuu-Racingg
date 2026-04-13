using UnityEngine;

/// <summary>
/// Este script almacena los metadatos y palabras clave (SEO/ASO) 
/// del juego para su indexación en tiendas y web.
/// </summary>
public class GameSEO : MonoBehaviour
{
    [Header("SEO / Metadatos del Juego")]
    public string gameTitle = "Cuu Racing";

    [TextArea(3, 5)]
    public string seoDescription = "Un videojuego de carreras indie con físicas avanzadas. Elige tu auto en el garaje y demuestra tu velocidad en pistas 3D a 60 FPS.";

    [Header("Palabras Clave (Tags)")]
    [Tooltip("Etiquetas utilizadas para el posicionamiento (ASO/SEO) en Google Play o App Store")]
    public string[] searchTags = new string[] 
    {
        "racing",
        "autos",
        "velocidad",
        "indie",
        "carreras 3D"
    };
    
    // (Opcional) Un método que imprime los datos si los conectas a una web en el futuro
    public void ImprimirDatosSEO()
    {
        Debug.Log($"[SEO Info] Título: {gameTitle} | Tags: {searchTags.Length}");
    }
}
