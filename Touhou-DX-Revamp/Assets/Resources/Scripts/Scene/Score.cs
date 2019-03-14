using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {
    public GameObject highScoreText;
    public GameObject currScoreText;

    private SpriteRenderer[] highScoreDigits;
    private SpriteRenderer[] currScoreDigits;

    private Sprite[] digits;

    public static Score SharedInstance;
    private float score = 0;
    private float highScore = 0;
    private string highScoreKey = "high score: TEST";

    void Awake() {
        SharedInstance = this;
        if (PlayerPrefs.HasKey(highScoreKey)) highScore = PlayerPrefs.GetFloat(highScoreKey);
        else highScore = 0;

        highScoreDigits = highScoreText.GetComponentsInChildren<SpriteRenderer>();
        currScoreDigits = currScoreText.GetComponentsInChildren<SpriteRenderer>();
        digits = Resources.LoadAll<Sprite>("Sprites/UISprites/SideUI/SideUIFont");

        updateScore();
    }

    public void changeScore(float change) {
        score += change;
        if (score > 999999999f) score = 999999999f;
        if (score > highScore) highScore = score;

        updateScore();
    }

    public void updateScore() {
        float tempScore = score;
        for (int i = 0; i < 9; i++) {
            int digit = (int)(tempScore % 10);
            currScoreDigits[8 - i].sprite = digits[digit];
            tempScore /= 10;
        }

        tempScore = highScore;
        for (int i = 0; i < 9; i++) {
            int digit = (int)(tempScore % 10);
            highScoreDigits[8 - i].sprite = digits[digit];
            tempScore /= 10;
        }
    }

    public void saveScore() {
        if (highScore > PlayerPrefs.GetFloat(highScoreKey)) PlayerPrefs.SetFloat(highScoreKey, highScore);
    }
}
