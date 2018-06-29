using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBomb : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag.Equals("Enemy Projectile")) {
            Destroy(collision.gameObject);
        }
    }
}
