using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsCounter : MonoBehaviour {
    public Text gauge;
    public Slider powerGauge;

    public GameObject power;
    private SpriteRenderer powerDigit;
    private Sprite[] digits;

    public GameObject[] lifeIcons;
    public GameObject[] bombIcons;

    void Awake() {
        powerDigit = power.GetComponentInChildren<SpriteRenderer>();
        digits = Resources.LoadAll<Sprite>("Sprites/UISprites/SideUI/SideUIFont");
    }

    public void updateLives(int num) {
        if (num >= lifeIcons.Length) num = 3;
        int i = 0;
        for (; i < num; i++) lifeIcons[i].SetActive(false);
        for (; i < lifeIcons.Length; i++) lifeIcons[i].SetActive(true);
    }
    public void updateBombs(int num) {
        if (num >= bombIcons.Length) num = 3;
        int i = 0;
        for (; i < num; i++) bombIcons[i].SetActive(false);
        for (; i < bombIcons.Length; i++) bombIcons[i].SetActive(true);
    }
    public void updatePowerGauge(int pow, int currG, int maxG) {
        powerDigit.sprite = digits[pow + 1];

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
