using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : Projectile { 
    private GameObject player;
    private Transform playerTransform;
    private Effect effect;

    private float attractStrength;  //lower is better LUL

    public static Drop Create(string prefab, Vector3 pos, Effect e, float s, float a) {
        Drop drop = (Instantiate((GameObject)Resources.Load(prefab), pos, Quaternion.identity) as GameObject).GetComponent<Drop>();
        //drop.effect = e;
        drop.attractStrength = s;
        drop.transform.Rotate(new Vector3(0, 0, a));
        drop.name = prefab.Substring(prefab.LastIndexOf("/") + 1);
        return drop;
    }
    
    public void setValues(Vector3 pos) {
        trans.position = pos;
    }

    void Awake() {
        base.Awake();

        player = GameObject.Find("Player");
        if (player != null) playerTransform = player.transform;

        path = (float t, Vector3 pos) => {
            Vector3 temp = trans.position;
            Vector3 lookPos = temp - playerTransform.position;
            float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
            trans.rotation = Quaternion.Slerp(trans.rotation, Quaternion.AngleAxis(angle, Vector3.forward), totalTime / attractStrength);
            temp -= trans.right * 0.32f;
            return temp;
        };
    }

    void Update() {
        if (player != null) {
            base.Update();
        }
        else {
            ProjectilePool.SharedInstance.ReturnToPool(gameObject);
        }
    }
}
public delegate void Effect();