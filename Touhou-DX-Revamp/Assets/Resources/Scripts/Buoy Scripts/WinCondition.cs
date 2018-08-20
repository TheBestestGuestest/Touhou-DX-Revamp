using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour {
    private string queueName = "Queue Collider";

    public void OnTriggerEnter2D(Collider2D collider) {
        if (collider.name.Equals(queueName)) {
            Destroy(gameObject);
            Main.SharedInstance.Win();
        }
    }
}
