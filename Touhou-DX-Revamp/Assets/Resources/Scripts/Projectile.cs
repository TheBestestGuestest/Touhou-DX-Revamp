using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public int damage;
    public bool isPiercing;
    
    private MovePath path;
    private float totalTime = 0;
    private Vector3 spawnPos;

    private Transform trans;
    public static Projectile Create(string prefab, Vector3 pos, MovePath mp) {
        Projectile projectile = (Instantiate((GameObject)Resources.Load(prefab), pos, Quaternion.identity) as GameObject).GetComponent<Projectile>();
        projectile.path = mp;
        projectile.name = prefab.Substring(prefab.LastIndexOf("/") + 1);
        projectile.damage = 0;
        projectile.isPiercing = false;
        return projectile;
    }
    public static Projectile Create(string prefab, Vector3 pos, MovePath mp, int dmg, bool p) {
        Projectile projectile = Create(prefab, pos, mp);
        projectile.damage = dmg;
        projectile.isPiercing = p;
        return projectile;
    }

    void Awake() {
        trans = transform;
    }
    void Start() {
        spawnPos = trans.position;
    }
    void Update() {
        totalTime += Time.deltaTime;
        trans.position = path(totalTime, spawnPos);
    }

    private void OnBecameInvisible() {
        Destroy(gameObject);
    }
}

public delegate Vector3 MovePath(float t, Vector3 pos);