using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour {
    public static ProjectilePool SharedInstance;

    private volatile Dictionary<string, List<GameObject>> pooledProjectiles;
    private volatile Dictionary<string, List<GameObject>> pooledDrops;

    void Awake() {
        SharedInstance = this;
    }

    void Start() {
        pooledProjectiles = new Dictionary<string, List<GameObject>>();
        pooledDrops = new Dictionary<string, List<GameObject>>();
    }

    public GameObject GetPooledProjectile(string prefab, Vector3 pos, MovePath mp, int dmg = 0, float t = -1, bool p = false) {
        if (!pooledProjectiles.ContainsKey(prefab)) pooledProjectiles.Add(prefab, new List<GameObject>());
        List<GameObject> pool = pooledProjectiles[prefab];

        foreach (GameObject pp in pool) {
            if (!pp.activeInHierarchy) {
                Projectile projectile = isProjectile(pp);
                projectile.setValues(pos, mp, dmg, t, p);
                pp.SetActive(true);
                return pp;
            }
        }

        GameObject obj = Projectile.Create(prefab, pos, mp, dmg, t, p).gameObject;
        pool.Add(obj);
        return obj;
    }
    public GameObject GetPooledDrop(string prefab, Vector3 pos, Effect e, float s, float a) {
        if (!pooledDrops.ContainsKey(prefab)) pooledDrops.Add(prefab, new List<GameObject>());
        List<GameObject> pool = pooledDrops[prefab];

        foreach (GameObject pd in pool) {
            if (!pd.activeInHierarchy) {
                Drop drop = isDrop(pd);
                drop.setValues(pos);
                pd.SetActive(true);
                return pd;
            }
        }

        GameObject obj = Drop.Create(prefab, pos, e, s, a).gameObject;
        pool.Add(obj);
        return obj;
    }

    public void ReturnToPool(GameObject obj) {
        if (isProjectile(obj) != null) obj.SetActive(false);
    }

    public void ReturnEnemyProjectilesToPool(){
        //todo sus
    }
    public void ReturnAllProjectilesToPool() {
        foreach (KeyValuePair<string, List<GameObject>> entry in pooledProjectiles) {
            List<GameObject> pool = entry.Value;
            foreach (GameObject pp in pool) {
                ReturnToPool(pp);
            }
        }
    }
    public void ReturnAllDropsToPool() {
        foreach (KeyValuePair<string, List<GameObject>> entry in pooledDrops) {
            List<GameObject> pool = entry.Value;
            foreach (GameObject pd in pool) {
                ReturnToPool(pd);
            }
        }
    }

    public Projectile isProjectile(GameObject obj) {
        Projectile projectile = obj.GetComponent<Projectile>();
        if (projectile == null) throw new System.Exception("Object is NOT a Projectile!");
        return projectile;
    }
    public Drop isDrop(GameObject obj) {
        Drop drop = obj.GetComponent<Drop>();
        if (drop == null) throw new System.Exception("Object is NOT a Drop!");
        return drop;
    }
}
