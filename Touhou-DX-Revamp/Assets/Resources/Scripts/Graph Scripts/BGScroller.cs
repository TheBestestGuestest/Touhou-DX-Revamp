using UnityEngine;
using System.Collections;

public class BGScroller : MonoBehaviour {

    public float scrollSpeed;
    public float tileSizeY;

    private Vector3 startPosition;
    private Transform trans;

    private void Awake() {
        trans = transform;
    }

    void Start() {
        startPosition = trans.position;
    }

    void Update() {
        float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeY);
        trans.position = startPosition + Vector3.up * newPosition;
    }
}