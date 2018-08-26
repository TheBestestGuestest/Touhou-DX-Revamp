using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {
    public int maxPowerLevel;
    public int powerLevel;
    private int getNextPowerLevel() { return Mathf.Min(powerLevel + 1, maxPowerLevel); }
    public int powerGauge;
    private int[] powerReqs;
    private int getCurrPowerReq() { return powerReqs[getNextPowerLevel()]; }

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
        maxPowerLevel = 4;  //CHANGE THIS LATER WHEN POWER LEVELS ARE UNLOCKABLE (MAKE SURE ITS NEVER OVER 4)
        powerReqs = new int[] { 0, 100, 200, 300, 400 };
        powerLevel = 0;
        powerGauge = 0;
        incrementPowerLevel(0);
        incrementPowerGauge(0);

        maxLives = 3;
        maxBombs = 3;
        currLives = maxLives;
        currBombs = maxBombs;
        psc.updateLives(currLives);
        psc.updateBombs(currBombs);

        speed = 5;
        isInvuln = false;
    }

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
        psc.updateLives(currLives);

        if (isAlive()) {
            currBombs = maxBombs + 1;
            pic.useBomb();
            psc.updateBombs(currBombs);

            trans.position = SPAWN_LOC;
            incrementPowerGauge(-powerReqs[getNextPowerLevel()]);

            yield return new WaitForSeconds(2f);
            isInvuln = false;
        }
        else {
            Destroy(gameObject);
            Main.SharedInstance.GameOver();
        }
    }

    public IEnumerator getDrop(Collider2D col) {
        ProjectilePool.SharedInstance.ReturnToPool(col.gameObject);

        //powergauge stuff
        incrementPowerGauge(1);

        Score.SharedInstance.changeScore(1 * scoreMultiplier);

        yield return null;
    }

    public void incrementPowerGauge(int change) {
        //LATER (POWER GAUGE DRAIN)
        powerGauge = Mathf.Max(powerGauge + change, 0);
        if (change > 0) {
            if (powerGauge >= getCurrPowerReq()) {
                if (powerLevel < maxPowerLevel - 1) {
                    powerGauge -= getCurrPowerReq();
                }
                else powerGauge = getCurrPowerReq();
                if (powerLevel < maxPowerLevel) incrementPowerLevel(1);
            }
        }
        else if (powerLevel == maxPowerLevel) {
            incrementPowerLevel(-1);
            powerGauge = 0;
        }

        psc.updatePowerGauge(powerLevel, powerGauge, getCurrPowerReq());
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

        psc.updatePowerGauge(powerLevel, powerGauge, getCurrPowerReq());
    }

    public bool isAlive() {
        return currLives > 0;
    }
}
