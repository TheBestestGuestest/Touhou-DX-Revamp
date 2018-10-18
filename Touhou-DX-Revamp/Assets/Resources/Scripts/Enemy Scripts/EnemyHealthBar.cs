using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour {
    private Enemy enemy;
    public Vector3 initScale = new Vector3(1, 7.47f, 1);
    private Transform trans;

    private void Awake() {
        trans = transform;
        enemy = GetComponentInParent<Enemy>() as Enemy;
    }

    public virtual void updateBar() {
        float ratio = (float)enemy.currHealth / enemy.maxHealth;
        trans.localScale = new Vector3(initScale.x, initScale.y * ratio, initScale.z);
    }
}
