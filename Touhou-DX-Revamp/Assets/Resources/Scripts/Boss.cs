using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour {
    public int maxHealth;
    public int currHealth;

    private float shootingRate = 2f;
    private float shootTimer = 1f;

    private Transform trans;
    private BossHealthBar bhb;

    private delegate void makeBulletPatterns();
    private List<makeBulletPatterns> bulletPatterns;

    private Vector3 destination;
    private Vector3 temp;
    private float moveTimer = 0f;
    private float moveRate = 3f;


    void Awake() {
        trans = transform;
        bhb = GetComponentInChildren<BossHealthBar>() as BossHealthBar;
    }
    void Start() {
        maxHealth = 10000;
        currHealth = maxHealth;

        bulletPatterns = new List<makeBulletPatterns>();
        bulletPatterns.Add(makeCirclePattern);
        bulletPatterns.Add(makeSwirlPattern);
        bulletPatterns.Add(makeSweepPattern);

        destination = trans.position;
        temp = destination;
    }

    void Update() {
        //shooting
        if (shootTimer >= shootingRate) {
            shootTimer = 0f;
            bulletPatterns[(int)(Random.value * bulletPatterns.Count)]();
        }

        //movement
        if (moveTimer >= moveRate * 3f) {
            temp = destination;
            destination = new Vector3(Random.value * 12f - 6, Random.value * 3 + 1, temp.z);
            moveTimer = 0f;
        }
        else if (moveTimer <= moveRate || !trans.position.Equals(destination)) trans.position = Vector3.Lerp(temp, destination, moveTimer / moveRate);

        shootTimer += Time.deltaTime;
        moveTimer += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag.Equals("Player Projectile")) {
            string colName = collision.gameObject.name;
            Projectile p = collision.gameObject.GetComponent<Projectile>();

            currHealth = Mathf.Max(currHealth - p.damage, 0);
            bhb.updateBar();
            if (!p.isPiercing) Destroy(collision.gameObject);

            Drop.Create("Prefabs/Drop", trans.position + new Vector3(Random.value - 0.5f, Random.value - 0.5f, 0) * 0.6f, null, 0.07f, (Random.value - 0.5f) * 300 + 90);

            //Debug.Log(currHealth);
            if (!isAlive()) {
                Destroy(gameObject);
            }
        }
    }

    public bool isAlive() {
        return currHealth > 0;
    }

    //bullet patterns
    public void makeCirclePattern() {
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 16; j++) Projectile.Create("Prefabs/BossBomb", new Vector3(trans.position.x, trans.position.y, trans.position.z), circlePattern(i, j));
        }
    }
    public MovePath circlePattern(int i, int j) {
        return delegate (float t, Vector3 pos) {
            float rads = j * Mathf.PI / 8;
            float tOffset = Mathf.Max(t - i / 10f, 0);
            float distX = tOffset * 5 * Mathf.Cos(rads);
            float distY = tOffset * 5 * Mathf.Sin(rads);
            return new Vector3(pos.x + distX, pos.y + distY, pos.z);
        };
    }

    public void makeSwirlPattern() {
        int dir = Random.value >= 0.5 ? 1 : -1;
        for (int i = 0; i < 4; i++) {
            dir *= -1;
            for (int j = 0; j < 16; j++) Projectile.Create("Prefabs/BossSwirl", new Vector3(trans.position.x, trans.position.y, trans.position.z), swirlPattern(i, j, dir));
        }
    }
    public MovePath swirlPattern(int i, int j, int dir) {
        return delegate (float t, Vector3 pos) {
            float rads = (j + 1) * Mathf.PI / 16;
            float tOffset = Mathf.Max(t - i / 2f, 0);
            float distX = tOffset * 3 * dir * Mathf.Cos(rads + tOffset);
            float distY = tOffset * 3 * Mathf.Sin(rads + tOffset);
            return new Vector3(pos.x + distX, pos.y + distY, pos.z);
        };
    }

    public void makeSweepPattern() {
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < (i == 1 ? 7 : 8); j++) {
                float dir = -1f + j / 3.5f + (i == 1 ? 1 / 7f : 0);
                Projectile.Create("Prefabs/BossPotato", new Vector3(trans.position.x, trans.position.y, trans.position.z), sweepPattern(i, j, dir));
            }
        }
    }
    public MovePath sweepPattern(int i, int j, float dir) {
        return delegate (float t, Vector3 pos) {
            float tOffset = Mathf.Max(t - i / 5f, 0);
            float distX = tOffset * 10 * dir;
            float distY = tOffset * -7;
            return new Vector3(pos.x + distX, pos.y + distY, pos.z);
        };
    }
}
