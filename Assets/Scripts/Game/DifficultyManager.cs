using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager instance;

    [Header("Configuración de Dificultad")]
    public int currentDifficulty = 1;
    public int scoresPerLevel = 4; // Puntos necesarios para subir de nivel

    [Header("Dificultad 1 - Fácil")]
    public Vector2 diff1_XRange = new Vector2(-1f, 1f);
    public Vector2 diff1_YRange = new Vector2(1.5f, 2.5f);
    public Vector2 diff1_ZRange = new Vector2(4f, 5f);

    [Header("Dificultad 2 - Media")]
    public Vector2 diff2_XRange = new Vector2(-2.5f, 2.5f);
    public Vector2 diff2_YRange = new Vector2(1f, 3.5f);
    public Vector2 diff2_ZRange = new Vector2(5f, 7f);

    [Header("Dificultad 3 - Con Movimiento")]
    public Vector2 diff3_XRange = new Vector2(-2.5f, 2.5f);
    public Vector2 diff3_YRange = new Vector2(1f, 3.5f);
    public Vector2 diff3_ZRange = new Vector2(5f, 7f);
    public bool diff3_EnableMovement = true;
    public float diff3_MoveSpeed = 2f;
    public float diff3_MoveRange = 2f;

    [Header("Dificultad 4 - Con Obstáculos")]
    public Vector2 diff4_XRange = new Vector2(-3f, 3f);
    public Vector2 diff4_YRange = new Vector2(0.5f, 4f);
    public Vector2 diff4_ZRange = new Vector2(6f, 8f);
    public bool diff4_EnableMovement = true;
    public float diff4_MoveSpeed = 2.5f;
    public float diff4_MoveRange = 2.5f;
    public GameObject obstaclePrefab; // Prefab del obstáculo

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        currentDifficulty = 1;
    }

    public void CheckDifficultyIncrease()
    {
        int currentScore = ScoreManager.instance.GetScore();
        int targetDifficulty = (currentScore / scoresPerLevel) + 1;
        targetDifficulty = Mathf.Clamp(targetDifficulty, 1, 4);

        if (targetDifficulty > currentDifficulty)
        {
            currentDifficulty = targetDifficulty;
            Debug.Log($"¡Dificultad aumentada a nivel {currentDifficulty}! (Score: {currentScore})");
        }
    }

    public Vector3 GetRandomHoopPosition(Vector3 currentHoopPos)
    {
        Vector2 xRange, yRange, zRange;

        switch (currentDifficulty)
        {
            case 1:
                xRange = diff1_XRange;
                yRange = diff1_YRange;
                zRange = diff1_ZRange;
                break;
            case 2:
                xRange = diff2_XRange;
                yRange = diff2_YRange;
                zRange = diff2_ZRange;
                break;
            case 3:
                xRange = diff3_XRange;
                yRange = diff3_YRange;
                zRange = diff3_ZRange;
                break;
            case 4:
            default:
                xRange = diff4_XRange;
                yRange = diff4_YRange;
                zRange = diff4_ZRange;
                break;
        }

        return new Vector3(
            Random.Range(xRange.x, xRange.y),
            Random.Range(yRange.x, yRange.y),
            currentHoopPos.z + Random.Range(zRange.x, zRange.y)
        );
    }

    public bool ShouldHoopMove()
    {
        return (currentDifficulty >= 3);
    }

    public float GetMoveSpeed()
    {
        if (currentDifficulty == 3)
            return diff3_MoveSpeed;
        else if (currentDifficulty >= 4)
            return diff4_MoveSpeed;
        return 0f;
    }

    public float GetMoveRange()
    {
        if (currentDifficulty == 3)
            return diff3_MoveRange;
        else if (currentDifficulty >= 4)
            return diff4_MoveRange;
        return 0f;
    }

    public bool ShouldSpawnObstacle()
    {
        // Solo spawnar obstáculos si estamos en dificultad 4 o superior
        if (currentDifficulty < 4 || obstaclePrefab == null)
            return false;
        
        // En dificultad 4, 50% de probabilidad de obstáculo
        return Random.value > 0.5f;
    }

    public void ResetDifficulty()
    {
        currentDifficulty = 1;
    }
}