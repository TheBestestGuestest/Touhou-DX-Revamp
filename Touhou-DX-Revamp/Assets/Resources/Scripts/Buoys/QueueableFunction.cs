using UnityEngine;

public class QueueableFunction : MonoBehaviour {
    private FunctionSpawner funcSpawn;
    public int functionIndex;

    void Start() {
        funcSpawn = GameObject.Find("Queue Objects").GetComponent<FunctionSpawner>();
    }

    public void OnTriggerEnter2D(Collider2D collider) {
        if (collider.name.Equals("Queue Collider")) {
            //funcSpawn.setFunctionActiveState(functionIndex, true);
            Destroy(gameObject);
        }
    }
}
