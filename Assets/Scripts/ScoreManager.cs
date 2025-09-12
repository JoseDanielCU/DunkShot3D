using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public TMP_Text scoreText;
    public TMP_Text highScoreText; // Nuevo campo para mostrar High Score

    private int score = -1;
    private int highScore = 0;

    private const string HighScoreKey = "HighScore";

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        // Cargar High Score guardado
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateUI();

        // Revisar si hay nuevo High Score
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
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
            scoreText.text = "Score: " + score;

        if (highScoreText != null)
            highScoreText.text = "High Score: " + highScore;
    }
}
