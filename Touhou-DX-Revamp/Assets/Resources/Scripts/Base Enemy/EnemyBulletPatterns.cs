using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PatternState { IDLE, FIRING }
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
    protected virtual void cacheAll() { }

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
            ProjectilePool.SharedInstance.GetPooledDrop(dropPrefab, trans.position + new Vector3(UnityEngine.Random.value - 0.5f, UnityEngine.Random.value - 0.5f, 0) * 0.6f, null);
            yield return delay;
        }
    }
    public int getCount() {
        return bulletPatterns.Count;
    }
    public bool allPatternsIdle() {
        foreach (PatternState ps in patternStates) if (ps != PatternState.IDLE) return false;
        return true;
    }
}
