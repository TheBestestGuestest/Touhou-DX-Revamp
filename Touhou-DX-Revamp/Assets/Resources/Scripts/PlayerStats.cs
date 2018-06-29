using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {
    public int powerLevel;
    public int powerGauge;
    private int[] powerReqs;
    private float powerTimer = 0f;
    private float powerDecRate = 0.2f;

    public int speed;
    public float scoreMultiplier;
    public int bulletDamage;
    public float shootingRate = 0.05f;
    public int bombDamage;

    private int maxLives;
    private int currLives;
    private int maxBombs;
    public int currBombs;

    private readonly Vector3 SPAWN_LOC = new Vector3(0, -3, 0);
    private Transform trans;

    public bool isInvuln;
    public bool isInvincible;
    public bool isLaser;

    private PlayerInputControl pic;
    private PlayerStatsCounter psc;

    void Awake() {
        pic = GetComponent<PlayerInputControl>();
        psc = GetComponent<PlayerStatsCounter>();
        trans = transform;
    }
    void Start() {
        changePowerLevelStats(0);
        powerGauge = 0;
        powerReqs = new int[] { 0, 100, 300 };

        maxLives = 3;
        maxBombs = 3;
        currLives = maxLives;
        currBombs = maxBombs;

        isInvuln = false;

        psc.lives.text = currLives.ToString();
        psc.bombs.text = currBombs.ToString();
    }

    void Update() {
        //power levels
        int dec = powerReqs[Mathf.Min(powerLevel + 1, powerReqs.Length - 1)] / 100;
        if (powerTimer >= powerDecRate) {
            if (powerGauge - dec <= 0) {
                powerLevel = Mathf.Max(0, powerLevel - 1);
                changePowerLevelStats(--powerLevel);
                psc.power.text = powerLevel.ToString();
                powerGauge = powerReqs[powerLevel + 1];
            }
            else powerGauge -= dec;
            psc.gauge.text = powerGauge.ToString();
            powerTimer = 0f;
        }

        //timers
        powerTimer += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag.Equals("Enemy Projectile") && !isInvuln) StartCoroutine(takeDamage());
        if (collision.tag.Equals("Drop")) StartCoroutine(getDrop(collision));
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

            isInvuln = true;
            yield return new WaitForSeconds(2f);
            isInvuln = false;
        }
        else {
            Destroy(gameObject);
        }
    }

    public IEnumerator getDrop(Collider2D col) {
        Destroy(col.gameObject);

        //powergauge stuff
        if (!(powerLevel == powerReqs.Length - 1 && powerGauge >= powerReqs[powerLevel])) powerGauge++;
        if (powerLevel < powerReqs.Length - 1 && powerGauge >= powerReqs[powerLevel + 1]) {
            changePowerLevelStats(++powerLevel);
            psc.power.text = powerLevel.ToString();
            powerGauge = 0;
        }
        psc.gauge.text = powerGauge.ToString();

        yield return null;
    }

    public void changePowerLevelStats(int power) {
        powerLevel = power;
        switch (power) {
            case 0:
                speed = 5;
                scoreMultiplier = 1;
                bulletDamage = 7;
                shootingRate = 0.05f;
                bombDamage = 100;
                break;
            case 1:
                speed = 6;
                scoreMultiplier = 1.1f;
                bulletDamage = 10;
                shootingRate = 0.05f;
                bombDamage = 100;
                break;
            case 2:
                speed = 7;
                scoreMultiplier = 1.2f;
                bulletDamage = 20;
                shootingRate = 0.03f;
                bombDamage = 200;
                break;
        }
    }

    public bool isAlive() {
        return currLives > 0;
    }
}
