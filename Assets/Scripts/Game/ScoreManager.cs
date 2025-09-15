using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;

    [Header("Popups")]
    public GameObject pointPopupPrefab; // prefab de +1 / +2
    public Canvas canvas;               // canvas donde aparecerán los popups

    private int score = -1;
    private int highScore = 0;

    private const string HighScoreKey = "HighScore";

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddScore(int value, Vector3 worldPosition = default)
    {
        score += value;
        UpdateUI();

        // Revisar High Score
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
        }

        // Mostrar popup si se asignó prefab
        if (pointPopupPrefab != null && canvas != null)
        {
            GameObject popup = Instantiate(pointPopupPrefab, canvas.transform);
            popup.GetComponent<TextMeshProUGUI>().text = "+" + value;

            // Si se pasa una posición en mundo, convertir a pantalla
            if (worldPosition != default)
                popup.transform.position = Camera.main.WorldToScreenPoint(worldPosition);
            else
                popup.transform.position = canvas.transform.position; // centro por defecto
        }
    }

    public void ResetScore()
    {
        score = -1;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "SCORE: " + score;

        if (highScoreText != null)
            highScoreText.text = "HIGH SCORE: " + highScore;
    }
}
