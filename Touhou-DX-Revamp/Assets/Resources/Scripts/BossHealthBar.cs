using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthBar : MonoBehaviour {
    private Boss boss;
    private Vector3 initScale;

    void Start() {
        boss = GetComponentInParent<Boss>() as Boss;
        initScale = transform.localScale;
    }

    public void updateBar() {
        float ratio = (float)boss.currHealth / boss.maxHealth;
        transform.localScale = new Vector3(initScale.x, initScale.y * ratio, initScale.z);
    }
}
