using UnityEngine;
using UnityEngine.InputSystem; // Importante para el nuevo sistema

public class ControladorCarro : MonoBehaviour
{
    [Header("Ajustes de Manejo")]
    public float velocidadMovimiento = 10f;
    public float velocidadRotacion = 100f;

    // Referencias componentes
    private Rigidbody rb;
    private Keyboard keyboard;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        keyboard = Keyboard.current;
    }

    void FixedUpdate() // Usamos FixedUpdate para físicas
    {
        if (keyboard == null || rb == null) return;

        // Leer inputs WASD o Flechas
        float movimiento = 0f;
        float rotacion = 0f;

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) movimiento = 1f;
        else if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) movimiento = -1f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) rotacion = -1f;
        else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) rotacion = 1f;

        // --- Aplicar Movimiento Físico (sin tocar transform.localScale) ---

        // Movimiento hacia adelante/atrás
        Vector3 forwardMove = transform.forward * movimiento * velocidadMovimiento * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + forwardMove);

        // Rotación (solo girar si se está moviendo)
        if (movimiento != 0)
        {
            float anguloRotacion = rotacion * velocidadRotacion * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, anguloRotacion, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }
}