using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {
    public Text highScore;
    public Text currScore;

    public static Score SharedInstance;
    public float score;

    // Use this for initialization
    void Start() {
        SharedInstance = this;
        score = 0;
        updateScore();
        setHighScore();
    }
    private void setHighScore() {
        //something herer
    }

    public void changeScore(float change) {
        score += change;
        updateScore();
    }

    public void updateScore() {
        currScore.text = ((int)(score)).ToString().PadLeft(7, '0');
    }
}
