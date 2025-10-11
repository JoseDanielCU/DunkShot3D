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
    public float forceMultiplier = 0.15f;
    public float maxForce = 20f;

    [Header("Trayectoria")]
    public LineRenderer lineRenderer;
    public int trajectoryPoints = 40;
    public float timeStep = 0.1f;

    [Header("Referencia de Cámara")]
    public Camera cam;

    [Header("Indicador de impacto")]
    public GameObject impactPointPrefab;
    private GameObject impactPointInstance;

    [Header("Raycast settings")]
    public LayerMask collisionMask;

    // Nueva bandera
    public bool canShoot = true;
    private bool insideHoop = false;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (cam == null)
            cam = Camera.main;

        // LineRenderer configuración
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.yellow;
            lineRenderer.endColor = Color.red;
        }
        lineRenderer.positionCount = trajectoryPoints;
        lineRenderer.enabled = false;

        // Crear instancia del punto de impacto
        if (impactPointPrefab != null)
        {
            impactPointInstance = Instantiate(impactPointPrefab);
            impactPointInstance.SetActive(false);
        }
    }

    private void Update()
    {
        if (PauseMenu.instance != null && PauseMenu.instance.isPaused)
            return;

        if (!canShoot) return; // 🚫 Si no puedes disparar, ignora input

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

        Vector3 currentPos = transform.position;
        Vector3 currentVelocity = velocity;

        for (int i = 0; i < trajectoryPoints; i++)
        {
            float t = timeStep;
            Vector3 nextPos = currentPos + currentVelocity * t;
            nextPos.y += Physics.gravity.y * 0.5f * t * t;

            // 🔹 Lanzamos raycast entre currentPos y nextPos
            Vector3 dir = (nextPos - currentPos).normalized;
            float dist = (nextPos - currentPos).magnitude;

            if (Physics.Raycast(currentPos, dir, out RaycastHit hit, dist, collisionMask))
            {
                // Dibujamos hasta el punto de impacto
                lineRenderer.positionCount = i + 2;
                lineRenderer.SetPosition(i, currentPos);
                lineRenderer.SetPosition(i + 1, hit.point);

                // Dibujar un "gizmo" en el impacto (opcional)
                Debug.DrawRay(hit.point, Vector3.up * 0.3f, Color.red, 0.1f);

                return; // detener dibujo, ya hubo impacto
            }

            // Si no hubo impacto, seguir dibujando
            if (i < lineRenderer.positionCount)
            {
                lineRenderer.SetPosition(i, currentPos);
            }

            // Actualizar posición y velocidad
            currentPos = nextPos;
            currentVelocity.y += Physics.gravity.y * t;
        }

        // Restablecer el número de puntos al máximo si no hubo colisión
        lineRenderer.positionCount = trajectoryPoints;
    }

    private void Shoot()
    {
        Vector3 velocity = GetLaunchVelocity();
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = false;
        rb.AddForce(velocity, ForceMode.Impulse);

        canShoot = false; //Bloquear nuevos disparos
    }

    private Vector3 GetLaunchVelocity()
    {
        if (cam == null)
        {
            Debug.LogError("No hay cámara asignada en BallShooter.");
            return Vector3.zero;
        }

        Vector3 dragVector = startDragPos - endDragPos;

        Vector3 screenDir = new Vector3(dragVector.x, dragVector.y, 0f).normalized;
        Vector3 worldDir = cam.transform.TransformDirection(screenDir);
        worldDir.z = Mathf.Abs(worldDir.z);
        worldDir.y += 0.2f;

        float dragDistance = dragVector.magnitude * forceMultiplier;
        float force = Mathf.Clamp(dragDistance, 0, maxForce);

        return worldDir * force;
    }
}