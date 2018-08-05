using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {
    public int powerLevel;
    public int powerGauge;
    private int[] powerReqs;

    public int speed;
    public float scoreMultiplier;
    public int bulletDamage;
    public float shootingRate = 0.05f;
    public int bombDamage;

    private int maxLives;
    private int currLives;
    private int maxBombs;
    public int currBombs;

    private readonly Vector3 SPAWN_LOC = new Vector3(InGameDimentions.centerX, -3, 0);
    private Transform trans;

    public bool isInvuln;
    public bool isInvincible;
    public bool isLaser;

    private PlayerInputControl pic;
    private PlayerStatsCounter psc;

    private string enemyProjectileTag = "Enemy Projectile";
    private string functionTag = "Function";  //enum?
    private string dropTag = "Drop";
    void Awake() {
        pic = GetComponent<PlayerInputControl>();
        psc = GetComponent<PlayerStatsCounter>();
        trans = transform;
    }
    void Start() {
        powerReqs = new int[] { 0, 100, 200, 300, 400 };
        powerLevel = 0;
        powerGauge = 0;
        incrementPowerLevel(0);
        incrementPowerGauge(0);

        maxLives = 3;
        maxBombs = 3;
        currLives = maxLives;
        currBombs = maxBombs;

        speed = 5;
        isInvuln = false;

        psc.lives.text = currLives.ToString();
        psc.bombs.text = currBombs.ToString();
    }

    //void Update() {}

    private void OnTriggerStay2D(Collider2D collision) {
        if (!isInvuln && (collision.CompareTag(enemyProjectileTag) || collision.CompareTag(functionTag))) {
            isInvuln = true;
            StartCoroutine(takeDamage());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag(dropTag)) StartCoroutine(getDrop(collision));
    }

    public IEnumerator takeDamage() {
        currLives--;
        psc.lives.text = currLives.ToString();

        if (isAlive()) {
            currBombs = maxBombs + 1;
            pic.useBomb();
            psc.bombs.text = currBombs.ToString();

            trans.position = SPAWN_LOC;
            powerGauge = 0;

            yield return new WaitForSeconds(2f);
            isInvuln = false;
        }
        else {
            Destroy(gameObject);
        }
    }

    public IEnumerator getDrop(Collider2D col) {
        ProjectilePool.SharedInstance.ReturnToPool(col.gameObject);

        //powergauge stuff
        incrementPowerGauge(1);

        yield return null;
    }

    public void incrementPowerGauge(int change) {
        //LATER (POWER GAUGE DRAIN)
        powerGauge = Mathf.Max(powerGauge + change, 0);
        if (powerLevel == 4) powerGauge = powerReqs[powerLevel];
        else if (powerGauge > powerReqs[powerLevel + 1]) {
            powerGauge -= powerReqs[powerLevel + 1];
            incrementPowerLevel(1);
        }

        psc.gauge.text = powerGauge.ToString();
    }
    private void incrementPowerLevel(int change) {  //will balance later
        powerLevel = Mathf.Max(powerLevel + change, 0);
        switch (powerLevel) {
            case 0:
                scoreMultiplier = 1;
                bulletDamage = 7;
                shootingRate = 0.07f;
                bombDamage = 1;
                break;
            case 1:
                scoreMultiplier = 1.25f;
                bulletDamage = 8;
                shootingRate = 0.06f;
                bombDamage = 1;
                break;
            case 2:
                scoreMultiplier = 1.5f;
                bulletDamage = 9;
                shootingRate = 0.05f;
                bombDamage = 1;
                break;
            case 3:
                scoreMultiplier = 1.75f;
                bulletDamage = 10;
                shootingRate = 0.04f;
                bombDamage = 1;
                break;
            case 4:
                scoreMultiplier = 2f;
                bulletDamage = 11;
                shootingRate = 0.03f;
                bombDamage = 1;
                break;
        }
        psc.power.text = Convert.ToString(powerLevel + 1);
    }

    public bool isAlive() {
        return currLives > 0;
    }
}
