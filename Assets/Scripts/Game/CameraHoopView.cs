using UnityEngine;

public class CameraHoopView : MonoBehaviour
{
    [Header("Referencias")]
    public HoopSpawner spawner;
    public Camera cam;

    [Header("Posicionamiento")]
    public Vector3 offset = new Vector3(0, 12f, -12f); // Vista superior/diagonal
    public Vector3 rotationEuler = new Vector3(45f, 0f, 0f); // Mirada inclinada

    [Header("Transición")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 3f;

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private void Start()
    {
        if (spawner == null)
            spawner = FindAnyObjectByType<HoopSpawner>();

        if (cam == null)
            cam = Camera.main;

        UpdateCameraTarget(true);
    }

    private void LateUpdate()
    {
        if (spawner == null || spawner.CurrentHoop == null || spawner.NextHoop == null)
            return;

        // Suavizado de movimiento entre posiciones objetivo
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // 🔹 Llamar este método cuando cambian los aros (por ejemplo, en HoopSpawner.OnBallScored)
    public void UpdateCameraTarget(bool instant = false)
    {
        if (spawner == null || spawner.CurrentHoop == null || spawner.NextHoop == null)
            return;

        // Punto medio entre los dos aros
        Vector3 midpoint = (spawner.CurrentHoop.transform.position + spawner.NextHoop.transform.position) / 2f;

        targetPosition = midpoint + offset;
        targetRotation = Quaternion.Euler(rotationEuler);

        if (instant)
        {
            transform.position = targetPosition;
            transform.rotation = targetRotation;
        }
    }
}
