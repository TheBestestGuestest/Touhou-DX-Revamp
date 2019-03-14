using System;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [HideInInspector]
    public int maxHealth, currHealth;

    protected Transform trans;
    protected EnemyHealthBar hb;
    protected EnemyBulletPatterns bp;
    protected EnemyFunctions funcs;
    protected Coroutine[] shootingCoroutines;
    protected Coroutine[] functionCoroutines;

    protected float globalTimer;

    private string playerProjectileTag = "Player Projectile";

    protected virtual void Awake() {
        trans = transform;
        hb = GetComponentInChildren<EnemyHealthBar>() as EnemyHealthBar;
        setUpBPandFUNCS();
        gameObject.SetActive(false);
    }
    protected virtual void setUpBPandFUNCS() {
        bp = new EnemyBulletPatterns(trans);
        funcs = new EnemyFunctions(trans);
        shootingCoroutines = new Coroutine[bp.getCount()];
        functionCoroutines = new Coroutine[funcs.getCount()];
    }
    private void OnEnable() {
        initiateStats();
        funcs.setAllActiveState(true);
        globalTimer = 0.0f;
        hb.updateBar();
    }
    protected virtual void initiateStats() { }

    private void Update() {
        if (isAlive()) {
            shoot(globalTimer);
            move(globalTimer);
            function(globalTimer);
        }
        globalTimer += Time.deltaTime;
    }
    protected virtual void shoot(float globalTimer) { }
    protected virtual void move(float globalTimer) { }
    protected virtual void function(float globalTimer) { }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag(playerProjectileTag) && isAlive()) OnPlayerCollision(collision);
    }
    protected virtual void OnPlayerCollision(Collider2D collision) {  //sus
        Projectile p = collision.gameObject.GetComponent<Projectile>();
        if (!p.isPiercing) ProjectilePool.SharedInstance.ReturnToPool(collision.gameObject);

        currHealth = Mathf.Max(currHealth - p.damage, 0);
        hb.updateBar();
        if (!isAlive()) DeathHandler();
        else StartCoroutine(bp.makeDrops(1));
    }
    protected virtual void DeathHandler() {
        Score.SharedInstance.changeScore(10000);
        terminateShootingAndFunctions();
        funcs.setAllActiveState(false);
        StartCoroutine(bp.makeDrops(100));
        Invoke("DisableSelf", 2.5f);
    }
    protected void terminateShootingAndFunctions() {
        foreach (Coroutine shooting in shootingCoroutines) if (shooting != null) StopCoroutine(shooting);
        foreach (Coroutine function in functionCoroutines) if (function != null) StopCoroutine(function);
    }
    protected virtual void DisableSelf() {

        StopAllCoroutines();
        gameObject.SetActive(false);
        GameQueue.SharedInstance.isQueueing = true;
    }
    public bool isAlive() {
        return currHealth > 0;
    }
}
