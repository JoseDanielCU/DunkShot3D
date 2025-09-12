using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;         // El aro activo (se actualizar� desde el HoopSpawner)

    [Header("Ajustes de c�mara")]
    public Vector3 offset = new Vector3(0, 3, -6);
    public float followSpeed = 5f;   // Suavizado de posici�n
    public float rotateSpeed = 5f;   // Suavizado de rotaci�n

    void LateUpdate()
    {
        if (target == null) return;

        // --- Seguir posici�n del aro ---
        Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // --- Rotar din�micamente hacia el aro ---
        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotateSpeed * Time.deltaTime);
    }

    // M�todo para cambiar el aro seguido
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
