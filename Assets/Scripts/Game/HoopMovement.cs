using UnityEngine;

public class HoopMovement : MonoBehaviour
{
    private Vector3 startPosition;
    private float moveSpeed;
    private float moveRange;
    private bool moveHorizontal;
    private float timer = 0f;

    public void Initialize(float speed, float range)
    {
        moveSpeed = speed;
        moveRange = range;
        startPosition = transform.position;
        
        // 50% de probabilidad de moverse horizontal o verticalmente
        moveHorizontal = Random.value > 0.5f;
        
        // Empezar en un offset aleatorio
        timer = Random.Range(0f, Mathf.PI * 2f);

        Debug.Log($"Aro con movimiento iniciado: Speed={speed}, Range={range}, Horizontal={moveHorizontal}");
    }

    private void Update()
    {
        if (moveSpeed <= 0f || moveRange <= 0f) return;

        timer += Time.deltaTime * moveSpeed;

        float offset = Mathf.Sin(timer) * moveRange;

        if (moveHorizontal)
        {
            // Movimiento horizontal (eje X)
            transform.position = startPosition + new Vector3(offset, 0, 0);
        }
        else
        {
            // Movimiento vertical (eje Y)
            transform.position = startPosition + new Vector3(0, offset, 0);
        }
    }
}