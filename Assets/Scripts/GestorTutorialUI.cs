using UnityEngine;
using System.Collections;

public class GestorTutorialUI : MonoBehaviour
{
    public static GestorTutorialUI instancia;

    [Header("Textos del Canvas")]
    public GameObject textoWASD;
    public GameObject textoMOUSE1;
    public GameObject textoMOUSE2;

    [Header("Configuración de Tiempo")]
    public float tiempoVisible = 5f;

    void Awake()
    {
        if (instancia == null) instancia = this;

        // Estado inicial: Solo el primer texto (WASD) debe estar activo al empezar
        if (textoWASD != null) textoWASD.SetActive(true);
        if (textoMOUSE1 != null) textoMOUSE1.SetActive(false);
        if (textoMOUSE2 != null) textoMOUSE2.SetActive(false);
    }

    void Start()
    {
        // Iniciamos la secuencia de tiempos
        StartCoroutine(SecuenciaTextos());
    }

    IEnumerator SecuenciaTextos()
    {
        // 1. El texto WASD ya está encendido gracias al Awake. Esperamos 5 segundos.
        yield return new WaitForSeconds(tiempoVisible);
        if (textoWASD != null) textoWASD.SetActive(false);

        // 2. Encendemos MOUSE1 y esperamos 5 segundos.
        if (textoMOUSE1 != null) textoMOUSE1.SetActive(true);
        yield return new WaitForSeconds(tiempoVisible*2);
        if (textoMOUSE1 != null) textoMOUSE1.SetActive(false);

        // 3. Encendemos MOUSE2 y esperamos 5 segundos.
        if (textoMOUSE2 != null) textoMOUSE2.SetActive(true);
        yield return new WaitForSeconds(tiempoVisible*3);
        if (textoMOUSE2 != null) textoMOUSE2.SetActive(false);
    }
}