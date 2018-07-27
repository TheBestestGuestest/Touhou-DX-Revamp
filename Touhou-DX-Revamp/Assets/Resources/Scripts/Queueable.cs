using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queueable : MonoBehaviour {
    public GameObject objectInQueue;
    public Vector3 spawnPos = new Vector3(-1.83333f, 3f, 0f);

    private string queueName = "Queue Collider";

    public void Start() {
        objectInQueue.SetActive(false);
    }
    
    public void OnTriggerEnter2D(Collider2D collider) {
        if (collider.name.Equals(queueName)) {
            objectInQueue.transform.position = spawnPos;
            objectInQueue.SetActive(true);
            Destroy(gameObject);
        }
    }
}
