using UnityEngine;
using UnityEngine.InputSystem; 

[RequireComponent(typeof(Rigidbody))]
public class BallShooter : MonoBehaviour
{
    private Rigidbody rb;

    private Vector3 startDragPos;
    private Vector3 endDragPos;
    private bool isDragging = false;

    [Header("Disparo")]
    public float forceMultiplier = 0.05f;
    public float maxForce = 15f;

    [Header("Trayectoria")]
    public LineRenderer lineRenderer;
    public int trajectoryPoints = 30;
    public float timeStep = 0.1f;

    [Header("Referencia de Cámara")]
    public Camera cam;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null)
            {
                Debug.LogError("No hay cámara asignada en BallShooter y tampoco se encontró 'MainCamera'.");
            }
        }

        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = trajectoryPoints;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.yellow;
            lineRenderer.endColor = Color.red;
        }

        lineRenderer.enabled = false;
    }

    private void Update()
    {
        // Mouse / Touch press
        if ((Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
            (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame))
        {
            isDragging = true;
            startDragPos = GetPointerPosition();
        }

        // Mouse / Touch hold
        if (isDragging &&
            ((Mouse.current != null && Mouse.current.leftButton.isPressed) ||
             (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)))
        {
            endDragPos = GetPointerPosition();
            ShowTrajectory();
        }

        // Mouse / Touch release
        if (isDragging &&
            ((Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame) ||
             (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)))
        {
            isDragging = false;
            lineRenderer.enabled = false;
            Shoot();
        }
    }

    private Vector3 GetPointerPosition()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();
        }

        return Vector3.zero;
    }

    private void ShowTrajectory()
    {
        Vector3 velocity = GetLaunchVelocity();
        lineRenderer.enabled = true;

        for (int i = 0; i < trajectoryPoints; i++)
        {
            float t = i * timeStep;
            Vector3 point = transform.position + velocity * t;
            point.y += Physics.gravity.y * 0.5f * t * t;
            lineRenderer.SetPosition(i, point);
        }
    }

    private void Shoot()
    {
        Vector3 velocity = GetLaunchVelocity();
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = false;
        rb.AddForce(velocity, ForceMode.Impulse);
    }

    private Vector3 GetLaunchVelocity()
    {
        if (cam == null)
        {
            Debug.LogError("No hay cámara asignada en BallShooter.");
            return Vector3.zero;
        }

        Vector3 dragVector = startDragPos - endDragPos;

        // Convertirlo a dirección en el mundo
        Vector3 screenDir = new Vector3(dragVector.x, dragVector.y, 0f).normalized;
        Vector3 worldDir = cam.transform.TransformDirection(screenDir);
        worldDir.z = Mathf.Abs(worldDir.z);

        float dragDistance = dragVector.magnitude * forceMultiplier;
        float force = Mathf.Clamp(dragDistance, 0, maxForce);

        return worldDir * force;
    }
}
