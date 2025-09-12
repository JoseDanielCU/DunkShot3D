using UnityEngine;

public class CameraFollowHoops : MonoBehaviour
{
    public HoopSpawner spawner; // referencia al spawner
    public Camera cam;

    [Header("Movimiento")]
    public float smoothSpeed = 2f;

    [Header("Zoom")]
    public float minDistance = 8f;
    public float extraPadding = 2f;

    [Header("Ajustes Manuales")]
    public Vector3 offset = new Vector3(0, 2, -10);
    // 🔥 offset relativo al punto medio de los aros
    // Ej: (0, 2, -10) → 2 unidades arriba y 10 detrás en Z

    public Vector3 rotationOffset = Vector3.zero;
    // 🔥 rotación adicional manual (en grados)

    private void LateUpdate()
    {
        if (spawner == null || spawner.CurrentHoop == null || spawner.NextHoop == null || cam == null)
            return;

        Transform current = spawner.CurrentHoop.transform;
        Transform next = spawner.NextHoop.transform;

        // Punto medio entre ambos aros
        Vector3 middlePoint = (current.position + next.position) / 2f;

        // Distancia entre ellos
        float distance = Vector3.Distance(current.position, next.position);

        // Posición deseada con offset configurable
        Vector3 desiredPos = middlePoint + offset;
        desiredPos.z -= distance * 0.5f; // retrocede más si hay más distancia

        // Suavizado
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

        // Mirar al centro
        transform.LookAt(middlePoint);

        // Aplicar rotación adicional manual
        transform.rotation *= Quaternion.Euler(rotationOffset);

        // Ajustar FOV (zoom dinámico)
        float targetFOV = Mathf.Clamp(distance * 10f + extraPadding, 40f, 80f);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, smoothSpeed * Time.deltaTime);
    }
}
