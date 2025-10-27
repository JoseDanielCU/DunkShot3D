using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class BallPreviewTrail : MonoBehaviour
{
    [Header("🌀 Movimiento del trail alrededor de la bola")]
    public Transform target; // 🔸 La bola del preview
    public float orbitSpeed = 3f;
    public float radius = 10f;
    public float heightOffset = 0.2f;

    private TrailRenderer trail;
    private float angle;

    private void Awake()
    {
        trail = GetComponent<TrailRenderer>();
    }

    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("[BallPreviewTrail] No hay target asignado.");
            enabled = false;
            return;
        }

        // 🔹 Configurar trail visualmente bonito por defecto
        trail.emitting = true;
        trail.time = 2f;
        trail.widthMultiplier = 0.1f;
    }

    private void Update()
    {
        if (target == null) return;

        // 🔹 Gira en torno a la bola
        angle += orbitSpeed * Time.deltaTime;
        if (angle > 360f) angle -= 360f;

        Vector3 offset = new Vector3(Mathf.Cos(angle), heightOffset, Mathf.Sin(angle)) * radius;
        transform.position = target.position + offset;
    }

    public void SetTrailMaterial(Material mat)
    {
        if (trail != null && mat != null)
            trail.material = mat;
    }
}
