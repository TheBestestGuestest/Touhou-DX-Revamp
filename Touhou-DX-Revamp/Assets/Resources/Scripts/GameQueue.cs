using UnityEngine;

public class GameQueue : MonoBehaviour {
    public static GameQueue SharedInstance;
    [System.NonSerialized]
    public bool isQueueing = true;
    public float queueSpeed = 1f;

    public Transform trans;

    void Awake() {
        SharedInstance = this;
        trans = transform;
    }

    void Update() {
        if (isQueueing) trans.position += Time.deltaTime * Vector3.left * queueSpeed;
    }
}
