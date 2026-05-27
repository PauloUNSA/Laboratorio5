using UnityEngine;
using UnityEngine.InputSystem;

public class PerspectivaForzada : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float distanciaMaxima = 200f;
    [Tooltip("Distancia mínima (qué tan cerca de la cámara puede llegar el objeto sin encogerse de más)")]
    public float distanciaMinima = 5f; // Ajusta este valor en Unity
    [Tooltip("Velocidad a la que el objeto se aleja/acerca al presionar Shift")]
    public float sensibilidadProfundidad = 0.1f; 
    [Tooltip("Altura mínima a la que puede bajar el objeto (para no atravesar el plano)")]
    public float limiteSuelo = 0f; 
    
    private GameObject objetoSostenido;
    private Rigidbody rbObjeto;
    
    // Variables de la ilusión óptica
    private float distanciaInicialZ;
    private float distanciaActualZ;
    private Vector3 escalaInicial;
    private Vector3 offsetInicial;
    private int capaOriginal;

    // Nuestro cursor fantasma
    private Vector2 posicionVirtualPantalla;

    private Camera cam;
    private Mouse mouse;
    private Keyboard kb;

    private int capaIgnorar = 2; // Capa Ignore Raycast

    void Start()
    {
        // Se usa Camera.main para evitar el error si el script está en otro lado
        cam = Camera.main; 
        mouse = Mouse.current;
        kb = Keyboard.current;
    }

    void Update()
    {
        if (mouse == null || kb == null) return;

        // 1. INICIAR AGARRE
        if (mouse.leftButton.wasPressedThisFrame && objetoSostenido == null)
        {
            IntentarAgarrarObjeto();
        }
        
        // 2. MANTENER AGARRE, MOVER Y ESCALAR
        if (mouse.leftButton.isPressed && objetoSostenido != null)
        {
            MoverYEscalarObjeto();
        }

        // 3. SOLTAR
        if (mouse.leftButton.wasReleasedThisFrame && objetoSostenido != null)
        {
            SoltarObjeto();
        }
    }

    void IntentarAgarrarObjeto()
    {
        Vector2 mousePos = mouse.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mousePos);
        
        if (Physics.Raycast(ray, out RaycastHit hit, distanciaMaxima))
        {
            if (hit.collider.attachedRigidbody != null && hit.collider.attachedRigidbody.CompareTag("Agarrable"))
            {
                rbObjeto = hit.collider.attachedRigidbody;
                objetoSostenido = rbObjeto.gameObject;

                rbObjeto.isKinematic = true; 
                rbObjeto.useGravity = false;

                ControladorCarro scriptCarro = objetoSostenido.GetComponent<ControladorCarro>();
                if (scriptCarro != null) scriptCarro.enabled = false;

                capaOriginal = objetoSostenido.layer;
                CambiarCapaRecursivamente(objetoSostenido.transform, capaIgnorar); 

                // --- CONFIGURACIÓN INICIAL ---
                distanciaInicialZ = cam.WorldToScreenPoint(objetoSostenido.transform.position).z;
                distanciaInicialZ = Mathf.Max(distanciaInicialZ, 0.1f); 
                
                distanciaActualZ = distanciaInicialZ;
                escalaInicial = objetoSostenido.transform.localScale;
                
                Vector3 posicionCursorEnMundo = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, distanciaInicialZ));
                offsetInicial = objetoSostenido.transform.position - posicionCursorEnMundo;

                // CURSOR
                posicionVirtualPantalla = mousePos;
                Cursor.visible = false; 
            }
        }
    }

    void MoverYEscalarObjeto()
    {
        // --- 1. MODO PROFUNDIDAD (SHIFT PRESIONADO) ---
        if (kb.shiftKey.isPressed)
        {
            float movimientoY = mouse.delta.ReadValue().y;
            distanciaActualZ += movimientoY * sensibilidadProfundidad;
            
            // EL SECRETO ESTÁ AQUÍ: Reemplazamos el "1f" por "distanciaMinima"
            distanciaActualZ = Mathf.Clamp(distanciaActualZ, distanciaMinima, distanciaMaxima);
        }
        // --- 2. MODO MOVIMIENTO NORMAL ---
        else
        {
            posicionVirtualPantalla += mouse.delta.ReadValue();
            
            posicionVirtualPantalla.x = Mathf.Clamp(posicionVirtualPantalla.x, 0, Screen.width);
            posicionVirtualPantalla.y = Mathf.Clamp(posicionVirtualPantalla.y, 0, Screen.height);
        }

        // --- 3. CÁLCULOS DE ESCALA Y POSICIÓN ---
        float factorEscala = distanciaActualZ / distanciaInicialZ;
        objetoSostenido.transform.localScale = escalaInicial * factorEscala;

        Vector3 nuevaPosicionCursor = cam.ScreenToWorldPoint(new Vector3(posicionVirtualPantalla.x, posicionVirtualPantalla.y, distanciaActualZ));
        Vector3 offsetEscalado = offsetInicial * factorEscala;
        
        Vector3 posicionFinal = nuevaPosicionCursor + offsetEscalado;

        // --- 4. PROTECCIÓN ANTI-SUELO ---
        if (posicionFinal.y < limiteSuelo)
        {
            posicionFinal.y = limiteSuelo;
        }

        objetoSostenido.transform.position = posicionFinal;
    }

    void SoltarObjeto()
    {
        rbObjeto.isKinematic = false;
        rbObjeto.useGravity = true;

        ControladorCarro scriptCarro = objetoSostenido.GetComponent<ControladorCarro>();
        if (scriptCarro != null) scriptCarro.enabled = true;

        CambiarCapaRecursivamente(objetoSostenido.transform, capaOriginal);
        objetoSostenido = null;

        // --- RESTAURAR CURSOR ---
        Mouse.current.WarpCursorPosition(posicionVirtualPantalla);
        Cursor.visible = true;
    }

    void CambiarCapaRecursivamente(Transform objetoAct, int nuevaCapa)
    {
        objetoAct.gameObject.layer = nuevaCapa;
        foreach (Transform hijo in objetoAct)
        {
            CambiarCapaRecursivamente(hijo, nuevaCapa);
        }
    }
}