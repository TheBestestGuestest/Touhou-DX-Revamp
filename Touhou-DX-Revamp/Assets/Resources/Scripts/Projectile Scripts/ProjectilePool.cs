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

    private bool CustomEndsWith(string a, string b) {
        int ap = a.Length - 1;
        int bp = b.Length - 1;

        while (ap >= 0 && bp >= 0 && a[ap] == b[bp]) {
            ap--;
            bp--;
        }
        return (bp < 0 && a.Length >= b.Length) ||

                (ap < 0 && b.Length >= a.Length);
    }

    public GameObject GetPooledProjectile(string prefab, Vector3 pos, MovePath mp, int dmg = 0, bool p = false) {
        foreach (GameObject pp in pooledProjectiles) {
            if (!pp.gameObject.activeInHierarchy && CustomEndsWith(prefab, pp.name)) {
                Projectile projectile = isProjectile(pp);
                projectile.setValues(pos, mp, dmg, p);
                pp.SetActive(true);
                return pp;
            }
        }

        GameObject obj = Projectile.Create(prefab, pos, mp, dmg, p).gameObject;
        pooledProjectiles.Add(obj);
        return obj;
    }
    public GameObject GetPooledDrop(string prefab, Vector3 pos, Effect e, float s, float a) {
        foreach(GameObject pd in pooledDrops) { 
            if (!pd.gameObject.activeInHierarchy && CustomEndsWith(prefab, pd.name)) {
                Drop drop = isDrop(pd);
                drop.setValues(pos);
                pd.SetActive(true);
                return pd;
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
