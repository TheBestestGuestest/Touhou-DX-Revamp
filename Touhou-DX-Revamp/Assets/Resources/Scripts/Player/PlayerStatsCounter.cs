using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatsCounter : MonoBehaviour {
    public Slider powerGauge;
    private TextMeshProUGUI power;
    private TextMeshProUGUI gauge;

    public GameObject[] lifeIcons;
    public GameObject[] bombIcons;

    void Awake() {
        power = GameObject.Find("Power").GetComponent<TextMeshProUGUI>();
        gauge = GameObject.Find("Gauge").GetComponent<TextMeshProUGUI>();
    }

    public void updateLives(int num) {
        if (num >= lifeIcons.Length) num = 3;
        for(int i = 0; i < 3; i++) lifeIcons[i].SetActive(i < num);
    }
    public void updateBombs(int num) {
        if (num >= bombIcons.Length) num = 3;
        for(int i = 0; i < 3; i++) bombIcons[i].SetActive(i < num);
    }
    public void updatePowerGauge(int pow, int currG, int maxG) {
        power.text = ""+pow;

        if (maxG != 0) {
            gauge.text = currG + "/" + maxG;
            powerGauge.value = (float)currG / maxG;
        }
        else {
            gauge.text = "";
            powerGauge.value = 1;
        }
    }
}
