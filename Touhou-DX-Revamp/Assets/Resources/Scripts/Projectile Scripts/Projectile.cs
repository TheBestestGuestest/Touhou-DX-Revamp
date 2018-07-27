using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    [HideInInspector]
    public int damage;
    [HideInInspector]
    public bool isPiercing;
    [HideInInspector]
    public Transform trans;
    public MovePath path;
    public float totalTime;

    private Vector3 spawnPos;
    
    public static Projectile Create(string prefab, Vector3 pos, MovePath mp, int dmg = 0, bool p = false) {
        Projectile projectile = (Instantiate((GameObject)Resources.Load(prefab), pos, Quaternion.identity) as GameObject).GetComponent<Projectile>();
        projectile.name = prefab.Substring(prefab.LastIndexOf("/") + 1);
        projectile.setValues(pos, mp, dmg, p);
        return projectile;
    }
    public void setValues(Vector3 pos, MovePath mp, int dmg, bool p) {
        spawnPos = pos;
        trans.position = pos;
        path = mp;
        damage = dmg;
        isPiercing = p;
    }

    public void Awake() {
        trans = transform;
    }
    public void OnEnable() {
        totalTime = 0;
    }
    public void Update() {
        totalTime += Time.deltaTime;
        trans.position = path(totalTime, spawnPos);
    }

    private void OnBecameInvisible() {
        ProjectilePool.SharedInstance.ReturnToPool(gameObject);
    }
}

public delegate Vector3 MovePath(float t, Vector3 pos);