using UnityEngine;

public class BotonReinicioCarro : MonoBehaviour
{
    [Header("Arrastra aquí el Jetta")]
    public Transform jettaTransform;
    private Rigidbody rbJetta;

    void Start()
    {
        if (jettaTransform != null)
        {
            rbJetta = jettaTransform.GetComponent<Rigidbody>();
        }
    }

    // Esta es la función que llamará el botón
    public void ResetearVehiculo()
    {
        if (jettaTransform == null) return;

        // 1. Resetear Valores de Transform según tu imagen
        jettaTransform.position = new Vector3(-1.499f, 0.6f, -11.4f);
        jettaTransform.rotation = Quaternion.identity; // Quaternion.identity es igual a X:0, Y:0, Z:0
        jettaTransform.localScale = Vector3.one; // Vector3.one es igual a X:1, Y:1, Z:1

        // 2. Detener las físicas para que no salga volando
        if (rbJetta != null)
        {
            rbJetta.linearVelocity = Vector3.zero;
            rbJetta.angularVelocity = Vector3.zero;
        }
    }
}