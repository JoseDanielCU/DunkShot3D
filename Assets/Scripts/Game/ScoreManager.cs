using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;

    [Header("Popups")]
    public GameObject pointPopupPrefab;
    public Canvas canvas;

    private int score = 0;
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

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
        }

        if (pointPopupPrefab != null && canvas != null)
        {
            GameObject popup = Instantiate(pointPopupPrefab, canvas.transform);
            popup.GetComponent<TextMeshProUGUI>().text = "+" + value;

            if (worldPosition != default)
                popup.transform.position = Camera.main.WorldToScreenPoint(worldPosition);
            else
                popup.transform.position = canvas.transform.position;
        }
    }

    public void ResetScore()
    {
        score = 0;
        UpdateUI();
    }

    public int GetScore()
    {
        return score;
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "SCORE: " + score;

        if (highScoreText != null)
            highScoreText.text = "HIGH SCORE: " + highScore;
    }
}