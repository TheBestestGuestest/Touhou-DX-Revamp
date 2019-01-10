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
    protected virtual void setUpBPandFUNCS(){
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
    protected virtual void initiateStats() {}

    private void Update() {
        if(isAlive()){
            shoot(globalTimer);
            move(globalTimer);
            function(globalTimer);
        }
        globalTimer += Time.deltaTime;
    }
    protected virtual void shoot(float globalTimer) {}
    protected virtual void move(float globalTimer) {}
    protected virtual void function(float globalTimer) {}

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
    protected virtual void DeathHandler(){
        Score.SharedInstance.changeScore(10000);
        terminateShootingAndFunctions();
        StartCoroutine(bp.makeDrops(100));
        Invoke("DisableSelf", 2.5f);
    }
    protected void terminateShootingAndFunctions(){
        foreach(Coroutine shooting in shootingCoroutines) if(shooting != null) StopCoroutine(shooting);
        foreach(Coroutine function in functionCoroutines) if(function != null) StopCoroutine(function);
    }
    protected virtual void DisableSelf(){
        funcs.setAllActiveState(false);
        StopAllCoroutines();
        gameObject.SetActive(false);
        GameQueue.SharedInstance.isQueueing = true;
    }
    public bool isAlive() {
        return currHealth > 0;
    }
}

public enum PatternState {IDLE, FIRING}
public class EnemyBulletPatterns {
    protected Transform trans;
    protected delegate IEnumerator makeBulletPatterns();
    protected List<makeBulletPatterns> bulletPatterns;
    protected List<PatternState> patternStates;
    private string dropPrefab = "Prefabs/Projectiles/Drop";

    public EnemyBulletPatterns(Transform transform) {
        trans = transform;
        bulletPatterns = new List<makeBulletPatterns>();
        patternStates = new List<PatternState>();
        cacheAll();
    }
    protected virtual void cacheAll() {}

    public IEnumerator runBulletPattern(int i) {
        if (i >= 0 && i < bulletPatterns.Count) return bulletPatterns[i]();
        throw new System.Exception();
    }
    public PatternState getPatternState(int i) {
        if (i >= 0 && i < patternStates.Count) return patternStates[i];
        throw new System.Exception();
    }
    public virtual IEnumerator makeDrops(int num) {  //sus
        WaitForSeconds delay = new WaitForSeconds(0.01f);
        for (int i = 0; i < num; i++) {
            ProjectilePool.SharedInstance.GetPooledDrop(dropPrefab, trans.position + new Vector3(UnityEngine.Random.value - 0.5f, UnityEngine.Random.value - 0.5f, 0) * 0.6f, null, 0.07f, (UnityEngine.Random.value - 0.5f) * 300 + 90);
            yield return delay;
        }
    }
    public int getCount() {
        return bulletPatterns.Count;
    }
    public bool allPatternsIdle(){
        foreach(PatternState ps in patternStates) if(ps != PatternState.IDLE) return false;
        return true;
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
    protected virtual void cacheAll() {}
    protected void setFunctionActiveState(int index, bool active) {
        Function func = getFunction(index);
        if(active){
            if (func.gameObject.activeInHierarchy) throw new Exception("Function is already active!");
            else func.gameObject.SetActive(true);
        }
        else{
            if (!func.gameObject.activeInHierarchy) throw new Exception("Function is already inactive!");
            else func.disableFunction();
        }
    }
    public void setAllActiveState(bool active){
        for(int i = 0; i < funcList.Count; i++) setFunctionActiveState(i, active);
    }
    public Function getFunction(int index){
        if (index >= funcList.Count || index < 0) throw new Exception("Function index out of range! (" + index + ")");
        return funcList[index];
    }
    public int getCount(){
        return funcList.Count;
    }
    public bool allFunctionsIdle(){
        foreach(Function func in funcList) if(func.getCurrProcess() != FunctionProcess.IDLE) return false;
        return true;
    }
}