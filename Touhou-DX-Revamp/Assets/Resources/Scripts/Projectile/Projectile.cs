using UnityEngine;

public class Projectile : MonoBehaviour {
    public int damage { get; set; }
    public bool isPiercing { get; set; }

    protected Transform trans;
    protected MovePath path;
    protected float totalTime;

    protected float destroyTime = -1;
    private Vector3 spawnPos;

    public static Projectile Create(string prefab, Vector3 pos, MovePath mp, int dmg = 0, float t = -1, bool p = false) {
        Projectile projectile = (Instantiate((GameObject)Resources.Load(prefab), pos, Quaternion.identity, ProjectilePool.SharedInstance.transform) as GameObject).GetComponent<Projectile>();
        projectile.name = prefab.Substring(prefab.LastIndexOf("/") + 1); //optimize?
        projectile.setValues(pos, mp, dmg, t, p);
        return projectile;
    }
    public void setValues(Vector3 pos, MovePath mp, int dmg, float t, bool p) {
        spawnPos = pos;
        trans.position = pos;
        path = mp;
        damage = dmg;
        destroyTime = t;
        isPiercing = p;
    }

    protected void Awake() {
        trans = transform;
    }

    protected void OnEnable() {
        totalTime = 0;
    }

    protected void Update() {
        trans.position = path(totalTime, spawnPos);
        if (destroyTime != -1 && totalTime >= destroyTime) ProjectilePool.SharedInstance.ReturnToPool(gameObject);
        
        totalTime += Time.deltaTime;
    }

    protected void OnBecameInvisible() {
        if (destroyTime == -1) ProjectilePool.SharedInstance.ReturnToPool(gameObject);
    }
}

public delegate Vector3 MovePath(float t, Vector3 pos);