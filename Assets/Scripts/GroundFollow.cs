using UnityEngine;

public class GroundFollow : MonoBehaviour
{
    public Transform hoop1;
    public Transform hoop2;

    [Header("Offset")]
    public float yOffset = -1f; // Ajusta la altura del suelo respecto al midpoint
    public float zOffset = 0f;  // Offset opcional en Z

    void Update()
    {
        if (hoop1 == null || hoop2 == null) return;

        // Sacamos el punto medio entre los dos aros
        Vector3 midpoint = (hoop1.position + hoop2.position) / 2f;

        // Posicionamos el ground con offset
        transform.position = new Vector3(midpoint.x, midpoint.y + yOffset, midpoint.z + zOffset);
    }

    public void SetHoops(Transform h1, Transform h2)
    {
        hoop1 = h1;
        hoop2 = h2;
    }
}
