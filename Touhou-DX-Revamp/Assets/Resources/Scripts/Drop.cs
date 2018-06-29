using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour {
    private GameObject player;
    private Transform trans;
    private Transform playerTransform;
    private Effect effect;

    private float totalTime = 0;

    private float attractStrength;  //lower is better LUL

    public static Drop Create(string prefab, Vector3 pos, Effect e, float s, float a) {
        Drop drop = (Instantiate((GameObject)Resources.Load(prefab), pos, Quaternion.identity) as GameObject).GetComponent<Drop>();
        //drop.effect = e;
        drop.attractStrength = s;
        drop.transform.Rotate(new Vector3(0, 0, a));
        drop.name = prefab.Substring(prefab.LastIndexOf("/") + 1);
        return drop;
    }

    private void Awake() {
        player = GameObject.Find("Player");
        if (player != null) playerTransform = player.transform;
    }
    void Start() {
        trans = transform;
    }

    void Update() {
        if (player != null) {
            totalTime += Time.deltaTime;
            Vector3 lookPos = trans.position - playerTransform.position;
            float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
            trans.rotation = Quaternion.Slerp(trans.rotation, Quaternion.AngleAxis(angle, Vector3.forward), totalTime / attractStrength);
            trans.position -= trans.right * 0.32f;
        }
        else {
            Destroy(gameObject);
        }
    }
}
public delegate void Effect();