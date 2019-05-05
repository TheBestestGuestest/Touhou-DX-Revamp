using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Score : MonoBehaviour {
    public static Score SharedInstance;
    public const float MAX_SCORE = 999999999f;
    private TextMeshProUGUI highScoreText;
    private TextMeshProUGUI currScoreText;
    private float score = 0;
    private float highScore = 0;
    private string highScoreKey = "high score: TEST";

    void Awake() {
        SharedInstance = this;
        currScoreText = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();
        highScoreText = GameObject.Find("High Score").GetComponent<TextMeshProUGUI>();

        if (PlayerPrefs.HasKey(highScoreKey)) highScore = PlayerPrefs.GetFloat(highScoreKey);
        else highScore = 0;
        updateScore();
    }

    public void changeScore(float change) {
        score = Mathf.Min(score + change, MAX_SCORE);
        highScore = Mathf.Max(score, highScore);

        updateScore();
    }

    public void updateScore() {
        currScoreText.text = score.ToString("000,000,000");  //sus
        highScoreText.text = highScore.ToString("000,000,000");
    }

    public void saveScore() {
        if (highScore > PlayerPrefs.GetFloat(highScoreKey)) PlayerPrefs.SetFloat(highScoreKey, highScore);
    }
}
