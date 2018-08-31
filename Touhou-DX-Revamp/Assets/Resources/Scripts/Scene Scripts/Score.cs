using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {
    public Text highScoreText;
    public Text currScoreText;

    public static Score SharedInstance;
    private float score = 0;
    private float highScore = 0;
    private string highScoreKey = "high score: TEST";

    void Start() {
        SharedInstance = this;
        if (PlayerPrefs.HasKey(highScoreKey)) highScore = PlayerPrefs.GetFloat(highScoreKey);
        else highScore = 0;

        updateScore();
    }

    public void changeScore(float change) {
        score += change;
        if (score > highScore) highScore = score;

        updateScore();
    }

    public void updateScore() {
        currScoreText.text = ((int)(score)).ToString().PadLeft(7, '0');
        highScoreText.text = ((int)(highScore)).ToString().PadLeft(7, '0');
    }

    public void saveScore() {
        if (highScore > PlayerPrefs.GetFloat(highScoreKey)) PlayerPrefs.SetFloat(highScoreKey, highScore);
    }
}
