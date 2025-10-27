using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BallShooter : MonoBehaviour
{
    private Rigidbody rb;
    private BallTrailController trailController;

    private Vector3 startDragPos;
    private Vector3 endDragPos;
    private bool isDragging = false;

    [Header("🎧 Sonidos del disparo")]
    public AudioSource audioSource;
    public AudioClip aimSound;        // 🔸 cuando se dibuja la trayectoria
    public AudioClip shootSound;      // 🔹 al soltar y lanzar la bola
    public AudioClip flyingSound;     // 🔸 mientras vuela (loop corto)

    private AudioSource flyingSource;  // instancia separada para vuelo
    private bool isAimingSoundPlaying = false;

    [Header("⚙️ Disparo")]
    public float forceMultiplier = 0.015f;
    public float maxForce = 20f;

    [Header("⤴️ Elevación obligatoria")]
    [Tooltip("Fuerza vertical mínima que siempre se aplica hacia arriba al lanzar.")]
    public float extraVerticalForce = 5f;

    [Header("📈 Trayectoria")]
    public LineRenderer lineRenderer;
    public int trajectoryPoints = 40;
    public float timeStep = 0.05f;

    [Header("📷 Referencia de Cámara")]
    public Camera cam;

    [Header("🎯 Indicador de impacto")]
    public GameObject impactPointPrefab;
    private GameObject impactPointInstance;

    [Header("🔍 Raycast settings")]
    public LayerMask collisionMask;

    public bool canShoot = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        trailController = GetComponent<BallTrailController>();

        if (trailController != null)
            trailController.StopTrail();

        if (cam == null)
            cam = Camera.main;

        // 🎵 Crear audio source para vuelo
        if (flyingSound != null)
        {
            flyingSource = gameObject.AddComponent<AudioSource>();
            flyingSource.clip = flyingSound;
            flyingSource.loop = true;
            flyingSource.playOnAwake = false;
        }

        // 🎵 Reusar audioSource general si existe
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // 🔶 Configurar línea de trayectoria
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.yellow;
            lineRenderer.endColor = Color.red;
        }

        lineRenderer.enabled = false;

        // 🎯 Crear punto de impacto
        if (impactPointPrefab != null)
        {
            impactPointInstance = Instantiate(impactPointPrefab);
            impactPointInstance.SetActive(false);
        }
    }

    private void Update()
    {
        if (!canShoot) return;

        // 📍 Inicio del arrastre
        if ((Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
            (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame))
        {
            isDragging = true;
            startDragPos = GetPointerPosition();
        }

        // 🎯 Mostrando trayectoria y sonido de apuntado
        if (isDragging &&
            ((Mouse.current != null && Mouse.current.leftButton.isPressed) ||
             (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)))
        {
            endDragPos = GetPointerPosition();
            ShowTrajectory();

            if (!isAimingSoundPlaying && aimSound != null && audioSource != null)
            {
                audioSource.pitch = Random.Range(0.95f, 1.05f);
                audioSource.PlayOneShot(aimSound);
                isAimingSoundPlaying = true;
            }
        }

        // 🏀 Al soltar el disparo
        if (isDragging &&
            ((Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame) ||
             (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)))
        {
            isDragging = false;
            lineRenderer.enabled = false;
            isAimingSoundPlaying = false;
            Shoot();
        }

        // ⛔ Detener sonido de vuelo si bola se detiene
        if (flyingSource != null && flyingSource.isPlaying && rb.linearVelocity.magnitude < 0.2f)
        {
            flyingSource.Stop();
        }
    }

    private Vector3 GetPointerPosition()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            return Touchscreen.current.primaryTouch.position.ReadValue();
        else if (Mouse.current != null)
            return Mouse.current.position.ReadValue();
        return Vector3.zero;
    }

    private void ShowTrajectory()
    {
        Vector3 velocity = GetLaunchVelocity();
        if (velocity.sqrMagnitude < 0.001f)
        {
            lineRenderer.enabled = false;
            if (impactPointInstance != null) impactPointInstance.SetActive(false);
            return;
        }

        lineRenderer.enabled = true;
        lineRenderer.positionCount = trajectoryPoints;

        Vector3 startPos = transform.position + Vector3.up * 0.5f + cam.transform.forward * 0.3f;

        Vector3 currentPos = startPos;
        Vector3 currentVelocity = velocity;
        Vector3 gravity = Physics.gravity;

        bool hitDetected = false;
        Vector3 hitPoint = Vector3.zero;

        for (int i = 0; i < trajectoryPoints; i++)
        {
            lineRenderer.SetPosition(i, currentPos);

            Vector3 nextPos = currentPos + currentVelocity * timeStep + 0.5f * gravity * timeStep * timeStep;
            Vector3 dir = (nextPos - currentPos).normalized;
            float dist = (nextPos - currentPos).magnitude;

            if (Physics.SphereCast(currentPos, 0.15f, dir, out RaycastHit hit, dist, collisionMask))
            {
                lineRenderer.positionCount = i + 2;
                lineRenderer.SetPosition(i + 1, hit.point);
                hitDetected = true;
                hitPoint = hit.point;
                break;
            }

            currentPos = nextPos;
            currentVelocity += gravity * timeStep;
        }

        if (impactPointInstance != null)
        {
            impactPointInstance.SetActive(true);
            impactPointInstance.transform.position = hitDetected
                ? hitPoint + Vector3.up * 0.05f
                : lineRenderer.GetPosition(lineRenderer.positionCount - 1);
        }
    }

    private void Shoot()
    {
        Vector3 velocity = GetLaunchVelocity();
        if (velocity.sqrMagnitude < 0.001f) return;

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = false;
        rb.AddForce(velocity, ForceMode.Impulse);
        canShoot = false;

        // 🔊 Sonido de disparo
        if (shootSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(shootSound);
        }

        // 🔁 Sonido de vuelo (loop)
        if (flyingSource != null)
        {
            flyingSource.pitch = Random.Range(0.95f, 1.05f);
            flyingSource.Play();
        }

        if (trailController != null)
            trailController.PlayTrail();
    }

    private Vector3 GetLaunchVelocity()
    {
        if (cam == null)
        {
            Debug.LogError("No hay cámara asignada.");
            return Vector3.zero;
        }

        // 🔹 Calcular el vector de arrastre en píxeles
        Vector3 dragVector = startDragPos - endDragPos;
        if (dragVector.magnitude < 10f)
            return Vector3.zero;

        // --- Descomponer arrastre ---
        float dragX = (startDragPos.x - endDragPos.x) / Screen.width;   // izquierda/derecha
        float dragY = (startDragPos.y - endDragPos.y) / Screen.height;  // adelante/atrás

        // --- Dirección base ---
        Vector3 forward = cam.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = cam.transform.right;

        // --- Control lateral (solo orientación, sin afectar fuerza) ---
        float lateralFactor = Mathf.Clamp(dragX * 2f, -1f, 1f);
        Vector3 direction = (forward + right * lateralFactor).normalized;

        // --- Ángulo vertical basado solo en arrastre Y ---
        float minAngle = 35f;
        float maxAngle = 70f;
        float normalizedY = Mathf.Clamp01(dragY);
        float verticalAngle = Mathf.Lerp(minAngle, maxAngle, Mathf.InverseLerp(0f, 0.4f, normalizedY));

        Quaternion elevation = Quaternion.AngleAxis(-verticalAngle, cam.transform.right);
        direction = elevation * direction;

        // --- Magnitud de fuerza ---
        float dragDistance = Mathf.Abs(dragY) * Screen.height;
        float force = Mathf.Clamp(dragDistance * forceMultiplier, 0f, maxForce);

        // --- Construir velocidad ---
        Vector3 velocity = direction * force;

        // --- Asegurar elevación mínima ---
        if (velocity.y < extraVerticalForce)
            velocity.y = extraVerticalForce;

        Debug.DrawRay(transform.position + Vector3.up * 0.5f, velocity, Color.green, 1f);
        return velocity;
    }

    // 🛬 Detener sonido de vuelo al aterrizar
    private void OnCollisionEnter(Collision collision)
    {
        if (flyingSource != null && flyingSource.isPlaying)
            flyingSource.Stop();
    }
}
