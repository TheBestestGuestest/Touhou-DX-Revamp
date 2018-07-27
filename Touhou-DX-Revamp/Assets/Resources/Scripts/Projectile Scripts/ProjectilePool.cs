using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour {
    public static ProjectilePool SharedInstance;

    public volatile List<GameObject> pooledProjectiles;
    public volatile List<GameObject> pooledDrops;
    public int amountToPool;

    void Awake() {
        SharedInstance = this;
    }

    void Start() {
        pooledProjectiles = new List<GameObject>();
        pooledDrops = new List<GameObject>();
    }

    public GameObject GetPooledProjectile(string prefab, Vector3 pos, MovePath mp, int dmg = 0, bool p = false) {
        for (int i = 0; i < pooledProjectiles.Count; i++) {
            GameObject pp = pooledProjectiles[i];
            if (!pp.gameObject.activeInHierarchy && prefab.EndsWith(pp.name)) {
                Projectile projectile = isProjectile(pp);
                projectile.setValues(pos, mp, dmg, p);
                pp.SetActive(true);
                return pooledProjectiles[i];
            }
        }

        GameObject obj = Projectile.Create(prefab, pos, mp, dmg, p).gameObject;
        pooledProjectiles.Add(obj);
        return obj;
    }
    public GameObject GetPooledDrop(string prefab, Vector3 pos, Effect e, float s, float a) {
        for (int i = 0; i < pooledDrops.Count; i++) {
            GameObject pp = pooledDrops[i];
            if (!pp.gameObject.activeInHierarchy && prefab.EndsWith(pp.name)) {
                Drop drop = isDrop(pp);
                drop.setValues(pos);
                pp.SetActive(true);
                return pooledDrops[i];
            }
        }

        GameObject obj = Drop.Create(prefab, pos, e, s, a).gameObject;
        pooledDrops.Add(obj);
        return obj;
    }

    public void ReturnToPool(GameObject obj) {
        //check if the obj actually exists?
        if(isProjectile(obj) != null) obj.SetActive(false);
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
