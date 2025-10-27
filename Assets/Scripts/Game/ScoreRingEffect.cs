using UnityEngine;

public class ScoreRingEffect : MonoBehaviour
{
    [Header("Animación del anillo")]
    public float duration = 1f;
    public float finalScale = 2f;

    private Material mat;
    private Color baseColor;
    private float timer = 0f;

    void Start()
    {
        // Crear instancia del material para no modificar el original
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            mat = renderer.material;
            baseColor = mat.color;
        }
        else
        {
            Debug.LogWarning("ScoreRingEffect: No hay Renderer en el prefab del anillo.");
            Destroy(gameObject);
        }

        transform.localScale = Vector3.one * 0.5f; // empieza pequeño
    }

    void Update()
    {
        if (mat == null) return;

        timer += Time.deltaTime;
        float t = timer / duration;

        // Aumenta tamaño
        transform.localScale = Vector3.one * Mathf.Lerp(0.5f, finalScale, t);

        // Se desvanece
        float alpha = Mathf.Lerp(1f, 0f, t);
        mat.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);

        // Eliminar después del tiempo
        if (t >= 1f)
            Destroy(gameObject);
    }
}
