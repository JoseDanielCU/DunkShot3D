using UnityEngine;
using TMPro;

public class PointPopup : MonoBehaviour
{
    public float riseSpeed = 50f;       // velocidad de subida
    public float duration = 1f;         // tiempo hasta desaparecer

    private TextMeshProUGUI text;
    private float timer = 0f;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // Subir
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        // Temporizador para desaparecer
        timer += Time.deltaTime;
        if (timer >= duration)
            Destroy(gameObject);

        // Opcional: desvanecer
        if (text != null)
        {
            Color c = text.color;
            c.a = Mathf.Lerp(1f, 0f, timer / duration);
            text.color = c;
        }
    }
}
