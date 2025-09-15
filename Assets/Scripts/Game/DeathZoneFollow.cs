using UnityEngine;

public class DeathZoneFollow : MonoBehaviour
{
    public Transform hoop1;
    public Transform hoop2;

    [Header("Offset")]
    public float yOffset = -5f; // offset en y para colocar el cubo invisible debajo
    public float zOffset = 0f;  //offset en z si es necesario

    void Update()
    {
        if (hoop1 == null || hoop2 == null) return;

        // Sacamos un punto medio entre los dos aros
        Vector3 midpoint = (hoop1.position + hoop2.position) / 2f;

        // Colocamos el cubo invisible debajo del midpoint
        transform.position = new Vector3(midpoint.x, midpoint.y + yOffset, midpoint.z + zOffset);
    }

    public void SetHoops(Transform h1, Transform h2)
    {
        hoop1 = h1;
        hoop2 = h2;
    }
}
