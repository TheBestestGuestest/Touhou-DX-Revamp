using UnityEngine;

public class GameQueue : MonoBehaviour {
    public static GameQueue SharedInstance;
    private Transform trans;

    public bool isQueueing { get; set; }
    public float queueSpeed = 1f;

    void Awake() {
        SharedInstance = this;
        trans = transform;
    }

    void Update() {
        if (isQueueing) trans.position += Time.deltaTime * Vector3.left * queueSpeed;
    }
}
