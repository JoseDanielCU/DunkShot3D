using UnityEngine;
using System.Collections;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager instance;

    [Header("Referencias Visuales")]
    public Material skyboxMaterial;

    [Header("Colores por dificultad")]
    public Color easyTop = new Color(0.2f, 0.5f, 1f);
    public Color easyBottom = new Color(0.8f, 0.9f, 1f);

    public Color mediumTop = new Color(1f, 0.7f, 0.3f);
    public Color mediumBottom = new Color(1f, 0.9f, 0.6f);

    public Color hardTop = new Color(0.8f, 0.2f, 0.2f);
    public Color hardBottom = new Color(0.4f, 0.05f, 0.05f);

    public Color extremeTop = new Color(0.05f, 0.05f, 0.05f);
    public Color extremeBottom = new Color(0.2f, 0.0f, 0.2f);

    private Coroutine colorTransition;

    [Header("Configuración de Dificultad")]
    public int currentDifficulty = 1;

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
    public GameObject obstaclePrefab;

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
        // Establece colores iniciales del cielo
        if (skyboxMaterial != null)
        {
            skyboxMaterial.SetColor("_TopColor", easyTop);
            skyboxMaterial.SetColor("_BottomColor", easyBottom);
        }
    }

    /// <summary>
    /// Cambia la dificultad según el puntaje actual
    /// 0–9 → Nivel 1
    /// 10–24 → Nivel 2
    /// 25–54 → Nivel 3
    /// 55+ → Nivel 4
    /// </summary>
    public void CheckDifficultyIncrease()
    {
        int score = ScoreManager.instance.GetScore();
        int newDifficulty = 1;

        if (score >= 55) newDifficulty = 4;
        else if (score >= 25) newDifficulty = 3;
        else if (score >= 10) newDifficulty = 2;

        newDifficulty = Mathf.Clamp(newDifficulty, 1, 4);

        if (newDifficulty != currentDifficulty)
        {
            currentDifficulty = newDifficulty;
            Debug.Log($"¡Dificultad cambiada a nivel {currentDifficulty}! (Puntaje: {score})");

            // 🔥 Cambiar los colores del cielo al subir de nivel
            switch (currentDifficulty)
            {
                case 1:
                    UpdateSkyboxColors(easyTop, easyBottom);
                    break;
                case 2:
                    UpdateSkyboxColors(mediumTop, mediumBottom);
                    break;
                case 3:
                    UpdateSkyboxColors(hardTop, hardBottom);
                    break;
                case 4:
                    UpdateSkyboxColors(extremeTop, extremeBottom);
                    break;
            }
        }
    }

    // 🎨 Transición suave de colores del skybox
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
            t += Time.deltaTime / 2f; // duración: 2 segundos
            skyboxMaterial.SetColor("_TopColor", Color.Lerp(currentTop, targetTop, t));
            skyboxMaterial.SetColor("_BottomColor", Color.Lerp(currentBottom, targetBottom, t));
            yield return null;
        }
    }

    // 🏀 Posición del aro según dificultad
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
        if (currentDifficulty < 4 || obstaclePrefab == null)
            return false;

        return Random.value > 0.5f; // 50% de probabilidad en nivel 4
    }

    public void ResetDifficulty()
    {
        currentDifficulty = 1;
        if (skyboxMaterial != null)
        {
            skyboxMaterial.SetColor("_TopColor", easyTop);
            skyboxMaterial.SetColor("_BottomColor", easyBottom);
        }
    }
}
