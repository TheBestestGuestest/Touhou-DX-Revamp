using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equation {
    public EquationType type;
    public string name;
    public System.Func<float, Vector3> func;

    public Equation(EquationType type, string name, System.Func<float, Vector3> func) {
        this.type = type;
        this.name = name;
        this.func = func;
    }
    public Vector3 getPoint(float input) {
        return func(input);
    }
    public Vector3 posRelativeToPlane(float input) {
        return CartesianPlane.SharedPlane.pointRelativeToOrigin(getPoint(input));
    }
}

public enum EquationType { RECTANGULAR, PARAMETRIC, POLAR }