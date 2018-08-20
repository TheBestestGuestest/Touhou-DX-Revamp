using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueableFunction : MonoBehaviour {
    public int functionIndex;
    private string queueName = "Queue Collider";
    private FunctionSpawner funcSpawn;

    void Start(){
        funcSpawn = GameObject.Find("Queue Objects").GetComponent<FunctionSpawner>();
    }

    public void OnTriggerEnter2D(Collider2D collider) {
        if (collider.name.Equals(queueName)) {
            funcSpawn.activateFunction(functionIndex);
            Destroy(gameObject);
        }
    }
}
