using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour {
    private Equation eq;
    private float input;
    private Transform trans;
    private float destroyAfter;

    public static Point Create(string prefab, Equation equation, float i, float dA) {
        Point point = (Instantiate((GameObject)Resources.Load(prefab)) as GameObject).GetComponent<Point>();
        point.name = prefab.Substring(prefab.LastIndexOf("/") + 1);
        point.eq = equation;
        point.input = i;
        point.destroyAfter = dA;
        return point;
    }
    void Awake() {
        trans = transform;
    }
    void Start() {
        trans.position = getPosition();
        Destroy(gameObject, destroyAfter);
    }
    void Update() {
        if (CartesianPlane.SharedPlane.isGridShifting() || !trans.position.Equals(getPosition())) trans.position = getPosition();
    }
    public Vector3 getPosition() {
        return eq.posRelativeToPlane(input);
    }

    private void OnBecameInvisible() {
        Destroy(gameObject);
    }
    
}
