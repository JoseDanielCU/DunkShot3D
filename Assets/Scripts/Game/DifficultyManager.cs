using UnityEngine;
using System.Collections;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager instance;

    [Header("Referencias Visuales")]
    public Material skyboxMaterial;

    [Header("Colores por dificultad")]
    public Color easyTop = new Color(0.3f, 0.8f, 0.4f);       // Verde fresco
    public Color easyBottom = new Color(0.6f, 1f, 0.7f);

    public Color mediumTop = new Color(1f, 0.8f, 0.2f);       // Amarillo cálido
    public Color mediumBottom = new Color(1f, 0.95f, 0.6f);

    public Color hardTop = new Color(1f, 0.4f, 0.3f);         // Rojo anaranjado
    public Color hardBottom = new Color(0.5f, 0.15f, 0.1f);

    public Color veryHardTop = new Color(0.7f, 0.1f, 0.5f);   // Fucsia / violeta
    public Color veryHardBottom = new Color(0.3f, 0.05f, 0.25f);

    public Color chaosTop = new Color(0.3f, 0.0f, 0.5f);      // Púrpura oscuro
    public Color chaosBottom = new Color(0.1f, 0.0f, 0.2f);

    public Color nightmareTop = new Color(0.0f, 0.0f, 0.0f);  // Negro total
    public Color nightmareBottom = new Color(0.05f, 0.05f, 0.05f);

    private Coroutine colorTransition;

    [Header("Configuración de Dificultad")]
    public int currentDifficulty = 1;

    [Header("Dificultad 1 - Fácil")]
    public Vector2 diff1_XRange = new Vector2(-1f, 1f);
    public Vector2 diff1_YRange = new Vector2(1.5f, 2.5f);
    public Vector2 diff1_ZRange = new Vector2(4f, 5f);

    [Header("Dificultad 2 - Media")]
    public Vector2 diff2_XRange = new Vector2(-2f, 2f);
    public Vector2 diff2_YRange = new Vector2(1f, 3f);
    public Vector2 diff2_ZRange = new Vector2(5f, 6.5f);

    [Header("Dificultad 3 - Movimiento Suave")]
    public Vector2 diff3_XRange = new Vector2(-2.5f, 2.5f);
    public Vector2 diff3_YRange = new Vector2(1f, 3.5f);
    public Vector2 diff3_ZRange = new Vector2(5f, 7f);
    public float diff3_MoveSpeed = 2f;
    public float diff3_MoveRange = 2f;

    [Header("Dificultad 4 - Movimiento + Rápido")]
    public Vector2 diff4_XRange = new Vector2(-3f, 3f);
    public Vector2 diff4_YRange = new Vector2(0.8f, 3.8f);
    public Vector2 diff4_ZRange = new Vector2(5.5f, 7.5f);
    public float diff4_MoveSpeed = 3f;
    public float diff4_MoveRange = 3f;

    [Header("Dificultad 5 - Obstáculos")]
    public Vector2 diff5_XRange = new Vector2(-3f, 3f);
    public Vector2 diff5_YRange = new Vector2(0.5f, 4f);
    public Vector2 diff5_ZRange = new Vector2(6f, 8f);
    public float diff5_MoveSpeed = 2f;
    public float diff5_MoveRange = 2f;

    [Header("Dificultad 6 - Caos total 😈")]
    public Vector2 diff6_XRange = new Vector2(-3.5f, 3.5f);
    public Vector2 diff6_YRange = new Vector2(0.3f, 4.5f);
    public Vector2 diff6_ZRange = new Vector2(6.5f, 8.5f);
    public float diff6_MoveSpeed = 3f;
    public float diff6_MoveRange = 3f;

    [Header("Obstáculos")]
    public GameObject obstaclePrefab;
    [Header("Material del Piso")]
    public Material floorMaterial;

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
        ApplyDifficultyColors(1); // ✅ Forzar color inicial verde
    }

    public void CheckDifficultyIncrease()
    {
        int score = ScoreManager.instance.GetScore();
        int newDifficulty = 1;

        if (score >= 100) newDifficulty = 6;
        else if (score >= 60) newDifficulty = 5;
        else if (score >= 40) newDifficulty = 4;
        else if (score >= 20) newDifficulty = 3;
        else if (score >= 10) newDifficulty = 2;

        if (newDifficulty != currentDifficulty)
        {
            currentDifficulty = newDifficulty;
            Debug.Log($"🔥 Dificultad aumentada a nivel {currentDifficulty} (Puntaje: {score})");
            UpdateSkyboxByDifficulty();
        }
    }

    private void UpdateFloorColor(Color color)
    {
        if (floorMaterial != null)
            floorMaterial.SetColor("_BaseColor", color);
    }

    private void UpdateSkyboxByDifficulty()
    {
        ApplyDifficultyColors(currentDifficulty);
    }

    private void ApplyDifficultyColors(int difficulty)
    {
        Color top, bottom, floorColor;

        switch (difficulty)
        {
            case 1:
                top = easyTop; bottom = easyBottom; floorColor = new Color(0.5f, 1f, 0.5f);
                break;
            case 2:
                top = mediumTop; bottom = mediumBottom; floorColor = new Color(1f, 0.9f, 0.5f);
                break;
            case 3:
                top = hardTop; bottom = hardBottom; floorColor = new Color(1f, 0.5f, 0.4f);
                break;
            case 4:
                top = veryHardTop; bottom = veryHardBottom; floorColor = new Color(0.6f, 0.2f, 0.6f);
                break;
            case 5:
                top = chaosTop; bottom = chaosBottom; floorColor = new Color(0.3f, 0.05f, 0.4f);
                break;
            case 6:
            default:
                top = nightmareTop; bottom = nightmareBottom; floorColor = new Color(0.1f, 0.1f, 0.1f);
                break;
        }

        UpdateSkyboxColors(top, bottom);
        UpdateFloorColor(floorColor);
    }

    private void UpdateSkyboxColors(Color top, Color bottom)
    {
        if (skyboxMaterial == null) return;

        if (colorTransition != null)
            StopCoroutine(colorTransition);

        colorTransition = StartCoroutine(LerpSkybox(top, bottom));
    }

    private IEnumerator LerpSkybox(Color targetTop, Color targetBottom)
    {
        Color currentTop = skyboxMaterial.GetColor("_TopColor");
        Color currentBottom = skyboxMaterial.GetColor("_BottomColor");

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 2f;
            skyboxMaterial.SetColor("_TopColor", Color.Lerp(currentTop, targetTop, t));
            skyboxMaterial.SetColor("_BottomColor", Color.Lerp(currentBottom, targetBottom, t));
            yield return null;
        }
    }

    public Vector3 GetRandomHoopPosition(Vector3 currentHoopPos)
    {
        Vector2 xRange, yRange, zRange;
        switch (currentDifficulty)
        {
            case 1: xRange = diff1_XRange; yRange = diff1_YRange; zRange = diff1_ZRange; break;
            case 2: xRange = diff2_XRange; yRange = diff2_YRange; zRange = diff2_ZRange; break;
            case 3: xRange = diff3_XRange; yRange = diff3_YRange; zRange = diff3_ZRange; break;
            case 4: xRange = diff4_XRange; yRange = diff4_YRange; zRange = diff4_ZRange; break;
            case 5: xRange = diff5_XRange; yRange = diff5_YRange; zRange = diff5_ZRange; break;
            case 6:
            default:
                xRange = diff6_XRange; yRange = diff6_YRange; zRange = diff6_ZRange;
                break;
        }

        return new Vector3(
            Random.Range(xRange.x, xRange.y),
            Random.Range(yRange.x, yRange.y),
            currentHoopPos.z + Random.Range(zRange.x, zRange.y)
        );
    }

    public bool ShouldHoopMove() => currentDifficulty >= 3;

    public float GetMoveSpeed()
    {
        switch (currentDifficulty)
        {
            case 3: return diff3_MoveSpeed;
            case 4: return diff4_MoveSpeed;
            case 5: return diff5_MoveSpeed;
            case 6: return diff6_MoveSpeed;
            default: return 0f;
        }
    }

    public float GetMoveRange()
    {
        switch (currentDifficulty)
        {
            case 3: return diff3_MoveRange;
            case 4: return diff4_MoveRange;
            case 5: return diff5_MoveRange;
            case 6: return diff6_MoveRange;
            default: return 0f;
        }
    }

    public bool ShouldSpawnObstacle()
    {
        if (obstaclePrefab == null || currentDifficulty < 4)
            return false;

        float probability = 0f;
        switch (currentDifficulty)
        {
            case 4: probability = 0.3f; break;
            case 5: probability = 0.55f; break;
            case 6: probability = 0.8f; break;
        }

        return Random.value < probability;
    }

    public void ResetDifficulty()
    {
        currentDifficulty = 1;
        ApplyDifficultyColors(1); // ✅ Ahora también reinicia piso + cielo al verde
    }
}
