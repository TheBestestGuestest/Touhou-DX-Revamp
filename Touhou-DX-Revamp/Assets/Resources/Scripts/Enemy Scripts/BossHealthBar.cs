using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthBar : MonoBehaviour {
    private Boss boss;
    private Vector3 initScale;
    private Transform trans;

    void Awake() {
        trans = transform;
        boss = GetComponentInParent<Boss>() as Boss;
    }

    void OnEnable() {
        initScale = trans.localScale;
    }

    public void updateBar() {
        float ratio = (float)boss.currHealth / boss.maxHealth;
        trans.localScale = new Vector3(initScale.x, initScale.y * ratio, initScale.z);
    }
}
