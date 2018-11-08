using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [HideInInspector]
    public int maxHealth, currHealth;

    protected Transform trans;
    protected EnemyHealthBar hb;
    protected EnemyBulletPatterns bp;
    protected EnemyFunctions funcs;

    protected float globalTimer;

    private string playerProjectileTag = "Player Projectile";

    protected virtual void Awake() {
        trans = transform;

        hb = GetComponentInChildren<EnemyHealthBar>() as EnemyHealthBar;
        bp = new EnemyBulletPatterns(trans);
        funcs = new EnemyFunctions(trans);

        gameObject.SetActive(false);
    }
    private void OnEnable() {
        initiateStats();
        globalTimer = 0.0f;
        hb.updateBar();
        trans = transform;
    }
    protected virtual void initiateStats() {
        GameQueue.SharedInstance.isQueueing = false;
        maxHealth = 10000;
        currHealth = maxHealth;
    }

    private void Update() {
        shoot();
        move();
        function();
        globalTimer += Time.deltaTime;
    }
    protected virtual void shoot() {
        //StartCoroutine(bbp.runBulletPattern((int)(UnityEngine.Random.value * bbp.getCount())));
    }
    protected virtual void move() {
    }
    protected virtual void function() {
        //funcs.activateFunction(-1);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag(playerProjectileTag)) OnPlayerCollision(collision);
    }
    protected virtual void OnPlayerCollision(Collider2D collision) {
        string colName = collision.gameObject.name;
        Projectile p = collision.gameObject.GetComponent<Projectile>();

        currHealth = Mathf.Max(currHealth - p.damage, 0);
        hb.updateBar();
        if (!p.isPiercing) ProjectilePool.SharedInstance.ReturnToPool(collision.gameObject);

        if (!isAlive()) {
            StartCoroutine(bp.makeDrops(100));  //WHY
            GameQueue.SharedInstance.isQueueing = true;
            funcs.deactivateAll();
            gameObject.SetActive(false);
            Score.SharedInstance.changeScore(10000);
        }
        else {
            StartCoroutine(bp.makeDrops(1));
        }
    }

    public bool isAlive() {
        return currHealth > 0;
    }
}

public class EnemyBulletPatterns {
    protected Transform trans;
    protected delegate IEnumerator makeBulletPatterns();
    protected List<makeBulletPatterns> bulletPatterns;
    protected List<float> patternDurations;

    private IEnumerator makeCirclePattern() {
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 16; j++) {
                ProjectilePool.SharedInstance.GetPooledProjectile(circlePrefab, trans.position, cachedCirclePattern[i][j]);
            }
            yield return circleWait;
        }
        yield break;
    }
    private IEnumerator makeSwirlPattern() {
        int dir = UnityEngine.Random.value >= 0.5 ? 1 : -1;
        for (int i = 0; i < 4; i++) {
            dir *= -1;
            for (int j = 0; j < 16; j++) {
                ProjectilePool.SharedInstance.GetPooledProjectile(swirlPrefab, trans.position, cachedSwirlPattern[(dir + 1) / 2][i][j], 0, 6, false);
            }
            yield return swirlWait;
        }
        yield break;
    }
    private IEnumerator makeSweepPattern() {
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < (i == 1 ? 7 : 8); j++) {
                float dir = -1f + j / 3.5f + (i == 1 ? 1 / 7f : 0);
                ProjectilePool.SharedInstance.GetPooledProjectile(sweepPrefab, trans.position, cachedSweepPattern[i][j]);
            }
            yield return sweepWait;
        }
        yield break;
    }

    private WaitForSeconds circleWait = new WaitForSeconds(0.1f);
    private WaitForSeconds swirlWait = new WaitForSeconds(0.5f);
    private WaitForSeconds sweepWait = new WaitForSeconds(0.2f);

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

    private string circlePrefab = "Prefabs/Projectiles/BossBomb";
    private string swirlPrefab = "Prefabs/Projectiles/BossSwirl";
    private string sweepPrefab = "Prefabs/Projectiles/BossPotato";
    private string dropPrefab = "Prefabs/Projectiles/Drop";

    public EnemyBulletPatterns(Transform transform) {
        trans = transform;
        bulletPatterns = new List<makeBulletPatterns>();
        patternDurations = new List<float>();
        cacheAll();
    }

    protected virtual void cacheAll() {
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
    public float getPatternDuration(int i) {
        if (i >= 0 && i < patternDurations.Count) return patternDurations[i];
        throw new System.Exception();
    }

    public virtual IEnumerator makeDrops(int num) {
        for (int i = 0; i < num; i++) {
            ProjectilePool.SharedInstance.GetPooledDrop(dropPrefab, trans.position + new Vector3(UnityEngine.Random.value - 0.5f, UnityEngine.Random.value - 0.5f, 0) * 0.6f, null, 0.07f, (UnityEngine.Random.value - 0.5f) * 300 + 90);
            yield return null;
        }
    }
    public int getCount() {
        return bulletPatterns.Count;
    }
}

public class EnemyFunctions {
    protected Transform trans;
    protected Func<float, Vector3> temp;
    protected Equation eq;
    protected List<Function> funcList;

    public EnemyFunctions(Transform transform) {
        trans = transform;
        funcList = new List<Function>();
        cacheAll();
    }

    protected virtual void cacheAll() {
        temp = (theta) => {
            return new Vector3(theta, 0.5f * (theta + Mathf.Sin(theta * 2.0f)));
        };
        eq = new Equation(EquationType.RECTANGULAR, "y = x + sin(x)", temp);
        funcList.Add(Function.Create(trans, "Prefabs/Function", eq, 100, 1.5f, -6, 6));

        temp = (theta) => {
            return new Vector3(theta, -0.5f * (theta + Mathf.Sin(theta * 2.0f)));
        };
        eq = new Equation(EquationType.RECTANGULAR, "y = -x + sin(x)", temp);
        funcList.Add(Function.Create(trans, "Prefabs/Function", eq, 100, 1.5f, -6, 6));
    }

    public void activateFunction(int index) {
        if (index >= funcList.Count || index < -1) throw new Exception("Function index out of range! (" + index + ")");
        if (index == -1) index = (int)(UnityEngine.Random.value * funcList.Count);
        Function func = funcList[index];
        if (func.gameObject.activeInHierarchy) throw new Exception("Function is already active!");
        else func.gameObject.SetActive(true);
    }
    public void deactivateAll() {
        foreach (Function func in funcList) {
            func.gameObject.SetActive(false);
            func.destroyLines();
        }
    }
}