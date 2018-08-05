using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBomb : MonoBehaviour {
    private string enemyProjectileTag = "Enemy Projectile";
    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag(enemyProjectileTag)) {
            ProjectilePool.SharedInstance.ReturnToPool(collision.gameObject);
        }
        else {
            //Debug.Log("bomb hit line");
        }
    }
}
