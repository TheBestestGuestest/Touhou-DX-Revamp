using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gears : Enemy {
    private float radius;

    private Transform gearSprite;

    private float maxLinearSpd;
    private float currLinearSpeed;
    private Vector3 angleOfMovement;

    private float maxRotationSpd;
    private float currRotationSpd;
    private int rotationDirection;

    private float shootRate = 3f;
    private float shootTimer = 0f;
    private float functionRate = 8f;
    private float functionTimer = 0f;

    protected override void Awake() {
        base.Awake();
        gearSprite = transform.Find("Sprite");

        bp = new GearsBulletPatterns(transform, gearSprite);
        funcs = new GearsFunctions(transform);
    }
    protected override void initiateStats() {
        GameQueue.SharedInstance.isQueueing = false;
        radius = 0.75f;
        maxHealth = 5000;
        currHealth = maxHealth;

        maxLinearSpd = 4f;
        currLinearSpeed = maxLinearSpd;
        angleOfMovement = new Vector3(0.2f, 0.05f, 0);

        maxRotationSpd = 360f;
        currRotationSpd = maxRotationSpd / 3;
        rotationDirection = -1;
        ((GearsBulletPatterns)bp).setRotationDirection(rotationDirection);
    }

    protected override void OnPlayerCollision(Collider2D collision) {
        base.OnPlayerCollision(collision);
    }

    protected override void shoot() {
        if (globalTimer - shootTimer >= shootRate) {
            currRotationSpd = maxRotationSpd;
            currLinearSpeed = maxLinearSpd / 2;

            int bpIndex = (int)(Random.value * bp.getCount());
            StartCoroutine(bp.runBulletPattern(bpIndex));
            shootRate = bp.getPatternDuration(bpIndex);

            shootTimer = globalTimer;
        }
        else if (globalTimer - shootTimer >= shootRate / 3) {
            currRotationSpd = maxRotationSpd / 3;
            currLinearSpeed = maxLinearSpd;
        }
    }
    protected override void move() {
        trans.position += currLinearSpeed * angleOfMovement * Time.deltaTime;
        if (trans.position.x + radius > InGameDimentions.rightEdge ||
            trans.position.x - radius < InGameDimentions.leftEdge) angleOfMovement.x *= -1;
        if (trans.position.y + radius > InGameDimentions.topEdge ||
            trans.position.y - radius < InGameDimentions.bottomEdge) angleOfMovement.y *= -1;

        gearSprite.Rotate(rotationDirection * currRotationSpd * Vector3.forward * Time.deltaTime);
    }
    protected override void function() {
        if (globalTimer - functionTimer >= functionRate) {
            rotationDirection *= -1;
            ((GearsBulletPatterns)bp).setRotationDirection(rotationDirection);
            //funcs.activateFunction(-1);
            functionTimer = globalTimer;
        }
        else if (globalTimer - functionTimer >= functionRate / 4 && rotationDirection > 0) {
            rotationDirection *= -1;
            ((GearsBulletPatterns)bp).setRotationDirection(rotationDirection);
        }
    }
}

public class GearsBulletPatterns : EnemyBulletPatterns {
    private int rotationDirection;
    private Transform gearSprite;

    private float offset = 0;  //for the current rotation of the sprite
    private int circleRotationDirection;  //so the pattern doesnt change between rotation flips for the circle pattern

    public GearsBulletPatterns(Transform transform, Transform gS) : base(transform) {
        gearSprite = gS;
    }

    private IEnumerator makeCirclePattern() {
        circleRotationDirection = rotationDirection;
        for (int i = 0; i < cachedCirclePattern.Length; i++) {
            offset = Mathf.Deg2Rad * gearSprite.eulerAngles.z;
            for (int j = 0; j < cachedCirclePattern[i].Length; j++) {
                ProjectilePool.SharedInstance.GetPooledProjectile(gearProjPrefab_DG, trans.position, cachedCirclePattern[i][j]);
            }
            yield return circleWait;
        }
        yield break;
    }
    private IEnumerator makeSpiralPattern() {
        circleRotationDirection = rotationDirection;
        for (int i = 0; i < cachedSpiralPattern.Length; i++) {
            offset = Mathf.Deg2Rad * gearSprite.eulerAngles.z;
            for (int j = 0; j < cachedSpiralPattern[i].Length; j++) {
                ProjectilePool.SharedInstance.GetPooledProjectile(gearProjPrefab_LG, trans.position, cachedSpiralPattern[i][j]);
            }
            yield return spiralWait;
        }
        yield break;
    }
    private IEnumerator makeSuccPattern() {
        circleRotationDirection = rotationDirection;
        Vector3 succPos = trans.position;
        for (int i = 0; i < cachedSuccPattern.Length; i++) {
            for (int j = 0; j < cachedSuccPattern[i].Length; j++) {
                ProjectilePool.SharedInstance.GetPooledProjectile(gearProjPrefab_INV, succPos, cachedSuccPattern[i][j], 0, 4);
            }
            yield return succWait;
        }
        yield break;
    }

    private WaitForSeconds circleWait = new WaitForSeconds(0.1f);
    private WaitForSeconds spiralWait = new WaitForSeconds(0.14f);
    private WaitForSeconds succWait = new WaitForSeconds(0.4f);

    private MovePath[][] cachedCirclePattern = new MovePath[3][];
    private MovePath[][] cachedSpiralPattern = new MovePath[24][];
    private MovePath[][] cachedSuccPattern = new MovePath[6][];

    private MovePath circlePattern(int i, int j) {
        return delegate (float t, Vector3 pos) {
            float rads = circleRotationDirection * (j * Mathf.PI / 8 - i * Mathf.PI / 24) + offset;
            float distX = t * 5 * Mathf.Cos(rads);
            float distY = t * 5 * Mathf.Sin(rads);
            return new Vector3(pos.x + distX, pos.y + distY, pos.z);
        };
    }
    private MovePath spiralPattern(int i, int j) {
        return delegate (float t, Vector3 pos) {
            int radianCoeff = i + 1;
            float distCoeff = (25 * radianCoeff - radianCoeff * radianCoeff) / 20f;
            float rads = circleRotationDirection * ((j * 2 * Mathf.PI / 3) + radianCoeff * Mathf.PI / 11);
            float distX = t * distCoeff * Mathf.Cos(rads);
            float distY = t * distCoeff * Mathf.Sin(rads);
            return new Vector3(pos.x + distX, pos.y + distY, pos.z);
        };
    }
    private MovePath succPattern(int i, int j) {
        return delegate (float t, Vector3 pos) {
            float rads = circleRotationDirection * (j - t) * Mathf.PI / 6;
            float distX = t / 3f * Mathf.Max(16f - t * t) * Mathf.Cos(rads);
            float distY = t / 3f * Mathf.Max(16f - t * t) * Mathf.Sin(rads);
            return new Vector3(pos.x + distX, pos.y + distY, pos.z);
        };
    }

    private string gearProjPrefab_DG = "Prefabs/Projectiles/Level 1/gearProjDG";
    private string gearProjPrefab_LG = "Prefabs/Projectiles/Level 1/gearProjLG";
    private string gearProjPrefab_INV = "Prefabs/Projectiles/Level 1/gearProjINV";
    private string dropPrefab = "Prefabs/Projectiles/Drop";

    public override IEnumerator makeDrops(int num) {
        return base.makeDrops(num);
    }

    protected override void cacheAll() {
        bulletPatterns.Add(makeCirclePattern);
        for (int i = 0; i < cachedCirclePattern.Length; i++) {
            cachedCirclePattern[i] = new MovePath[16];
            for (int j = 0; j < cachedCirclePattern[i].Length; j++) {
                cachedCirclePattern[i][j] = circlePattern(i, j);
            }
        }
        patternDurations.Add(3f);

        bulletPatterns.Add(makeSuccPattern);
        for (int i = 0; i < cachedSuccPattern.Length; i++) {
            cachedSuccPattern[i] = new MovePath[12];
            for (int j = 0; j < cachedSuccPattern[i].Length; j++) {
                cachedSuccPattern[i][j] = succPattern(i, j);
            }
        }
        patternDurations.Add(7f);

        bulletPatterns.Add(makeSpiralPattern);
        for (int i = 0; i < cachedSpiralPattern.Length; i++) {
            cachedSpiralPattern[i] = new MovePath[3];
            for (int j = 0; j < cachedSpiralPattern[i].Length; j++) {
                cachedSpiralPattern[i][j] = spiralPattern(i, j);
            }
        }
        patternDurations.Add(8f);
    }

    public void setRotationDirection(int RD) {
        rotationDirection = RD;
    }
}

public class GearsFunctions : EnemyFunctions {
    public GearsFunctions(Transform transform) : base(transform) {
    }

    protected override void cacheAll() {

    }
}