using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;         // El aro activo (se actualizará desde el HoopSpawner)

    [Header("Ajustes de cámara")]
    public Vector3 offset = new Vector3(0, 3, -6);
    public float followSpeed = 5f;   // Suavizado de posición
    public float rotateSpeed = 5f;   // Suavizado de rotación

    void LateUpdate()
    {
        if (target == null) return;

        // --- Seguir posición del aro ---
        Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // --- Rotar dinámicamente hacia el aro ---
        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotateSpeed * Time.deltaTime);
    }

    // Método para cambiar el aro seguido
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
