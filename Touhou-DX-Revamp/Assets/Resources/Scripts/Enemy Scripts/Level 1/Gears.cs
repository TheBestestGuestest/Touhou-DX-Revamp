using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gears : Enemy {
    private float width, height;

    private float maxLinearSpd;
    private float currLinearSpeed;
    private Vector3 angleOfMovement;

    private float maxRotationSpd;
    private float minRotationSpd;
    private float currRotationSpd;

    protected override void Awake() {
        base.Awake();
        bp = new GearsBulletPatterns(transform);
        funcs = new GearsFunctions(transform);
    }
    protected override void initiateStats() {
        GameQueue.SharedInstance.isQueueing = false;
        maxHealth = 3000;
        currHealth = maxHealth;

        maxLinearSpd = 3f;
        currLinearSpeed = maxLinearSpd;
        angleOfMovement = new Vector3(0.2f, -0.1f, 0);

        maxRotationSpd = 3.14f;
        minRotationSpd = maxRotationSpd / 4f;
        currRotationSpd = minRotationSpd;
    }

    protected override void OnPlayerCollision(Collider2D collision) {
        base.OnPlayerCollision(collision);
    }

    protected override void shoot() {
        //StartCoroutine(bp.runBulletPattern((int)(UnityEngine.Random.value * bp.getCount())));
    }
    protected override void move() {
        trans.position += currLinearSpeed * angleOfMovement * Time.deltaTime;
    }
    protected override void function() {
        //funcs.activateFunction(-1);
    }
}

public class GearsBulletPatterns : EnemyBulletPatterns {
    public GearsBulletPatterns(Transform transform) : base(transform) {
    }

    public override IEnumerator makeDrops(int num) {
        return base.makeDrops(num);
    }

    protected override void cacheAll() {

    }
}

public class GearsFunctions : EnemyFunctions {
    public GearsFunctions(Transform transform) : base(transform) {
    }

    protected override void cacheAll() {

    }
}