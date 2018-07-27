using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour {
    [HideInInspector]
    public int maxHealth, currHealth;

    private float shootingRate = 2f;
    private float shootTimer;

    private BossHealthBar bhb;
    private BossBulletPatterns bbp;

    private Transform trans;

    private Vector3 destination;
    private Vector3 temp;
    private float moveTimer;
    private float moveRate = 3f;

    void Awake() {
        trans = transform;
        bhb = GetComponentInChildren<BossHealthBar>() as BossHealthBar;
        bbp = new BossBulletPatterns(trans);

        gameObject.SetActive(false);
    }
    void OnEnable() {
        GameQueue.SharedInstance.isQueueing = false;

        maxHealth = 10;
        currHealth = maxHealth;
        moveTimer = 0f;
        shootTimer = 0f;

        destination = trans.position;
        temp = destination;
    }

    void Update() {
        //shooting
        if (shootTimer >= shootingRate) {
            shootTimer = 0f;
            StartCoroutine(bbp.runBulletPattern((int)(Random.value * bbp.getCount())));
        }

        //movement
        if (moveTimer >= moveRate * 3f) {
            temp = destination;
            destination = new Vector3(Random.value * (-InGameDimentions.screenWidth + 0.333f) - InGameDimentions.centerX, Random.value * 3 + 1, temp.z);
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
            if (!p.isPiercing) ProjectilePool.SharedInstance.ReturnToPool(collision.gameObject);

            ProjectilePool.SharedInstance.GetPooledDrop("Prefabs/Drop", trans.position + new Vector3(Random.value - 0.5f, Random.value - 0.5f, 0) * 0.6f, null, 0.07f, (Random.value - 0.5f) * 300 + 90);

            //Debug.Log(currHealth);
            if (!isAlive()) {
                GameQueue.SharedInstance.isQueueing = true;
                gameObject.SetActive(false);
            }
        }
    }

    public bool isAlive() {
        return currHealth > 0;
    }

}

public class BossBulletPatterns {
    public Transform trans;
    private delegate IEnumerator makeBulletPatterns();
    private List<makeBulletPatterns> bulletPatterns;

    private MovePath[][] cachedCirclePattern = new MovePath[4][];
    private MovePath[][][] cachedSwirlPattern = new MovePath[2][][];
    private MovePath[][] cachedSweepPattern = new MovePath[3][];

    private MovePath circlePattern(int i, int j) {
        return delegate (float t, Vector3 pos) {
            float rads = j * Mathf.PI / 8;
            float distX = t * 5 * Mathf.Cos(rads);
            float distY = t * 5 * Mathf.Sin(rads);
            return new Vector3(pos.x + distX, pos.y + distY, pos.z);
        };
    }
    private MovePath swirlPattern(int i, int j, int dir) {
        return delegate (float t, Vector3 pos) {
            float rads = (j + 1) * Mathf.PI / 16;
            float distX = t * 3 * dir * Mathf.Cos(rads + t);
            float distY = t * 3 * Mathf.Sin(rads + t);
            return new Vector3(pos.x + distX, pos.y + distY, pos.z);
        };
    }
    private MovePath sweepPattern(int i, int j, float dir) {
        return delegate (float t, Vector3 pos) {
            float distX = t * 10 * dir;
            float distY = t * -7;
            return new Vector3(pos.x + distX, pos.y + distY, pos.z);
        };
    }

    private IEnumerator makeCirclePattern() {
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 16; j++) {
                ProjectilePool.SharedInstance.GetPooledProjectile("Prefabs/BossBomb", trans.position, cachedCirclePattern[i][j]);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    private IEnumerator makeSwirlPattern() {
        int dir = Random.value >= 0.5 ? 1 : -1;
        for (int i = 0; i < 4; i++) {
            dir *= -1;
            for (int j = 0; j < 16; j++) {
                ProjectilePool.SharedInstance.GetPooledProjectile("Prefabs/BossSwirl", trans.position, cachedSwirlPattern[(dir + 1) / 2][i][j]);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    private IEnumerator makeSweepPattern() {
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < (i == 1 ? 7 : 8); j++) {
                float dir = -1f + j / 3.5f + (i == 1 ? 1 / 7f : 0);
                ProjectilePool.SharedInstance.GetPooledProjectile("Prefabs/BossPotato", trans.position, cachedSweepPattern[i][j]);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    public BossBulletPatterns(Transform transform) {
        trans = transform;
        cacheAll();
    }
    private void cacheAll() {
        bulletPatterns = new List<makeBulletPatterns>();
        bulletPatterns.Add(makeCirclePattern);
        for (int i = 0; i < 4; i++) {
            cachedCirclePattern[i] = new MovePath[16];
            for (int j = 0; j < 16; j++)
                cachedCirclePattern[i][j] = circlePattern(i, j);
        }

        bulletPatterns.Add(makeSwirlPattern);
        for (int i = 0; i < 2; i++) {
            int dir = i * 2 - 1;
            cachedSwirlPattern[i] = new MovePath[4][];
            for (int j = 0; j < 4; j++) {
                cachedSwirlPattern[i][j] = new MovePath[16];
                for (int k = 0; k < 16; k++)
                    cachedSwirlPattern[i][j][k] = swirlPattern(j, k, dir);
            }
        }


        bulletPatterns.Add(makeSweepPattern);
        for (int i = 0; i < 3; i++) {
            int jMax = i == 1 ? 7 : 8;
            cachedSweepPattern[i] = new MovePath[jMax];
            for (int j = 0; j < jMax; j++) {
                float dir = -1f + j / 3.5f + (i == 1 ? 1 / 7f : 0);
                cachedSweepPattern[i][j] = sweepPattern(i, j, dir);
            }
        }
    }

    public IEnumerator runBulletPattern(int i) {
        if (i >= 0 && i < bulletPatterns.Count) return bulletPatterns[i]();
        throw new System.Exception();
    }
    public int getCount() {
        return bulletPatterns.Count;
    }
}
